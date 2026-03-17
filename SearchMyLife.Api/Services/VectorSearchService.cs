using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;
using SearchMyLife.Api.Config;

namespace SearchMyLife.Api.Services;

public class VectorSearchService : IVectorSearchService
{
    private readonly SearchIndexClient? _indexClient;
    private readonly SearchClient? _searchClient;
    private readonly string _indexName;
    private readonly ILogger<VectorSearchService> _logger;
    private readonly bool _isConfigured;

    private const int EmbeddingDimensions = 1536;

    public VectorSearchService(IOptions<AzureSearchSettings> settings, ILogger<VectorSearchService> logger)
    {
        _logger = logger;
        var options = settings.Value;
        _indexName = options.IndexName;

        if (string.IsNullOrEmpty(options.Endpoint) || string.IsNullOrEmpty(options.ApiKey))
        {
            _logger.LogWarning("Azure AI Search is not configured. Vector search will be unavailable.");
            _isConfigured = false;
            return;
        }

        _isConfigured = true;
        var credential = new AzureKeyCredential(options.ApiKey);
        _indexClient = new SearchIndexClient(new Uri(options.Endpoint), credential);
        _searchClient = new SearchClient(new Uri(options.Endpoint), _indexName, credential);
    }

    public async Task EnsureIndexExistsAsync()
    {
        if (!_isConfigured) return;
        var definition = new SearchIndex(_indexName)
        {
            Fields =
            {
                new SimpleField("entryId", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
                new SimpleField("userId", SearchFieldDataType.String) { IsFilterable = true },
                new SearchField("embedding", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    IsSearchable = true,
                    VectorSearchDimensions = EmbeddingDimensions,
                    VectorSearchProfileName = "embedding-profile"
                }
            },
            VectorSearch = new VectorSearch
            {
                Profiles =
                {
                    new VectorSearchProfile("embedding-profile", "hnsw-config")
                },
                Algorithms =
                {
                    new HnswAlgorithmConfiguration("hnsw-config")
                }
            }
        };

        try
        {
            await _indexClient!.CreateOrUpdateIndexAsync(definition);
            _logger.LogInformation("Azure AI Search index '{IndexName}' is ready.", _indexName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create or update Azure AI Search index '{IndexName}'. Search will be unavailable.", _indexName);
        }
    }

    public async Task UpsertEmbeddingAsync(Guid entryId, Guid userId, ReadOnlyMemory<float> embedding)
    {
        if (!_isConfigured) return;

        var doc = new SearchDocument
        {
            ["entryId"] = entryId.ToString(),
            ["userId"] = userId.ToString(),
            ["embedding"] = embedding.ToArray()
        };

        await _searchClient.MergeOrUploadDocumentsAsync(new[] { doc });
        _logger.LogDebug("Upserted embedding for entry {EntryId}.", entryId);
    }

    public async Task DeleteEmbeddingAsync(Guid entryId)
    {
        if (!_isConfigured) return;

        try
        {
            var doc = new SearchDocument { ["entryId"] = entryId.ToString() };
            await _searchClient.DeleteDocumentsAsync(new[] { doc });
            _logger.LogDebug("Deleted embedding for entry {EntryId}.", entryId);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogDebug("No embedding found to delete for entry {EntryId}.", entryId);
        }
    }

    public async Task<List<VectorSearchResult>> SearchAsync(
        Guid userId, ReadOnlyMemory<float> queryEmbedding, int topK = 10)
    {
        if (!_isConfigured) return [];

        var searchOptions = new SearchOptions
        {
            Filter = $"userId eq '{userId}'",
            Size = topK,
            Select = { "entryId" },
            VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(queryEmbedding)
                    {
                        KNearestNeighborsCount = topK,
                        Fields = { "embedding" }
                    }
                }
            }
        };

        var response = await _searchClient.SearchAsync<SearchDocument>(null, searchOptions);
        var results = new List<VectorSearchResult>();

        await foreach (var result in response.Value.GetResultsAsync())
        {
            if (result.Document.TryGetValue("entryId", out var entryIdObj)
                && Guid.TryParse(entryIdObj?.ToString(), out var entryId))
            {
                results.Add(new VectorSearchResult(entryId, result.Score ?? 0));
            }
        }

        return results;
    }
}
