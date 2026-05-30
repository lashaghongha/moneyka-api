namespace MoneyKa.Api.Models;

public class AppUser
{
    public int    Id        { get; set; }
    public string DeviceId  { get; set; } = "";
    public string Plan      { get; set; } = "free";
    public string Name      { get; set; } = "";
    public DateTime FirstSeen { get; set; } = DateTime.UtcNow;
    public DateTime LastSeen  { get; set; } = DateTime.UtcNow;
}
