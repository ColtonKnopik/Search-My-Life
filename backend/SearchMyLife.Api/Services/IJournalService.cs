using SearchMyLife.Api.DTOs;

namespace SearchMyLife.Api.Services;

public interface IJournalService
{
    Task<IEnumerable<EntryResponse>> GetAllAsync(Guid userId);
    Task<EntryResponse?> GetByIdAsync(Guid id, Guid userId);
    Task<EntryResponse> CreateAsync(Guid userId, CreateEntryRequest request);
    Task<EntryResponse?> UpdateAsync(Guid id, Guid userId, UpdateEntryRequest request);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
