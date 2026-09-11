namespace AI.SocialNetwork.Application.Services;

using System.Net.Http.Json;

// Клиент Ollama (OpenAI-совместимый API). При недоступности сервера — graceful fallback.
public sealed class OllamaClient
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public OllamaClient(HttpClient http, string baseUrl = "http://localhost:11434")
    {
        _http = http;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public bool IsAvailable { get; private set; }

    // Возвращает null если сервер недоступен; имя модели; ответ
    public async Task<(bool Ok, string? Model, string? Reply)> ChatAsync(
        string model,
        string systemPrompt,
        string userMessage,
        CancellationToken ct = default)
    {
        try
        {
            var payload = new
            {
                model,
                stream = false,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt ?? "Ты — полезный AI-ассистент." },
                    new { role = "user", content = userMessage }
                }
            };

            using var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/chat", payload, ct);
            if (!response.IsSuccessStatusCode)
            {
                IsAvailable = false;
                return (false, model, null);
            }

            var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: ct);
            IsAvailable = true;
            return (true, result?.Model, result?.Message?.Content);
        }
        catch
        {
            IsAvailable = false;
            return (false, model, null);
        }
    }
}

internal sealed class OllamaChatResponse
{
    public string? Model { get; set; }
    public OllamaChatMessage? Message { get; set; }
}

internal sealed class OllamaChatMessage
{
    public string? Role { get; set; }
    public string? Content { get; set; }
}