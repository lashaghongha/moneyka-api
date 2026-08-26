namespace MoneyKa.Api.Models;

public class UserSync
{
    public string DeviceId        { get; set; } = "";
    public string TransactionsJson { get; set; } = "[]";
    public string GoalsJson        { get; set; } = "[]";
    public string SubsJson         { get; set; } = "[]";
    public string BudgetsJson      { get; set; } = "{}";
    public DateTime UpdatedAt      { get; set; } = DateTime.UtcNow;
}
