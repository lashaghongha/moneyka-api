namespace MoneyKa.Api.Models;

public class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "📱";
    public string Color { get; set; } = "#4CAF82";
    public decimal Price { get; set; }
    public string Billing { get; set; } = "monthly"; // monthly | yearly
    public string Category { get; set; } = "სხვა";
    public string NextDate { get; set; } = "";
    public bool Active { get; set; } = true;
}
