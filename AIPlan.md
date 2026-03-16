# AI Integration Plan — "Search My Life"

## Overview

This plan adds two AI capabilities to the existing journal app:

1. **Entry Analysis** — When a user saves a journal entry, the frontend sends the plaintext to the backend, which calls OpenAI GPT to extract emotion, sentiment, tags, and a summary. The backend stores only the structured metadata (never the plaintext).
2. **Semantic Search** — Entries are converted to vector embeddings and stored in Azure AI Search. When a user searches with natural language, the query is embedded and compared against stored vectors to find the most relevant entries.

### Architecture

```
┌─────────────┐     ┌──────────────────┐     ┌─────────────┐
│  Vue 3 SPA  │────▶│  ASP.NET Core    │────▶│   OpenAI    │
│  (browser)  │◀────│  Web API         │◀────│   API       │
└─────────────┘     └──────┬───────────┘     └─────────────┘
                           │       │
                    ┌──────┴──┐  ┌─┴──────────────┐
                    │ SQLite  │  │ Azure AI Search │
                    │ (data)  │  │ (vectors)       │
                    └─────────┘  └─────────────────┘
```

- **SQLite** — stores users, encrypted content, and AI metadata (emotion, tags, etc.)
- **Azure AI Search** — stores entry embeddings for vector similarity search
- **OpenAI API** — generates completions (analysis) and embeddings (vectors)

### Privacy Flow

The backend **never stores plaintext** journal content. The flow for analysis:

1. User saves entry → frontend encrypts content → backend stores ciphertext in SQLite
2. Frontend still has the plaintext in memory → calls `POST /api/entries/{id}/analyze` with plaintext
3. Backend sends plaintext to OpenAI → receives structured metadata + embedding
4. Backend stores metadata in SQLite, embedding in Azure AI Search
5. Backend discards the plaintext — it is never written to any database

---

## Step 1: External Service Setup

### 1A — OpenAI API Key

1. Go to [platform.openai.com](https://platform.openai.com) and create an account (or log in)
2. Navigate to **API Keys** → **Create new secret key**
3. Copy the key (starts with `sk-`). You will not see it again.
4. Add billing (Settings → Billing → add a payment method). GPT-5-mini and text-embedding-3-small are pay-per-token; expected cost for this app is **<$1/month**.

**Models used:**

| Purpose | Model | Cost |
|---|---|---|
| Entry analysis (emotion, sentiment, tags, summary) | `gpt-5-mini` | ~$0.0001 per entry |
| Embeddings (for vector search) | `text-embedding-3-small` | ~$0.00002 per entry |

### 1B — Azure AI Search Resource

1. Go to [Azure Portal](https://portal.azure.com)
2. **Create a resource** → search for **"Azure AI Search"**
3. Configure:
   - **Resource group**: use the same one as your App Services
   - **Service name**: `searchmylife-search` (or similar — must be globally unique)
   - **Region**: same region as your App Services
   - **Pricing tier**: **Free (F)** — provides 3 indexes, 50 MB storage (plenty for this app)
4. Click **Review + Create** → **Create**
5. Once deployed, go to the resource:
   - **Overview** → copy the **URL** (e.g., `https://searchmylife-search.search.windows.net`)
   - **Settings → Keys** → copy the **Primary admin key**

### 1C — Azure App Settings for JournalSiteAPI

In **Azure Portal → App Services → JournalSiteAPI → Configuration → Application settings**, add:

| Name | Value | Notes |
|---|---|---|
| `OpenAI__ApiKey` | `sk-...` | Your OpenAI secret key |
| `OpenAI__CompletionModel` | `gpt-5-mini` | Model for analysis |
| `OpenAI__EmbeddingModel` | `text-embedding-3-small` | Model for embeddings |
| `AzureSearch__Endpoint` | `https://searchmylife-search.search.windows.net` | From Azure AI Search overview |
| `AzureSearch__ApiKey` | `(admin key)` | From Azure AI Search keys page |
| `AzureSearch__IndexName` | `journal-embeddings` | Index name (code will create it) |

**Save** and **Restart** the API app after adding these settings.

---

## Step 2: Backend Code Changes

### 2A — New NuGet Packages

Add to `SearchMyLife.Api.csproj`:

| Package | Purpose |
|---|---|
| `OpenAI` | Official OpenAI .NET SDK — completions + embeddings |
| `Azure.Search.Documents` | Azure AI Search SDK — index management + vector queries |

### 2B — Configuration Classes

Create `SearchMyLife.Api/Config/OpenAISettings.cs`:
```csharp
public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string CompletionModel { get; set; } = "gpt-5-mini";
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}
```

Create `SearchMyLife.Api/Config/AzureSearchSettings.cs`:
```csharp
public class AzureSearchSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string IndexName { get; set; } = "journal-embeddings";
}
```

### 2C — AI Service (OpenAI Calls)

Create `Services/IAiService.cs` and `Services/AiService.cs`.

**Interface:**
```csharp
public interface IAiService
{
    Task<EntryAnalysis> AnalyzeAsync(string plaintext);
    Task<ReadOnlyMemory<float>> EmbedAsync(string text);
}
```

**`AnalyzeAsync`** — Sends the plaintext to GPT-5-mini with a structured prompt:
```
You are an AI journal analyst. Given the following journal entry, extract:
- emotion: one of (happy, sad, anxious, stressed, calm, excited, grateful, neutral)
- sentimentScore: a number from -1.0 (very negative) to 1.0 (very positive)
- tags: an array of 2-5 short descriptive tags
- summary: a one-sentence summary

Respond with valid JSON only. No markdown, no explanation.
```

Returns a parsed `EntryAnalysis` record:
```csharp
public record EntryAnalysis(
    string Emotion,
    double SentimentScore,
    string[] Tags,
    string Summary
);
```

**`EmbedAsync`** — Calls the OpenAI embeddings endpoint with `text-embedding-3-small`, returns a `ReadOnlyMemory<float>` (1536-dimensional vector).

### 2D — Vector Search Service (Azure AI Search Calls)

Create `Services/IVectorSearchService.cs` and `Services/VectorSearchService.cs`.

**Interface:**
```csharp
public interface IVectorSearchService
{
    Task EnsureIndexExistsAsync();
    Task UpsertEmbeddingAsync(Guid entryId, Guid userId, ReadOnlyMemory<float> embedding);
    Task DeleteEmbeddingAsync(Guid entryId);
    Task<List<VectorSearchResult>> SearchAsync(Guid userId, ReadOnlyMemory<float> queryEmbedding, int topK = 10);
}
```

**Index schema** (created programmatically on app startup):

| Field | Type | Purpose |
|---|---|---|
| `entryId` | `string` (key) | Links back to SQLite entry |
| `userId` | `string` (filterable) | Ensures user isolation |
| `embedding` | `Collection(Single)` (1536 dims) | Vector for HNSW similarity |

**`SearchAsync`** — Filters by `userId`, runs vector similarity query, returns entry IDs + scores:
```csharp
public record VectorSearchResult(Guid EntryId, double Score);
```

### 2E — New DTOs

Create `DTOs/AnalyzeRequest.cs`:
```csharp
public class AnalyzeRequest
{
    [Required]
    public string Plaintext { get; set; } = string.Empty;
}
```

Create `DTOs/AnalyzeResponse.cs`:
```csharp
public class AnalyzeResponse
{
    public string Emotion { get; set; } = string.Empty;
    public double SentimentScore { get; set; }
    public string[] Tags { get; set; } = [];
    public string Summary { get; set; } = string.Empty;
}
```

Create `DTOs/SearchRequest.cs`:
```csharp
public class SearchRequest
{
    [Required]
    public string Query { get; set; } = string.Empty;
}
```

Create `DTOs/SearchResultResponse.cs` (extends `EntryResponse` with a score):
```csharp
public class SearchResultResponse : EntryResponse
{
    public double Score { get; set; }
}
```

### 2F — Analysis Controller

Create `Controllers/AnalysisController.cs`:

| Method | Route | What it does |
|---|---|---|
| `POST` | `/api/entries/{id}/analyze` | Accepts `{ plaintext }`, calls `IAiService.AnalyzeAsync` + `IAiService.EmbedAsync`, updates the entry's metadata in SQLite, upserts embedding in Azure AI Search, returns `AnalyzeResponse` |

**Flow inside the endpoint:**
1. Verify the entry belongs to the authenticated user
2. Call `_aiService.AnalyzeAsync(request.Plaintext)` → get emotion, sentiment, tags, summary
3. Call `_aiService.EmbedAsync(analysis.Summary + " " + string.Join(" ", analysis.Tags))` → get embedding vector
4. Update `JournalEntry` in SQLite: set `Emotion`, `SentimentScore`, `Summary`, `Tags`
5. Call `_vectorSearchService.UpsertEmbeddingAsync(entryId, userId, embedding)`
6. Return the `AnalyzeResponse`

**Important:** The `request.Plaintext` is never saved to any database. It exists only in memory for the duration of this request.

### 2G — Search Controller

Create `Controllers/SearchController.cs`:

| Method | Route | What it does |
|---|---|---|
| `POST` | `/api/search` | Accepts `{ query }`, embeds it, searches Azure AI Search for top matches, fetches those entries from SQLite, returns `SearchResultResponse[]` |

**Flow inside the endpoint:**
1. Call `_aiService.EmbedAsync(request.Query)` → get query embedding
2. Call `_vectorSearchService.SearchAsync(userId, queryEmbedding, topK: 10)` → get `List<VectorSearchResult>` (entry IDs + scores)
3. Fetch those entries from SQLite by ID (scoped to user)
4. Map to `SearchResultResponse` (includes all `EntryResponse` fields + `Score`)
5. Return the list sorted by score descending

### 2H — Program.cs Changes

Add to DI registration (after existing service registrations):
```csharp
// AI services
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<AzureSearchSettings>(builder.Configuration.GetSection("AzureSearch"));
builder.Services.AddSingleton<IAiService, AiService>();
builder.Services.AddSingleton<IVectorSearchService, VectorSearchService>();
```

Add to startup (after `db.Database.EnsureCreated()`):
```csharp
// Ensure Azure AI Search index exists
var vectorSearch = app.Services.GetRequiredService<IVectorSearchService>();
await vectorSearch.EnsureIndexExistsAsync();
```

### 2I — Modify EntriesController (Delete)

When an entry is deleted, also remove its embedding from Azure AI Search:
```csharp
// In the Delete action, after successful SQLite delete:
await _vectorSearchService.DeleteEmbeddingAsync(id);
```

This requires injecting `IVectorSearchService` into `EntriesController`.

---

## Step 3: Frontend Changes

### 3A — Call Analyze After Save

In `journalStore.js`, after `createEntry` and `updateEntry` return successfully, call the analyze endpoint with the plaintext content (which the frontend still has in memory):

```javascript
async function analyzeEntry(id, plaintext) {
    try {
        const response = await apiClient.post(`/entries/${id}/analyze`, { plaintext })
        // Update the local entry with AI metadata
        const index = entries.value.findIndex((e) => e.id === id)
        if (index !== -1) {
            entries.value[index] = { ...entries.value[index], ...response.data }
        }
        return response.data
    } catch (err) {
        // Analysis failure is non-critical — entry is already saved
        console.warn('AI analysis failed:', err.message)
    }
}
```

Call this from `JournalEntryView.vue` after saving — the save and the analysis are separate operations so a failed analysis doesn't block the save.

### 3B — Search (Already Wired)

The `searchStore.js` already calls `POST /api/search` with `{ query }` and the `SearchResultsView.vue` already renders `entry.score` as a percentage match badge. No frontend search changes needed.

---

## Step 4: Seed Data Migration

The existing `DbSeeder.cs` entries already have `Emotion`, `SentimentScore`, `Summary`, and `Tags` populated. After the AI integration is deployed:

1. The seeder entries will have metadata but no embeddings in Azure AI Search
2. To populate embeddings for existing entries, add a one-time migration step in the seeder or create a manual endpoint

**Option:** Add to `DbSeeder.SeedAsync()` after creating entries:
```csharp
// Generate embeddings for seed data
var aiService = db.GetService<IAiService>(); // or pass it in
foreach (var entry in entries)
{
    var text = entry.Summary + " " + entry.Tags;
    var embedding = await aiService.EmbedAsync(text);
    await vectorSearchService.UpsertEmbeddingAsync(entry.Id, entry.UserId, embedding);
}
```

This only runs in Development when the database is first created.

---

## Step 5: Azure App Settings Summary

All settings needed on **JournalSiteAPI** in Azure Portal:

| Setting | Value | Purpose |
|---|---|---|
| `OpenAI__ApiKey` | `sk-...` | OpenAI API authentication |
| `OpenAI__CompletionModel` | `gpt-5-mini` | Model for journal analysis |
| `OpenAI__EmbeddingModel` | `text-embedding-3-small` | Model for vector embeddings |
| `AzureSearch__Endpoint` | `https://<name>.search.windows.net` | Azure AI Search endpoint |
| `AzureSearch__ApiKey` | `(admin key)` | Azure AI Search authentication |
| `AzureSearch__IndexName` | `journal-embeddings` | Name of the vector index |
| `ConnectionStrings__DefaultConnection` | `Data Source=/home/data/searchmylife.db` | Persistent SQLite path |
| `Jwt__Key` | `(strong random key)` | JWT signing key |
| `AllowedOrigins__0` | `https://journalsite.azurewebsites.net` | CORS allowed origin |

For **local development**, add an `appsettings.Development.json` (git-ignored) with your OpenAI key and Azure Search credentials so you can test locally.

---

## File Summary

### New Files

| File | Purpose |
|---|---|
| `Config/OpenAISettings.cs` | OpenAI configuration POCO |
| `Config/AzureSearchSettings.cs` | Azure AI Search configuration POCO |
| `Services/IAiService.cs` | Interface for OpenAI calls |
| `Services/AiService.cs` | Implementation: completions + embeddings |
| `Services/IVectorSearchService.cs` | Interface for Azure AI Search calls |
| `Services/VectorSearchService.cs` | Implementation: index management, upsert, search |
| `DTOs/AnalyzeRequest.cs` | `{ Plaintext }` request body |
| `DTOs/AnalyzeResponse.cs` | `{ Emotion, SentimentScore, Tags[], Summary }` |
| `DTOs/SearchRequest.cs` | `{ Query }` request body |
| `DTOs/SearchResultResponse.cs` | `EntryResponse` + `Score` |
| `Controllers/AnalysisController.cs` | `POST /api/entries/{id}/analyze` |
| `Controllers/SearchController.cs` | `POST /api/search` |

### Modified Files

| File | Change |
|---|---|
| `SearchMyLife.Api.csproj` | Add `OpenAI` and `Azure.Search.Documents` NuGet packages |
| `Program.cs` | Register AI services, configure settings, ensure search index on startup |
| `Controllers/EntriesController.cs` | Inject `IVectorSearchService`, delete embedding on entry delete |
| `Data/DbSeeder.cs` | Optionally generate embeddings for seed entries |
| `appsettings.json` | Add `OpenAI` and `AzureSearch` config sections (empty defaults) |

### Frontend Changes

| File | Change |
|---|---|
| `stores/journalStore.js` | Add `analyzeEntry(id, plaintext)` function |
| `views/JournalEntryView.vue` | Call `analyzeEntry` after successful save |

---

## Implementation Order

1. **Create Azure AI Search resource** (portal, 5 min)
2. **Get OpenAI API key** (platform.openai.com, 5 min)
3. **Add NuGet packages** (`OpenAI`, `Azure.Search.Documents`)
4. **Build `AiService`** — OpenAI completions + embeddings
5. **Build `VectorSearchService`** — Azure AI Search index + upsert + query
6. **Build `AnalysisController`** — `POST /api/entries/{id}/analyze`
7. **Build `SearchController`** — `POST /api/search`
8. **Wire up `Program.cs`** — DI, config, startup index creation
9. **Update `EntriesController`** — delete embedding on entry delete
10. **Update frontend** — call analyze after save
11. **Add Azure App Settings** — all keys from Step 5
12. **Push and deploy** — CI/CD handles the rest
