using AI.SocialNetwork.Application.Contracts;
using System.Text.Json;

namespace AI.SocialNetwork.Application.Services;

// A3: модерация контента LLM-фильтром (Ollama, OpenAI-совместимый API).
// При недоступности модели — мягкий fallback (контент не теряется, статус "skipped").
public sealed class OllamaModerationService : IModerationService
{
    private readonly OllamaClient _ollama;
    private readonly string _model;

    public OllamaModerationService(OllamaClient ollama, string model = "nemotron-mini")
    {
        _ollama = ollama;
        _model = model;
    }

    private const string SystemPrompt =
        "Ты — модератор контента деловой социальной сети. " +
        "Классифицируй пользовательский текст и ответь СТРОГО одним JSON-объектом вида: " +
        "{\"category\": \"allowed\" | \"flagged\" | \"rejected\", \"reason\": \"краткое обоснование на русском\"}. " +
        "allowed — нормальный контент, можно публиковать; " +
        "flagged — сомнительный (спам, навязчивая реклама, ссылки, кликбейт); " +
        "rejected — запрещённый (оскорбления, угрозы, дискриминация, незаконный контент, персональные данные, мошенничество). " +
        "Ничего, кроме JSON, не пиши.";

    public async Task<ModerationResult> ModerateTextAsync(string content, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return ModerationResult.Skipped("Пустой контент");
        }

        var (ok, _, reply) = await _ollama.ChatAsync(_model, SystemPrompt, content, ct);
        if (!ok || string.IsNullOrWhiteSpace(reply))
        {
            return ModerationResult.Skipped("LLM-сервис недоступен");
        }

        var parsed = ParseVerdict(reply);
        return parsed ?? ModerationResult.Skipped("Нераспознанный ответ модератора");
    }

    internal static ModerationResult? ParseVerdict(string reply)
    {
        var start = reply.IndexOf('{');
        var end = reply.LastIndexOf('}');
        if (start < 0 || end <= start)
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(reply.Substring(start, end - start + 1));
            var root = doc.RootElement;
            if (!root.TryGetProperty("category", out var categoryProp))
            {
                return null;
            }

            var category = categoryProp.GetString()?.Trim().ToLowerInvariant();
            var reason = root.TryGetProperty("reason", out var reasonProp) ? reasonProp.GetString() : null;

            return category switch
            {
                "allowed" => new ModerationResult("approved", reason),
                "flagged" => new ModerationResult("flagged", reason),
                "rejected" => new ModerationResult("rejected", reason),
                _ => new ModerationResult("flagged", reason)
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }
}