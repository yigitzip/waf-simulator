using System.Text;
using WAF.Engine;
using WAF.Models;

public class WafMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WafEngine _wafEngine;

    public WafMiddleware(RequestDelegate next, WafEngine wafEngine)
    {
        _next = next;
        _wafEngine = wafEngine;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Request body'sini okuyabilmek için buffer'ı enable et
        context.Request.EnableBuffering();

        // Request body'sini oku
        string requestBody = "";
        if (context.Request.ContentLength > 0)
        {
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Stream'i başa sar ki sonraki middleware'ler okuyabilsin
            context.Request.Body.Position = 0;
        }

        // WafRequest oluştur
        var wafRequest = new WafRequest
        {
            Method = context.Request.Method,
            Path = context.Request.Path.Value ?? string.Empty,
            QueryString = context.Request.QueryString.Value ?? string.Empty,
            Body = requestBody,
            Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        };

        // WAF Engine'e yönlendir
        var sqlInjectionDetected = _wafEngine.ProcessRequest(wafRequest);

        if (sqlInjectionDetected)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var blockedResponse = new
            {
                message = "Request blocked by WAF",
                sqlInjectionDetected = true,
                capturedRequest = new
                {
                    method = wafRequest.Method,
                    path = wafRequest.Path,
                    queryString = wafRequest.QueryString,
                    body = wafRequest.Body
                }
            };

            var blockedJson = System.Text.Json.JsonSerializer.Serialize(blockedResponse);
            await context.Response.WriteAsync(blockedJson);
            return;
        }

        // Original response stream'i sakla
        var originalBodyStream = context.Response.Body;

        // Yeni bir memory stream oluştur
        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            // Devam et
            await _next(context);

            // Response'u oku
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string originalResponse = await new StreamReader(context.Response.Body).ReadToEndAsync();

            // İstek içeriğini ve engine işaretini response'a ekle
            var responseWithRequest = new
            {
                originalResponse = originalResponse,
                sqlInjectionDetected = sqlInjectionDetected,
                engineProcessed = "Request checked by WAF Engine",
                capturedRequest = new
                {
                    method = wafRequest.Method,
                    path = wafRequest.Path,
                    queryString = wafRequest.QueryString,
                    body = wafRequest.Body,
                    headers = wafRequest.Headers
                }
            };

            // Response body'sini temizle ve yeni veriyi yaz
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            context.Response.ContentType = "application/json";
            
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseWithRequest);
            var bytes = Encoding.UTF8.GetBytes(jsonResponse);
            
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

            // Original stream'e kopyala
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
        }
    }
}