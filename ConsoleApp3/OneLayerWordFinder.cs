namespace ConsoleApp3;

public class OneLayerWordFinder : IWordFinder
{
    private readonly Words _words;
    private readonly Judge _judge;
    private readonly NextWordFinder _nextWordFinder;
    private readonly Dictionary<string, string> _cacheBestWord = new();

    public OneLayerWordFinder(Words words, Judge judge, NextWordFinder nextWordFinder)
    {
        _words = words;
        _judge = judge;
        _nextWordFinder = nextWordFinder;
    }

    public string FindNextGuess(List<Entry> entries)
    {
        var cacheKey = string.Join(',', entries.Select(e => $"{e.Guess} {StateUtil.StatesToString(e.States)}"));
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
}
