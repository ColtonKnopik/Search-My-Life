# Backend Development Plan — "Search My Life"

## Current State
Phase 1 (Authentication) is complete:
- `User` model, `AppDbContext`, JWT auth, `AuthController` with login/register
- SQLite via EF Core, BCrypt password hashing
- Running on `http://localhost:5000`

---

## Phase 2: Journal Entries — CRUD API
**Goal:** Store encrypted journal entries per-user with full CRUD operations.

The frontend sends `{ title, content }` — the content will eventually be encrypted client-side (ciphertext). The backend stores it as-is and never interprets the content field.

### Model

| Field | Type | Notes |
|---|---|---|
| `Id` | `Guid` | Primary key |
| `UserId` | `Guid` | Foreign key → `User.Id` |
| `Title` | `string` | Optional, max 256 chars |
| `EncryptedContent` | `string` | Ciphertext from the frontend (required) |
| `Iv` | `string` | Base64 initialization vector for AES-GCM |
| `Salt` | `string` | Base64 salt for PBKDF2 key derivation |
| `Emotion` | `string?` | AI-classified emotion (nullable, set later) |
| `SentimentScore` | `double?` | AI sentiment score (nullable, set later) |
| `Summary` | `string?` | AI-generated short summary (nullable, set later) |
| `Tags` | `string?` | JSON array of tags stored as string (nullable, set later) |
| `CreatedAt` | `DateTime` | UTC timestamp |
| `UpdatedAt` | `DateTime` | UTC timestamp |

### Files to Create/Modify

| File | Action | Details |
|---|---|---|
| `Models/JournalEntry.cs` | **Create** | Entity with fields above, navigation property to `User` |
| `Data/AppDbContext.cs` | **Modify** | Add `DbSet<JournalEntry>`, configure relationship `User` → many `JournalEntry` |
| `DTOs/CreateEntryRequest.cs` | **Create** | `{ Title, Content, Iv, Salt }` with validation |
| `DTOs/UpdateEntryRequest.cs` | **Create** | `{ Title, Content, Iv, Salt }` with validation |
| `DTOs/EntryResponse.cs` | **Create** | Maps entity → JSON shape the frontend expects (`id`, `title`, `content`, `iv`, `salt`, `emotion`, `sentimentScore`, `summary`, `tags`, `createdAt`, `updatedAt`) |
| `Services/IJournalService.cs` | **Create** | Interface: `GetAllAsync(userId)`, `GetByIdAsync(id, userId)`, `CreateAsync(userId, request)`, `UpdateAsync(id, userId, request)`, `DeleteAsync(id, userId)` |
| `Services/JournalService.cs` | **Create** | Implementation — all queries scoped to `userId` for data isolation |
| `Controllers/EntriesController.cs` | **Create** | `[Authorize]` controller at `api/entries` with GET, GET/:id, POST, PUT/:id, DELETE/:id |
| `Program.cs` | **Modify** | Register `IJournalService` / `JournalService` in DI |

### API Endpoints

| Method | Route | Request Body | Response | Auth |
|---|---|---|---|---|
| `GET` | `/api/entries` | — | `EntryResponse[]` | ✅ |
| `GET` | `/api/entries/{id}` | — | `EntryResponse` | ✅ |
| `POST` | `/api/entries` | `CreateEntryRequest` | `EntryResponse` | ✅ |
| `PUT` | `/api/entries/{id}` | `UpdateEntryRequest` | `EntryResponse` | ✅ |
| `DELETE` | `/api/entries/{id}` | — | `204 No Content` | ✅ |

### Key Design Decisions
- All queries are filtered by the authenticated user's ID (extracted from JWT `sub` claim) — a user can never access another user's entries
- The `EncryptedContent`, `Iv`, and `Salt` fields store exactly what the frontend sends — the backend never decrypts
- `Emotion`, `SentimentScore`, `Summary`, and `Tags` are nullable — they will be populated by the AI pipeline in Phase 4
- Entries are returned sorted by `CreatedAt` descending (newest first)

---

## Phase 3: Semantic Search API
**Goal:** Accept a natural-language query, generate an embedding, and return the most relevant entries via vector similarity search.

### Dependencies
- **Azure OpenAI** — `text-embedding-ada-002` (or `text-embedding-3-small`) for generating embeddings
- **Vector storage** — either add a `pgvector` column to PostgreSQL or use an in-process vector distance calculation on SQLite for dev (upgrade to Azure AI Search or PostgreSQL for production)

### Model Changes

| Field | Type | Notes |
|---|---|---|
| `JournalEntry.Embedding` | `string?` | Stored as JSON array of floats (e.g., `"[0.012, -0.034, ...]"`) |

### Files to Create/Modify

| File | Action | Details |
|---|---|---|
| `Models/JournalEntry.cs` | **Modify** | Add `Embedding` property |
| `Services/IEmbeddingService.cs` | **Create** | Interface: `GenerateEmbeddingAsync(text)` → `float[]` |
| `Services/EmbeddingService.cs` | **Create** | Calls Azure OpenAI embeddings endpoint |
| `Services/ISearchService.cs` | **Create** | Interface: `SearchAsync(userId, query, topK)` → `SearchResult[]` |
| `Services/SearchService.cs` | **Create** | Generates embedding for query, computes cosine similarity against user's entry embeddings, returns top-K matches with scores |
| `DTOs/SearchRequest.cs` | **Create** | `{ Query }` with validation |
| `DTOs/SearchResultResponse.cs` | **Create** | Extends `EntryResponse` with `Score` (similarity match percentage) |
| `Controllers/SearchController.cs` | **Create** | `[Authorize]` controller at `api/search` with POST |
| `Program.cs` | **Modify** | Register embedding and search services, add Azure OpenAI config |
| `appsettings.json` | **Modify** | Add `AzureOpenAI` section (`Endpoint`, `ApiKey`, `EmbeddingDeployment`) |

### API Endpoints

| Method | Route | Request Body | Response | Auth |
|---|---|---|---|---|
| `POST` | `/api/search` | `SearchRequest` | `SearchResultResponse[]` | ✅ |

### Embedding Pipeline
When a journal entry is created or updated:
1. The backend **does not** embed the encrypted content (it can't decrypt it)
2. The frontend must send a separate plaintext field (like `summary` or `tags`) that the AI can embed — OR the embedding is generated from the AI metadata fields after they are set
3. **Alternative approach:** The frontend sends a plaintext "searchable hint" alongside the encrypted content — a short phrase the user provides to make the entry findable (this preserves privacy while enabling search)

> **Decision point:** Determine whether embeddings are generated from AI metadata (Phase 4 dependency) or from a user-supplied searchable hint (no dependency). Document the chosen approach before implementation.

---

## Phase 4: AI Emotional Metadata
**Goal:** Analyze journal entries to generate emotion classification, sentiment scores, tags, and summaries.

### Dependencies
- **Azure OpenAI** — GPT-4o or GPT-4o-mini for completions

### Privacy Consideration
The backend stores only encrypted content. To generate AI metadata, there are two approaches:
1. **Client-side AI call** — The frontend decrypts the content, calls an Azure OpenAI proxy endpoint on the backend with the plaintext, receives metadata, and then the backend stores only the metadata (not the plaintext). The plaintext never persists on the server.
2. **Proxy endpoint** — A dedicated endpoint that accepts plaintext, generates metadata + embedding, returns it to the frontend, and stores only the structured metadata.

### Files to Create/Modify

| File | Action | Details |
|---|---|---|
| `Services/IAiAnalysisService.cs` | **Create** | Interface: `AnalyzeEntryAsync(plaintext)` → `EntryAnalysis` (emotion, sentiment, tags, summary) |
| `Services/AiAnalysisService.cs` | **Create** | Calls Azure OpenAI completions with a structured prompt, parses JSON response |
| `DTOs/AnalyzeEntryRequest.cs` | **Create** | `{ Plaintext }` — sent from the frontend after client-side decryption |
| `DTOs/EntryAnalysisResponse.cs` | **Create** | `{ Emotion, SentimentScore, Tags[], Summary, Embedding }` |
| `Controllers/AnalysisController.cs` | **Create** | `[Authorize]` POST at `api/entries/{id}/analyze` — accepts plaintext, generates metadata, updates the entry's metadata fields + embedding, returns analysis |
| `Program.cs` | **Modify** | Register `IAiAnalysisService` |
| `appsettings.json` | **Modify** | Add `CompletionDeployment` to `AzureOpenAI` section |

### API Endpoints

| Method | Route | Request Body | Response | Auth |
|---|---|---|---|---|
| `POST` | `/api/entries/{id}/analyze` | `AnalyzeEntryRequest` | `EntryAnalysisResponse` | ✅ |

### Flow
1. User creates/edits an entry → encrypted content saved via Phase 2 CRUD
2. Frontend decrypts the content client-side
3. Frontend calls `POST /api/entries/{id}/analyze` with the plaintext
4. Backend sends plaintext to Azure OpenAI → receives emotion, sentiment, tags, summary
5. Backend generates embedding from the summary + tags
6. Backend updates the `JournalEntry` with metadata + embedding (plaintext is **not** stored)
7. Backend returns the analysis to the frontend for display

---

## Phase 5: Unit Testing
**Goal:** Comprehensive test coverage for all backend services.

### Files to Create

| File | Tests |
|---|---|
| `Tests/AuthServiceTests.cs` | Register (success, duplicate email), Login (success, wrong password, unknown email) |
| `Tests/JournalServiceTests.cs` | CRUD operations, user isolation (can't access other user's entries), not-found cases |
| `Tests/SearchServiceTests.cs` | Cosine similarity calculation, result ordering, empty results |
| `Tests/AiAnalysisServiceTests.cs` | Prompt construction, response parsing, error handling |
| `Tests/AuthControllerTests.cs` | HTTP status codes, validation errors, response shapes |
| `Tests/EntriesControllerTests.cs` | HTTP status codes, auth requirement, response shapes |

### Setup
- Create `SearchMyLife.Api.Tests` xUnit project
- Use EF Core in-memory SQLite for database tests
- Mock `IEmbeddingService` and `IAiAnalysisService` (external API dependencies)

---

## Phase 6: Production Readiness
**Goal:** Configuration and infrastructure for Azure deployment.

| Task | Details |
|---|---|
| Switch to PostgreSQL | Replace SQLite with `Npgsql.EntityFrameworkCore.PostgreSQL`, add EF migration |
| Azure Key Vault | Move JWT key, Azure OpenAI secrets, and connection string out of `appsettings.json` |
| EF Migrations | Switch from `EnsureCreated()` to proper migrations (`dotnet ef migrations add`) |
| Health check endpoint | `GET /api/health` for Azure App Service probes |
| Logging | Structured logging with Application Insights |
| CI/CD | GitHub Actions workflow: build → test → publish → deploy to Azure App Service |

---

## Recommended Implementation Order
1. **Phase 2** — Journal CRUD (direct frontend dependency, no AI needed yet)
2. **Phase 3** — Search API skeleton (can stub with keyword search, wire up embeddings later)
3. **Phase 4** — AI analysis endpoint (requires Azure OpenAI access)
4. **Phase 5** — Unit tests (write alongside each phase, formalize here)
5. **Phase 6** — Production prep (last, after features work end-to-end)
