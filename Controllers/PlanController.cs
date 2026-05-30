using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyKa.Api.Data;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var plan = await db.UserPlans.FirstOrDefaultAsync();
        return Ok(new { plan = plan?.Plan ?? "free" });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PlanRequest req)
    {
        var userPlan = await db.UserPlans.FirstOrDefaultAsync();
        if (userPlan is null) return NotFound();
        userPlan.Plan = req.Plan;
        userPlan.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(new { plan = userPlan.Plan });
    }
}

public record PlanRequest(string Plan);
