namespace ConsoleApp3;

public class Analysis
{
    public const string TRIED_FILE_PATH = "./text/tried.txt";
    public const string CANNOT_FILE_PATH = "./text/cannot.txt";
    private readonly Judge _judge;
    private readonly Words _words;
    private readonly NextWordFinder _nextWordFinder;
    private readonly IWordFinder _wordFinder;
    private readonly WordGrouper _wordGrouper;

    public Analysis(Judge judge, Words words)
    {
        _judge = judge;
        _words = words;
        _nextWordFinder = new NextWordFinder(words, judge);
        _wordFinder = new OneLayerWordFinder(words, judge, _nextWordFinder);
        _wordGrouper = new WordGrouper(words, judge);
    }

    public void FindBestFirstWord()
    {
        int minCount = Words.MaxValue;
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

        IWordCounter counter = new GroupWordCounter(_words, _judge);

        foreach (var _first in _words.AllValidWords)
        {
            if (skipped.Contains(_first))
            {
                continue;
            }

            int maxCount = counter.FindMaxCount(_first, _words.AllAnswers, minCount + 50);

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

        var groups = _wordGrouper.FindPatterns(_words.AllAnswers);

        var cannot = new HashSet<string>();

        foreach (var group in groups)
        {
            var maxDistinct = FindMaxDistinct(group.Key, group.Value);

            if (maxDistinct > 6)
            {
                foreach (var w in _words.AllValidWords)
                {
                    if (StateUtil.IsSamePattern(group.Key, w))
                    {
                        cannot.Add(w);
                    }
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

    public int Play(string firstGuess, string answer)
    {
        var entry = _judge.MakeGuess(firstGuess, answer);
        var entries = new List<Entry>();
        Console.WriteLine($"Guess 1: {firstGuess} on {answer}");
        int numberOfGuesses = 1;

        while (!_judge.HasWon(entry))
        {
            entries.Add(entry);
            var decidedGuess = _wordFinder.FindNextGuess(entries);
            Console.WriteLine($"Guess {numberOfGuesses+1}: {decidedGuess}");
            entry = _judge.MakeGuess(decidedGuess, answer);
            ++numberOfGuesses;
        }

        return numberOfGuesses;
    }

    public int WorstGames(string firstGuess)
    {
        var worstWords = _wordGrouper.FindPatterns(_words.AllAnswers)
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

    public int WorstGames2(string firstGuess)
    {
        var wordGrouper = new WordGrouper(_words, _judge);
        var groups = wordGrouper.GroupByState(firstGuess, _words.AllAnswers.ToList());
        var orderByDescending = groups.OrderByDescending(x => x.Value.Item2.Count).ToList();
        var cheatSheet = new Dictionary<string, string>(); // which level, which pattern, which word

        bool allSolved = true;

        foreach (var group in orderByDescending)
        {
            IWordCounter counter = new OneLayerGroupWordCounter(_words, _judge);
            var thisEntry = new Entry(firstGuess, group.Value.Item1);
            if (_judge.HasWon(thisEntry))
                continue;
            var entries = new List<Entry> { thisEntry };
            var validWords = _words.GetValidWords(entries, true);
            var equals = MinMaxFinder.GetBestWords(validWords, counter, group.Value.Item2.ToArray());

            //Console.WriteLine($"Best first word(s): {string.Join(',', equals)} {minCount} {group.Value.Item2.Count}");

            bool hasSolution = false;
            foreach (var x in equals)
            {
                if (AbleToSolve(entries, x, group.Value.Item2, 5, cheatSheet))
                {
                    hasSolution = true;
                    cheatSheet.Add($"5_{group.Key}", x);
                    Console.WriteLine($"Choose {x} for {group.Key} at 5");
                    break;
                }
            }

            if (!hasSolution)
            {
                allSolved = false;

                var wg = wordGrouper.FindPatterns(group.Value.Item2.ToArray());
                foreach (var g in wg)
                {
                    if (g.Value.Count >= 5)
                    {
                        Console.WriteLine($"Group: {string.Join(',', g.Value)}");
                    }
                }

                Console.WriteLine($"Cannot solve {group.Key} {group.Value.Item2.Count}");
            }
        }

        if (allSolved)
        {
            SolveWithCheatSheet(cheatSheet, firstGuess);
        }

        return 0;
    }

    private bool AbleToSolve(List<Entry> previousEntries, string guess, List<string> validAnswers, int remainingGuess, Dictionary<string, string> cheatSheet)
    {
        if (remainingGuess <= 0)
        {
            return false;
        }
        if (remainingGuess > validAnswers.Count)
        {
            return true;
        }

        var wordGrouper = new WordGrouper(_words, _judge);
        wordGrouper.VerifyGuess(previousEntries, guess, validAnswers);

        var groups = wordGrouper.GroupByState(guess, validAnswers);
        if (groups.Sum(x => x.Value.Item2.Count) != validAnswers.Count)
        {
            throw new InvalidOperationException();
        }

        foreach (var group in groups)
        {
            IWordCounter counter = new OneLayerGroupWordCounter(_words, _judge);
            var thisEntry = new Entry(guess, group.Value.Item1);
            if (_judge.HasWon(thisEntry))
                continue;

            var entries = new List<Entry>(previousEntries) { thisEntry };
            var validWords = _words.GetValidWords(entries, false);
            var equals = MinMaxFinder.GetBestWords(validWords, counter, group.Value.Item2.ToArray());

            //Console.WriteLine($"Best first word(s): {string.Join(',', equals)} {minCount} {group.Value.Item2.Count}");

            bool hasSolution = false;
            foreach (var x in equals)
            {
                if (AbleToSolve(entries, x, group.Value.Item2, remainingGuess - 1, cheatSheet))
                {
                    var answers = _words.GetValidAnswers(entries, _words.AllAnswers);
                    if (answers.Count != group.Value.Item2.Count)
                    {
                        throw new InvalidOperationException();
                    }

                    hasSolution = true;
                    var wordList = string.Join(',', entries.Select(x => x.Guess));
                    var key = $"{remainingGuess - 1}_{wordList}_{group.Key}";
                    cheatSheet.Add($"{remainingGuess - 1}_{wordList}_{group.Key}", x);
                    if (remainingGuess == 4)
                    {
                        Console.WriteLine($"Choose {x} for {group.Key} at {remainingGuess}");
                    }
                    break;
                }
            }

            if (!hasSolution)
            {
                return false;
            }
        }

        return true;
    }

    private void SolveWithCheatSheet(Dictionary<string, string> cheatSheet, string guess)
    {
        var wordList = new List<string> { guess };
        foreach (var word in _words.AllAnswers)
        {
            var entry = _judge.MakeGuess(guess, word);
            var entries = new List<Entry> { entry };
            SolveWithCheatSheet(cheatSheet, entries, wordList, word, 5);
        }
    }

    private void SolveWithCheatSheet(Dictionary<string, string> cheatSheet, List<Entry> entries, List<string> guess, string answer, int remainingGuess)
    {
        if (remainingGuess <= 0)
        {
            throw new InvalidOperationException();
        }

        var stateString = StateUtil.StatesToString(entries[entries.Count - 1].States);
        var wordList = string.Join(',', entries.Select(x => x.Guess));
        var key = remainingGuess == 5 ?
            $"{remainingGuess}_{stateString}" :
            $"{remainingGuess}_{wordList}_{stateString}";
        if (cheatSheet.TryGetValue(key, out var nextGuess))
        {
            var newEntry = _judge.MakeGuess(nextGuess, answer);
            var newEntries = new List<Entry>(entries) { newEntry };
            if (_judge.HasWon(newEntry))
            {
                var list = string.Join("->", newEntries.Select(x => x.Guess));
                Console.WriteLine($"L1: {list}");
                return;
            }
            var newGuess = new List<string>(guess) { nextGuess };
            SolveWithCheatSheet(cheatSheet, newEntries, newGuess, answer, remainingGuess - 1);
            return;
        }

        var validAnswers = _words.GetValidAnswers(entries, _words.AllAnswers);
        if (!validAnswers.Contains(answer))
        {
            throw new InvalidOperationException();
        }
        if (validAnswers.Count == 1)
        {
            var list = string.Join("->", entries.Select(x => x.Guess));
            Console.WriteLine($"L2: {list}->{validAnswers[0]}");
            return;
        }
        foreach (var word in validAnswers)
        {
            if (word == answer)
            {
                continue;
            }

            var newEntries = new List<Entry>(entries) { _judge.MakeGuess(word, answer) };
            var newGuess = new List<string>(guess) { word };
            SolveWithCheatSheet(cheatSheet, newEntries, newGuess, answer, remainingGuess - 1);
        }
    }
}
