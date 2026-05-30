using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyKa.Api.Data;
using MoneyKa.Api.Models;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Goals.ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Goal goal)
    {
        goal.Id = 0;
        db.Goals.Add(goal);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = goal.Id }, goal);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var goal = await db.Goals.FindAsync(id);
        return goal is null ? NotFound() : Ok(goal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Goal goal)
    {
        if (id != goal.Id) return BadRequest();
        db.Entry(goal).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return Ok(goal);
    }

    [HttpPatch("{id}/deposit")]
    public async Task<IActionResult> Deposit(int id, [FromBody] DepositRequest req)
    {
        var goal = await db.Goals.FindAsync(id);
        if (goal is null) return NotFound();
        goal.Saved = Math.Min(goal.Saved + req.Amount, goal.Target);
        await db.SaveChangesAsync();
        return Ok(goal);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await db.Goals.FindAsync(id);
        if (goal is null) return NotFound();
        db.Goals.Remove(goal);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

public record DepositRequest(decimal Amount);
