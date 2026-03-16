namespace SearchMyLife.Api.Services;

public record EntryAnalysis(
    string Emotion,
    double SentimentScore,
    string[] Tags,
    string Summary
);

public interface IAiService
{
    Task<EntryAnalysis> AnalyzeAsync(string plaintext);
    Task<ReadOnlyMemory<float>> EmbedAsync(string text);
}
