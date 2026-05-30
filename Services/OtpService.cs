using System.Collections.Concurrent;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MoneyKa.Api.Services;

public class OtpService(IConfiguration config, ILogger<OtpService> logger)
{
    // phone → (code, expires)
    private static readonly ConcurrentDictionary<string, (string Code, DateTime Expires)> _store = new();

    private static readonly Random _rng = new();

    public async Task<bool> SendAsync(string phone)
    {
        // normalize: keep only digits + leading +
        var normalized = NormalizePhone(phone);
        if (string.IsNullOrEmpty(normalized)) return false;

        var code = _rng.Next(100_000, 999_999).ToString();
        _store[normalized] = (code, DateTime.UtcNow.AddMinutes(5));

        var sid   = config["Twilio:AccountSid"];
        var token = config["Twilio:AuthToken"];
        var from  = config["Twilio:From"];

        // Dev mode: no Twilio credentials → log code to console
        if (string.IsNullOrEmpty(sid) || sid == "YOUR_ACCOUNT_SID")
        {
            logger.LogWarning("⚡ DEV MODE — OTP for {Phone}: {Code}", normalized, code);
            return true;
        }

        try
        {
            TwilioClient.Init(sid, token);
            await MessageResource.CreateAsync(
                body: $"MoneyKa — შენი PIN-ის აღდგენის კოდია: {code}\nმოქმედებს 5 წუთი.",
                from: new Twilio.Types.PhoneNumber(from),
                to:   new Twilio.Types.PhoneNumber(normalized)
            );
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SMS send failed for {Phone}", normalized);
            return false;
        }
    }

    public bool Verify(string phone, string code)
    {
        var normalized = NormalizePhone(phone);
        if (!_store.TryGetValue(normalized, out var entry)) return false;
        if (DateTime.UtcNow > entry.Expires) { _store.TryRemove(normalized, out _); return false; }
        if (entry.Code != code.Trim()) return false;
        _store.TryRemove(normalized, out _); // single use
        return true;
    }

    private static string NormalizePhone(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "";
        var digits = new string(raw.Where(char.IsDigit).ToArray());
        // Georgian mobile: 9 digits starting with 5 → prefix +995
        if (digits.Length == 9 && digits.StartsWith("5"))
            return "+995" + digits;
        // already has country code (995XXXXXXXXX or +995XXXXXXXXX)
        if (digits.Length == 12 && digits.StartsWith("995"))
            return "+" + digits;
        if (raw.StartsWith("+")) return "+" + digits;
        return "";
    }
}
