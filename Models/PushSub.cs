using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyKa.Api.Models;

public class PushSub
{
    public int    Id       { get; set; }
    public string DeviceId { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public string P256dh   { get; set; } = "";
    public string Auth     { get; set; } = "";
    [Column(TypeName = "timestamp with time zone")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
