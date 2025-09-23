namespace ConsoleApp3;

public class WordGrouper
{
    private readonly Words _words;
    private readonly Judge _judge;

    public WordGrouper(Words words, Judge judge)
    {
        _words = words;
        _judge = judge;
    }

    public Dictionary<string, (State[], List<string>)> GroupByState(string guess, List<string> answers)
    {
        var groups = new Dictionary<string, (State[], List<string>)>();

        foreach (var word in answers)
        {
            var states = _judge.Check(word, guess);
            var key = StateUtil.StatesToString(states);
            if (groups.ContainsKey(key))
            {
                groups[key].Item2.Add(word);
            }
            else
            {
                groups[key] = (states, new List<string>() { word });
            }
        }

        return groups;
    }

    public Dictionary<string, List<string>> FindPatterns(string[] allAnswers)
    {
        var groups = new Dictionary<string, List<string>>();

        for (int pattern = 1; pattern < 32; pattern++)
        {
            var states = new State[5]
            {
                (pattern & 1)==0? State.Gray : State.Green,
                (pattern & 2)==0? State.Gray : State.Green,
                (pattern & 4)==0? State.Gray : State.Green,
                (pattern & 8)==0? State.Gray : State.Green,
                (pattern & 16)==0? State.Gray : State.Green
            };

            var c = states.Count(x => x == State.Green);
            if (c <= 2)
                continue;

            var g = GroupBySameState(allAnswers, states);
            foreach (var pair in g)
                groups.Add(pair.Key, pair.Value);
        }

        return groups;
    }

    public Dictionary<string, List<string>> GroupBySameState(string[] words, State[] states)
    {
        var groups = new Dictionary<string, List<string>>();
        for (int big = 0; big < words.Length; big++)
        {
            string? word = words[big];
            var prefix = WordUtil.GeneratePrefix(word, states);
            if (groups.ContainsKey(prefix))
                continue;

            var collection = new List<string>
            {
                word
            };

            for (int small = big + 1; small < words.Length; small++)
            {
                string? anotherWord = words[small];
                bool samePattern = StateUtil.IsSamePattern(states, word, anotherWord);
                if (samePattern)
                {
                    collection.Add(anotherWord);
                }
            }

            groups.Add(prefix, collection);
        }
        return groups;
    }

    public void VerifyGuess(List<Entry> previousEntries, string guess, List<string> validAnswers)
    {
        foreach (var entry in previousEntries)
        {
            if (!_judge.IsLegitimateGuess(entry, guess))
            {
                throw new InvalidOperationException();
            }
        }

        var verification = _words.GetValidAnswers(previousEntries, _words.AllAnswers);
        if (verification.Count != validAnswers.Count)
        {
            throw new InvalidOperationException();
        }
    }
}
