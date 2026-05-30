using Microsoft.AspNetCore.Mvc;
using MoneyKa.Api.Services;

namespace MoneyKa.Api.Controllers;

[ApiController]
[Route("api/otp")]
public class OtpController(OtpService otp) : ControllerBase
{
    // POST /api/otp/send  { "phone": "+995599123456" }
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] PhoneRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Phone))
            return BadRequest(new { error = "phone required" });

        var ok = await otp.SendAsync(req.Phone);
        if (!ok) return BadRequest(new { error = "invalid phone number" });

        return Ok(new { sent = true });
    }

    // POST /api/otp/verify  { "phone": "+995599123456", "code": "483920" }
    [HttpPost("verify")]
    public IActionResult Verify([FromBody] OtpVerifyRequest req)
    {
        var ok = otp.Verify(req.Phone, req.Code);
        if (!ok) return BadRequest(new { error = "invalid or expired code" });
        return Ok(new { verified = true });
    }
}

public record PhoneRequest(string Phone);
public record OtpVerifyRequest(string Phone, string Code);
