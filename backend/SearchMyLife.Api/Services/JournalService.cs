using Microsoft.EntityFrameworkCore;
using SearchMyLife.Api.Data;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Models;

namespace SearchMyLife.Api.Services;

public class JournalService : IJournalService
{
    private readonly AppDbContext _db;

    public JournalService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EntryResponse>> GetAllAsync(Guid userId)
    {
        var entries = await _db.JournalEntries
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return entries.Select(EntryResponse.FromEntity);
    }

    public async Task<EntryResponse?> GetByIdAsync(Guid id, Guid userId)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        return entry is null ? null : EntryResponse.FromEntity(entry);
    }

    public async Task<EntryResponse> CreateAsync(Guid userId, CreateEntryRequest request)
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title ?? string.Empty,
            EncryptedContent = request.Content,
            Iv = request.Iv,
            Salt = request.Salt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();

        return EntryResponse.FromEntity(entry);
    }

    public async Task<EntryResponse?> UpdateAsync(Guid id, Guid userId, UpdateEntryRequest request)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (entry is null)
            return null;

        entry.Title = request.Title ?? string.Empty;
        entry.EncryptedContent = request.Content;
        entry.Iv = request.Iv;
        entry.Salt = request.Salt;
        entry.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return EntryResponse.FromEntity(entry);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (entry is null)
            return false;

        _db.JournalEntries.Remove(entry);
        await _db.SaveChangesAsync();

        return true;
    }
}
