namespace SearchMyLife.Api.Services;

public record VectorSearchResult(Guid EntryId, double Score);

public interface IVectorSearchService
{
    Task EnsureIndexExistsAsync();
    Task UpsertEmbeddingAsync(Guid entryId, Guid userId, ReadOnlyMemory<float> embedding);
    Task DeleteEmbeddingAsync(Guid entryId);
    Task<List<VectorSearchResult>> SearchAsync(Guid userId, ReadOnlyMemory<float> queryEmbedding, int topK = 10);
}
