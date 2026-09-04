using System.Text.RegularExpressions;

namespace WAF.Rules;

public class SQLiRule
{
    private static readonly Regex[] Patterns =
    {
        new(@"(?i)(?:union\s+select|select\s+.*\s+from|insert\s+into|update\s+.*\s+set|delete\s+from)", RegexOptions.Compiled),
        new(@"(?i)(?:or\s+\d+\s*=\s*\d+|or\s+1\s*=\s*1|--|#)", RegexOptions.Compiled),
        new(@"(?i)(?:'\s*or\s*'\d+'\s*=\s*'\d+'|""\s*or\s*""\d+""\s*=\s*""\d+"")", RegexOptions.Compiled),
        new(@"(?i)(?:sleep\s*\(|benchmark\s*\(|waitfor\s+delay)", RegexOptions.Compiled),
        new(@"(?i)(?:;\s*(?:drop|alter|truncate|exec|declare))", RegexOptions.Compiled)
    };

    public bool IsSqlInjectionAttempt(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        foreach (var pattern in Patterns)
        {
            if (pattern.IsMatch(input))
            {
                return true;
            }
        }

        return false;
    }
}
