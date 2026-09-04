namespace WAF.Engine;

using WAF.Models;
using WAF.Rules;

public class WafEngine
{
    private readonly SQLiRule _sqliRule;

    public WafEngine(SQLiRule sqliRule)
    {
        _sqliRule = sqliRule;
    }

    public bool ProcessRequest(WafRequest request)
    {
        return HasSqlInjectionAttempt(request);
    }

    public bool HasSqlInjectionAttempt(WafRequest request)
    {
        if (request is null)
        {
            return false;
        }

        var combinedInput = string.Join(" ",
            request.Body ?? string.Empty,
            request.QueryString ?? string.Empty,
            request.Path ?? string.Empty,
            request.Method ?? string.Empty);

        return _sqliRule.IsSqlInjectionAttempt(combinedInput);
    }
}