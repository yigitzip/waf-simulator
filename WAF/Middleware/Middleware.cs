using System.Text;
using WAF.Engine;
using WAF.Models;

public class WafMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WafEngine _wafEngine;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WafMiddleware> _logger;

    public WafMiddleware(
        RequestDelegate next,
        WafEngine wafEngine,
        IHttpClientFactory httpClientFactory,
        ILogger<WafMiddleware> logger)
    {
        _next = next;
        _wafEngine = wafEngine;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // enable buffering to allow reading the request body multiple times
        context.Request.EnableBuffering();

        // read the request body as a string
        string requestBody = "";
        if (context.Request.Body.CanRead &&
            (context.Request.ContentLength is null || context.Request.ContentLength > 0))
        {
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // reset the request body stream position to allow further processing
            context.Request.Body.Position = 0;
        }

        // WafRequest
        var wafRequest = new WafRequest
        {
            Method = context.Request.Method,
            Path = context.Request.Path.Value ?? string.Empty,
            QueryString = context.Request.QueryString.Value ?? string.Empty,
            Body = requestBody,
            Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        };

        // WAF Engine
        var sqlInjectionDetected = _wafEngine.HasSqlInjectionAttempt(wafRequest);
        var xssDetected = _wafEngine.HasXssAttempt(wafRequest);
        var pathTraversalDetected = _wafEngine.HasPathTraversalAttempt(wafRequest);

        if (sqlInjectionDetected || xssDetected || pathTraversalDetected)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var blockedResponse = new
            {
                message = "Request blocked by WAF",
                sqlInjectionDetected = sqlInjectionDetected,
                xssDetected = xssDetected,
                pathTraversalDetected = pathTraversalDetected,
            };

            var blockedJson = System.Text.Json.JsonSerializer.Serialize(blockedResponse);
            await context.Response.WriteAsync(blockedJson);
            return;
        }

        // Forward to the backend

        var client = _httpClientFactory.CreateClient();

        var targetUrl = "http://localhost:8080" +
        context.Request.Path +
        context.Request.QueryString;

        _logger.LogInformation(
            "WAF proxy request: {Method} {IncomingPath}{IncomingQueryString} -> {TargetUrl}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            targetUrl);

        var requestMessage = new HttpRequestMessage {
            Method = new HttpMethod(context.Request.Method),
            RequestUri = new Uri(targetUrl),
        };

        if (!string.IsNullOrEmpty(requestBody)) {

            requestMessage.Content = new StringContent(
                requestBody,
                Encoding.UTF8,
                context.Request.ContentType ?? "application/json"
            );
        }

        //request headers
        var excludedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Host",
            "Content-Length",
            "Content-Type",
            "Connection",
            "Keep-Alive",
            "Proxy-Authenticate",
            "Proxy-Authorization",
            "TE",
            "Trailer",
            "Transfer-Encoding",
            "Upgrade"
        };

        foreach (var header in context.Request.Headers)
        {
            if (excludedHeaders.Contains(header.Key))
            {
                continue;
            }

            if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        // send the request to the backend
        var responseMessage = await client.SendAsync(requestMessage);

        _logger.LogInformation(
            "WAF proxy response: {StatusCode} from {TargetUrl}",
            (int)responseMessage.StatusCode,
            targetUrl);

        //response status code
        context.Response.StatusCode = (int)responseMessage.StatusCode;

        //response headers
        var excludedResponseHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Transfer-Encoding",
            "Connection",
            "Keep-Alive",
            "Proxy-Authenticate",
            "Proxy-Authorization",
            "TE",
            "Trailer",
            "Upgrade"
        };

        foreach (var header in responseMessage.Headers)
        {
            if (!excludedResponseHeaders.Contains(header.Key))
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }

        foreach (var header in responseMessage.Content.Headers)
        {
            if (!excludedResponseHeaders.Contains(header.Key))
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }

        //response body
        var responseBody = await responseMessage.Content.ReadAsByteArrayAsync();

        await context.Response.Body.WriteAsync(responseBody);

    }
}