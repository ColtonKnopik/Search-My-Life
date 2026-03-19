using FluentAssertions;
using SearchMyLife.Api.Data;
using SearchMyLife.Api.Tests.Helpers;

namespace SearchMyLife.Api.Tests.Data;

public class DbSeederTests : IDisposable
{
    private readonly AppDbContext _db;

    public DbSeederTests()
    {
        _db = TestDbContextFactory.Create();
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task SeedAsync_CreatesUserAndEntries()
    {
        await DbSeeder.SeedAsync(_db);

        _db.Users.Should().HaveCount(1);
        _db.Users.First().Email.Should().Be("merchant@demo.com");

        _db.JournalEntries.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task SeedAsync_IsIdempotent_DoesNotDuplicateOnSecondCall()
    {
        await DbSeeder.SeedAsync(_db);
        var userCount = _db.Users.Count();
        var entryCount = _db.JournalEntries.Count();

        await DbSeeder.SeedAsync(_db);

        _db.Users.Should().HaveCount(userCount);
        _db.JournalEntries.Should().HaveCount(entryCount);
    }

    [Fact]
    public async Task SeedAsync_AllEntriesHaveSummariesAndTags()
    {
        await DbSeeder.SeedAsync(_db);

        var entries = _db.JournalEntries.ToList();
        entries.Should().AllSatisfy(e =>
        {
            e.Summary.Should().NotBeNullOrWhiteSpace();
            e.Tags.Should().NotBeNullOrWhiteSpace();
            e.Title.Should().NotBeNullOrWhiteSpace();
            e.EncryptedContent.Should().NotBeNullOrWhiteSpace();
            e.Emotion.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task SeedAsync_AllEntriesBelongToSeedUser()
    {
        await DbSeeder.SeedAsync(_db);

        var user = _db.Users.Single();
        var entries = _db.JournalEntries.ToList();
        entries.Should().AllSatisfy(e => e.UserId.Should().Be(user.Id));
    }
}
