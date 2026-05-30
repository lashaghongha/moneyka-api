using Microsoft.AspNetCore.Mvc;
using MoneyKa.Api.Data;
using MoneyKa.Api.Models;
using MoneyKa.Api.Services;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PushController(AppDbContext db, IConfiguration config, PushService push) : ControllerBase
{
    // GET /api/push/vapid-public
    [HttpGet("vapid-public")]
    public IActionResult GetPublicKey() =>
        Ok(new { key = config["Vapid:PublicKey"] });

    // POST /api/push/subscribe
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest req)
    {
        var old = db.PushSubs.Where(s => s.DeviceId == req.DeviceId).ToList();
        db.PushSubs.RemoveRange(old);

        db.PushSubs.Add(new PushSub
        {
            DeviceId  = req.DeviceId,
            Endpoint  = req.Endpoint,
            P256dh    = req.P256dh,
            Auth      = req.Auth,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        return Ok();
    }

    // DELETE /api/push/unsubscribe/{deviceId}
    [HttpDelete("unsubscribe/{deviceId}")]
    public async Task<IActionResult> Unsubscribe(string deviceId)
    {
        var subs = db.PushSubs.Where(s => s.DeviceId == deviceId).ToList();
        db.PushSubs.RemoveRange(subs);
        await db.SaveChangesAsync();
        return Ok();
    }

    // POST /api/push/check — sends due-tomorrow sub reminders
    [HttpPost("check")]
    public async Task<IActionResult> Check([FromBody] PushCheckRequest req)
    {
        var tomorrow = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");

        foreach (var sub in req.Subs.Where(s => s.Active && s.NextDate == tomorrow))
        {
            await push.SendAsync(
                req.DeviceId,
                $"📅 ხვალ გასაახლებელია: {sub.Name}",
                $"{sub.Name}-ის გადასახადი ხვალ არის — {sub.Price:F2}₾"
            );
        }

        return Ok();
    }
}

public record SubscribeRequest(string DeviceId, string Endpoint, string P256dh, string Auth);
public record SubDto(int Id, string Name, decimal Price, string NextDate, bool Active);
public record PushCheckRequest(string DeviceId, List<SubDto> Subs);
