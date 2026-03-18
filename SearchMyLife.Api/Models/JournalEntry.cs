namespace SearchMyLife.Api.Models;

public class JournalEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string EncryptedContent { get; set; } = string.Empty;
    public string? Iv { get; set; }
    public string? Salt { get; set; }
    public string? Emotion { get; set; }
    public double? SentimentScore { get; set; }
    public string? Summary { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
}
