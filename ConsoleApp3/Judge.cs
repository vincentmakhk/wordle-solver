namespace ConsoleApp3;

public class Judge
{
    public State[] Check(string answer, string guess)
    {
        var states = new State[5];
        var answerUsed = new bool[5];
        var guessUsed = new bool[5];

        for (int i=0; i<5; ++i)
        {
            if (answer[i] == guess[i])
            {
                answerUsed[i] = true;
                guessUsed[i] = true;
                states[i] = State.Green;
            }
        }

        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5 && !answerUsed[i]; ++j)
            {
                if (guessUsed[j])
                    continue;

                if (answer[i] != guess[j])
                    continue;

                answerUsed[i] = true;
                guessUsed[j] = true;
                states[j] = State.Yellow;
            }
        }

        return states;
    }

    public bool Fulfill(List<Entry> entries, string word, bool grayCheck)
    {
        foreach (var entry in entries)
        {
            if (entry.Guess == word)
            {
                if (HasWon(entry))
                    throw new InvalidOperationException();

                return false;
            }

            if (!Fulfill(entry, word, false, grayCheck))
            {
                return false;
            }
        }
        return true;
    }

    public bool Fulfill(Entry entry, string word, bool asAnswer = false, bool grayCheck = true)
    {
        var match = new bool[5];
        for (int i = 0; i < 5; ++i)
        {
            if (asAnswer)
            {
                if (entry.States[i] != State.Green && word[i] == entry.Guess[i])
                {
                    return false;
                }
            }

            if (entry.States[i] == State.Green)
            {
                if (entry.Guess[i] == word[i])
                {
                    match[i] = true;
                }
                else
                {
                    return false;
                }
            }
        }

        for (int i = 0; i < 5; ++i)
        {
            if (entry.States[i] == State.Yellow)
            {
                bool found = false;
                for (int j = 0; j < 5; ++j)
                {
                    if (entry.Guess[i] == word[j] && !match[j])
                    {
                        match[j] = true;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
        }

        if (grayCheck || asAnswer)
        {
            for (int i = 0; i < 5; ++i)
            {
                if (entry.States[i] == State.Gray)
                {
                    for (int j = 0; j < 5; ++j)
                    {
                        if (entry.Guess[i] == word[j] && !match[j])
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    public Entry MakeGuess(string guess, string answer)
    {
        return new Entry(
            guess,
            Check(answer, guess)
        );
    }

    public bool HasWon(Entry entry)
    {
        return entry.States.All(x => x == State.Green);
    }

    internal bool FulfillAsAnswer(List<Entry> entries, string word)
    {
        foreach (var entry in entries)
        {
            if (entry.Guess == word)
            {
                if (!HasWon(entry))
                    return false;
            }

            if (!FulfillAsAnswer(entry, word))
            {
                return false;
            }
        }
        return true;
    }

    public bool FulfillAsAnswer(Entry entry, string word)
    {
        return Fulfill(entry, word, true, true);
    }

    public bool IsLegitimateGuess(Entry entry, string guess)
    {
        bool[] match = new bool[5];
        for (int i = 0; i < 5; ++i)
        {
            if (entry.States[i] == State.Green)
            {
                if (entry.Guess[i] != guess[i])
                    return false;

                match[i] = true;
            }
        }

        for (int i = 0; i < 5; ++i)
        {
            if (entry.States[i] == State.Yellow)
            {
                bool found = false;
                for (int j = 0; j < 5; ++j)
                {
                    if (!match[j] && entry.Guess[i] == guess[j])
                    {
                        match[j] = true;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return false;
            }
        }

        return true;
    }
}
