// See https://aka.ms/new-console-template for more information
using System;
using System.Text.RegularExpressions;
using ConsoleApp3;

Console.WriteLine("Hello, World!");




var judge = new Judge();
var words = new Words();
//var answer = words.RandomizeAnswer();
var answer = "nadir";
var firstGuess = "olate";
//var game = new Game(answer);
var entry = judge.MakeGuess(firstGuess, answer);
var entries = new List<Entry>();
Console.WriteLine($"{firstGuess} on {answer}");
/*
{
    HashSet<string> skipped = new HashSet<string>();

    for (int i = 0; i < words.AllValidWords.Length - 1; i++)
    {
        if (words.AllValidWords[i] == "aesir")
            continue;

        if (words.AllValidWords[i].CompareTo("daaaa") <= 0)
        {
            if (words.AllValidWords[i].CompareTo(words.AllValidWords[i + 1]) >= 0)
                throw new InvalidOperationException();

            skipped.Add(words.AllValidWords[i]);
        }
        else
            break;
    }
    int minCount = int.MaxValue;
    HashSet<string> equals = new HashSet<string>();
    foreach (var _first in words.AllValidWords)
    {
        if (skipped.Contains(_first))
        {
            continue;
        }

        int maxCount = 0;

        // if nextGuess, assume answer, given results
        foreach (var possibleAnswer in words.AllAnswers)
        {
            var nextEntry = judge.MakeGuess(_first, possibleAnswer);
            //entries.Add(nextEntry);
            var nextEntries = new List<Entry> { nextEntry };
            var nextCount = words.CountValidAnswers(nextEntries, words.AllAnswers, minCount);
            //var nextValidAnswers = words.GetValidAnswers(nextEntries, words.AllAnswers);
            //int nextCount = nextValidAnswers.Count;
            if (nextCount > maxCount)
            {
                maxCount = nextCount;
            }
            //entries.RemoveAt(entries.Count - 1);
        }

        Console.WriteLine($"{_first} {maxCount} {minCount}");

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
    }

    if (minCount > 0)
        return 0;
}
*/

while (!judge.HasWon(entry))
{
    entries.Add(entry);
    var validGuess = words.GetValidWords(entries);
    var validAnswers = words.GetValidAnswers(entries, words.AllAnswers).ToArray();
    Console.WriteLine($"{validGuess.Count} guess {validAnswers.Length} answers");
    int minCount = int.MaxValue;
    HashSet<string> equals = new HashSet<string>();
    foreach (var nextGuess in validGuess)
    {
        int maxCount = 0;

        // if nextGuess, assume answer, given results
        foreach (var possibleAnswer in validAnswers)
        {
            var nextEntry = judge.MakeGuess(nextGuess, possibleAnswer);
            //entries.Add(nextEntry);
            var nextEntries = new List<Entry> { nextEntry };
            var nextValidAnswers = words.GetValidAnswers(nextEntries, validAnswers);
            int nextCount = nextValidAnswers.Count;
            if (nextCount > maxCount)
            {
                maxCount = nextCount;
            }
            //entries.RemoveAt(entries.Count - 1);
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

    var decidedGuess = words.PickBest(equals);
    Console.WriteLine($"{decidedGuess} on {answer}");
    entry = judge.MakeGuess(decidedGuess, answer);
}

return 0;

var groups = new Dictionary<string, List<string>>();

var cannot = new HashSet<string>();

for (int pattern = 0; pattern<5; pattern++)
{
    var states = new State[5];
    for (int i = 0; i < 5; ++i)
        states[i] = (i == pattern) ? State.Gray : State.Green;

    foreach (var word in words.AllAnswers)
    {
        var prefix = word.Remove(pattern, 1);
        prefix = prefix.Insert(pattern, "_");
        if (groups.ContainsKey(prefix))
            continue;

        var collection = new List<string>();
        collection.Add(word);

        foreach (var anotherWord in words.AllAnswers)
        {
            var res = judge.Check(word, anotherWord);
            if (judge.EqualState(states, res))
            {
                collection.Add(anotherWord);
            }
        }

        groups.Add(prefix, collection);
    }
}

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

        foreach (var w in words.AllValidWords)
        {
            var common = w.ToCharArray().Intersect(allLetters);
            if (!common.Any())
            {
                cannot.Add(w);
            }
        }
    }
}

var cannotAnswer = cannot.Intersect(words.AllAnswers).ToList();

foreach (var group1 in groups)
{
    foreach (var group2 in groups)
    {
        var common = group1.Value.Intersect(group2.Value);
        if (common.Count() < 2)
        {
            int l = 1;
        }
    }
}

var excluded = new int[] { 1,2,4,8,16, 3,5,9,17, 6,10,18, 12,20, 24 };
int count = 0;
var firstDegree = new HashSet<string>();
var secondDegree = new HashSet<string>();
var allAnswers = words.AllAnswers;

foreach (var _answer in allAnswers)
{
    for (int i=1; i<=31; ++i)
    {
        if (excluded.Contains(i))
            continue;

        if (FindOrthogonal(_answer, allAnswers, i))
        {
            Console.WriteLine(_answer);
            ++count;
            firstDegree.Add(_answer);
            break;
        }
    }
}

return 0;

bool FindOrthogonal(string answer, string[] allAnswers, int pattern)
{
    var matches = FilterMatch(answer, allAnswers, pattern);

    if (matches.Count < 6)
        return false;

    var queue = new List<string>();

    foreach (var word in matches)
    {
        var diff = Strip(word, pattern);
        bool isOrthogonal = true;
        foreach (var w in queue)
        {
            if (!IsOrthogonal(w, diff))
            {
                isOrthogonal = false;
                break;
            }
        }

        if (isOrthogonal)
        {
            queue.Add(diff);
        }
    }

    if (queue.Count > 6)
    {
        foreach (var word in allAnswers)
        {
            bool isOrthogonal = true;
            foreach (var m in matches)
            {
                if (!IsOrthogonal(m, word))
                {
                    isOrthogonal = false;
                    break;
                }
            }

            if (isOrthogonal)
            {
                secondDegree.Add(word);
            }
        }

        return true;
    }

    //if (queue.Count == 6 && matches.Count > 6)
    //{
    //    return true;
    //}

    return false;
}

bool IsOrthogonal(string w, string diff)
{
    return 0 == w.Intersect(diff).Count();
}

string Strip(string word, int pattern)
{
    string w = "";
    for (int j = 0, k = 1; k < 32; k *= 2, ++j)
    {
        if ((pattern & k) == 0) // not match
        {
            w += word[j];
        }
    }
    return w;
}

List<string> FilterMatch(string answer, string[] allAnswers, int pattern)
{
    var match = new List<string>();
    foreach(var word in allAnswers)
    {
        bool found = true;
        for (int j = 0, k = 1; k < 32; k*=2, ++j)
        {
            if ((pattern & k) == 0) // not match
            {
                if (answer[j] == word[j])
                {
                    found = false;
                    break;
                }
            }
            else
            {
                if (answer[j] != word[j])
                {
                    found = false;
                    break;
                }
            }
        }

        if (found)
            match.Add(word);
    }
    return match;
}
