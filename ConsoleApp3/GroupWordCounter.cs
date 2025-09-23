namespace ConsoleApp3;

public class GroupWordCounter : IWordCounter
{
    private readonly Words _words;
    private readonly Judge _judge;
    private readonly WordGrouper _wordGrouper;

    public GroupWordCounter(Words words, Judge judge)
    {
        _words = words;
        _judge = judge;
        _wordGrouper = new WordGrouper(words, judge);
    }

    public int FindMaxCount(string nextGuess, string[] validAnswers, int minCount)
    {
        var groups = _wordGrouper.GroupByState(nextGuess, validAnswers.ToList());

        int max = 0;
        foreach (var group in groups)
        {
            var states = group.Value.Item1;
            var entries = new List<Entry>() { new Entry(nextGuess, states) };
            if (_judge.HasWon(entries[0]))
            {
                Console.WriteLine($"HasWon {nextGuess}");
                continue;
            }
            var answers = group.Value.Item2;
            var validWords = _words.GetValidWords(entries, true);
            int maxInner = 0;
            foreach (var word in validWords)
            {
                var g2 = _wordGrouper.GroupByState(word, answers);
                var m2 = g2.Values.Max(x => x.Item2.Count);
                if (m2 >= maxInner)
                {
                    maxInner = m2;
                }

                if (maxInner > minCount)
                {
                    break;
                }
            }

            if (maxInner >= max)
            {
                //Console.WriteLine($"{group.Key} {maxInner}");
                max = maxInner;
            }

            if (maxInner > minCount)
            {
                break;
            }
        }

        return max;
    }
}
