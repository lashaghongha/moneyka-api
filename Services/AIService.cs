using System.Text;
using System.Text.Json;

namespace MoneyKa.Api.Services;

public record AIRequest(string SystemPrompt, List<AIMessage> Messages, int MaxTokens = 1000);
public record AIMessage(string Role, string Content);

public class AIService(IHttpClientFactory httpClientFactory, IConfiguration config)
{
    private const string ApiUrl = "https://api.anthropic.com/v1/messages";
    private const string Model = "claude-sonnet-4-6";

    public async Task<string> GetCompletionAsync(AIRequest request)
    {
        var apiKey = config["Anthropic:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            return "API key არ არის კონფიგურირებული.";

        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var body = new
        {
            model = Model,
            max_tokens = request.MaxTokens,
            system = request.SystemPrompt,
            messages = request.Messages.Select(m => new { role = m.Role, content = m.Content }).ToList()
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(ApiUrl, content);
        var responseJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseJson);
        var text = doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();

        return text ?? "პასუხი ვერ მოვიძიე.";
    }
}
