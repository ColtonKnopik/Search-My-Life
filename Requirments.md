AI Memory Timeline – “Search My Life”

Elevator Pitch

Search my life is a privacy-first journaling web application that allows users to search their personal memories using semantic AI search instead of keywords. Users can ask natural language questions like “When did I last feel confident about school?” and receive relevant past entries. Journal entries are encrypted in the browser so even the developer cannot read them, while vector embeddings enable intelligent retrieval and AI-generated reflections.


The app helps users identify emotional patterns, track personal growth, and reflect meaningfully on their experiences.





Target Audience



    College students
    Professionals focused on self-improvement
    People who journal regularly
    Privacy-conscious users






Core Features




1. Secure Authentication



    User registration and login
    JWT-based authentication
    Per-user encrypted data storage






2. End-to-End Encrypted Journaling



    Entries encrypted in the browser (AES via Web Crypto API)
    Backend stores only encrypted content
    Developer cannot access plaintext entries
    Password loss means data cannot be recovered






3. AI Embeddings + Vector Search



    Journal entries converted into embeddings using Azure OpenAI
    Embeddings stored in vector-enabled database
    Natural language queries are embedded and compared via similarity search
    Retrieved entries decrypted client-side






4. Emotional Metadata & AI Insights



For each entry, AI generates:


    Sentiment score
    Emotion classification
    Tags
    Short summary



Users can:


    Search by emotion (“When was I stressed about exams?”)
    Generate weekly/monthly reflections
    Detect emotional trends over time






5. Interactive Timeline UI



    Clean, modern interface built with Vue + Vuetify
    Timeline view grouped by month
    Color-coded by emotion
    Semantic search bar
    Entry expansion and reflection generation






Use Cases



    Search for emotionally similar past experiences
    Generate AI-powered growth reflections
    Detect recurring stress patterns
    Compare emotional trends over time






Technical Stack




Frontend



    Vue 3 + Vuetify
    Web Crypto API (AES encryption)
    Component-based architecture
    Unit tests for core logic




Backend



    ASP.NET Core Web API
    Controllers, Services, DTOs, Dependency Injection
    Entity Framework Core
    JWT authentication
    Unit testing (xUnit or similar)




AI & Data



    Azure OpenAI (embeddings + completions)
    Azure SQL or PostgreSQL
    Vector search (Azure AI Search or pgvector)
    Azure Key Vault for secrets




Deployment



    Azure App Service
    CI/CD pipeline via GitHub Actions






Technical Requirements



    Vector search must be central to functionality
    AI-generated embeddings and insights required
    No plaintext journal entries stored in database
    Proper backend layering (Controllers, Services, DTOs)
    Frontend component architecture
    Unit tests for core services
    Deployed to Azure with CI/CD






Ethical Considerations



    AI emotional analysis may not be perfectly accurate
    Not a mental health diagnostic tool
    Clear privacy communication required
    Users are responsible for safeguarding passwords