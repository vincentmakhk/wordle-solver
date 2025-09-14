namespace ConsoleApp3;

public class Words
{
    public string[] AllValidWords { get; }

    public string[] AllAnswers { get; }

    public Words()
    {
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
        var judge = new Judge();
        foreach (var word in AllValidWords)
        {
            if (judge.Fulfill(entries, word, grayCheck))
            {
                words.Add(word);
            }
        }
        return words;
    }

    public int CountValidAnswers(Entry entry, string[] answers, int minCount)
    {
        int count = 0;
        var judge = new Judge();
        foreach (var word in answers)
        {
            if (judge.FulfillAsAnswer(entry, word))
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
        var judge = new Judge();
        foreach (var word in answers)
        {
            if (judge.FulfillAsAnswer(entries, word))
            {
                words.Add(word);
            }
        }
        return words;
    }

}
