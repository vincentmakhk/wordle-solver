using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3;

public class StateUtil
{
    public static bool IsSamePattern(State[] state, string pattern, string word)
    {
        for (int i = 0; i < 5; ++i)
        {
            if (state[i] == State.Green && pattern[i] != word[i])
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsSamePattern(string pattern, string word)
    {
        for (int i = 0; i < 5; ++i)
        {
            if (pattern[i] != '_' && pattern[i] != word[i])
            {
                return false;
            }
        }

        return true;
    }

    public static string StatesToString(State[] states)
    {
        return string.Join(',', states.Select(x => x.ToString()));
    }
}
