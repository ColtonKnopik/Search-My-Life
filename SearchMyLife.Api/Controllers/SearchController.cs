using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SearchMyLife.Api.Data;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Models;
using SearchMyLife.Api.Services;

namespace SearchMyLife.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAiService _aiService;
    private readonly IVectorSearchService _vectorSearchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        AppDbContext db,
        IAiService aiService,
        IVectorSearchService vectorSearchService,
        ILogger<SearchController> logger)
    {
        _db = db;
        _aiService = aiService;
        _vectorSearchService = vectorSearchService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(sub!);
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        var userId = GetUserId();

        try
        {
        // Embed the search query
        var queryEmbedding = await _aiService.EmbedAsync(request.Query);

        // Search Azure AI Search for similar entries (fetch up to 13: 3 top + 10 other)
        var vectorResults = await _vectorSearchService.SearchAsync(userId, queryEmbedding, topK: 13);

        if (vectorResults.Count == 0)
            return Ok(new SearchResponse());

        // Fetch matching entries from SQLite
        var entryIds = vectorResults.Select(r => r.EntryId).ToList();
        var entries = await _db.JournalEntries
            .Where(e => entryIds.Contains(e.Id) && e.UserId == userId)
            .ToListAsync();

        // Map to response with scores, sorted by score descending
        var scoreLookup = vectorResults.ToDictionary(r => r.EntryId, r => r.Score);
        var allResults = entries
            .Select(e =>
            {
                var response = EntryResponse.FromEntity(e);
                return new SearchResultResponse
                {
                    Id = response.Id,
                    Title = response.Title,
                    Content = response.Content,
                    Iv = response.Iv,
                    Salt = response.Salt,
                    Emotion = response.Emotion,
                    SentimentScore = response.SentimentScore,
                    Tags = response.Tags,
                    CreatedAt = response.CreatedAt,
                    UpdatedAt = response.UpdatedAt,
                    Score = scoreLookup.GetValueOrDefault(e.Id, 0)
                };
            })
            .OrderByDescending(r => r.Score)
            .ToList();

        // Split into top 3 and the rest (up to 10)
        var topResults = allResults.Take(3).ToList();
        var otherResults = allResults.Skip(3).Take(10).ToList();

        // Generate AI summary for the top 3
        // Use DB entities directly — summary is internal and not included in response DTOs
        var entryById = entries.ToDictionary(e => e.Id);
        var overview = string.Empty;
        var topSummaries = topResults
            .Select(r => entryById.TryGetValue(r.Id, out var e) ? (e.Summary ?? r.Title) : r.Title)
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();

        if (topSummaries.Length > 0)
        {
            try
            {
                var searchSummary = await _aiService.SummarizeSearchAsync(request.Query, topSummaries);
                overview = searchSummary.Overview;

                for (var i = 0; i < Math.Min(topResults.Count, searchSummary.RelevanceExplanations.Length); i++)
                {
                    topResults[i].RelevanceReason = searchSummary.RelevanceExplanations[i];
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Search summary generation failed; returning results without AI overview.");
            }
        }

        return Ok(new SearchResponse
        {
            Overview = overview,
            TopResults = topResults,
            OtherResults = otherResults
        });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed for user {UserId}.", userId);
            return StatusCode(500, new { message = "Search failed. Check that OpenAI and Azure Search are configured in App Settings." });
        }
    }
}
