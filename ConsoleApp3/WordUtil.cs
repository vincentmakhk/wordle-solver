using System.Text;

namespace ConsoleApp3;

public class WordUtil
{
    public static string GeneratePrefix(string word, State[] states)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 5; ++i)
        {
            if (states[i] != State.Green)
                sb.Append('_');
            else
                sb.Append(word[i]);
        }
        return sb.ToString();
    }

    public static string Strip(string pattern, string word)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 5; ++i)
        {
            if (pattern[i] == '_')
            {
                sb.Append(word[i]);
            }
        }
        return sb.ToString();
    }
}
