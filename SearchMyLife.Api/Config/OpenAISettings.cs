namespace SearchMyLife.Api.Config;

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string CompletionModel { get; set; } = "gpt-4.1-mini";
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}
