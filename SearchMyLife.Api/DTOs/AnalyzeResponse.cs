namespace SearchMyLife.Api.DTOs;

public class AnalyzeResponse
{
    public string Emotion { get; set; } = string.Empty;
    public double SentimentScore { get; set; }
    public string[] Tags { get; set; } = [];
}
