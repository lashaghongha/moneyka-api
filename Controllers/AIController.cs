using Microsoft.AspNetCore.Mvc;
using MoneyKa.Api.Services;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController(AIService ai, OpenAIService openAi) : ControllerBase
{
    // ყველა endpoint-ისთვის საერთო კონტექსტის builder
    private static string BuildContext(UserFinanceContext ctx)
    {
        var lines = new List<string>();

        lines.Add($"ბალანსი: {ctx.Balance:F2}₾  |  შემოსავალი: {ctx.Income:F2}₾  |  ხარჯები: {ctx.TotalSpend:F2}₾");

        if (ctx.ByCat?.Count > 0)
            lines.Add("ხარჯები კატეგორიების მიხედვით: " +
                string.Join(", ", ctx.ByCat.Select(c => $"{c.Label} {c.Total:F2}₾")));

        if (ctx.Subs?.Count > 0)
            lines.Add("აქტიური გამოწერები: " +
                string.Join(", ", ctx.Subs.Select(s => $"{s.Name} {s.Price}{s.Currency} (შემდეგი: {s.NextDate})")));

        if (ctx.Goals?.Count > 0)
            lines.Add("დანაზოგის მიზნები: " +
                string.Join(", ", ctx.Goals.Select(g => $"{g.Name}: დაგროვილია {g.Saved}{g.Currency}/{g.Target}{g.Currency}")));

        if (ctx.Budgets?.Count > 0)
            lines.Add("ბიუჯეტის ლიმიტები (თვიური): " +
                string.Join(", ", ctx.Budgets.Select(b => $"{b.Category} {b.MonthlyBudget:F2}₾")));

        if (ctx.Recurring?.Count > 0)
            lines.Add("განმეორებადი ტრანზაქციები: " +
                string.Join(", ", ctx.Recurring.Select(r => $"{r.Desc} {r.Amount:F2}₾ ({r.Freq})")));

        return string.Join("\n", lines);
    }

    [HttpPost("advice")]
    public async Task<IActionResult> GetAdvice([FromBody] AdviceRequest req)
    {
        var context = BuildContext(req);
        var system  = "You are a personal finance advisor for a Georgian user. Always respond ONLY in Georgian language. No markdown, no asterisks. Give exactly 3 practical tips, each on a new line starting with emoji. Reference the actual numbers. Use informal შენ. Be short and direct.";
        var user    = $"ჩემი ფინანსური სურათი:\n{context}\n\nმომეცი 3 კონკრეტური რჩევა.";

        var result = await openAi.GenerateAsync(system, user);
        return Ok(new { text = result });
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest req)
    {
        var context = BuildContext(req);
        var system  = $"შენ ხარ MoneyKa-ს AI ფინანსური მრჩეველი. ყოველთვის პასუხობ მხოლოდ ქართულ ენაზე. გამოიყენე მხოლოდ ქვემოთ მოცემული მონაცემები კითხვებზე პასუხისთვის. No markdown. Be short and practical. Use informal შენ.\n\nმომხმარებლის ფინანსური მონაცემები:\n{context}";

        var lastMsg = req.Messages.LastOrDefault(m => m.Role == "user")?.Content ?? "";
        var result  = await openAi.GenerateAsync(system, lastMsg);
        return Ok(new { text = result });
    }

    [HttpPost("habits")]
    public async Task<IActionResult> GetHabitsSuggestions([FromBody] HabitsRequest req)
    {
        var context = BuildContext(req);
        var system  = "You are a personal finance advisor. Always respond in Georgian language only. No markdown, no asterisks. Give exactly 3 short tips, each on a new line starting with emoji. Reference actual numbers. Use informal შენ. Be direct and specific.";

        var user = $"ჩემი ფინანსური მონაცემები:\n{context}\n\nდამატებითი სტატისტიკა: ყველაზე ძვირი დღე: {req.BusiestDay}, საღამოს ხარჯები: {req.EveningPct}%, საკვები კვირაში {req.FoodFreqPerWeek}x, საშ. {req.FoodAvg}₾, შაბათ-კვირა vs კვირა: {req.WeekendPct}% მეტი.\n\nმომეცი 3 კონკრეტური რჩევა.";

        var result = await openAi.GenerateAsync(system, user);
        return Ok(new { text = result });
    }
}

// ――― shared finance context ―――
public record CategoryStat(string Label, decimal Total);
public record SubInfo(string Name, decimal Price, string Currency, string NextDate);
public record GoalInfo(string Name, decimal Target, decimal Saved, string Currency);
public record BudgetInfo(string Category, decimal MonthlyBudget);
public record RecurringInfo(string Desc, decimal Amount, string Freq);

public record UserFinanceContext(
    List<CategoryStat> ByCat,
    decimal TotalSpend,
    decimal Income,
    decimal Balance,
    List<SubInfo>? Subs,
    List<GoalInfo>? Goals,
    List<BudgetInfo>? Budgets,
    List<RecurringInfo>? Recurring
);

// ――― request records ―――
public record AdviceRequest(
    List<CategoryStat> ByCat, decimal TotalSpend, decimal Income, decimal Balance,
    List<SubInfo>? Subs, List<GoalInfo>? Goals, List<BudgetInfo>? Budgets, List<RecurringInfo>? Recurring
) : UserFinanceContext(ByCat, TotalSpend, Income, Balance, Subs, Goals, Budgets, Recurring);

public record ChatRequest(
    List<AIMessage> Messages,
    List<CategoryStat> ByCat, decimal TotalSpend, decimal Income, decimal Balance,
    List<SubInfo>? Subs, List<GoalInfo>? Goals, List<BudgetInfo>? Budgets, List<RecurringInfo>? Recurring
) : UserFinanceContext(ByCat, TotalSpend, Income, Balance, Subs, Goals, Budgets, Recurring);

public record HabitsRequest(
    List<CategoryStat> ByCat, decimal TotalSpend, decimal Income, decimal Balance,
    List<SubInfo>? Subs, List<GoalInfo>? Goals, List<BudgetInfo>? Budgets, List<RecurringInfo>? Recurring,
    string BusiestDay, int EveningPct, string FoodFreqPerWeek, int FoodAvg, int WeekendPct
) : UserFinanceContext(ByCat, TotalSpend, Income, Balance, Subs, Goals, Budgets, Recurring);
