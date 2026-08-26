using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyKa.Api.Data;
using MoneyKa.Api.Models;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController(AppDbContext db) : ControllerBase
{
    // POST /api/sync/push  — frontend → backend (upsert by deviceId)
    [HttpPost("push")]
    public async Task<IActionResult> Push([FromBody] SyncPushRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceId) || string.IsNullOrWhiteSpace(req.DeviceSecret))
            return BadRequest(new { error = "deviceId and deviceSecret required" });

        var existing = await db.UserSyncs.FindAsync(req.DeviceId);
        if (existing is null)
        {
            // პირველი push — ახალი device, secret-ი ინახება
            db.UserSyncs.Add(new UserSync
            {
                DeviceId         = req.DeviceId,
                DeviceSecret     = req.DeviceSecret,
                TransactionsJson = req.Transactions,
                GoalsJson        = req.Goals,
                SubsJson         = req.Subs,
                BudgetsJson      = req.Budgets,
                UpdatedAt        = DateTime.UtcNow,
            });
        }
        else
        {
            // არსებული device — secret-ი უნდა ემთხვეოდეს
            // (ძველი ჩანაწერები DeviceSecret="" — migration: ერთხელ ვუსვავთ)
            if (!string.IsNullOrEmpty(existing.DeviceSecret)
                && existing.DeviceSecret != req.DeviceSecret)
                return StatusCode(403, new { error = "invalid device secret" });

            existing.DeviceSecret     = req.DeviceSecret;   // migration-ზე ინახება
            existing.TransactionsJson = req.Transactions;
            existing.GoalsJson        = req.Goals;
            existing.SubsJson         = req.Subs;
            existing.BudgetsJson      = req.Budgets;
            existing.UpdatedAt        = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return Ok(new { ok = true, updatedAt = DateTime.UtcNow });
    }

    // GET /api/sync/pull?deviceId=xxx&deviceSecret=yyy  — backend → frontend
    [HttpGet("pull")]
    public async Task<IActionResult> Pull([FromQuery] string deviceId, [FromQuery] string deviceSecret)
    {
        if (string.IsNullOrWhiteSpace(deviceId) || string.IsNullOrWhiteSpace(deviceSecret))
            return BadRequest(new { error = "deviceId and deviceSecret required" });

        var sync = await db.UserSyncs.FindAsync(deviceId);
        if (sync is null)
            return Ok(new { found = false });

        // secret-ი სავალდებულოა (ძველი ჩანაწერები empty-ით გადის — migration)
        if (!string.IsNullOrEmpty(sync.DeviceSecret) && sync.DeviceSecret != deviceSecret)
            return StatusCode(403, new { error = "invalid device secret" });

        return Ok(new
        {
            found        = true,
            transactions = sync.TransactionsJson,
            goals        = sync.GoalsJson,
            subs         = sync.SubsJson,
            budgets      = sync.BudgetsJson,
            updatedAt    = sync.UpdatedAt,
        });
    }
}

public record SyncPushRequest(
    string DeviceId,
    string DeviceSecret,
    string Transactions,
    string Goals,
    string Subs,
    string Budgets
);
