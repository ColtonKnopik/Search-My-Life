# Search My Life

<div align="center">

![Vue](https://img.shields.io/badge/Vue-3-42b883.svg)
![Vuetify](https://img.shields.io/badge/Vuetify-4-1867C0.svg)
![dotnet](https://img.shields.io/badge/.NET-10-512BD4.svg)
![OpenAI](https://img.shields.io/badge/OpenAI-Embeddings%20%2B%20GPT-412991.svg)
![Azure](https://img.shields.io/badge/Azure-AI%20Search%20%2B%20App%20Service-0078D4.svg)

### [**OPEN APP**](https://journalsite.azurewebsites.net)

*A privacy-first journaling app that lets you search your own memories with natural language*

</div>

---

A privacy-first journaling web application that lets users search their personal memories using semantic AI search instead of keywords. Ask natural language questions like "When did I last feel confident about school?" and receive relevant past entries. Journal entries are encrypted in the browser so even the developer cannot read them, while vector embeddings enable intelligent retrieval and AI-generated reflections.

## Demo

[![Search My Life Demo](https://img.youtube.com/vi/6Zb0oCURV9c/0.jpg)](https://youtu.be/6Zb0oCURV9c)

## Features

- **End-to-End Encrypted Journaling** -- Entries are encrypted in the browser using AES via the Web Crypto API. The backend stores only ciphertext. Password loss means data cannot be recovered.
- **Semantic Search** -- Journal entries are converted to vector embeddings using OpenAI and stored in Azure AI Search. Natural language queries are embedded and compared via cosine similarity to find the most relevant entries.
- **AI-Powered Entry Analysis** -- Each entry is analyzed by GPT to extract emotion classification, sentiment score, descriptive tags, and a one-sentence summary. Plaintext is never persisted on the server.
- **Emotional Insights** -- Search by emotion, detect recurring stress patterns, and compare emotional trends over time.
- **Interactive Timeline UI** -- Clean, modern interface with timeline views grouped by month and color-coded by emotion.

## Architecture

```
+--------------+     +------------------+     +-------------+
|  Vue 3 SPA   |---->|  ASP.NET Core    |---->|   OpenAI    |
|  (browser)   |<----|  Web API         |<----|   API       |
+--------------+     +--------+---------+     +-------------+
                              |       |
                       +------+--+  +-+----------------+
                       | SQLite  |  | Azure AI Search  |
                       | (data)  |  | (vectors)        |
                       +---------+  +------------------+
```

- **SQLite** (dev) / **SQL Server** (prod) -- Stores users, encrypted content, and AI metadata
- **Azure AI Search** -- Stores entry embeddings for vector similarity search
- **OpenAI API** -- Generates completions (analysis) and embeddings (vectors)

### Privacy Flow

1. User saves an entry. The frontend encrypts the content and sends ciphertext to the backend for storage.
2. The frontend still has the plaintext in memory and calls `POST /api/entries/{id}/analyze` with it.
3. The backend sends plaintext to OpenAI, receives structured metadata and an embedding.
4. The backend stores metadata in the database and the embedding in Azure AI Search.
5. The backend discards the plaintext. It is never written to any database.

## Tech Stack

### Frontend

- Vue 3, Vue Router, Pinia
- Vuetify 4
- Web Crypto API (AES encryption)
- Axios
- Vite, Vitest

### Backend

- ASP.NET Core (.NET 10)
- Entity Framework Core (SQLite / SQL Server)
- JWT authentication (Bearer tokens)
- xUnit (unit tests)

### AI and Data

- OpenAI (`gpt-4o-mini` for analysis, `text-embedding-3-small` for embeddings)
- Azure AI Search (vector index with HNSW similarity)

### Deployment

- Azure App Service (two apps: API + frontend)
- GitHub Actions CI/CD (push to `main` triggers build and deploy)

## Project Structure

```
Search-My-Life/
  SearchMyLife.Api/          # ASP.NET Core Web API
    Config/                  # Settings classes (OpenAI, Azure Search)
    Controllers/             # Auth, Entries, Search, Analysis
    Data/                    # AppDbContext, DbSeeder
    DTOs/                    # Request/response models
    Models/                  # Entity models (User, JournalEntry)
    Services/                # AuthService, JournalService, AiService, VectorSearchService
  SearchMyLife.Api.Tests/    # xUnit backend tests
  SearchMyLife/              # Vue 3 SPA
    src/
      components/            # Reusable UI components
      router/                # Vue Router configuration
      services/              # API client (Axios)
      stores/                # Pinia stores (auth, journal, search)
      views/                 # Page-level components
      assets/                # Theme CSS
  .github/workflows/         # CI/CD pipelines
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- An [OpenAI API key](https://platform.openai.com/)
- An [Azure AI Search](https://portal.azure.com/) resource (Free tier works)

### Backend

1. Navigate to the API project:

   ```
   cd SearchMyLife.Api
   ```

2. Configure secrets. Add the following to `appsettings.Development.json` or use `dotnet user-secrets`:

   ```json
   {
     "OpenAI": {
       "ApiKey": "sk-..."
     },
     "AzureSearch": {
       "Endpoint": "https://<your-resource>.search.windows.net",
       "ApiKey": "<admin-key>",
       "IndexName": "journal-embeddings"
     }
   }
   ```

3. Run the API:

   ```
   dotnet run
   ```

   The database is created automatically on startup. Seed data is inserted if the database is empty.

### Frontend

1. Navigate to the frontend project:

   ```
   cd SearchMyLife
   ```

2. Install dependencies:

   ```
   npm install
   ```

3. Create a `.env` file (or set the environment variable) pointing to your local API:

   ```
   VITE_API_BASE_URL=https://localhost:5001
   ```

4. Start the dev server:

   ```
   npm run dev
   ```

### Running Tests

Backend:

```
dotnet test SearchMyLife.Api.Tests
```

Frontend:

```
cd SearchMyLife
npm run test:unit
```

## License

This project is provided as-is for educational and portfolio purposes.
