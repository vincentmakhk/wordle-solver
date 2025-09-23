using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3;

public class MaxWordCounter : IWordCounter
{
    private readonly Words _words;
    private readonly Judge _judge;

    public MaxWordCounter(Words words, Judge judge)
    {
        _words = words;
        _judge = judge;
    }

    public int FindMaxCount(string nextGuess, string[] validAnswers, int minCount)
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

}
