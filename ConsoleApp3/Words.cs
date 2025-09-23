namespace ConsoleApp3;

public class Words
{
    public const int MaxValue = 100000;
    private readonly Judge _judge;

    public string[] AllValidWords { get; }

    public string[] AllAnswers { get; }

    public Words()
    {
        _judge = new Judge();

        AllValidWords = File.ReadAllLines("./text/wordle-full-list.txt")
            .ToList().Select(x => x.ToLower()).ToArray();
        AllAnswers = File.ReadAllLines("./text/wordle-answers.txt")
            .ToList().Select(x => x.ToLower()).ToArray();
    }

    public string RandomizeAnswer()
    {
        var random = new Random();
        var index = random.Next(0, AllAnswers.Length);
        return AllAnswers[index];
    }

    public List<string> GetValidWords(List<Entry> entries, bool grayCheck)
    {
        var words = new List<string>();
        foreach (var word in AllValidWords)
        {
            if (_judge.Fulfill(entries, word, grayCheck))
            {
                words.Add(word);
            }
        }
        return words;
    }

    public int CountValidAnswers(Entry entry, string[] answers, int minCount)
    {
        int count = 0;
        foreach (var word in answers)
        {
            if (_judge.FulfillAsAnswer(entry, word))
            {
                ++count;
            }

            if (count > minCount)
                return count;
        }
        return count;
    }

    public List<string> GetValidAnswers(List<Entry> entries, string[] answers)
    {
        var words = new List<string>();
        foreach (var word in answers)
        {
            if (_judge.FulfillAsAnswer(entries, word))
            {
                words.Add(word);
            }
        }
        return words;
    }

}
