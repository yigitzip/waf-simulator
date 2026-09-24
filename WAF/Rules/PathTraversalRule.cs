using System.Net;
using System.Text.RegularExpressions;

namespace WAF.Rules;

public class PathTraversalRule
{
    private static readonly Regex ParentDirectorySegment =
        new(@"(?:^|/)\.\.(?:/|$)", RegexOptions.Compiled);

    public bool IsPathTraversalAttempt(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var normalizedInput = input;

        for (var i = 0; i < 3; i++)
        {
            var decodedInput = WebUtility.UrlDecode(normalizedInput);
            if (decodedInput == normalizedInput)
            {
                break;
            }

            normalizedInput = decodedInput;
        }

        normalizedInput = normalizedInput.Replace('\\', '/');
        return ParentDirectorySegment.IsMatch(normalizedInput);
    }
}
