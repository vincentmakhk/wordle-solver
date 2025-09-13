using System.Text.RegularExpressions;

namespace ConsoleApp3;

public class Analysis
{
    public const string TRIED_FILE_PATH = "./text/tried.txt";
    public const string CANNOT_FILE_PATH = "./text/cannot.txt";
    private readonly Judge _judge;
    private readonly Words _words;

    public Analysis(Judge judge, Words words)
    {
        _judge = judge;
        _words = words;
    }

    public void FindBestFirstWord()
    {
        int minCount = int.MaxValue;
        HashSet<string> skipped = new();
        HashSet<string> equals = new();

        if (File.Exists(TRIED_FILE_PATH))
        {
            var lines = File.ReadAllLines(TRIED_FILE_PATH);
            foreach (var line in lines)
            {
                var split = line.Split('\t');
                var word = split[0];
                var maxCount = int.Parse(split[1]);
                skipped.Add(word);
                if (maxCount < minCount)
                {
                    minCount = maxCount;
                    equals.Clear();
                    equals.Add(word);
                    Console.WriteLine($"Set {word} with count {maxCount}");
                }
                else if (maxCount == minCount)
                {
                    equals.Add(word);
                    Console.WriteLine($"Add {word} with equal count {maxCount}");
                }
            }
        }

        foreach (var _first in _words.AllValidWords)
        {
            if (skipped.Contains(_first))
            {
                continue;
            }

            int maxCount = FindMaxCount(_first, _words.AllAnswers, minCount);

            Console.WriteLine($"{_first} {maxCount} {minCount}");

            if (maxCount == 0)
            {
                continue;
            }

            if (maxCount < minCount)
            {
                minCount = maxCount;
                equals.Clear();
                equals.Add(_first);
                Console.WriteLine($"Set {_first} with count {maxCount}");
            }
            else if (maxCount == minCount)
            {
                equals.Add(_first);
                Console.WriteLine($"Add {_first} with equal count {minCount}");
            }

            File.AppendAllLines(TRIED_FILE_PATH, new string[] { $"{_first}\t{maxCount}" });
        }

        Console.WriteLine($"Best first word(s): {string.Join(',', equals)}");
    }

    public Dictionary<string, List<string>> FindPatterns()
    {
        var groups = new Dictionary<string, List<string>>();

        for (int pattern = 0; pattern < 5; pattern++)
        {
            var states = new State[5];
            for (int i = 0; i < 5; ++i)
                states[i] = (i == pattern) ? State.Gray : State.Green;

            foreach (var word in _words.AllAnswers)
            {
                var prefix = word.Remove(pattern, 1);
                prefix = prefix.Insert(pattern, "_");
                if (groups.ContainsKey(prefix))
                    continue;

                var collection = new List<string>();
                collection.Add(word);

                foreach (var anotherWord in _words.AllAnswers)
                {
                    var res = _judge.Check(word, anotherWord);
                    if (_judge.EqualState(states, res))
                    {
                        collection.Add(anotherWord);
                    }
                }

                groups.Add(prefix, collection);
            }
        }

        return groups;
    }

    public void FindNotFirstWords()
    {
        if (File.Exists(CANNOT_FILE_PATH))
        {
            return;
        }

        var groups = FindPatterns();

        var cannot = new HashSet<string>();

        foreach (var group in groups)
        {
            if (group.Value.Count > 6)
            {
                foreach (var w in group.Value)
                {
                    cannot.Add(w);
                }
            }

            if (group.Value.Count >= 6)
            {
                var allLetters = group.Value.SelectMany(x => x.ToCharArray());

                foreach (var w in _words.AllValidWords)
                {
                    var common = w.ToCharArray().Intersect(allLetters);
                    if (!common.Any())
                    {
                        cannot.Add(w);
                    }
                }
            }
        }

        var cannotAnswer = cannot.Intersect(_words.AllAnswers).ToList();

        if (!File.Exists(CANNOT_FILE_PATH))
            File.WriteAllLines(CANNOT_FILE_PATH, cannot.ToArray());
    }

    private int FindMaxCount(string nextGuess, string[] validAnswers, int minCount)
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

    public int Play(string firstGuess, string answer)
    {
        var entry = _judge.MakeGuess(firstGuess, answer);
        var entries = new List<Entry>();
        Console.WriteLine($"Guess 1: {firstGuess}");
        int numberOfGuesses = 1;

        while (!_judge.HasWon(entry))
        {
            entries.Add(entry);
            var validGuess = _words.GetValidWords(entries);
            var validAnswers = _words.GetValidAnswers(entries, _words.AllAnswers).ToArray();
            Console.WriteLine($"Remains {validGuess.Count} guesses {validAnswers.Length} answers");
            int minCount = int.MaxValue;
            HashSet<string> equals = new HashSet<string>();
            foreach (var nextGuess in validGuess)
            {
                int maxCount = FindMaxCount(nextGuess, validAnswers, minCount);

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
            var decidedGuess = _words.PickBest(equals, validAnswers);
            Console.WriteLine($"Guess {numberOfGuesses+1}: {decidedGuess}");
            entry = _judge.MakeGuess(decidedGuess, answer);
            ++numberOfGuesses;
        }

        return numberOfGuesses;
    }

    public int WorstGames(string firstGuess)
    {
        var worstWords = FindPatterns().Where(g => g.Value.Count >= 6).SelectMany(x => x.Value).ToArray();

        int max = 0;
        HashSet<string> equals = new();
        foreach (var word in worstWords)
        {
            int count = Play(firstGuess, word);
            if (count > max)
            {
                max = count;
                equals.Clear();
                equals.Add(word);
            }
            else if (count == max)
            {
                equals.Add(word);
            }
        }

        return max;
    }
}
