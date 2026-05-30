using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyKa.Api.Data;
using MoneyKa.Api.Models;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Transactions.OrderByDescending(t => t.Date).ThenByDescending(t => t.Time).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Transaction tx)
    {
        tx.Id = 0;
        db.Transactions.Add(tx);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = tx.Id }, tx);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tx = await db.Transactions.FindAsync(id);
        return tx is null ? NotFound() : Ok(tx);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Transaction tx)
    {
        if (id != tx.Id) return BadRequest();
        db.Entry(tx).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return Ok(tx);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tx = await db.Transactions.FindAsync(id);
        if (tx is null) return NotFound();
        db.Transactions.Remove(tx);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
