using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyKa.Api.Data;
using MoneyKa.Api.Models;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(AppDbContext db) : ControllerBase
{
    // ყოველ გაშვებაზე frontend-ი ამ endpoint-ს ეძახის
    [HttpPost("ping")]
    public async Task<IActionResult> Ping([FromBody] PingRequest req)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(u => u.DeviceId == req.DeviceId);
        if (user is null)
        {
            // პირველი შესვლა — plan ლოკალურიდან
            user = new AppUser
            {
                DeviceId  = req.DeviceId,
                Plan      = req.Plan ?? "free",
                Name      = req.Name ?? "",
                FirstSeen = DateTime.UtcNow,
                LastSeen  = DateTime.UtcNow
            };
            db.AppUsers.Add(user);
        }
        else
        {
            // Plan-ს არ ვეხებით — admin-მა შეიძლება შეცვალოს
            user.LastSeen = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(req.Name)) user.Name = req.Name;
        }
        await db.SaveChangesAsync();
        // Backend-ის plan-ს ვაბრუნებთ — frontend განაახლებს localStorage-ს
        return Ok(new { user.Plan });
    }

    // Plan-ის სინქრონიზაცია
    [HttpGet("{deviceId}/plan")]
    public async Task<IActionResult> GetPlan(string deviceId)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(u => u.DeviceId == deviceId);
        return Ok(new { plan = user?.Plan ?? "free" });
    }

    [HttpPut("{deviceId}/plan")]
    public async Task<IActionResult> SetPlan(string deviceId, [FromBody] PlanRequest req)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(u => u.DeviceId == deviceId);
        if (user is null) return NotFound();
        user.Plan     = req.Plan;
        user.LastSeen = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(new { user.Plan });
    }
}

public record PingRequest(string DeviceId, string Plan, string? Name);
