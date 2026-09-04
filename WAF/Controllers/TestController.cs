using Microsoft.AspNetCore.Mvc;

namespace WAF.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpPost("echo")]
    public IActionResult Echo([FromBody] object data)
    {
        return Ok(new { message = "You reached the echo endpoint", data = data });
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "Test endpoint working" });
    }
}