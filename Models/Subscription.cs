using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyKa.Api.Models;

public class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "📱";
    public string Color { get; set; } = "#4CAF82";
    [Column(TypeName = "numeric")] public decimal Price { get; set; }
    public string Billing { get; set; } = "monthly"; // monthly | yearly
    public string Category { get; set; } = "სხვა";
    public string NextDate { get; set; } = "";
    [Column(TypeName = "boolean")] public bool Active { get; set; } = true;
}
