using Microsoft.AspNetCore.Mvc;
using MoneyKa.Api.Services;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController(AIService ai, GroqService groq) : ControllerBase
{
    private const string SystemBase = "შენ ხარ MoneyKa-ს AI ფინანსური მრჩეველი. მომხმარებელი საქართველოდანაა და ლარში ხარჯავს. გასცე პრაქტიკული, პიროვნული, თბილი რჩევა ქართულ ენაზე. მოკლედ და ნათლად. გამოიყენე ემოჯი. პასუხი არ უნდა იყოს 5 წინადადებაზე მეტი.";

    [HttpPost("advice")]
    public async Task<IActionResult> GetAdvice([FromBody] AdviceRequest req)
    {
        var summary = string.Join(", ", req.ByCat.Select(c => $"{c.Label}: {c.Total}₾"));
        var system  = "You are a personal finance advisor. Always respond in Georgian language only. No markdown, no asterisks. Give exactly 3 tips, each on a new line starting with emoji. Be short, direct, reference actual numbers. Use informal შენ.";
        var user    = $"My spending this month: {summary}. Total spent: {req.TotalSpend}₾. Income: {req.Income}₾. Give me 3 concrete tips in Georgian.";

        var result = await groq.GenerateAsync(system, user);
        return Ok(new { text = result });
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest req)
    {
        var summary = string.Join(", ", req.ByCat.Select(c => $"{c.Label}: {c.Total}₾"));
        var system  = $"You are a personal finance advisor for a Georgian user. Always respond in Georgian language only. No markdown. Be short and practical. Context: spending {req.TotalSpend}₾, income {req.Income}₾, categories: {summary}. Use informal შენ.";

        // last user message
        var lastMsg = req.Messages.LastOrDefault(m => m.Role == "user")?.Content ?? "";
        var result  = await groq.GenerateAsync(system, lastMsg);
        return Ok(new { text = result });
    }

    [HttpPost("habits")]
    public async Task<IActionResult> GetHabitsSuggestions([FromBody] HabitsRequest req)
    {
        var catSummary = string.Join(", ", req.ByCat.Select(c => $"{c.Label}: {c.Total:F2}₾"));

        var system = "You are a personal finance advisor. Always respond in Georgian language (ქართული). Never use markdown, asterisks (**), or bullet points (-). Give exactly 3 short tips, each on a new line, starting with a relevant emoji. Reference the actual numbers from the data. Use informal 'შენ'. Be direct and specific.";

        var user = $"""
My last 30 days spending: total {req.TotalSpend:F2}₾. Categories: {catSummary}. Most expensive day: {req.BusiestDay}. Evening spending: {req.EveningPct}%. Food: {req.FoodFreqPerWeek}x per week, avg {req.FoodAvg}₾ per visit. Weekend vs weekday: {req.WeekendPct}% more on weekends.

Give me 3 specific, concrete tips in Georgian. Each tip = 1 short sentence. Start each with emoji.
""";

        var result = await groq.GenerateAsync(system, user);
        return Ok(new { text = result });
    }
}

public record CategoryStat(string Label, decimal Total);
public record AdviceRequest(List<CategoryStat> ByCat, decimal TotalSpend, decimal Income);
public record ChatRequest(List<AIMessage> Messages, List<CategoryStat> ByCat, decimal TotalSpend, decimal Income);
public record HabitsRequest(
    List<CategoryStat> ByCat,
    decimal TotalSpend,
    string BusiestDay,
    int EveningPct,
    string FoodFreqPerWeek,
    int FoodAvg,
    int WeekendPct
);
