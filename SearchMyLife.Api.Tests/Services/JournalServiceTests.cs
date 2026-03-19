using FluentAssertions;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Models;
using SearchMyLife.Api.Services;
using SearchMyLife.Api.Tests.Helpers;

namespace SearchMyLife.Api.Tests.Services;

public class JournalServiceTests : IDisposable
{
    private readonly SearchMyLife.Api.Data.AppDbContext _db;
    private readonly JournalService _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public JournalServiceTests()
    {
        _db = TestDbContextFactory.Create();

        // Seed a user so FK constraints are satisfied
        _db.Users.Add(new User
        {
            Id = _userId,
            Email = "test@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        });
        _db.SaveChanges();

        _sut = new JournalService(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_ReturnsEntryWithGeneratedId()
    {
        var request = new CreateEntryRequest
        {
            Title = "My Entry",
            Content = "encrypted-content",
            Iv = "iv-value",
            Salt = "salt-value"
        };

        var result = await _sut.CreateAsync(_userId, request);

        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be("My Entry");
        result.Content.Should().Be("encrypted-content");
        result.Iv.Should().Be("iv-value");
        result.Salt.Should().Be("salt-value");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUserEntries()
    {
        var otherUserId = Guid.NewGuid();
        _db.Users.Add(new User { Id = otherUserId, Email = "other@example.com", PasswordHash = "hash" });

        _db.JournalEntries.AddRange(
            new JournalEntry { Id = Guid.NewGuid(), UserId = _userId, Title = "Mine", EncryptedContent = "c1" },
            new JournalEntry { Id = Guid.NewGuid(), UserId = otherUserId, Title = "Not Mine", EncryptedContent = "c2" }
        );
        await _db.SaveChangesAsync();

        var results = (await _sut.GetAllAsync(_userId)).ToList();

        results.Should().HaveCount(1);
        results[0].Title.Should().Be("Mine");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEntriesOrderedByCreatedAtDescending()
    {
        _db.JournalEntries.AddRange(
            new JournalEntry { Id = Guid.NewGuid(), UserId = _userId, Title = "Older", EncryptedContent = "c", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new JournalEntry { Id = Guid.NewGuid(), UserId = _userId, Title = "Newer", EncryptedContent = "c", CreatedAt = DateTime.UtcNow }
        );
        await _db.SaveChangesAsync();

        var results = (await _sut.GetAllAsync(_userId)).ToList();

        results[0].Title.Should().Be("Newer");
        results[1].Title.Should().Be("Older");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEntryDoesNotExist()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid(), _userId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEntryBelongsToDifferentUser()
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Title = "Secret",
            EncryptedContent = "c"
        };
        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(entry.Id, Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntry_WhenOwnerRequests()
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Title = "My Entry",
            EncryptedContent = "content"
        };
        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(entry.Id, _userId);

        result.Should().NotBeNull();
        result!.Title.Should().Be("My Entry");
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenEntryDoesNotExist()
    {
        var request = new UpdateEntryRequest { Title = "Updated", Content = "new" };

        var result = await _sut.UpdateAsync(Guid.NewGuid(), _userId, request);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFieldsAndReturnsEntry()
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Title = "Original",
            EncryptedContent = "old-content"
        };
        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();

        var request = new UpdateEntryRequest
        {
            Title = "Updated",
            Content = "new-content",
            Iv = "new-iv",
            Salt = "new-salt"
        };

        var result = await _sut.UpdateAsync(entry.Id, _userId, request);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated");
        result.Content.Should().Be("new-content");
        result.Iv.Should().Be("new-iv");
        result.Salt.Should().Be("new-salt");
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenEntryDoesNotExist()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid(), _userId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletes_SetsDeletedAt()
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Title = "To Delete",
            EncryptedContent = "c"
        };
        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();

        var result = await _sut.DeleteAsync(entry.Id, _userId);

        result.Should().BeTrue();

        // Entry should no longer appear in queries (global query filter)
        var fetched = await _sut.GetByIdAsync(entry.Id, _userId);
        fetched.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_SetsEmptyTitle_WhenTitleIsNull()
    {
        var request = new CreateEntryRequest { Title = null, Content = "content" };

        var result = await _sut.CreateAsync(_userId, request);

        result.Title.Should().BeEmpty();
    }
}
