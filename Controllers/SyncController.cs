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
        if (string.IsNullOrWhiteSpace(req.DeviceId))
            return BadRequest("deviceId required");

        var existing = await db.UserSyncs.FindAsync(req.DeviceId);
        if (existing is null)
        {
            db.UserSyncs.Add(new UserSync
            {
                DeviceId         = req.DeviceId,
                TransactionsJson = req.Transactions,
                GoalsJson        = req.Goals,
                SubsJson         = req.Subs,
                BudgetsJson      = req.Budgets,
                UpdatedAt        = DateTime.UtcNow,
            });
        }
        else
        {
            existing.TransactionsJson = req.Transactions;
            existing.GoalsJson        = req.Goals;
            existing.SubsJson         = req.Subs;
            existing.BudgetsJson      = req.Budgets;
            existing.UpdatedAt        = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return Ok(new { ok = true, updatedAt = DateTime.UtcNow });
    }

    // GET /api/sync/pull?deviceId=xxx  — backend → frontend
    [HttpGet("pull")]
    public async Task<IActionResult> Pull([FromQuery] string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return BadRequest("deviceId required");

        var sync = await db.UserSyncs.FindAsync(deviceId);
        if (sync is null)
            return Ok(new { found = false });

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
    string Transactions,
    string Goals,
    string Subs,
    string Budgets
);
