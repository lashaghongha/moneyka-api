using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyKa.Api.Data;
using MoneyKa.Api.Models;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Subscriptions.ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Subscription sub)
    {
        sub.Id = 0;
        db.Subscriptions.Add(sub);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = sub.Id }, sub);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sub = await db.Subscriptions.FindAsync(id);
        return sub is null ? NotFound() : Ok(sub);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Subscription sub)
    {
        if (id != sub.Id) return BadRequest();
        db.Entry(sub).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return Ok(sub);
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var sub = await db.Subscriptions.FindAsync(id);
        if (sub is null) return NotFound();
        sub.Active = !sub.Active;
        await db.SaveChangesAsync();
        return Ok(sub);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sub = await db.Subscriptions.FindAsync(id);
        if (sub is null) return NotFound();
        db.Subscriptions.Remove(sub);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
