namespace MoneyKa.Api.Models;

public class Goal
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "🎯";
    public decimal Target { get; set; }
    public decimal Saved { get; set; }
    public string Color { get; set; } = "#4CAF82";
}
