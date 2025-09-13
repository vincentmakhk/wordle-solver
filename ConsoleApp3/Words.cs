namespace ConsoleApp3;

public class Words
{
    public string[] AllValidWords { get; }

    public string[] AllAnswers { get; }

    public Words()
    {
        AllValidWords = File.ReadAllLines("d:\\download\\wordle-full-list.txt")
            .ToList().Select(x => x.ToLower()).ToArray();
        AllAnswers = File.ReadAllLines("d:\\download\\wordle-answers.txt")
            .ToList().Select(x => x.ToLower()).ToArray();
    }

    public string RandomizeAnswer()
    {
        var random = new Random();
        var index = random.Next(0, AllAnswers.Length);
        return AllAnswers[index];
    }

    public List<string> GetValidWords(List<Entry> entries)
    {
        var words = new List<string>();
        var judge = new Judge();
        foreach (var word in AllValidWords)
        {
            if (judge.Fulfill(entries, word))
            {
                words.Add(word);
            }
        }
        return words;
    }

    public int CountValidAnswers(List<Entry> entries, string[] answers, int minCount)
    {
        int count = 0;
        var judge = new Judge();
        foreach (var word in answers)
        {
            if (judge.FulfillAsAnswer(entries, word))
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

    public string PickBest(HashSet<string> equals)
    {
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
