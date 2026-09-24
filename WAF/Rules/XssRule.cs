using System.Net;
using System.Text.RegularExpressions;

namespace WAF.Rules;

public class XssRule
{
    private static readonly Regex[] Patterns =
    {
        new(@"<\s*script\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"</\s*script\s*>", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bjavascript\s*:", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"\bon(?:error|load|click|mouseover|focus|input|submit)\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"<\s*(?:iframe|object|embed|svg)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)
    };

    public bool IsXssAttempt(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var normalizedInput = WebUtility.HtmlDecode(input);
        normalizedInput = Uri.UnescapeDataString(normalizedInput);

        return Patterns.Any(pattern => pattern.IsMatch(normalizedInput));
    }
}
