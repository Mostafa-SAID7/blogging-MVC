# Blogging Agent Stack - Complete Implementation Guide

## ?? Enhanced Project Structure

```
BloggingAgent/
??? Agents/
?   ??? BloggingAgent.cs
?   ??? IBloggingAgent.cs
?   ??? Prompts/
?       ??? BlogPromptTemplates.cs
??? Services/
?   ??? LLM/
?   ?   ??? ILlmConnector.cs
?   ?   ??? LlmConnector.cs
?   ?   ??? OllamaProvider.cs
?   ?   ??? OpenAIProvider.cs
?   ?   ??? ILlmProvider.cs
?   ??? Memory/
?   ?   ??? IMemoryService.cs
?   ?   ??? MemoryService.cs
?   ?   ??? MemoryAnalyzer.cs
?   ??? SEO/
?   ?   ??? ISeoService.cs
?   ?   ??? SeoService.cs
?   ?   ??? SeoAnalyzer.cs
?   ??? Content/
?   ?   ??? IContentFormatter.cs
?   ?   ??? ContentFormatter.cs
?   ?   ??? MarkdownProcessor.cs
?   ??? Cache/
?       ??? ICacheService.cs
?       ??? MemoryCacheService.cs
??? Controllers/
?   ??? BlogController.cs
?   ??? AnalyticsController.cs
?   ??? SettingsController.cs
??? Models/
?   ??? Domain/
?   ?   ??? BlogPost.cs
?   ?   ??? AgentMemory.cs
?   ?   ??? SeoMetadata.cs
?   ?   ??? ContentAnalytics.cs
?   ?   ??? AgentSettings.cs
?   ??? DTOs/
?   ?   ??? BlogPostDto.cs
?   ?   ??? GeneratePostRequest.cs
?   ?   ??? SeoAnalysisResult.cs
?   ??? ViewModels/
?       ??? BlogIndexViewModel.cs
?       ??? BlogDetailViewModel.cs
?       ??? AnalyticsViewModel.cs
?       ??? SettingsViewModel.cs
??? Data/
?   ??? ApplicationDbContext.cs
?   ??? Repositories/
?   ?   ??? IRepository.cs
?   ?   ??? Repository.cs
?   ?   ??? IBlogPostRepository.cs
?   ?   ??? BlogPostRepository.cs
?   ??? Migrations/
??? Views/
?   ??? Blog/
?   ?   ??? Index.cshtml
?   ?   ??? Details.cshtml
?   ?   ??? Edit.cshtml
?   ?   ??? _PostCard.cshtml
?   ??? Analytics/
?   ?   ??? Index.cshtml
?   ??? Settings/
?   ?   ??? Index.cshtml
?   ??? Shared/
?   ?   ??? _Layout.cshtml
?   ?   ??? _Header.cshtml
?   ?   ??? _Footer.cshtml
?   ?   ??? _ValidationScriptsPartial.cshtml
?   ?   ??? Error.cshtml
?   ??? _ViewImports.cshtml
?   ??? _ViewStart.cshtml
??? wwwroot/
?   ??? css/
?   ?   ??? site.css
?   ?   ??? blog.css
?   ?   ??? analytics.css
?   ??? js/
?   ?   ??? site.js
?   ?   ??? blog-generator.js
?   ?   ??? analytics.js
?   ??? lib/
?       ??? (third-party libraries)
??? Configuration/
?   ??? LlmSettings.cs
?   ??? OpenAISettings.cs
?   ??? SeoSettings.cs
?   ??? CacheSettings.cs
??? Middleware/
?   ??? ErrorHandlingMiddleware.cs
?   ??? RequestLoggingMiddleware.cs
??? Extensions/
?   ??? ServiceCollectionExtensions.cs
?   ??? StringExtensions.cs
?   ??? DateTimeExtensions.cs
??? Utilities/
?   ??? TextAnalyzer.cs
?   ??? SlugGenerator.cs
?   ??? WordCounter.cs
??? appsettings.json
??? appsettings.Development.json
??? Program.cs
??? BloggingAgent.csproj
??? .gitignore
??? README.md
```

## ?? Step-by-Step Implementation

### 1. Create the Project

```bash
dotnet new mvc -n BloggingAgent -f net9.0
cd BloggingAgent
```

### 2. Install Required NuGet Packages

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
```

### 3. Configuration Files

### 16. Enhanced Configuration Files

#### appsettings.json (Complete with all settings)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*",
  
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BloggingAgentDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  
  "Llm": {
    "Provider": "ollama",
    "OllamaEndpoint": "http://localhost:11434/api/generate",
    "OllamaModel": "llama2",
    "MaxRetries": 3,
    "TimeoutSeconds": 120
  },
  
  "OpenAI": {
    "ApiKey": "",
    "Model": "gpt-3.5-turbo",
    "Endpoint": "https://api.openai.com/v1/chat/completions",
    "MaxTokens": 1500,
    "Temperature": 0.7
  },
  
  "Seo": {
    "MinTitleLength": 40,
    "MaxTitleLength": 60,
    "MinMetaDescriptionLength": 120,
    "MaxMetaDescriptionLength": 160,
    "OptimalWordCount": 600,
    "FocusKeywords": []
  },
  
  "Cache": {
    "Enabled": true,
    "CacheDurationMinutes": 60
  }
}
```

### 17. Enhanced ViewModels

#### Models/ViewModels/BlogIndexViewModel.cs (Enhanced)
```csharp
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Models.ViewModels;

public class BlogIndexViewModel
{
    public List<BlogPost> BlogPosts { get; set; } = new();
    public string? NewTopic { get; set; }
    public bool IsGenerating { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public int TotalPosts { get; set; }
    public string CurrentTone { get; set; } = "friendly";
    public string? SearchTerm { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages => (int)Math.Ceiling(TotalPosts / (double)PageSize);
}
```

#### Models/ViewModels/BlogDetailViewModel.cs
```csharp
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Models.ViewModels;

public class BlogDetailViewModel
{
    public BlogPost Post { get; set; } = null!;
    public SeoAnalysisResult? SeoAnalysis { get; set; }
    public List<BlogPost> RelatedPosts { get; set; } = new();
    public bool CanEdit { get; set; } = true;
    public bool CanDelete { get; set; } = true;
}
```

#### Models/ViewModels/AnalyticsViewModel.cs
```csharp
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Models.ViewModels;

public class AnalyticsViewModel
{
    public int TotalPosts { get; set; }
    public int TotalWords { get; set; }
    public int AverageWordCount { get; set; }
    public int TotalViews { get; set; }
    public Dictionary<string, int> TopTopics { get; set; } = new();
    public Dictionary<string, int> ToneDistribution { get; set; } = new();
    public List<BlogPost> RecentPosts { get; set; } = new();
    public int PostsThisMonth { get; set; }
    public int PostsThisWeek { get; set; }
    public double AverageSeoScore { get; set; }
}
```

#### Models/ViewModels/SettingsViewModel.cs
```csharp
namespace BloggingAgent.Models.ViewModels;

public class SettingsViewModel
{
    public string CurrentProvider { get; set; } = "ollama";
    public string OllamaEndpoint { get; set; } = string.Empty;
    public string OllamaModel { get; set; } = string.Empty;
    public string OpenAIModel { get; set; } = string.Empty;
    public bool HasOpenAIKey { get; set; }
    public bool OllamaAvailable { get; set; }
    public bool OpenAIAvailable { get; set; }
    public string DefaultTone { get; set; } = "friendly";
    public int DefaultWordCount { get; set; } = 600;
    public bool AutoGenerateSeo { get; set; } = true;
    public bool AutoPublish { get; set; } = false;
}
```

#### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### 4. Models

#### Models/Domain/BlogPost.cs (Enhanced)
```csharp
using System.ComponentModel.DataAnnotations;

namespace BloggingAgent.Models.Domain;

public class BlogPost
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(300)]
    public string Slug { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Topic { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string MetaDescription { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    [StringLength(50)]
    public string ToneStyle { get; set; } = "friendly";
    
    public int WordCount { get; set; }
    
    public int ReadingTimeMinutes { get; set; }
    
    public bool IsPublished { get; set; } = true;
    
    public bool IsFeatured { get; set; }
    
    [StringLength(500)]
    public string Tags { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Keywords { get; set; } = string.Empty;
    
    public int ViewCount { get; set; }
    
    // Navigation property
    public SeoMetadata? SeoMetadata { get; set; }
    public ContentAnalytics? Analytics { get; set; }
}

public class SeoMetadata
{
    public int Id { get; set; }
    
    public int BlogPostId { get; set; }
    
    [StringLength(200)]
    public string FocusKeyword { get; set; } = string.Empty;
    
    public int KeywordDensity { get; set; }
    
    public int InternalLinks { get; set; }
    
    public int ExternalLinks { get; set; }
    
    public int HeadingCount { get; set; }
    
    public int ImageCount { get; set; }
    
    public double ReadabilityScore { get; set; }
    
    public int SeoScore { get; set; }
    
    [StringLength(1000)]
    public string SeoRecommendations { get; set; } = string.Empty;
    
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public BlogPost BlogPost { get; set; } = null!;
}

public class ContentAnalytics
{
    public int Id { get; set; }
    
    public int BlogPostId { get; set; }
    
    public int UniqueWords { get; set; }
    
    public int Sentences { get; set; }
    
    public int Paragraphs { get; set; }
    
    public double AverageSentenceLength { get; set; }
    
    public double FleschReadingEase { get; set; }
    
    [StringLength(50)]
    public string ReadabilityLevel { get; set; } = string.Empty;
    
    public int ShareCount { get; set; }
    
    public DateTime LastAnalyzedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public BlogPost BlogPost { get; set; } = null!;
}

public class AgentSettings
{
    public int Id { get; set; }
    
    [StringLength(50)]
    public string DefaultTone { get; set; } = "friendly";
    
    public int DefaultWordCount { get; set; } = 600;
    
    public bool AutoGenerateSeo { get; set; } = true;
    
    public bool AutoPublish { get; set; } = false;
    
    [StringLength(500)]
    public string CustomInstructions { get; set; } = string.Empty;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

#### Models/AgentMemory.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace BloggingAgent.Models;

public class AgentMemory
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string ToneStyle { get; set; } = "friendly";
    
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    
    public string Summary { get; set; } = string.Empty;
}
```

#### Models/ViewModels/BlogIndexViewModel.cs
```csharp
namespace BloggingAgent.Models.ViewModels;

public class BlogIndexViewModel
{
    public List<BlogPost> BlogPosts { get; set; } = new();
    public string? NewTopic { get; set; }
    public bool IsGenerating { get; set; }
    public string? ErrorMessage { get; set; }
    public int TotalPosts { get; set; }
    public string CurrentTone { get; set; } = "friendly";
}
```

### 5. Data Layer

### 18. Enhanced Database Context

#### Data/ApplicationDbContext.cs (Complete with all entities)
```csharp
using Microsoft.EntityFrameworkCore;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<BlogPost> BlogPosts { get; set; } = null!;
    public DbSet<AgentMemory> AgentMemories { get; set; } = null!;
    public DbSet<SeoMetadata> SeoMetadata { get; set; } = null!;
    public DbSet<ContentAnalytics> ContentAnalytics { get; set; } = null!;
    public DbSet<AgentSettings> AgentSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // BlogPost Configuration
        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Topic);
            entity.HasIndex(e => e.IsPublished);

            // Relationships
            entity.HasOne(e => e.SeoMetadata)
                .WithOne(e => e.BlogPost)
                .HasForeignKey<SeoMetadata>(e => e.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Analytics)
                .WithOne(e => e.BlogPost)
                .HasForeignKey<ContentAnalytics>(e => e.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AgentMemory Configuration
        modelBuilder.Entity<AgentMemory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Topic).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SavedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.SavedAt);
            entity.HasIndex(e => e.Topic);
        });

        // SeoMetadata Configuration
        modelBuilder.Entity<SeoMetadata>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AnalyzedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.BlogPostId);
        });

        // ContentAnalytics Configuration
        modelBuilder.Entity<ContentAnalytics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LastAnalyzedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.BlogPostId);
        });

        // AgentSettings Configuration
        modelBuilder.Entity<AgentSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Seed default settings
            entity.HasData(new AgentSettings
            {
                Id = 1,
                DefaultTone = "friendly",
                DefaultWordCount = 600,
                AutoGenerateSeo = true,
                AutoPublish = false,
                CustomInstructions = "",
                UpdatedAt = DateTime.UtcNow
            });
        });
    }
}
```

### 19. Project File

#### BloggingAgent.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```

### 20. .gitignore
```
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.

# User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
build/
bld/
[Bb]in/
[Oo]bj/

# Visual Studio
.vs/

# Database
*.db
*.db-shm
*.db-wal

# Environment
appsettings.Development.json
appsettings.Production.json

# Logs
logs/
*.log
```

### 6. Enhanced Configuration

#### Configuration/LlmSettings.cs
```csharp
namespace BloggingAgent.Configuration;

public class LlmSettings
{
    public string Provider { get; set; } = "ollama";
    public string OllamaEndpoint { get; set; } = "http://localhost:11434/api/generate";
    public string OllamaModel { get; set; } = "llama2";
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 120;
}

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-3.5-turbo";
    public string Endpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public int MaxTokens { get; set; } = 1500;
    public double Temperature { get; set; } = 0.7;
}

public class SeoSettings
{
    public int MinTitleLength { get; set; } = 40;
    public int MaxTitleLength { get; set; } = 60;
    public int MinMetaDescriptionLength { get; set; } = 120;
    public int MaxMetaDescriptionLength { get; set; } = 160;
    public int OptimalWordCount { get; set; } = 600;
    public List<string> FocusKeywords { get; set; } = new();
}

public class CacheSettings
{
    public bool Enabled { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 60;
}
```

### 7. Enhanced Services

#### Services/LLM/ILlmProvider.cs
```csharp
namespace BloggingAgent.Services.LLM;

public interface ILlmProvider
{
    Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
    string ProviderName { get; }
    bool IsAvailable();
}
```

#### Services/LLM/OllamaProvider.cs
```csharp
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BloggingAgent.Configuration;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.LLM;

public class OllamaProvider : ILlmProvider
{
    private readonly HttpClient _httpClient;
    private readonly LlmSettings _settings;
    private readonly ILogger<OllamaProvider> _logger;

    public string ProviderName => "Ollama";

    public OllamaProvider(
        HttpClient httpClient,
        IOptions<LlmSettings> settings,
        ILogger<OllamaProvider> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var retryCount = 0;
        Exception? lastException = null;

        while (retryCount < _settings.MaxRetries)
        {
            try
            {
                var request = new OllamaRequest
                {
                    Model = _settings.OllamaModel,
                    Prompt = prompt,
                    Stream = false,
                    Options = new OllamaOptions
                    {
                        Temperature = 0.7,
                        TopP = 0.9,
                        TopK = 40
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    _settings.OllamaEndpoint,
                    request,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(result?.Response))
                {
                    throw new InvalidOperationException("Empty response from Ollama");
                }

                return result.Response;
            }
            catch (Exception ex)
            {
                lastException = ex;
                retryCount++;
                _logger.LogWarning(ex, "Ollama request failed (attempt {Attempt}/{MaxRetries})", 
                    retryCount, _settings.MaxRetries);

                if (retryCount < _settings.MaxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)), cancellationToken);
                }
            }
        }

        throw new InvalidOperationException(
            $"Failed to generate text after {_settings.MaxRetries} attempts", 
            lastException);
    }

    public bool IsAvailable()
    {
        try
        {
            var response = _httpClient.GetAsync(_settings.OllamaEndpoint.Replace("/api/generate", "/api/tags"))
                .GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private class OllamaRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("options")]
        public OllamaOptions? Options { get; set; }
    }

    private class OllamaOptions
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("top_p")]
        public double TopP { get; set; }

        [JsonPropertyName("top_k")]
        public int TopK { get; set; }
    }

    private class OllamaResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;
    }
}
```

#### Services/LLM/OpenAIProvider.cs
```csharp
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BloggingAgent.Configuration;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.LLM;

public class OpenAIProvider : ILlmProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenAISettings _settings;
    private readonly ILogger<OpenAIProvider> _logger;

    public string ProviderName => "OpenAI";

    public OpenAIProvider(
        HttpClient httpClient,
        IOptions<OpenAISettings> settings,
        ILogger<OpenAIProvider> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured");
        }

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

        var request = new OpenAIRequest
        {
            Model = _settings.Model,
            Messages = new[]
            {
                new OpenAIMessage 
                { 
                    Role = "system", 
                    Content = "You are Mostafa Blogging Agent, an expert content creator who writes SEO-friendly, engaging blog posts." 
                },
                new OpenAIMessage { Role = "user", Content = prompt }
            },
            MaxTokens = _settings.MaxTokens,
            Temperature = _settings.Temperature
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                _settings.Endpoint,
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>(
                cancellationToken: cancellationToken);

            return result?.Choices?[0]?.Message?.Content 
                ?? throw new InvalidOperationException("Empty response from OpenAI");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI request failed");
            throw;
        }
    }

    public bool IsAvailable()
    {
        return !string.IsNullOrWhiteSpace(_settings.ApiKey);
    }

    private class OpenAIRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public OpenAIMessage[] Messages { get; set; } = Array.Empty<OpenAIMessage>();

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }
    }

    private class OpenAIMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private class OpenAIResponse
    {
        [JsonPropertyName("choices")]
        public OpenAIChoice[]? Choices { get; set; }
    }

    private class OpenAIChoice
    {
        [JsonPropertyName("message")]
        public OpenAIMessage? Message { get; set; }
    }
}
```

#### Services/LLM/LlmConnector.cs (Refactored)
```csharp
using BloggingAgent.Configuration;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.LLM;

public class LlmConnector : ILlmConnector
{
    private readonly IEnumerable<ILlmProvider> _providers;
    private readonly LlmSettings _settings;
    private readonly ILogger<LlmConnector> _logger;

    public LlmConnector(
        IEnumerable<ILlmProvider> providers,
        IOptions<LlmSettings> settings,
        ILogger<LlmConnector> logger)
    {
        _providers = providers;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var provider = _providers.FirstOrDefault(p => 
            p.ProviderName.Equals(_settings.Provider, StringComparison.OrdinalIgnoreCase));

        if (provider == null)
        {
            throw new InvalidOperationException($"Provider '{_settings.Provider}' not found");
        }

        if (!provider.IsAvailable())
        {
            throw new InvalidOperationException($"Provider '{provider.ProviderName}' is not available");
        }

        _logger.LogInformation("Generating text using provider: {Provider}", provider.ProviderName);

        return await provider.GenerateAsync(prompt, cancellationToken);
    }

    public string GetCurrentProvider()
    {
        return _settings.Provider;
    }

    public bool IsProviderAvailable(string providerName)
    {
        var provider = _providers.FirstOrDefault(p => 
            p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));
        
        return provider?.IsAvailable() ?? false;
    }
}
```

### 7. Enhanced Services (Continued)

#### Services/LLM/ILlmConnector.cs (Enhanced)
```csharp
namespace BloggingAgent.Services.LLM;

public interface ILlmConnector
{
    Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
    string GetCurrentProvider();
    bool IsProviderAvailable(string providerName);
}
```

#### Services/SEO/ISeoService.cs
```csharp
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Services.SEO;

public interface ISeoService
{
    Task<SeoAnalysisResult> AnalyzeContentAsync(string content, string title);
    Task<SeoMetadata> GenerateSeoMetadataAsync(BlogPost post);
    Task<List<string>> ExtractKeywordsAsync(string content);
    string GenerateMetaDescription(string content, int maxLength = 160);
    int CalculateSeoScore(SeoAnalysisResult analysis);
}
```

#### Services/SEO/SeoService.cs
```csharp
using System.Text.RegularExpressions;
using BloggingAgent.Configuration;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Utilities;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.SEO;

public class SeoService : ISeoService
{
    private readonly SeoSettings _settings;
    private readonly ILogger<SeoService> _logger;

    public SeoService(IOptions<SeoSettings> settings, ILogger<SeoService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<SeoAnalysisResult> AnalyzeContentAsync(string content, string title)
    {
        return await Task.Run(() =>
        {
            var analysis = new SeoAnalysisResult
            {
                Title = title,
                TitleLength = title.Length,
                WordCount = WordCounter.CountWords(content),
                KeywordDensity = CalculateKeywordDensity(content, title),
                HeadingCount = CountHeadings(content),
                ReadabilityScore = CalculateReadability(content),
                HasMetaDescription = false
            };

            analysis.Issues = GenerateIssues(analysis);
            analysis.Recommendations = GenerateRecommendations(analysis);

            return analysis;
        });
    }

    public async Task<SeoMetadata> GenerateSeoMetadataAsync(BlogPost post)
    {
        var analysis = await AnalyzeContentAsync(post.Content, post.Title);
        var keywords = await ExtractKeywordsAsync(post.Content);

        return new SeoMetadata
        {
            BlogPostId = post.Id,
            FocusKeyword = keywords.FirstOrDefault() ?? post.Topic,
            KeywordDensity = analysis.KeywordDensity,
            HeadingCount = analysis.HeadingCount,
            ReadabilityScore = analysis.ReadabilityScore,
            SeoScore = CalculateSeoScore(analysis),
            SeoRecommendations = string.Join("; ", analysis.Recommendations),
            AnalyzedAt = DateTime.UtcNow
        };
    }

    public async Task<List<string>> ExtractKeywordsAsync(string content)
    {
        return await Task.Run(() =>
        {
            var words = content.Split(new[] { ' ', '\n', '\r', ',', '.', '!', '?' }, 
                StringSplitOptions.RemoveEmptyEntries);

            var stopWords = new HashSet<string> 
            { 
                "the", "is", "at", "which", "on", "a", "an", "and", "or", "but", 
                "in", "with", "to", "for", "of", "as", "by", "that", "this" 
            };

            return words
                .Where(w => w.Length > 3 && !stopWords.Contains(w.ToLower()))
                .GroupBy(w => w.ToLower())
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList();
        });
    }

    public string GenerateMetaDescription(string content, int maxLength = 160)
    {
        var cleanContent = Regex.Replace(content, @"<[^>]+>|&nbsp;", "").Trim();
        var sentences = cleanContent.Split(new[] { '. ', '! ', '? ' }, StringSplitOptions.RemoveEmptyEntries);

        var description = string.Empty;
        foreach (var sentence in sentences)
        {
            if (description.Length + sentence.Length + 2 <= maxLength)
            {
                description += sentence + ". ";
            }
            else
            {
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(description) && cleanContent.Length > maxLength)
        {
            description = cleanContent.Substring(0, maxLength - 3) + "...";
        }

        return description.Trim();
    }

    public int CalculateSeoScore(SeoAnalysisResult analysis)
    {
        var score = 100;

        if (analysis.TitleLength < _settings.MinTitleLength || 
            analysis.TitleLength > _settings.MaxTitleLength)
            score -= 15;

        if (analysis.WordCount < _settings.OptimalWordCount * 0.8)
            score -= 20;

        if (analysis.HeadingCount < 3)
            score -= 10;

        if (analysis.ReadabilityScore < 40)
            score -= 15;

        if (analysis.KeywordDensity < 1 || analysis.KeywordDensity > 3)
            score -= 10;

        return Math.Max(0, score);
    }

    private int CalculateKeywordDensity(string content, string title)
    {
        var titleWords = title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var contentWords = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (contentWords.Length == 0) return 0;

        var matchCount = titleWords.Sum(tw => 
            contentWords.Count(cw => cw.Equals(tw, StringComparison.OrdinalIgnoreCase)));

        return (int)((matchCount / (double)contentWords.Length) * 100);
    }

    private int CountHeadings(string content)
    {
        return Regex.Matches(content, @"<h[1-6]>|#{1,6}\s", RegexOptions.IgnoreCase).Count;
    }

    private double CalculateReadability(string content)
    {
        var sentences = content.Split(new[] { '. ', '! ', '? ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        var syllables = EstimateSyllables(content);

        if (sentences == 0 || words == 0) return 0;

        var fleschScore = 206.835 - 1.015 * (words / (double)sentences) - 84.6 * (syllables / (double)words);
        return Math.Round(Math.Max(0, Math.Min(100, fleschScore)), 2);
    }

    private int EstimateSyllables(string content)
    {
        var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Sum(word => Math.Max(1, Regex.Matches(word.ToLower(), @"[aeiouy]+").Count));
    }

    private List<string> GenerateIssues(SeoAnalysisResult analysis)
    {
        var issues = new List<string>();

        if (analysis.TitleLength < _settings.MinTitleLength)
            issues.Add("Title is too short");
        if (analysis.TitleLength > _settings.MaxTitleLength)
            issues.Add("Title is too long");
        if (analysis.WordCount < _settings.OptimalWordCount * 0.8)
            issues.Add("Content is shorter than recommended");
        if (analysis.HeadingCount < 3)
            issues.Add("Add more headings to improve structure");
        if (analysis.ReadabilityScore < 40)
            issues.Add("Content may be difficult to read");

        return issues;
    }

    private List<string> GenerateRecommendations(SeoAnalysisResult analysis)
    {
        var recommendations = new List<string>();

        if (analysis.WordCount < _settings.OptimalWordCount)
            recommendations.Add($"Aim for at least {_settings.OptimalWordCount} words");
        if (analysis.HeadingCount < 5)
            recommendations.Add("Use more subheadings to break up content");
        if (analysis.ReadabilityScore < 60)
            recommendations.Add("Simplify sentences for better readability");

        return recommendations;
    }
}
```

#### Services/Content/IContentFormatter.cs
```csharp
namespace BloggingAgent.Services.Content;

public interface IContentFormatter
{
    string FormatForDisplay(string content);
    string ConvertMarkdownToHtml(string markdown);
    string ExtractPlainText(string html);
    int CalculateReadingTime(string content);
}
```

#### Services/Content/ContentFormatter.cs
```csharp
using System.Text.RegularExpressions;

namespace BloggingAgent.Services.Content;

public class ContentFormatter : IContentFormatter
{
    public string FormatForDisplay(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        content = content.Replace("\r\n", "\n").Replace("\r", "\n");
        
        content = Regex.Replace(content, @"#{1,6}\s+(.+)", m => 
            $"<h{m.Groups[0].Value.Count(c => c == '#')}>{m.Groups[1].Value}</h{m.Groups[0].Value.Count(c => c == '#')}>");

        content = Regex.Replace(content, @"\*\*(.+?)\*\*", "<strong>$1</strong>");
        content = Regex.Replace(content, @"\*(.+?)\*", "<em>$1</em>");
        
        content = Regex.Replace(content, @"\n\n+", "</p><p>");
        content = $"<p>{content}</p>";

        return content;
    }

    public string ConvertMarkdownToHtml(string markdown)
    {
        return FormatForDisplay(markdown);
    }

    public string ExtractPlainText(string html)
    {
        return Regex.Replace(html, @"<[^>]+>", " ").Trim();
    }

    public int CalculateReadingTime(string content)
    {
        var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        var wordsPerMinute = 200;
        return Math.Max(1, (int)Math.Ceiling(words / (double)wordsPerMinute));
    }
}
```

#### Services/LlmConnector.cs
```csharp
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BloggingAgent.Configuration;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services;

public class LlmConnector : ILlmConnector
{
    private readonly HttpClient _httpClient;
    private readonly LlmSettings _llmSettings;
    private readonly OpenAISettings _openAISettings;
    private readonly ILogger<LlmConnector> _logger;

    public LlmConnector(
        HttpClient httpClient,
        IOptions<LlmSettings> llmSettings,
        IOptions<OpenAISettings> openAISettings,
        ILogger<LlmConnector> logger)
    {
        _httpClient = httpClient;
        _llmSettings = llmSettings.Value;
        _openAISettings = openAISettings.Value;
        _logger = logger;
    }

    public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            return _llmSettings.Provider.ToLower() switch
            {
                "openai" => await GenerateWithOpenAIAsync(prompt, cancellationToken),
                "ollama" => await GenerateWithOllamaAsync(prompt, cancellationToken),
                _ => throw new InvalidOperationException($"Unsupported LLM provider: {_llmSettings.Provider}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating text with provider {Provider}", _llmSettings.Provider);
            throw;
        }
    }

    private async Task<string> GenerateWithOllamaAsync(string prompt, CancellationToken cancellationToken)
    {
        var request = new OllamaRequest
        {
            Model = _llmSettings.OllamaModel,
            Prompt = prompt,
            Stream = false
        };

        var response = await _httpClient.PostAsJsonAsync(
            _llmSettings.OllamaEndpoint,
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
            cancellationToken: cancellationToken);

        return result?.Response ?? throw new InvalidOperationException("Empty response from Ollama");
    }

    private async Task<string> GenerateWithOpenAIAsync(string prompt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_openAISettings.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured");
        }

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAISettings.ApiKey}");

        var request = new OpenAIRequest
        {
            Model = _openAISettings.Model,
            Messages = new[]
            {
                new OpenAIMessage { Role = "system", Content = "You are Mostafa Blogging Agent, an expert content creator who writes SEO-friendly, engaging blog posts." },
                new OpenAIMessage { Role = "user", Content = prompt }
            },
            MaxTokens = 1500,
            Temperature = 0.7
        };

        var response = await _httpClient.PostAsJsonAsync(
            _openAISettings.Endpoint,
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>(
            cancellationToken: cancellationToken);

        return result?.Choices?[0]?.Message?.Content 
            ?? throw new InvalidOperationException("Empty response from OpenAI");
    }

    // Ollama DTOs
    private class OllamaRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }

    private class OllamaResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;
    }

    // OpenAI DTOs
    private class OpenAIRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public OpenAIMessage[] Messages { get; set; } = Array.Empty<OpenAIMessage>();

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }
    }

    private class OpenAIMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private class OpenAIResponse
    {
        [JsonPropertyName("choices")]
        public OpenAIChoice[]? Choices { get; set; }
    }

    private class OpenAIChoice
    {
        [JsonPropertyName("message")]
        public OpenAIMessage? Message { get; set; }
    }
}
```

#### Services/Cache/ICacheService.cs
```csharp
namespace BloggingAgent.Services.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}
```

#### Services/Cache/MemoryCacheService.cs
```csharp
using Microsoft.Extensions.Caching.Memory;
using BloggingAgent.Configuration;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.Cache;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _settings;

    public MemoryCacheService(IMemoryCache cache, IOptions<CacheSettings> settings)
    {
        _cache = cache;
        _settings = settings.Value;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        if (!_settings.Enabled)
            return Task.CompletedTask;

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(_settings.CacheDurationMinutes)
        };

        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }
}
```

#### Services/Memory/MemoryAnalyzer.cs
```csharp
using BloggingAgent.Data;
using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Services.Memory;

public class MemoryAnalyzer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MemoryAnalyzer> _logger;

    public MemoryAnalyzer(ApplicationDbContext context, ILogger<MemoryAnalyzer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Dictionary<string, int>> GetTopTopicsAsync(int count = 10)
    {
        return await _context.AgentMemories
            .GroupBy(m => m.Topic)
            .Select(g => new { Topic = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToDictionaryAsync(x => x.Topic, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetToneDistributionAsync()
    {
        return await _context.AgentMemories
            .GroupBy(m => m.ToneStyle)
            .Select(g => new { Tone = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Tone, x => x.Count);
    }

    public async Task<List<string>> GetRecommendedTopicsAsync(int count = 5)
    {
        var recentTopics = await _context.AgentMemories
            .OrderByDescending(m => m.SavedAt)
            .Select(m => m.Topic)
            .Take(20)
            .ToListAsync();

        // Simple recommendation: find related topics
        var allTopics = await _context.AgentMemories
            .Select(m => m.Topic)
            .Distinct()
            .ToListAsync();

        return allTopics
            .Where(t => !recentTopics.Contains(t))
            .Take(count)
            .ToList();
    }
}
```
```csharp
using BloggingAgent.Data;
using BloggingAgent.Models;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Services;

public interface IMemoryService
{
    Task SaveMemoryAsync(string topic, string content, string toneStyle);
    Task<string> GetLastToneStyleAsync();
    Task<List<AgentMemory>> GetRecentMemoriesAsync(int count = 5);
    Task<bool> HasWrittenAboutTopicAsync(string topic);
}

public class MemoryService : IMemoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MemoryService> _logger;

    public MemoryService(ApplicationDbContext context, ILogger<MemoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveMemoryAsync(string topic, string content, string toneStyle)
    {
        try
        {
            var memory = new AgentMemory
            {
                Topic = topic,
                Content = content,
                ToneStyle = toneStyle,
                Summary = content.Length > 200 ? content.Substring(0, 200) + "..." : content,
                SavedAt = DateTime.UtcNow
            };

            _context.AgentMemories.Add(memory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Memory saved for topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving memory for topic: {Topic}", topic);
            throw;
        }
    }

    public async Task<string> GetLastToneStyleAsync()
    {
        var lastMemory = await _context.AgentMemories
            .OrderByDescending(m => m.SavedAt)
            .FirstOrDefaultAsync();

        return lastMemory?.ToneStyle ?? "friendly";
    }

    public async Task<List<AgentMemory>> GetRecentMemoriesAsync(int count = 5)
    {
        return await _context.AgentMemories
            .OrderByDescending(m => m.SavedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> HasWrittenAboutTopicAsync(string topic)
    {
        return await _context.AgentMemories
            .AnyAsync(m => EF.Functions.Like(m.Topic, $"%{topic}%"));
    }
}
```

### 8. Utilities

#### Utilities/TextAnalyzer.cs
```csharp
using System.Text.RegularExpressions;

namespace BloggingAgent.Utilities;

public static class TextAnalyzer
{
    public static int CountSentences(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return Regex.Matches(text, @"[.!?]+").Count;
    }

    public static int CountParagraphs(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    public static double CalculateAverageSentenceLength(string text)
    {
        var sentences = CountSentences(text);
        var words = WordCounter.CountWords(text);

        return sentences > 0 ? Math.Round(words / (double)sentences, 2) : 0;
    }

    public static int CountUniqueWords(string text)
    {
        var words = text.Split(new[] { ' ', '\n', '\r', ',', '.', '!', '?' }, 
            StringSplitOptions.RemoveEmptyEntries);

        return words.Select(w => w.ToLower()).Distinct().Count();
    }
}
```

#### Utilities/WordCounter.cs
```csharp
namespace BloggingAgent.Utilities;

public static class WordCounter
{
    public static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new[] { ' ', '\n', '\r', '\t' }, 
            StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
```

#### Utilities/SlugGenerator.cs
```csharp
using System.Text;
using System.Text.RegularExpressions;

namespace BloggingAgent.Utilities;

public static class SlugGenerator
{
    public static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        var slug = title.ToLowerInvariant();
        
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug.Length > 100 ? slug.Substring(0, 100) : slug;
    }

    public static string EnsureUniqueSlug(string baseSlug, Func<string, bool> existsCheck)
    {
        var slug = baseSlug;
        var counter = 1;

        while (existsCheck(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }
}
```

### 9. DTOs

#### Models/DTOs/GeneratePostRequest.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace BloggingAgent.Models.DTOs;

public class GeneratePostRequest
{
    [Required(ErrorMessage = "Topic is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Topic must be between 3 and 200 characters")]
    public string Topic { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ToneStyle { get; set; }

    [Range(300, 2000)]
    public int? TargetWordCount { get; set; }

    public bool AutoPublish { get; set; }

    public string[]? Tags { get; set; }
}
```

#### Models/DTOs/SeoAnalysisResult.cs
```csharp
namespace BloggingAgent.Models.DTOs;

public class SeoAnalysisResult
{
    public string Title { get; set; } = string.Empty;
    public int TitleLength { get; set; }
    public int WordCount { get; set; }
    public int KeywordDensity { get; set; }
    public int HeadingCount { get; set; }
    public double ReadabilityScore { get; set; }
    public bool HasMetaDescription { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}
```

#### Models/DTOs/BlogPostDto.cs
```csharp
namespace BloggingAgent.Models.DTOs;

public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ToneStyle { get; set; } = string.Empty;
    public int WordCount { get; set; }
    public int ReadingTimeMinutes { get; set; }
    public bool IsPublished { get; set; }
    public List<string> Tags { get; set; } = new();
    public int SeoScore { get; set; }
}
```

### 14. Enhanced Agent System

#### Agents/IBloggingAgent.cs
```csharp
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Agents;

public interface IBloggingAgent
{
    Task<BlogPost> GenerateBlogPostAsync(string topic, CancellationToken cancellationToken = default);
    Task<BlogPost> GenerateBlogPostAsync(GeneratePostRequest request, CancellationToken cancellationToken = default);
    Task<string> RegenerateContentAsync(int postId, CancellationToken cancellationToken = default);
    Task<SeoAnalysisResult> AnalyzePostSeoAsync(int postId);
}
```

#### Agents/Prompts/BlogPromptTemplates.cs
```csharp
namespace BloggingAgent.Agents.Prompts;

public static class BlogPromptTemplates
{
    public static string GetBlogGenerationPrompt(
        string topic, 
        string toneStyle, 
        int targetWordCount, 
        bool hasWrittenBefore,
        List<string>? previousTopics = null)
    {
        var contextNote = hasWrittenBefore 
            ? $"\n\nNote: You've written about similar topics before: {string.Join(", ", previousTopics ?? new List<string>())}. Bring a fresh perspective and avoid repeating content." 
            : "";

        return $@"You are Mostafa Blogging Agent, an expert content creator and SEO specialist.

Write a comprehensive, SEO-friendly blog post about: {topic}

Requirements:
- Write approximately {targetWordCount} words (minimum {targetWordCount * 0.8}, maximum {targetWordCount * 1.2})
- Use a {toneStyle} tone throughout
- Structure: Title, Introduction, Main Body (with multiple subheadings), and Conclusion
- Make it engaging, informative, and human-like
- Use natural language, avoid robotic phrases
- Include actionable insights and practical examples
- Use proper heading hierarchy (##, ###)
- Write in a way that keeps readers engaged
- Include transition phrases between sections
- End with a compelling call-to-action
{contextNote}

SEO Guidelines:
- Create an attention-grabbing title (40-60 characters)
- Use keywords naturally throughout the content
- Include relevant subheadings every 200-300 words
- Write in short, scannable paragraphs (3-4 sentences max)
- Use bullet points or numbered lists where appropriate

Format your response EXACTLY as follows:
TITLE: [Your compelling title here]

INTRODUCTION:
[Hook the reader with an engaging introduction that sets up the topic]

BODY:
## [First Main Point]
[Content for first section]

## [Second Main Point]
[Content for second section]

## [Third Main Point]
[Content for third section]

[Add more sections as needed]

CONCLUSION:
[Wrap up the key points and provide a call-to-action]

Begin writing now:";
    }

    public static string GetContentRegenerationPrompt(
        string originalTitle,
        string originalContent,
        string topic,
        string toneStyle)
    {
        return $@"You are Mostafa Blogging Agent. Regenerate and improve the following blog post.

Original Title: {originalTitle}
Topic: {topic}
Tone: {toneStyle}

Requirements:
- Keep the same topic but write completely new content
- Improve SEO optimization
- Make it more engaging and actionable
- Maintain approximately the same word count
- Use different examples and perspectives

Original Content:
{originalContent.Substring(0, Math.Min(500, originalContent.Length))}...

Generate an improved version following the same format:
TITLE: [New improved title]

[Rest of content...]";
    }

    public static string GetSeoOptimizationPrompt(string content)
    {
        return $@"Analyze the following blog post content for SEO optimization.

Content:
{content}

Provide specific recommendations for:
1. Title optimization
2. Keyword usage
3. Heading structure
4. Content readability
5. Meta description suggestion

Format your response as:
SEO_ANALYSIS:
[Your detailed analysis and recommendations]";
    }
}
```

#### Agents/BloggingAgent.cs (Enhanced)
```csharp
using BloggingAgent.Agents.Prompts;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;
using BloggingAgent.Utilities;

namespace BloggingAgent.Agents;

public class BloggingAgent : IBloggingAgent
{
    private readonly ILlmConnector _llmConnector;
    private readonly IMemoryService _memoryService;
    private readonly ISeoService _seoService;
    private readonly IContentFormatter _contentFormatter;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ILogger<BloggingAgent> _logger;

    public BloggingAgent(
        ILlmConnector llmConnector,
        IMemoryService memoryService,
        ISeoService seoService,
        IContentFormatter contentFormatter,
        IBlogPostRepository blogPostRepository,
        ILogger<BloggingAgent> logger)
    {
        _llmConnector = llmConnector;
        _memoryService = memoryService;
        _seoService = seoService;
        _contentFormatter = contentFormatter;
        _blogPostRepository = blogPostRepository;
        _logger = logger;
    }

    public async Task<BlogPost> GenerateBlogPostAsync(
        string topic, 
        CancellationToken cancellationToken = default)
    {
        var request = new GeneratePostRequest
        {
            Topic = topic,
            TargetWordCount = 600,
            AutoPublish = true
        };

        return await GenerateBlogPostAsync(request, cancellationToken);
    }

    public async Task<BlogPost> GenerateBlogPostAsync(
        GeneratePostRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating blog post for topic: {Topic}", request.Topic);

            // Get agent's context from memory
            var preferredTone = request.ToneStyle ?? await _memoryService.GetLastToneStyleAsync();
            var hasWrittenBefore = await _memoryService.HasWrittenAboutTopicAsync(request.Topic);
            var recentMemories = await _memoryService.GetRecentMemoriesAsync(5);
            var previousTopics = recentMemories.Select(m => m.Topic).ToList();

            // Generate the prompt
            var prompt = BlogPromptTemplates.GetBlogGenerationPrompt(
                request.Topic,
                preferredTone,
                request.TargetWordCount ?? 600,
                hasWrittenBefore,
                previousTopics);

            // Generate content
            var generatedContent = await _llmConnector.GenerateTextAsync(prompt, cancellationToken);

            // Parse the response
            var (title, content) = ParseGeneratedContent(generatedContent, request.Topic);

            // Calculate metrics
            var wordCount = WordCounter.CountWords(content);
            var readingTime = _contentFormatter.CalculateReadingTime(content);

            // Generate slug
            var slug = SlugGenerator.GenerateSlug(title);
            slug = SlugGenerator.EnsureUniqueSlug(slug, 
                s => _blogPostRepository.SlugExistsAsync(s).GetAwaiter().GetResult());

            // Generate meta description
            var metaDescription = _seoService.GenerateMetaDescription(content);

            // Create the blog post
            var blogPost = new BlogPost
            {
                Title = title,
                Slug = slug,
                Content = content,
                Topic = request.Topic,
                MetaDescription = metaDescription,
                ToneStyle = preferredTone,
                WordCount = wordCount,
                ReadingTimeMinutes = readingTime,
                IsPublished = request.AutoPublish,
                Tags = request.Tags != null ? string.Join(",", request.Tags) : string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            // Generate SEO metadata
            var seoAnalysis = await _seoService.AnalyzeContentAsync(content, title);
            blogPost.SeoMetadata = await _seoService.GenerateSeoMetadataAsync(blogPost);

            // Generate content analytics
            blogPost.Analytics = GenerateContentAnalytics(content);

            // Save to memory
            await _memoryService.SaveMemoryAsync(request.Topic, content, preferredTone);

            _logger.LogInformation("Blog post generated successfully: {Title} ({WordCount} words)", 
                title, wordCount);

            return blogPost;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating blog post for topic: {Topic}", request.Topic);
            throw;
        }
    }

    public async Task<string> RegenerateContentAsync(
        int postId, 
        CancellationToken cancellationToken = default)
    {
        var post = await _blogPostRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new InvalidOperationException($"Post with ID {postId} not found");
        }

        var prompt = BlogPromptTemplates.GetContentRegenerationPrompt(
            post.Title,
            post.Content,
            post.Topic,
            post.ToneStyle);

        return await _llmConnector.GenerateTextAsync(prompt, cancellationToken);
    }

    public async Task<SeoAnalysisResult> AnalyzePostSeoAsync(int postId)
    {
        var post = await _blogPostRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new InvalidOperationException($"Post with ID {postId} not found");
        }

        return await _seoService.AnalyzeContentAsync(post.Content, post.Title);
    }

    private (string title, string content) ParseGeneratedContent(string generated, string fallbackTopic)
    {
        try
        {
            var lines = generated.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var title = "Untitled Post";
            var contentBuilder = new System.Text.StringBuilder();
            var foundTitle = false;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("TITLE:", StringComparison.OrdinalIgnoreCase))
                {
                    title = trimmedLine.Substring(6).Trim();
                    foundTitle = true;
                }
                else if (foundTitle && 
                        !trimmedLine.StartsWith("INTRODUCTION:", StringComparison.OrdinalIgnoreCase) &&
                        !trimmedLine.StartsWith("BODY:", StringComparison.OrdinalIgnoreCase) &&
                        !trimmedLine.StartsWith("CONCLUSION:", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        contentBuilder.AppendLine(trimmedLine);
                    }
                }
            }

            var content = contentBuilder.ToString().Trim();

            // Fallback if parsing fails
            if (string.IsNullOrWhiteSpace(content))
            {
                content = generated;
            }

            if (title == "Untitled Post")
            {
                title = $"Exploring {fallbackTopic}";
            }

            return (title, content);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error parsing generated content, using fallback");
            return ($"Exploring {fallbackTopic}", generated);
        }
    }

    private ContentAnalytics GenerateContentAnalytics(string content)
    {
        return new ContentAnalytics
        {
            UniqueWords = TextAnalyzer.CountUniqueWords(content),
            Sentences = TextAnalyzer.CountSentences(content),
            Paragraphs = TextAnalyzer.CountParagraphs(content),
            AverageSentenceLength = TextAnalyzer.CalculateAverageSentenceLength(content),
            FleschReadingEase = 0, // Can be calculated if needed
            ReadabilityLevel = "Medium",
            LastAnalyzedAt = DateTime.UtcNow
        };
    }
}
```

### 10. Repositories

#### Data/Repositories/IRepository.cs
```csharp
using System.Linq.Expressions;

namespace BloggingAgent.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync();
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}
```

#### Data/Repositories/Repository.cs
```csharp
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}
```

#### Data/Repositories/IBlogPostRepository.cs
```csharp
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Data.Repositories;

public interface IBlogPostRepository : IRepository<BlogPost>
{
    Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count = 10);
    Task<BlogPost?> GetBySlugAsync(string slug);
    Task<IEnumerable<BlogPost>> SearchAsync(string searchTerm);
    Task<IEnumerable<BlogPost>> GetByTopicAsync(string topic);
    Task<bool> SlugExistsAsync(string slug);
}
```

#### Data/Repositories/BlogPostRepository.cs
```csharp
using BloggingAgent.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Data.Repositories;

public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
{
    public BlogPostRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count = 10)
    {
        return await _dbSet
            .Include(p => p.SeoMetadata)
            .Include(p => p.Analytics)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<BlogPost?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.SeoMetadata)
            .Include(p => p.Analytics)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<BlogPost>> SearchAsync(string searchTerm)
    {
        return await _dbSet
            .Where(p => p.Title.Contains(searchTerm) || 
                       p.Content.Contains(searchTerm) || 
                       p.Topic.Contains(searchTerm))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetByTopicAsync(string topic)
    {
        return await _dbSet
            .Where(p => p.Topic.Contains(topic))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _dbSet.AnyAsync(p => p.Slug == slug);
    }
}
```

### 15. Enhanced Controllers

#### Controllers/BlogController.cs (Enhanced with all features)
```csharp
using Microsoft.AspNetCore.Mvc;
using BloggingAgent.Agents;
using BloggingAgent.Data;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;

namespace BloggingAgent.Controllers;

public class BlogController : Controller
{
    private readonly IBloggingAgent _agent;
    private readonly IBlogPostRepository _repository;
    private readonly IMemoryService _memoryService;
    private readonly ISeoService _seoService;
    private readonly ILogger<BlogController> _logger;

    public BlogController(
        IBloggingAgent agent,
        IBlogPostRepository repository,
        IMemoryService memoryService,
        ISeoService seoService,
        ILogger<BlogController> logger)
    {
        _agent = agent;
        _repository = repository;
        _memoryService = memoryService;
        _seoService = seoService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        IEnumerable<BlogPost> posts;

        if (!string.IsNullOrWhiteSpace(search))
        {
            posts = await _repository.SearchAsync(search);
        }
        else
        {
            posts = await _repository.GetRecentPostsAsync(pageSize * page);
        }

        var currentTone = await _memoryService.GetLastToneStyleAsync();
        var totalPosts = await _repository.CountAsync();

        var viewModel = new BlogIndexViewModel
        {
            BlogPosts = posts.ToList(),
            TotalPosts = totalPosts,
            CurrentTone = currentTone,
            SearchTerm = search
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var post = await _repository.GetBySlugAsync(slug);
        if (post == null)
        {
            return NotFound();
        }

        // Increment view count
        post.ViewCount++;
        await _repository.UpdateAsync(post);

        var viewModel = new BlogDetailViewModel
        {
            Post = post,
            SeoAnalysis = post.SeoMetadata != null 
                ? await _seoService.AnalyzeContentAsync(post.Content, post.Title)
                : null
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate([FromForm] GeneratePostRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please provide valid input.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _logger.LogInformation("Generating blog post for topic: {Topic}", request.Topic);

            var blogPost = await _agent.GenerateBlogPostAsync(request);

            await _repository.AddAsync(blogPost);

            TempData["Success"] = $"Blog post '{blogPost.Title}' generated successfully! ({blogPost.WordCount} words)";
            
            return RedirectToAction(nameof(Details), new { slug = blogPost.Slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating blog post");
            TempData["Error"] = $"Failed to generate blog post: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPost model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var post = await _repository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.Title = model.Title;
            post.Content = model.Content;
            post.MetaDescription = model.MetaDescription;
            post.Tags = model.Tags;
            post.IsPublished = model.IsPublished;
            post.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(post);

            TempData["Success"] = "Blog post updated successfully.";
            return RedirectToAction(nameof(Details), new { slug = post.Slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blog post");
            TempData["Error"] = "Failed to update blog post.";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var post = await _repository.GetByIdAsync(id);
            if (post != null)
            {
                await _repository.DeleteAsync(post);
                TempData["Success"] = "Blog post deleted successfully.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog post");
            TempData["Error"] = "Failed to delete blog post.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Regenerate(int id)
    {
        try
        {
            var post = await _repository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var newContent = await _agent.RegenerateContentAsync(id);
            
            post.Content = newContent;
            post.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(post);

            TempData["Success"] = "Content regenerated successfully.";
            return RedirectToAction(nameof(Details), new { slug = post.Slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating content");
            TempData["Error"] = "Failed to regenerate content.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> SeoAnalysis(int id)
    {
        try
        {
            var analysis = await _agent.AnalyzePostSeoAsync(id);
            return Json(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing SEO");
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

#### Controllers/AnalyticsController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.ViewModels;
using BloggingAgent.Services.Memory;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Controllers;

public class AnalyticsController : Controller
{
    private readonly IBlogPostRepository _repository;
    private readonly MemoryAnalyzer _memoryAnalyzer;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IBlogPostRepository repository,
        MemoryAnalyzer memoryAnalyzer,
        ILogger<AnalyticsController> logger)
    {
        _repository = repository;
        _memoryAnalyzer = memoryAnalyzer;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _repository.GetAllAsync();
        var topTopics = await _memoryAnalyzer.GetTopTopicsAsync();
        var toneDistribution = await _memoryAnalyzer.GetToneDistributionAsync();

        var viewModel = new AnalyticsViewModel
        {
            TotalPosts = posts.Count(),
            TotalWords = posts.Sum(p => p.WordCount),
            AverageWordCount = posts.Any() ? (int)posts.Average(p => p.WordCount) : 0,
            TotalViews = posts.Sum(p => p.ViewCount),
            TopTopics = topTopics,
            ToneDistribution = toneDistribution,
            RecentPosts = posts.OrderByDescending(p => p.CreatedAt).Take(5).ToList()
        };

        return View(viewModel);
    }
}
```

#### Controllers/SettingsController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using BloggingAgent.Models.ViewModels;
using BloggingAgent.Services.LLM;
using BloggingAgent.Configuration;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Controllers;

public class SettingsController : Controller
{
    private readonly ILlmConnector _llmConnector;
    private readonly LlmSettings _llmSettings;
    private readonly OpenAISettings _openAISettings;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ILlmConnector llmConnector,
        IOptions<LlmSettings> llmSettings,
        IOptions<OpenAISettings> openAISettings,
        ILogger<SettingsController> logger)
    {
        _llmConnector = llmConnector;
        _llmSettings = llmSettings.Value;
        _openAISettings = openAISettings.Value;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var viewModel = new SettingsViewModel
        {
            CurrentProvider = _llmSettings.Provider,
            OllamaEndpoint = _llmSettings.OllamaEndpoint,
            OllamaModel = _llmSettings.OllamaModel,
            OpenAIModel = _openAISettings.Model,
            HasOpenAIKey = !string.IsNullOrWhiteSpace(_openAISettings.ApiKey),
            OllamaAvailable = _llmConnector.IsProviderAvailable("ollama"),
            OpenAIAvailable = _llmConnector.IsProviderAvailable("openai")
        };

        return View(viewModel);
    }
}
```
```csharp
using Microsoft.AspNetCore.Mvc;
using BloggingAgent.Agents;
using BloggingAgent.Data;
using BloggingAgent.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using BloggingAgent.Services;

namespace BloggingAgent.Controllers;

public class BlogController : Controller
{
    private readonly BloggingAgent _agent;
    private readonly ApplicationDbContext _context;
    private readonly IMemoryService _memoryService;
    private readonly ILogger<BlogController> _logger;

    public BlogController(
        BloggingAgent agent,
        ApplicationDbContext context,
        IMemoryService memoryService,
        ILogger<BlogController> logger)
    {
        _agent = agent;
        _context = context;
        _memoryService = memoryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.BlogPosts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var currentTone = await _memoryService.GetLastToneStyleAsync();

        var viewModel = new BlogIndexViewModel
        {
            BlogPosts = posts,
            TotalPosts = posts.Count,
            CurrentTone = currentTone
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            TempData["Error"] = "Please enter a topic.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _logger.LogInformation("Generating blog post for topic: {Topic}", topic);

            var blogPost = await _agent.GenerateBlogPostAsync(topic);

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Blog post '{blogPost.Title}' generated successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating blog post");
            TempData["Error"] = "Failed to generate blog post. Please check your LLM configuration.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post != null)
        {
            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Blog post deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}
```

### 11. Program.cs (Enhanced with Full DI)

```csharp
using BloggingAgent.Agents;
using BloggingAgent.Configuration;
using BloggingAgent.Data;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Middleware;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));

// Configure Settings
builder.Services.Configure<LlmSettings>(builder.Configuration.GetSection("Llm"));
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<SeoSettings>(builder.Configuration.GetSection("Seo"));
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("Cache"));

// Register HttpClient for LLM providers
builder.Services.AddHttpClient<OllamaProvider>();
builder.Services.AddHttpClient<OpenAIProvider>();

// Register LLM Services
builder.Services.AddScoped<ILlmProvider, OllamaProvider>();
builder.Services.AddScoped<ILlmProvider, OpenAIProvider>();
builder.Services.AddScoped<ILlmConnector, LlmConnector>();

// Register Memory Services
builder.Services.AddScoped<IMemoryService, MemoryService>();
builder.Services.AddScoped<MemoryAnalyzer>();

// Register Content Services
builder.Services.AddScoped<IContentFormatter, ContentFormatter>();

// Register SEO Services
builder.Services.AddScoped<ISeoService, SeoService>();

// Register Cache Services
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();

// Register Agent
builder.Services.AddScoped<IBloggingAgent, BloggingAgent.Agents.BloggingAgent>();

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Custom Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseResponseCompression();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "blog-slug",
    pattern: "blog/{slug}",
    defaults: new { controller = "Blog", action = "Details" });

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
    }
}

app.Run();
```

### 12. Middleware

#### Middleware/ErrorHandlingMiddleware.cs
```csharp
using System.Net;
using System.Text.Json;

namespace BloggingAgent.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;

        if (exception is ArgumentException or InvalidOperationException)
        {
            code = HttpStatusCode.BadRequest;
        }
        else if (exception is UnauthorizedAccessException)
        {
            code = HttpStatusCode.Unauthorized;
        }

        var result = JsonSerializer.Serialize(new
        {
            error = exception.Message,
            statusCode = (int)code
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
```

#### Middleware/RequestLoggingMiddleware.cs
```csharp
using System.Diagnostics;

namespace BloggingAgent.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "Request {Method} {Path} completed in {ElapsedMilliseconds}ms with status code {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode);
        }
    }
}
```

### 13. Extensions

#### Extensions/ServiceCollectionExtensions.cs
```csharp
using BloggingAgent.Agents;
using BloggingAgent.Data;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;
using Microsoft.EntityFrameworkCore;

namespace BloggingAgent.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBloggingAgentServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILlmConnector, LlmConnector>();
        services.AddScoped<IMemoryService, MemoryService>();
        services.AddScoped<ISeoService, SeoService>();
        services.AddScoped<IContentFormatter, ContentFormatter>();
        services.AddScoped<IBloggingAgent, BloggingAgent.Agents.BloggingAgent>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();

        return services;
    }
}
```

#### Extensions/StringExtensions.cs
```csharp
namespace BloggingAgent.Extensions;

public static class StringExtensions
{
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    public static int WordCount(this string value)
    {
        return value.Split(new[] { ' ', '\n', '\r', '\t' }, 
            StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
```

#### Extensions/DateTimeExtensions.cs
```csharp
namespace BloggingAgent.Extensions;

public static class DateTimeExtensions
{
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 30)
            return $"{(int)(timeSpan.TotalDays / 7)} week{(timeSpan.TotalDays >= 14 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)} month{(timeSpan.TotalDays >= 60 ? "s" : "")} ago";

        return $"{(int)(timeSpan.TotalDays / 365)} year{(timeSpan.TotalDays >= 730 ? "s" : "")} ago";
    }

    public static string ToReadableDate(this DateTime dateTime)
    {
        return dateTime.ToString("MMMM dd, yyyy 'at' hh:mm tt");
    }
}
```

#### Views/_ViewImports.cshtml
```cshtml
@using BloggingAgent
@using BloggingAgent.Models
@using BloggingAgent.Models.ViewModels
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

#### Views/_ViewStart.cshtml
```cshtml
@{
    Layout = "_Layout";
}
```

#### Views/Shared/_Layout.cshtml
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Mostafa Blogging Agent</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-dark bg-primary mb-3">
            <div class="container">
                <a class="navbar-brand" href="/" aria-label="Mostafa Blogging Agent Home">
                    <strong>Mostafa Blogging Agent</strong>
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" 
                        aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarNav">
                    <ul class="navbar-nav ms-auto">
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Blog" asp-action="Index">Blog Posts</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted mt-5">
        <div class="container text-center py-3">
            &copy; 2025 - Mostafa Blogging Agent - Powered by AI
        </div>
    </footer>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

#### Views/Blog/Index.cshtml
```html
@model BlogIndexViewModel
@{
    ViewData["Title"] = "AI Blog Generator";
}

<div class="row">
    <div class="col-12">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h1 class="display-4">AI Blog Generator</h1>
            <span class="badge bg-info fs-6" role="status" aria-live="polite">
                @Model.TotalPosts Posts Generated
            </span>
        </div>

        @if (TempData["Success"] != null)
        {
            <div class="alert alert-success alert-dismissible fade show" role="alert">
                <strong>Success!</strong> @TempData["Success"]
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        }

        @if (TempData["Error"] != null)
        {
            <div class="alert alert-danger alert-dismissible fade show" role="alert">
                <strong>Error!</strong> @TempData["Error"]
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        }

        <!-- Generation Form -->
        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h2 class="card-title h5 mb-3">Generate New Blog Post</h2>
                <form asp-action="Generate" method="post" id="generateForm">
                    @Html.AntiForgeryToken()
                    <div class="row g-3">
                        <div class="col-md-9">
                            <label for="topic" class="form-label">Enter Blog Topic</label>
                            <input type="text" 
                                   class="form-control" 
                                   id="topic" 
                                   name="topic" 
                                   placeholder="e.g., The Future of Artificial Intelligence" 
                                   required
                                   aria-describedby="topicHelp"
                                   maxlength="200">
                            <div id="topicHelp" class="form-text">
                                Current tone: <strong>@Model.CurrentTone</strong>
                            </div>
                        </div>
                        <div class="col-md-3 d-flex align-items-end">
                            <button type="submit" class="btn btn-primary w-100" id="generateBtn">
                                <span class="spinner-border spinner-border-sm d-none" role="status" aria-hidden="true" id="spinner"></span>
                                <span id="btnText">Generate Post</span>
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>

        <!-- Blog Posts List -->
        <div class="row">
            @if (Model.BlogPosts.Any())
            {
                @foreach (var post in Model.BlogPosts)
                {
                    <div class="col-12 mb-4">
                        <article class="card shadow-sm h-100">
                            <div class="card-body">
                                <div class="d-flex justify-content-between align-items-start mb-2">
                                    <h2 class="card-title h4">@post.Title</h2>
                                    <form asp-action="Delete" asp-route-id="@post.Id" method="post" class="d-inline">
                                        @Html.AntiForgeryToken()
                                        <button type="submit" 
                                                class="btn btn-sm btn-outline-danger" 
                                                onclick="return confirm('Are you sure you want to delete this post?')"
                                                aria-label="Delete post: @post.Title">
                                            Delete
                                        </button>
                                    </form>
                                </div>
                                <div class="mb-3">
                                    <span class="badge bg-secondary me-2">@post.Topic</span>
                                    <span class="badge bg-info me-2">@post.ToneStyle</span>
                                    <span class="badge bg-light text-dark">@post.WordCount words</span>
                                </div>
                                <div class="blog-content mb-3">
                                    @Html.Raw(post.Content.Replace("\n", "<br>"))
                                </div>
                                <footer class="text-muted small">
                                    <time datetime="@post.CreatedAt.ToString("yyyy-MM-dd")">
                                        Published on @post.CreatedAt.ToString("MMMM dd, yyyy 'at' hh:mm tt")
                                    </time>
                                </footer>
                            </div>
                        </article>
                    </div>
                }
            }
            else
            {
                <div class="col-12">
                    <div class="alert alert-info" role="status">
                        <h4 class="alert-heading">No blog posts yet!</h4>
                        <p>Generate your first AI-powered blog post using the form above.</p>
                    </div>
                </div>
            }
        </div>
    </div>
</div>

@section Scripts {
    <script>
        document.getElementById('generateForm').addEventListener('submit', function() {
            const btn = document.getElementById('generateBtn');
            const spinner = document.getElementById('spinner');
            const btnText = document.getElementById('btnText');
            
            btn.disabled = true;
            spinner.classList.remove('d-none');
            btnText.textContent = ' Generating...';
        });
    </script>
}
```

### 21. README.md - Complete Setup and Usage Guide

```markdown
# Blogging Agent - AI-Powered Blog Generator

## ?? Overview
A comprehensive ASP.NET Core 9 MVC web application that uses AI (Ollama/OpenAI) to generate SEO-optimized blog posts with intelligent memory and learning capabilities.

## ? Features

### Core Functionality
- ?? **AI-Powered Content Generation** - Generate 600+ word blog posts using Ollama (local) or OpenAI
- ?? **Agent Memory System** - Remembers past topics and learns tone preferences
- ?? **SEO Optimization** - Automatic SEO analysis, keyword extraction, and meta descriptions
- ?? **Analytics Dashboard** - Track post performance, word counts, and topic trends
- ?? **Responsive UI** - Bootstrap 5 with WCAG AA accessibility compliance
- ?? **SQL Server Database** - Robust data persistence with EF Core
- ?? **Content Regeneration** - Improve existing posts with AI
- ??? **Tagging System** - Organize posts with custom tags
- ?? **Full-Text Search** - Search across titles, content, and topics
- ?? **Settings Management** - Configure LLM providers and agent behavior

### Advanced Features
- **Repository Pattern** - Clean architecture with abstraction layers
- **Caching** - Memory cache for improved performance
- **Error Handling** - Custom middleware for graceful error management
- **Logging** - Comprehensive logging throughout the application
- **SEO Scoring** - Automated content quality assessment
- **Reading Time Calculation** - Automatic estimation of article read time
- **Slug Generation** - SEO-friendly URLs with uniqueness validation

## ?? Prerequisites

- .NET 9.0 SDK
- SQL Server or SQL Server LocalDB
- Ollama (for local AI) **OR** OpenAI API key
- Visual Studio 2022 or VS Code

## ??? Installation Steps

### 1. Clone or Create Project

```bash
dotnet new mvc -n BloggingAgent -f net9.0
cd BloggingAgent
```

### 2. Install Required Packages

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
```

### 3. Configure Database Connection

Edit `appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BloggingAgentDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

For SQL Server:
```json
"DefaultConnection": "Server=localhost;Database=BloggingAgentDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
```

### 4. Configure LLM Provider

#### Option A: Ollama (Local, Free)

1. Install Ollama from https://ollama.ai
2. Pull a model:
```bash
ollama pull llama2
# OR for better quality:
ollama pull llama2:13b
ollama pull mistral
```

3. Verify Ollama is running:
```bash
ollama list
```

4. In `appsettings.json`:
```json
{
  "Llm": {
    "Provider": "ollama",
    "OllamaEndpoint": "http://localhost:11434/api/generate",
    "OllamaModel": "llama2"
  }
}
```

#### Option B: OpenAI (Cloud, Paid)

1. Get API key from https://platform.openai.com
2. In `appsettings.json`:
```json
{
  "Llm": {
    "Provider": "openai"
  },
  "OpenAI": {
    "ApiKey": "sk-your-api-key-here",
    "Model": "gpt-3.5-turbo"
  }
}
```

### 5. Create Database

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If you encounter errors, try:
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
```

### 6. Run the Application

```bash
dotnet run
```

Or use Visual Studio:
- Press F5 or click "Start Debugging"

Navigate to: `https://localhost:5001/Blog` or `http://localhost:5000/Blog`

## ?? Usage Guide

### Generating Your First Blog Post

1. **Navigate to Blog Page**
   - Open `https://localhost:5001/Blog`

2. **Enter a Topic**
   - Type your topic (e.g., "The Future of Artificial Intelligence")
   - Click "Generate Post"

3. **Wait for Generation**
   - The AI will generate a complete blog post (typically 30-90 seconds with Ollama, 10-30 seconds with OpenAI)

4. **View Your Post**
   - The post will appear with title, content, word count, and SEO metrics

### Managing Posts

#### View Post Details
- Click on any post title or "Read More"
- View full content, SEO analysis, and analytics

#### Edit Posts
- Click "Edit" on any post
- Modify title, content, meta description, or tags
- Save changes

#### Delete Posts
- Click "Delete" on any post
- Confirm deletion

#### Regenerate Content
- Open a post
- Click "Regenerate" to create improved version

### Using Analytics

Navigate to `/Analytics` to see:
- Total posts and words written
- Average word count
- Most popular topics
- Tone style distribution
- Recent activity

### Configuring Settings

Navigate to `/Settings` to:
- Switch between Ollama and OpenAI
- View provider availability
- Configure default settings

## ??? Architecture

### Project Structure
```
BloggingAgent/
??? Agents/              # AI agent logic
??? Services/            # Business services
?   ??? LLM/            # LLM providers
?   ??? Memory/         # Agent memory
?   ??? SEO/            # SEO analysis
?   ??? Content/        # Content formatting
?   ??? Cache/          # Caching
??? Controllers/         # MVC controllers
??? Models/             # Domain models, DTOs, ViewModels
??? Data/               # Database context & repositories
??? Views/              # Razor views
??? Utilities/          # Helper classes
??? Extensions/         # Extension methods
??? Middleware/         # Custom middleware
??? Configuration/      # Settings classes
```

### Design Patterns Used
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling
- **Strategy Pattern** - LLM provider switching
- **Factory Pattern** - Service creation
- **MVVM Pattern** - ViewModels for view logic

## ?? Configuration Options

### appsettings.json

```json
{
  "Llm": {
    "Provider": "ollama",           // "ollama" or "openai"
    "OllamaEndpoint": "http://localhost:11434/api/generate",
    "OllamaModel": "llama2",         // Model name
    "MaxRetries": 3,                 // Retry attempts
    "TimeoutSeconds": 120            // Request timeout
  },
  
  "OpenAI": {
    "ApiKey": "",                    // Your API key
    "Model": "gpt-3.5-turbo",       // Model name
    "MaxTokens": 1500,               // Max response tokens
    "Temperature": 0.7               // Creativity (0-2)
  },
  
  "Seo": {
    "MinTitleLength": 40,            // Min title chars
    "MaxTitleLength": 60,            // Max title chars
    "OptimalWordCount": 600,         // Target word count
    "FocusKeywords": []              // Default keywords
  },
  
  "Cache": {
    "Enabled": true,                 // Enable caching
    "CacheDurationMinutes": 60       // Cache TTL
  }
}
```

## ?? Troubleshooting

### Common Issues

#### "Cannot connect to Ollama"
```bash
# Check if Ollama is running
ollama list

# Start Ollama service
ollama serve

# Test endpoint
curl http://localhost:11434/api/tags
```

#### "Database migration failed"
```bash
# Drop database and recreate
dotnet ef database drop
dotnet ef database update
```

#### "OpenAI API error"
- Verify API key is correct
- Check API quota/billing
- Ensure internet connection

#### "Slow generation times"
- Ollama: Use smaller models (llama2 vs llama2:13b)
- OpenAI: Reduce MaxTokens in settings
- Check CPU/GPU resources for Ollama

### Performance Optimization

1. **Use GPU for Ollama**
   - Ollama automatically uses GPU if available
   - Check with `ollama ps`

2. **Optimize Database**
   ```sql
   -- Add indexes for frequent queries
   CREATE INDEX IX_BlogPosts_CreatedAt ON BlogPosts(CreatedAt DESC);
   ```

3. **Enable Response Compression**
   - Already configured in Program.cs

4. **Use Smaller LLM Models**
   - llama2 (7B) - Faster
   - mistral (7B) - Good balance
   - llama2:13b - Better quality, slower

## ?? Database Schema

### Key Tables
- **BlogPosts** - Main blog content
- **AgentMemories** - AI learning data
- **SeoMetadata** - SEO analysis results
- **ContentAnalytics** - Post analytics
- **AgentSettings** - Configuration

## ?? Security Considerations

- Store OpenAI API keys in User Secrets (not in source control)
- Use environment variables for production
- Implement authentication for public deployment
- Sanitize user input before LLM queries
- Rate limit API endpoints

### Using User Secrets (Recommended)

```bash
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "your-api-key"
```

## ?? Deployment

### Deploy to Azure

1. Create Azure SQL Database
2. Create Azure App Service
3. Update connection string
4. Deploy using Visual Studio or CLI

### Deploy to Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BloggingAgent.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BloggingAgent.dll"]
```

## ?? API Endpoints

### Blog Controller
- `GET /Blog` - List all posts
- `GET /Blog/Details/{slug}` - View post
- `POST /Blog/Generate` - Generate new post
- `GET /Blog/Edit/{id}` - Edit form
- `POST /Blog/Edit/{id}` - Update post
- `POST /Blog/Delete/{id}` - Delete post
- `POST /Blog/Regenerate/{id}` - Regenerate content
- `GET /Blog/SeoAnalysis/{id}` - Get SEO analysis

### Analytics Controller
- `GET /Analytics` - Dashboard

### Settings Controller
- `GET /Settings` - Settings page

## ?? Contributing

1. Fork the repository
2. Create feature branch
3. Commit changes
4. Push to branch
5. Create Pull Request

## ?? License

MIT License - feel free to use for personal or commercial projects

## ?? Acknowledgments

- ASP.NET Core Team
- Ollama Project
- OpenAI
- Bootstrap Team

## ?? Support

For issues or questions:
- Create an issue on GitHub
- Check documentation
- Review troubleshooting section

---

**Built with ?? using ASP.NET Core 9, C#, and AI**
```