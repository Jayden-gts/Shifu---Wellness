using Microsoft.AspNetCore.Mvc;

namespace Shifu.Controllers;

[Route("api/quotes")]
[ApiController]
public class QuotesController : ControllerBase
{
    private static readonly string[] Quotes = new[]
    {
        "The only limit to our realization of tomorrow is our doubts of today.",
        "Do not wait to strike till the iron is hot; but make it hot by striking.",
        "Success is not final, failure is not fatal: It is the courage to continue that counts.",
        "Believe you can and you're halfway there."
    };

    [HttpGet("getquote")]
    public IActionResult GetQuote()
    {
        var random = new Random();
        var quote = Quotes[random.Next(Quotes.Length)];
        return Ok(new { text = quote });
    }
}
