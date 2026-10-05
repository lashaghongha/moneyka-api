using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyKa.Api.Models;

public class Goal
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "🎯";
    [Column(TypeName = "numeric")] public decimal Target { get; set; }
    [Column(TypeName = "numeric")] public decimal Saved { get; set; }
    public string Color { get; set; } = "#4CAF82";
}
