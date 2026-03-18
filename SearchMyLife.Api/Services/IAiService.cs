namespace SearchMyLife.Api.Services;

public record EntryAnalysis(
    string Emotion,
    double SentimentScore,
    string[] Tags,
    string Summary
);

public record SearchSummary(
    string Overview,
    string[] RelevanceExplanations
);

public interface IAiService
{
    Task<EntryAnalysis> AnalyzeAsync(string plaintext);
    Task<ReadOnlyMemory<float>> EmbedAsync(string text);
    Task<SearchSummary> SummarizeSearchAsync(string query, string[] topSummaries);
}
