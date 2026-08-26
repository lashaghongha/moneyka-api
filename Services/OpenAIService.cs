using System.Text;
using System.Text.Json;

namespace MoneyKa.Api.Services;

public class OpenAIService(IHttpClientFactory httpClientFactory, IConfiguration config)
{
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
    private const string Model  = "gpt-4o-mini";

    // ერთი user→assistant გაცვლა (advice, habits)
    public Task<string> GenerateAsync(string systemPrompt, string userPrompt) =>
        CallOpenAI(systemPrompt, new[] { new { role = "user", content = userPrompt } });

    // სრული conversation history (chat)
    public Task<string> GenerateWithHistoryAsync(string systemPrompt, IEnumerable<AIMessage> history) =>
        CallOpenAI(systemPrompt, history.Select(m => new { role = m.Role, content = m.Content }));

    private async Task<string> CallOpenAI(string systemPrompt, IEnumerable<object> userMessages)
    {
        var apiKey = config["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            return "OpenAI API key არ არის კონფიგურირებული.";

        var messages = new List<object> { new { role = "system", content = systemPrompt } };
        messages.AddRange(userMessages);

        var body = new { model = Model, messages, temperature = 0.7, max_tokens = 600 };

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
                return $"AI შეცდომა: {msg}";
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
