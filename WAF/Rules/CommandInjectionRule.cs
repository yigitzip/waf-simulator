using System.Net;
using System.Text.RegularExpressions;

namespace WAF.Rules;

public class CommandInjectionRule
{
    private static readonly Regex[] Patterns =
    {
        new(@"(?:^|[\s""'=:{},;&|])(?:bash|sh|zsh|cmd(?:\.exe)?|powershell(?:\.exe)?|pwsh|python(?:3)?|perl|ruby|php)\s+\S+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"(?:&&|\|\||;|\|)\s*(?:whoami|id|uname|hostname|ipconfig|ifconfig|netstat|nslookup|curl|wget|nc|netcat|cat|type|dir|ls|rm|del|chmod|chown)\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new(@"(?:&&|\|\||;|\|)\s*[A-Za-z0-9_$./\\-]+", RegexOptions.Compiled),
        new(@"`[^`]+`|\$\([^)]*\)", RegexOptions.Compiled),
        new(@"(?:\r?\n|\r)\s*(?:bash|sh|cmd|powershell|pwsh|curl|wget|nc|cat|type|dir|ls|rm|del)\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase)
    };

    public bool IsCommandInjectionAttempt(string? input)
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

        return Patterns.Any(pattern => pattern.IsMatch(normalizedInput));
    }
}
