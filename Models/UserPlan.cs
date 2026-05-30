namespace MoneyKa.Api.Models;

public class UserPlan
{
    public int Id { get; set; }
    public string Plan { get; set; } = "free"; // free | pro | elite
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
