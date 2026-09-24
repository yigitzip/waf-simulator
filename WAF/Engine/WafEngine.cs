namespace WAF.Engine;

using WAF.Models;
using WAF.Rules;

public class WafEngine
{
    private readonly SQLiRule _sqliRule;
    private readonly XssRule _xssRule;
    private readonly PathTraversalRule _pathTraversalRule;

    public WafEngine(
        SQLiRule sqliRule,
        XssRule xssRule,
        PathTraversalRule pathTraversalRule)
    {
        _sqliRule = sqliRule;
        _xssRule = xssRule;
        _pathTraversalRule = pathTraversalRule;
    }

    public bool ProcessRequest(WafRequest request)
    {
        return HasSqlInjectionAttempt(request)
            || HasXssAttempt(request)
            || HasPathTraversalAttempt(request);
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

    public bool HasXssAttempt(WafRequest request)
    {
        if (request is null)
        {
            return false;
        }

        var combinedInput = string.Join(" ",
            request.Body ?? string.Empty,
            request.QueryString ?? string.Empty,
            request.Path ?? string.Empty);

        return _xssRule.IsXssAttempt(combinedInput);
    }

    public bool HasPathTraversalAttempt(WafRequest request)
    {
        if (request is null)
        {
            return false;
        }

        var combinedInput = string.Join(" ",
            request.Body ?? string.Empty,
            request.QueryString ?? string.Empty,
            request.Path ?? string.Empty);

        return _pathTraversalRule.IsPathTraversalAttempt(combinedInput);
    }
}