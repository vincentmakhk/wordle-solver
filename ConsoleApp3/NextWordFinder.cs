using System.Text;

namespace ConsoleApp3;

public class NextWordFinder
{
    private readonly Words _words;
    private readonly Judge _judge;

    public NextWordFinder(Words words, Judge judge)
    {
        _words = words;
        _judge = judge;
    }

    public int FindMaxCount(string nextGuess, string[] validAnswers, int minCount)
    {
        int maxCount = 0;

        // if nextGuess, assume answer, given results
        foreach (var possibleAnswer in validAnswers)
        {
            var nextEntry = _judge.MakeGuess(nextGuess, possibleAnswer);
            var nextCount = _words.CountValidAnswers(nextEntry, validAnswers, minCount);
            if (nextCount == 0)
            {
                continue;
            }
            if (nextCount > maxCount)
            {
                maxCount = nextCount;
            }
        }

        return maxCount;
    }

    public string Find(List<Entry> entries, string[] validAnswers)
    {
        var validGuess = _words.GetValidWords(entries, true);
        Console.WriteLine($"Remains {validGuess.Count} guesses {validAnswers.Length} answers");
        int minCount = int.MaxValue;
        HashSet<string> equals = new();
        foreach (var nextGuess in validGuess)
        {
            int maxCount = FindMaxCount(nextGuess, validAnswers, minCount + 25);

            //Console.WriteLine($"{nextGuess} {maxCount} {minCount}");

            if (maxCount == 0)
            {
                continue;
            }

            if (maxCount < minCount)
            {
                minCount = maxCount;
                equals.Clear();
                equals.Add(nextGuess);
            }
            else if (maxCount == minCount)
            {
                equals.Add(nextGuess);
            }
        }

        if (validAnswers.Length == 1)
        {
            if (!equals.Contains(validAnswers[0]))
                throw new InvalidOperationException("Impossible not to have only answer");
        }

        var decidedGuess = PickBest(equals, validAnswers);
        return decidedGuess;
    }

    public string ExhaustiveSearch(List<Entry> entries, string[] validAnswers)
    {
        var validGuess = _words.GetValidWords(entries, true);
        Console.WriteLine($"Exhausts {validGuess.Count} guesses {validAnswers.Length} answers");
        int minCount = int.MaxValue;
        HashSet<string> equals = new HashSet<string>();
        foreach (var nextGuess in validGuess)
        {
            int maxCount = FindMaxCountWithLimit(entries, nextGuess, validAnswers, minCount, 6 - entries.Count);

            Console.WriteLine($"{nextGuess} {maxCount} {minCount}");

            if (maxCount == 0)
            {
                continue;
            }

            if (maxCount < minCount)
            {
                minCount = maxCount;
                equals.Clear();
                equals.Add(nextGuess);
            }
            else if (maxCount == minCount)
            {
                equals.Add(nextGuess);
            }
        }

        if (validAnswers.Length == 1)
        {
            if (!equals.Contains(validAnswers[0]))
                throw new InvalidOperationException("Impossible not to have only answer");
        }

        var decidedGuess = PickBest(equals, validAnswers);
        return decidedGuess;
    }

    private int FindMaxCountWithLimit(List<Entry> entries, string nextGuess, string[] validAnswers, int minCount, int limit)
    {
        int maxCount = 0;

        // if nextGuess, assume answer, given results
        foreach (var possibleAnswer in validAnswers)
        {
            var nextEntries = new List<Entry>(entries)
            {
                _judge.MakeGuess(nextGuess, possibleAnswer)
            };
            var nextAnswers = _words.GetValidAnswers(nextEntries, validAnswers);
            int nextCount = nextAnswers.Count;
            if (nextCount == 0)
            {
                continue;
            }

            if (nextCount + 20 < maxCount)
                continue;

            if (Unresolved(6 - nextEntries.Count, nextAnswers.ToArray(), validAnswers))
            {
                nextCount += 20;
            }

            if (nextCount > maxCount)
            {
                maxCount = nextCount;
            }
        }

        return maxCount;
    }

    private bool Unresolved(int p, string[] nextAnswers, string[] validAnswers)
    {
        for (int pattern = 0; pattern < 5; pattern++)
        {
            var states = new State[5];
            for (int i = 0; i < 5; ++i)
                states[i] = (i == pattern) ? State.Gray : State.Green;

            var g1 = WordGrouper(nextAnswers, states);
            var g2 = WordGrouper(validAnswers, states);
            foreach (var g in g1)
            {
                if (g.Value.Count > p && g2[g.Key].Count == g.Value.Count)
                    return true;
            }
        }

        return false;
    }

    public Dictionary<string, List<string>> WordGrouper(string[] words, State[] states)
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
                bool samePattern = true;
                for (int i=0; i<5; ++i)
                {
                    if (states[i] == State.Green && word[i] != anotherWord[i])
                    {
                        samePattern = false;
                        break;
                    }
                }
                if (samePattern)
                {
                    collection.Add(anotherWord);
                }
            }

            groups.Add(prefix, collection);
        }
        return groups;
    }

    public string PickBest(HashSet<string> equals, string[] validAnswers)
    {
        if (equals.Count == 0)
        {
            throw new InvalidOperationException();
        }

        if (equals.Count == 1)
        {
            return equals.First();
        }

        if (validAnswers.Length == 1)
        {
            return validAnswers[0];
        }

        string w = "";
        int score = int.MinValue;
        foreach (var word in equals)
        {
            int s = Score(word);
            if (s > score)
            {
                score = s;
                w = word;
            }
        }
        return w;
    }

    public int Score(string word)
    {
        int duplicate = -word.GroupBy(c => c).Select(g => g.Count() - 1).Sum();
        int vowels = 0;
        if (word.Contains('a'))
            ++vowels;
        if (word.Contains('e'))
            ++vowels;
        if (word.Contains('i'))
            ++vowels;
        if (word.Contains('o'))
            ++vowels;
        if (word.Contains('u'))
            ++vowels;

        int no_vowels = 0;
        if (!word.Contains('a') &&
            !word.Contains('e') &&
            !word.Contains('i') &&
            !word.Contains('o') &&
            !word.Contains('u'))
            no_vowels = 3;

        return duplicate * 2 + vowels + no_vowels;
    }
}
