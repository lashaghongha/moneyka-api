using System.Text;
using System.Text.Json;

namespace MoneyKa.Api.Services;

public class GroqService(IHttpClientFactory httpClientFactory, IConfiguration config)
{
    private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string Model  = "llama-3.3-70b-versatile"; // უფასო, უკეთესი ქართული

    public async Task<string> GenerateAsync(string systemPrompt, string userPrompt)
    {
        var apiKey = config["Groq:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            return "Groq API key არ არის კონფიგურირებული.";

        var body = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userPrompt   }
            },
            temperature = 0.7,
            max_tokens  = 600
        };

        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var json    = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response     = await client.PostAsync(ApiUrl, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var err))
            {
                var msg = err.TryGetProperty("message", out var m) ? m.GetString() : "API შეცდომა";
                return $"Groq შეცდომა: {msg}";
            }

            return root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "პასუხი ვერ მოვიძიე.";
        }
        catch (Exception ex)
        {
            return $"კავშირის შეცდომა: {ex.Message}";
        }
    }
}
