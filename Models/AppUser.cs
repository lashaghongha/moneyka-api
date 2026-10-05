using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyKa.Api.Models;

public class AppUser
{
    public int    Id        { get; set; }
    public string DeviceId  { get; set; } = "";
    public string Plan      { get; set; } = "free";
    public string Name         { get; set; } = "";
    public string Phone        { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    [Column(TypeName = "timestamp with time zone")] public DateTime FirstSeen { get; set; } = DateTime.UtcNow;
    [Column(TypeName = "timestamp with time zone")] public DateTime LastSeen  { get; set; } = DateTime.UtcNow;
    // true = მომხმარებელმა რეალურად გადაიხადა; false = ადმინმა ხელით მიანიჭა
    [Column(TypeName = "boolean")] public bool IsPaidCustomer { get; set; } = false;
}
