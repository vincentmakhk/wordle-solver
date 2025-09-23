namespace ConsoleApp3;

public class MinMaxFinder
{
    public static List<string> GetBestWords(List<string> validWords, IWordCounter counter, string[] answers)
    {
        int minCount = Words.MaxValue;
        var equals = new List<string>();
        foreach (var word in validWords)
        {
            int maxCount = counter.FindMaxCount(word, answers, minCount + 20);

            //Console.WriteLine($"{_first} {maxCount} {minCount}");

            if (maxCount == 0)
            {
                continue;
            }

            if (maxCount < minCount)
            {
                minCount = maxCount;
                equals.Clear();
                equals.Add(word);
                //Console.WriteLine($"Set {_first} with count {maxCount}");
            }
            else if (maxCount == minCount)
            {
                equals.Add(word);
                //Console.WriteLine($"Add {_first} with equal count {minCount}");
            }
        }

        return equals;
    }
}
