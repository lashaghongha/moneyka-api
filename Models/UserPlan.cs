using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyKa.Api.Models;

public class UserPlan
{
    public int Id { get; set; }
    public string Plan { get; set; } = "free"; // free | pro | elite
    [Column(TypeName = "timestamp with time zone")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
