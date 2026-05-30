using System.Text;
using System.Text.Json;

namespace MoneyKa.Api.Services;

public class GeminiService(IHttpClientFactory httpClientFactory, IConfiguration config)
{
    private const string Model = "gemini-2.0-flash";

    public async Task<string> GenerateAsync(string prompt)
    {
        var apiKey = config["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            return "Gemini API key არ არის კონფიგურირებული.";

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={apiKey}";

        var body = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 600
            }
        };

        var client = httpClientFactory.CreateClient();
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            // Check for API error
            if (root.TryGetProperty("error", out var errProp))
            {
                var msg = errProp.TryGetProperty("message", out var m) ? m.GetString() : "API შეცდომა";
                return $"Gemini შეცდომა: {msg}";
            }

            // Parse successful response
            if (root.TryGetProperty("candidates", out var candidates) &&
                candidates.GetArrayLength() > 0)
            {
                var candidate = candidates[0];
                if (candidate.TryGetProperty("content", out var contentProp) &&
                    contentProp.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out var textProp))
                {
                    return textProp.GetString() ?? "პასუხი ვერ მოვიძიე.";
                }
            }

            return "Gemini-მ სტრუქტურა დააბრუნა მოულოდნელი ფორმატით.";
        }
        catch (Exception ex)
        {
            return $"კავშირის შეცდომა: {ex.Message}";
        }
    }
}
