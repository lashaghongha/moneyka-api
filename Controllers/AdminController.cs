using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyKa.Api.Data;
using MoneyKa.Api.Filters;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/admin")]
[AdminAuth]
public class AdminController(AppDbContext db, IConfiguration config) : ControllerBase
{
    private static readonly decimal[] Prices = [0m, 3.99m, 7.99m];
    private static readonly string[] PlanNames = ["free", "pro", "elite"];

    // POST /api/admin/login  — key-ს ამოწმებს (filter უკვე ამოწმებს, ეს უბრალოდ confirm-ია)
    [HttpPost("login")]
    public IActionResult Login() => Ok(new { ok = true });

    // GET /api/admin/stats
    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        var users = await db.AppUsers.ToListAsync();
        var now = DateTime.UtcNow;

        var planCounts = PlanNames.Select(p => new
        {
            plan  = p,
            count = users.Count(u => u.Plan == p)
        }).ToList();

        var monthlyRevenue = users.Sum(u => u.Plan switch
        {
            "pro"   => 3.99m,
            "elite" => 7.99m,
            _       => 0m
        });

        var newThisWeek  = users.Count(u => u.FirstSeen >= now.AddDays(-7));
        var activeToday  = users.Count(u => u.LastSeen  >= now.AddDays(-1));

        return Ok(new
        {
            totalUsers     = users.Count,
            newThisWeek,
            activeToday,
            monthlyRevenue,
            planCounts
        });
    }

    // GET /api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await db.AppUsers
            .OrderByDescending(u => u.LastSeen)
            .Select(u => new
            {
                u.Id,
                u.DeviceId,
                u.Plan,
                u.Name,
                firstSeen = u.FirstSeen,
                lastSeen  = u.LastSeen
            })
            .ToListAsync();
        return Ok(users);
    }

    // PUT /api/admin/users/{id}/plan
    [HttpPut("users/{id}/plan")]
    public async Task<IActionResult> SetPlan(int id, [FromBody] AdminPlanRequest req)
    {
        var user = await db.AppUsers.FindAsync(id);
        if (user is null) return NotFound();
        user.Plan = req.Plan;
        await db.SaveChangesAsync();
        return Ok(new { user.Id, user.Plan });
    }

    // DELETE /api/admin/users/{id}
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await db.AppUsers.FindAsync(id);
        if (user is null) return NotFound();
        db.AppUsers.Remove(user);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

public record AdminPlanRequest(string Plan);
