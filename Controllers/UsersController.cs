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
            user = new AppUser
            {
                DeviceId     = req.DeviceId,
                Plan         = req.Plan ?? "free",
                Name         = req.Name  ?? "",
                Phone        = req.Phone ?? "",
                PasswordHash = req.PasswordHash ?? "",
                FirstSeen    = DateTime.UtcNow,
                LastSeen     = DateTime.UtcNow
            };
            db.AppUsers.Add(user);
        }
        else
        {
            user.LastSeen = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(req.Name))  user.Name  = req.Name;
            if (!string.IsNullOrEmpty(req.Phone)) user.Phone = req.Phone;
            // პაროლი მხოლოდ ერთხელ ინახება — ცარიელია და ახალი მოვიდა
            if (!string.IsNullOrEmpty(req.PasswordHash) && string.IsNullOrEmpty(user.PasswordHash))
                user.PasswordHash = req.PasswordHash;
        }
        await db.SaveChangesAsync();
        return Ok(new { user.Plan });
    }

    // ნომრის უნიკალობის შემოწმება
    [HttpPost("check-phone")]
    public async Task<IActionResult> CheckPhone([FromBody] PhoneCheckRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Phone)) return BadRequest();
        var exists = await db.AppUsers.AnyAsync(u => u.Phone == req.Phone);
        return Ok(new { exists });
    }

    // ტელეფონი + პაროლი → ანგარიშის აღდგენა
    [HttpPost("login-password")]
    public async Task<IActionResult> LoginWithPassword([FromBody] LoginPasswordRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Phone) || string.IsNullOrWhiteSpace(req.PasswordHash))
            return BadRequest();
        var user = await db.AppUsers.FirstOrDefaultAsync(
            u => u.Phone == req.Phone && u.PasswordHash == req.PasswordHash);
        if (user is null) return Unauthorized(new { error = "invalid" });
        user.LastSeen = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(new { user.Name, user.Plan, user.DeviceId, user.Phone });
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

public record PingRequest(string DeviceId, string Plan, string? Name, string? Phone, string? PasswordHash);
public record PhoneCheckRequest(string Phone);
public record LoginPasswordRequest(string Phone, string PasswordHash);
