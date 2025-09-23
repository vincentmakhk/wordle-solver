using System.Text;

namespace ConsoleApp3;

public class NextWordFinder
{
    private readonly Words _words;
    private readonly Judge _judge;
    private readonly IWordCounter _counter;
    private const int MinScore = -100000;

    public NextWordFinder(Words words, Judge judge)
    {
        _words = words;
        _judge = judge;
        _counter = new MaxWordCounter(words, judge);
    }

    public string Find(List<Entry> entries, string[] validAnswers)
    {
        var validGuess = _words.GetValidWords(entries, true);
        Console.WriteLine($"Remains {validGuess.Count} guesses {validAnswers.Length} answers");
        var equals = MinMaxFinder.GetBestWords(validGuess, _counter, validAnswers);

        if (validAnswers.Length == 1)
        {
            if (!equals.Contains(validAnswers[0]))
                throw new InvalidOperationException("Impossible not to have only answer");
        }

        var decidedGuess = PickBest(equals, validAnswers);
        return decidedGuess;
    }

    //public string ExhaustiveSearch(List<Entry> entries, string[] validAnswers)
    //{
    //    var validGuess = _words.GetValidWords(entries, true);
    //    Console.WriteLine($"Exhausts {validGuess.Count} guesses {validAnswers.Length} answers");
    //    int minCount = Words.MaxValue;
    //    HashSet<string> equals = new HashSet<string>();
    //    foreach (var nextGuess in validGuess)
    //    {
    //        int maxCount = FindMaxCountWithLimit(entries, nextGuess, validAnswers, minCount, 6 - entries.Count);

    //        Console.WriteLine($"{nextGuess} {maxCount} {minCount}");

    //        if (maxCount == 0)
    //        {
    //            continue;
    //        }

    //        if (maxCount < minCount)
    //        {
    //            minCount = maxCount;
    //            equals.Clear();
    //            equals.Add(nextGuess);
    //        }
    //        else if (maxCount == minCount)
    //        {
    //            equals.Add(nextGuess);
    //        }
    //    }

    //    if (validAnswers.Length == 1)
    //    {
    //        if (!equals.Contains(validAnswers[0]))
    //            throw new InvalidOperationException("Impossible not to have only answer");
    //    }

    //    var decidedGuess = PickBest(equals, validAnswers);
    //    return decidedGuess;
    //}

    //private int FindMaxCountWithLimit(List<Entry> entries, string nextGuess, string[] validAnswers, int minCount, int limit)
    //{
    //    int maxCount = 0;

    //    // if nextGuess, assume answer, given results
    //    foreach (var possibleAnswer in validAnswers)
    //    {
    //        var nextEntries = new List<Entry>(entries)
    //        {
    //            _judge.MakeGuess(nextGuess, possibleAnswer)
    //        };
    //        var nextAnswers = _words.GetValidAnswers(nextEntries, validAnswers);
    //        int nextCount = nextAnswers.Count;
    //        if (nextCount == 0)
    //        {
    //            continue;
    //        }

    //        if (nextCount + 20 < maxCount)
    //            continue;

    //        if (Unresolved(6 - nextEntries.Count, nextAnswers.ToArray(), validAnswers))
    //        {
    //            nextCount += 20;
    //        }

    //        if (nextCount > maxCount)
    //        {
    //            maxCount = nextCount;
    //        }
    //    }

    //    return maxCount;
    //}

    //private bool Unresolved(int p, string[] nextAnswers, string[] validAnswers)
    //{
    //    for (int pattern = 0; pattern < 5; pattern++)
    //    {
    //        var states = new State[5];
    //        for (int i = 0; i < 5; ++i)
    //            states[i] = (i == pattern) ? State.Gray : State.Green;

    //        var g1 = _wordGrouper.GroupBySameState(nextAnswers, states);
    //        var g2 = _wordGrouper.GroupBySameState(validAnswers, states);
    //        foreach (var g in g1)
    //        {
    //            if (g.Value.Count > p && g2[g.Key].Count == g.Value.Count)
    //                return true;
    //        }
    //    }

    //    return false;
    //}

    public string PickBest(List<string> equals, string[] validAnswers)
    {
        if (equals.Count == 0)
        {
            throw new InvalidOperationException();
        }

        if (equals.Count == 1)
        {
            return equals[0];
        }

        if (validAnswers.Length == 1)
        {
            return validAnswers[0];
        }

        string w = "";
        int score = MinScore;
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
