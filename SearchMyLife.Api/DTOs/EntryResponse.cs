using System.Text.Json;
using SearchMyLife.Api.Models;

namespace SearchMyLife.Api.DTOs;

public class EntryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Iv { get; set; }
    public string? Salt { get; set; }
    public string? Emotion { get; set; }
    public double? SentimentScore { get; set; }
    public string[]? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static EntryResponse FromEntity(JournalEntry entry)
    {
        return new EntryResponse
        {
            Id = entry.Id,
            Title = entry.Title,
            Content = entry.EncryptedContent,
            Iv = entry.Iv,
            Salt = entry.Salt,
            Emotion = entry.Emotion,
            SentimentScore = entry.SentimentScore,
            Tags = DeserializeTags(entry.Tags),
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };
    }

    private static string[]? DeserializeTags(string? tagsJson)
    {
        if (string.IsNullOrWhiteSpace(tagsJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<string[]>(tagsJson);
        }
        catch
        {
            return null;
        }
    }
}
