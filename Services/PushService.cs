using WebPush;
using MoneyKa.Api.Data;
using System.Text.Json;

namespace MoneyKa.Api.Services;

public class PushService(AppDbContext db, IConfiguration config)
{
    public async Task SendAsync(string deviceId, string title, string body)
    {
        var subs = db.PushSubs.Where(s => s.DeviceId == deviceId).ToList();
        if (subs.Count == 0) return;

        var vapidPublic  = config["Vapid:PublicKey"]!;
        var vapidPrivate = config["Vapid:PrivateKey"]!;
        var vapidSubject = config["Vapid:Subject"]!;

        var client = new WebPushClient();
        client.SetVapidDetails(vapidSubject, vapidPublic, vapidPrivate);

        var payload = JsonSerializer.Serialize(new { title, body });

        foreach (var s in subs)
        {
            try
            {
                var subscription = new PushSubscription(s.Endpoint, s.P256dh, s.Auth);
                await client.SendNotificationAsync(subscription, payload);
            }
            catch
            {
                // subscription expired — delete it
                db.PushSubs.Remove(s);
            }
        }

        await db.SaveChangesAsync();
    }
}
