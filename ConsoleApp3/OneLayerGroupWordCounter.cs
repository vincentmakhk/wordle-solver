namespace ConsoleApp3;

public class OneLayerGroupWordCounter : IWordCounter
{
    private readonly Words _words;
    private readonly Judge _judge;
    private readonly WordGrouper _wordGrouper;

    public OneLayerGroupWordCounter(Words words, Judge judge)
    {
        _words = words;
        _judge = judge;
        _wordGrouper = new WordGrouper(words, judge);
    }

    public int FindMaxCount(string nextGuess, string[] validAnswers, int minCount)
    {
        var groups = _wordGrouper.GroupByState(nextGuess, validAnswers.ToList());
        return groups.Max(x => x.Value.Item2.Count);
    }
}
