using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SearchMyLife.Api.Config;
using SearchMyLife.Api.Data;
using SearchMyLife.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// EF Core — use SQL Server if the connection string looks like one, otherwise SQLite.
// Set ConnectionStrings__DefaultConnection in Azure App Settings for production.
var rawConnStr = builder.Configuration.GetConnectionString("DefaultConnection")!;
var isSqlServer = rawConnStr.Contains("Server=", StringComparison.OrdinalIgnoreCase)
                  || rawConnStr.Contains("Data Source=tcp:", StringComparison.OrdinalIgnoreCase)
                  || rawConnStr.Contains("Initial Catalog", StringComparison.OrdinalIgnoreCase);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (isSqlServer)
        options.UseSqlServer(rawConnStr);
    else
    {
        // Resolve relative SQLite path from content root
        var connStr = rawConnStr.Contains(':') || rawConnStr.Contains('/')
            ? rawConnStr
            : rawConnStr.Replace("Data Source=", $"Data Source={builder.Environment.ContentRootPath}/");
        options.UseSqlite(connStr);
    }
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});

builder.Services.AddAuthorization();

// Application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJournalService, JournalService>();

// AI services
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<AzureSearchSettings>(builder.Configuration.GetSection("AzureSearch"));
builder.Services.AddSingleton<IAiService, AiService>();
builder.Services.AddSingleton<IVectorSearchService, VectorSearchService>();

// Controllers
builder.Services.AddControllers();

// CORS — origins read from config so Azure App Settings can override
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Auto-create database and seed development data on startup
using (var scope = app.Services.CreateScope())
{
    var startupLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
        startupLogger.LogInformation("Database ready.");

        // Safe schema update: add DeletedAt column to existing databases
        try
        {
            var isSqlite = db.Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true;
            var sql = isSqlite
                ? "ALTER TABLE JournalEntries ADD COLUMN DeletedAt TEXT NULL"
                : "IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.JournalEntries') AND name = N'DeletedAt') ALTER TABLE dbo.JournalEntries ADD DeletedAt DATETIME2 NULL";
            db.Database.ExecuteSqlRaw(sql);
            startupLogger.LogInformation("Schema up to date.");
        }
        catch (Exception migEx)
        {
            // Duplicate column means the migration already ran — expected on subsequent startups
            var alreadyExists = migEx.Message.Contains("duplicate column", StringComparison.OrdinalIgnoreCase)
                             || migEx.Message.Contains("already has a column", StringComparison.OrdinalIgnoreCase)
                             || migEx.Message.Contains("Column names in each table must be unique", StringComparison.OrdinalIgnoreCase);
            if (alreadyExists)
                startupLogger.LogInformation("DeletedAt column already present — skipping migration.");
            else
                startupLogger.LogWarning(migEx, "Schema migration failed — column may require manual migration.");
        }
    }
    catch (Exception ex)
    {
        startupLogger.LogError(ex, "Database initialisation failed. Check the connection string and firewall rules.");
    }

    if (app.Environment.IsDevelopment())
    {
        await DbSeeder.SeedAsync(db);
    }
}

// Ensure Azure AI Search index exists
var vectorSearch = app.Services.GetRequiredService<IVectorSearchService>();
try
{
    await vectorSearch.EnsureIndexExistsAsync();
}
catch (Exception ex)
{
    var startupLog = app.Services.GetRequiredService<ILogger<Program>>();
    startupLog.LogError(ex, "Azure AI Search index setup failed. Search will be unavailable.");
}

// Seed embeddings for existing seed data (dev only, safe to re-run)
if (app.Environment.IsDevelopment())
{
    var aiConfig = app.Configuration.GetSection("OpenAI")["ApiKey"];
    if (!string.IsNullOrEmpty(aiConfig))
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var aiService = app.Services.GetRequiredService<IAiService>();
        var seederLogger = app.Services.GetRequiredService<ILogger<Program>>();
        await DbSeeder.SeedEmbeddingsAsync(db, aiService, vectorSearch, seederLogger);
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
