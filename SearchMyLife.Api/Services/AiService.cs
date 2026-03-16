using System.ClientModel;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using SearchMyLife.Api.Config;

namespace SearchMyLife.Api.Services;

public class AiService : IAiService
{
    private readonly ChatClient? _chatClient;
    private readonly EmbeddingClient? _embeddingClient;
    private readonly ILogger<AiService> _logger;
    private readonly bool _isConfigured;

    private const string AnalysisPrompt = """
        You are an AI journal analyst. Given the following journal entry, extract:
        - emotion: one of (happy, sad, anxious, stressed, calm, excited, grateful, neutral)
        - sentimentScore: a number from -1.0 (very negative) to 1.0 (very positive)
        - tags: an array of 2-5 short descriptive tags
        - summary: a one-sentence summary

        Respond with valid JSON only. No markdown, no explanation.
        """;

    public AiService(IOptions<OpenAISettings> settings, ILogger<AiService> logger)
    {
        _logger = logger;
        var options = settings.Value;

        if (string.IsNullOrEmpty(options.ApiKey))
        {
            _logger.LogWarning("OpenAI is not configured. AI features will be unavailable.");
            _isConfigured = false;
            return;
        }

        _isConfigured = true;
        var client = new OpenAIClient(new ApiKeyCredential(options.ApiKey));
        _chatClient = client.GetChatClient(options.CompletionModel);
        _embeddingClient = client.GetEmbeddingClient(options.EmbeddingModel);
    }

    public async Task<EntryAnalysis> AnalyzeAsync(string plaintext)
    {
        if (!_isConfigured)
            throw new InvalidOperationException("OpenAI is not configured.");

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(AnalysisPrompt),
            new UserChatMessage(plaintext)
        };

        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        var response = await _chatClient.CompleteChatAsync(messages, options);
        var json = response.Value.Content[0].Text;

        _logger.LogDebug("OpenAI analysis response: {Json}", json);

        var result = JsonSerializer.Deserialize<AnalysisJson>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to parse AI analysis response.");

        return new EntryAnalysis(
            Emotion: result.Emotion ?? "neutral",
            SentimentScore: result.SentimentScore,
            Tags: result.Tags ?? [],
            Summary: result.Summary ?? string.Empty
        );
    }

    public async Task<ReadOnlyMemory<float>> EmbedAsync(string text)
    {
        if (!_isConfigured)
            throw new InvalidOperationException("OpenAI is not configured.");

        var response = await _embeddingClient!.GenerateEmbeddingAsync(text);
        return response.Value.ToFloats();
    }

    private sealed class AnalysisJson
    {
        public string? Emotion { get; set; }
        public double SentimentScore { get; set; }
        public string[]? Tags { get; set; }
        public string? Summary { get; set; }
    }
}
