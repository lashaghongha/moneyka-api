using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyKa.Api.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Category { get; set; } = "other";
    public string Desc { get; set; } = "";
    [Column(TypeName = "numeric")] public decimal Amount { get; set; }
    public string Date { get; set; } = "";
    public string Time { get; set; } = "";
    public string Type { get; set; } = "expense"; // expense | income
    [Column(TypeName = "boolean")] public bool Recurring { get; set; }
    public string? RecFreq { get; set; } // daily | weekly | monthly
}
