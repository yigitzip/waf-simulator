namespace WAF.Models;

public class WafRequest
{
    public string? Method { get; set; }
    public string? Path { get; set; }
    public string? QueryString { get; set; }
    public string? Body { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
}
