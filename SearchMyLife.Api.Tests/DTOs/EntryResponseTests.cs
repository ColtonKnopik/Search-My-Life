using System.Text.Json;
using FluentAssertions;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Models;

namespace SearchMyLife.Api.Tests.DTOs;

public class EntryResponseTests
{
    [Fact]
    public void FromEntity_MapsAllFields()
    {
        var id = Guid.NewGuid();
        var tags = new[] { "alpha", "beta" };
        var entity = new JournalEntry
        {
            Id = id,
            UserId = Guid.NewGuid(),
            Title = "Title",
            EncryptedContent = "enc",
            Iv = "iv",
            Salt = "salt",
            Emotion = "happy",
            SentimentScore = 0.75,
            Summary = "A summary",
            Tags = JsonSerializer.Serialize(tags),
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = EntryResponse.FromEntity(entity);

        result.Id.Should().Be(id);
        result.Title.Should().Be("Title");
        result.Content.Should().Be("enc");
        result.Iv.Should().Be("iv");
        result.Salt.Should().Be("salt");
        result.Emotion.Should().Be("happy");
        result.SentimentScore.Should().Be(0.75);
        result.Tags.Should().BeEquivalentTo(tags);
        result.CreatedAt.Should().Be(entity.CreatedAt);
        result.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact]
    public void FromEntity_ReturnsNullTags_WhenTagsJsonIsNull()
    {
        var entity = new JournalEntry
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "T",
            EncryptedContent = "c",
            Tags = null
        };

        var result = EntryResponse.FromEntity(entity);

        result.Tags.Should().BeNull();
    }

    [Fact]
    public void FromEntity_ReturnsNullTags_WhenTagsJsonIsInvalid()
    {
        var entity = new JournalEntry
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "T",
            EncryptedContent = "c",
            Tags = "not-valid-json"
        };

        var result = EntryResponse.FromEntity(entity);

        result.Tags.Should().BeNull();
    }
}
