using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SearchMyLife.Api.Data;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Services;

namespace SearchMyLife.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/entries/{id:guid}/analyze")]
public class AnalysisController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAiService _aiService;
    private readonly IVectorSearchService _vectorSearchService;
    private readonly ILogger<AnalysisController> _logger;

    public AnalysisController(
        AppDbContext db,
        IAiService aiService,
        IVectorSearchService vectorSearchService,
        ILogger<AnalysisController> logger)
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
    public async Task<IActionResult> Analyze(Guid id, [FromBody] AnalyzeRequest request)
    {
        var userId = GetUserId();

        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (entry is null)
            return NotFound(new { message = "Entry not found." });

        // Call OpenAI for analysis
        var analysis = await _aiService.AnalyzeAsync(request.Plaintext);

        // Update entry metadata in SQLite
        entry.Emotion = analysis.Emotion;
        entry.SentimentScore = analysis.SentimentScore;
        entry.Summary = analysis.Summary;
        entry.Tags = JsonSerializer.Serialize(analysis.Tags);
        entry.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // Generate embedding from summary + tags for semantic search
        var embeddingText = analysis.Summary + " " + string.Join(" ", analysis.Tags);
        var embedding = await _aiService.EmbedAsync(embeddingText);

        // Store embedding in Azure AI Search
        await _vectorSearchService.UpsertEmbeddingAsync(id, userId, embedding);

        _logger.LogInformation("Analyzed entry {EntryId} for user {UserId}.", id, userId);

        return Ok(new AnalyzeResponse
        {
            Emotion = analysis.Emotion,
            SentimentScore = analysis.SentimentScore,
            Tags = analysis.Tags,
            Summary = analysis.Summary
        });
    }
}
