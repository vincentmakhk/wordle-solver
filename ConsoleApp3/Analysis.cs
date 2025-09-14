using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp3;

public class Analysis
{
    public const string TRIED_FILE_PATH = "./text/tried.txt";
    public const string CANNOT_FILE_PATH = "./text/cannot.txt";
    private readonly Judge _judge;
    private readonly Words _words;
    private readonly NextWordFinder _nextWordFinder;
    private Dictionary<string, string> _cacheBestWord = new();

    public Analysis(Judge judge, Words words)
    {
        _judge = judge;
        _words = words;
        _nextWordFinder = new NextWordFinder(words, judge);
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

            int maxCount = _nextWordFinder.FindMaxCount(_first, _words.AllAnswers, minCount);

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

            var g = _nextWordFinder.WordGrouper(_words.AllAnswers, states);
            foreach (var pair in g)
                groups.Add(pair.Key, pair.Value);
        }

        return groups;
    }

    public int FindMaxDistinct(string pattern, List<string> values)
    {
        var strip = values.Select(x => WordUtil.Strip(pattern, x)).ToList();

        int max = 1;
        var queue = new Queue<(List<string>, string, int)>();
        for (int i = 0; i < strip.Count; ++i)
        {
            var words = new List<string> { strip[i] };
            queue.Enqueue((words, strip[i], i));
        }

        while (queue.Any())
        {
            var first = queue.Dequeue();
            for (int i = first.Item3 + 1; i < strip.Count; ++i)
            {
                if (!first.Item2.Intersect(strip[i]).Any())
                {
                    var l = new List<string>(first.Item1)
                    {
                        strip[i]
                    };

                    var s = string.Concat(first.Item2.Union(strip[i]));

                    var count = l.Count;
                    if (count > max)
                    {
                        max = count;
                    }

                    queue.Enqueue((l, s, i));
                }
            }
        }

        return max;
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
            var maxDistinct = FindMaxDistinct(group.Key, group.Value);

            if (maxDistinct > 6)
            {
                foreach (var w in group.Value)
                {
                    cannot.Add(w);
                }
            }

            if (maxDistinct >= 6)
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

    private string FindNextGuess(List<Entry> entries)
    {
        var cacheKey = string.Join(',', entries.Select(e => $"{e.Guess} {e.States[0]} {e.States[1]} {e.States[2]} {e.States[3]} {e.States[4]}"));
        if (_cacheBestWord.TryGetValue(cacheKey, out var cachedValue))
            return cachedValue;

        var validAnswers = _words.GetValidAnswers(entries, _words.AllAnswers).ToArray();

        var decidedGuess = _nextWordFinder.Find(entries, validAnswers);
        //var decidedGuess = validAnswers.Length > 200 ?
        //    _nextWordFinder.Find(entries, validAnswers) :
        //    _nextWordFinder.ExhaustiveSearch(entries, validAnswers);

        _cacheBestWord[cacheKey] = decidedGuess;
        return decidedGuess;
    }

    public int Play(string firstGuess, string answer)
    {
        var entry = _judge.MakeGuess(firstGuess, answer);
        var entries = new List<Entry>();
        Console.WriteLine($"Guess 1: {firstGuess} on {answer}");
        int numberOfGuesses = 1;

        while (!_judge.HasWon(entry))
        {
            entries.Add(entry);
            var decidedGuess = FindNextGuess(entries);
            Console.WriteLine($"Guess {numberOfGuesses+1}: {decidedGuess}");
            entry = _judge.MakeGuess(decidedGuess, answer);
            ++numberOfGuesses;
        }

        return numberOfGuesses;
    }

    public int WorstGames(string firstGuess)
    {
        var worstWords = FindPatterns()
            .Where(p => p.Value.Where(v => _words.AllAnswers.Contains(v)).Count() >= 5)
            .SelectMany(p => p.Value)
            .ToList();
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
