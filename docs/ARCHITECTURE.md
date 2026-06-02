# Architecture Overview

## Project Structure

```
bloggingAgent/
├── bloggingAgent/                      # Main application
│   ├── Agents/                        # AI agents & prompt templates
│   │   └── *.cs                       # Agent implementations
│   │
│   ├── Controllers/                   # API & MVC controllers
│   │   ├── BlogController.cs          # Blog endpoints
│   │   ├── AnalyticsController.cs     # Analytics endpoints
│   │   ├── SeoController.cs           # SEO analysis endpoints
│   │   └── SettingsController.cs      # Settings endpoints
│   │
│   ├── Services/                      # Business logic layer
│   │   ├── ILlmService.cs             # LLM abstraction
│   │   ├── BlogService.cs             # Blog management
│   │   ├── SeoService.cs              # SEO analysis
│   │   └── AnalyticsService.cs        # Analytics tracking
│   │
│   ├── Data/                          # Data access layer
│   │   ├── ApplicationDbContext.cs    # EF Core context
│   │   ├── Repositories/              # Data repositories
│   │   └── Migrations/                # Database migrations
│   │
│   ├── Models/                        # Domain models & DTOs
│   │   ├── Domain/                    # Core entities
│   │   │   ├── BlogPost.cs
│   │   │   ├── Comment.cs
│   │   │   ├── SeoMetadata.cs
│   │   │   └── ContentAnalytics.cs
│   │   ├── Dtos/                      # Data transfer objects
│   │   └── ViewModels/                # View models
│   │
│   ├── Views/                         # Razor pages
│   │   ├── Blog/                      # Blog UI pages
│   │   ├── Analytics/                 # Analytics UI
│   │   ├── Settings/                  # Settings UI
│   │   └── Shared/                    # Layout & shared
│   │
│   ├── wwwroot/                       # Static assets
│   │   ├── css/                       # Stylesheets
│   │   ├── js/                        # JavaScript
│   │   └── images/                    # Images
│   │
│   ├── Configuration/                 # Settings classes
│   │   ├── OpenAISettings.cs
│   │   ├── LlmSettings.cs
│   │   └── SeoSettings.cs
│   │
│   ├── Extensions/                    # Helper extensions
│   │   └── *.cs                       # Extension methods
│   │
│   ├── Middleware/                    # Custom middleware
│   │   ├── ErrorHandlingMiddleware.cs
│   │   └── LoggingMiddleware.cs
│   │
│   ├── Utilities/                     # Utility functions
│   │   ├── TextProcessing.cs
│   │   ├── SlugGenerator.cs
│   │   └── MarkdownConverter.cs
│   │
│   ├── appsettings.json               # Configuration
│   ├── Program.cs                     # Application entry point
│   └── bloggingAgent.csproj           # Project file
│
├── bloggingAgent.Tests/               # Test projects
│   ├── UnitTests/                     # Unit tests
│   │   ├── Services/
│   │   └── Utilities/
│   └── IntegrationTests/              # Integration tests
│       └── Controllers/
│
├── docs/                              # Documentation
│   ├── GETTING_STARTED.md
│   ├── API.md
│   ├── CONFIGURATION.md
│   └── ARCHITECTURE.md
│
└── .env.example                       # Example environment file
```

## Layered Architecture

### 1. Presentation Layer
- **Components:** Controllers, Views, API endpoints
- **Responsibility:** Handle HTTP requests/responses
- **Examples:** `BlogController`, `AnalyticsController`

### 2. Business Logic Layer (Services)
- **Components:** Service classes implementing business rules
- **Responsibility:** Core application logic, orchestration
- **Key Services:**
  - `BlogService` - Post management
  - `SeoService` - SEO analysis
  - `AnalyticsService` - Metrics tracking
  - `ILlmService` - AI integration (abstraction)

### 3. Data Access Layer
- **Components:** Entity Framework Core, Repositories
- **Responsibility:** Database operations
- **Pattern:** Repository pattern for data access

### 4. Domain Models
- **Components:** Entities and DTOs
- **Responsibility:** Data structure definitions
- **Entities:**
  - `BlogPost` - Core blog post
  - `Comment` - User comments
  - `SeoMetadata` - SEO data
  - `ContentAnalytics` - Performance metrics

## AI Integration Architecture

### LLM Service Abstraction

```csharp
public interface ILlmService
{
    Task<string> GenerateContentAsync(GenerationRequest request);
    Task<string> OptimizeContentAsync(string content);
    Task<string[]> GenerateKeywordsAsync(string content);
}
```

### Implementations
- **OpenAIService** - Uses OpenAI API
- **OllamaService** - Uses local Ollama
- **FallbackService** - Handles provider failures

### Flow
1. Request comes to Controller
2. Service selects appropriate LLM provider
3. Fallback to alternative if primary fails
4. Response processed and returned

## Data Models

### BlogPost Entity
```csharp
public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Excerpt { get; set; }
    public string Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
    public List<string> Tags { get; set; }
    
    // Navigation
    public SeoMetadata SeoMetadata { get; set; }
    public ContentAnalytics Analytics { get; set; }
    public List<Comment> Comments { get; set; }
}
```

### SeoMetadata Entity
```csharp
public class SeoMetadata
{
    public int Id { get; set; }
    public int BlogPostId { get; set; }
    public string MetaDescription { get; set; }
    public string[] Keywords { get; set; }
    public int SeoScore { get; set; }
    public Dictionary<string, object> StructuredData { get; set; }
}
```

### ContentAnalytics Entity
```csharp
public class ContentAnalytics
{
    public int Id { get; set; }
    public int BlogPostId { get; set; }
    public int Views { get; set; }
    public int UniqueViews { get; set; }
    public int Shares { get; set; }
    public int Comments { get; set; }
    public double AverageReadTime { get; set; }
    public double BounceRate { get; set; }
    public Dictionary<string, int> TrafficSources { get; set; }
}
```

## Request Flow Diagram

```
HTTP Request
    ↓
Controller (BlogController, AnalyticsController, etc.)
    ↓
Service Layer (BlogService, SeoService, etc.)
    ↓
[LLM Service] → (OpenAI/Ollama/Fallback)
    ↓
Repository Layer
    ↓
Entity Framework Core
    ↓
SQLite Database
    ↓
Response returned (JSON/HTML)
```

## Key Design Patterns

### 1. Repository Pattern
Abstracts data access, enables easier testing:
```csharp
public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### 2. Service Pattern
Encapsulates business logic:
```csharp
public class BlogService : IBlogService
{
    public async Task<BlogPost> GeneratePostAsync(GenerationRequest request)
    {
        // Generate using LLM
        // Create entity
        // Save to database
        // Return result
    }
}
```

### 3. Dependency Injection
All services injected via constructor:
```csharp
public BlogController(IBlogService blogService, ISeoService seoService)
{
    _blogService = blogService;
    _seoService = seoService;
}
```

### 4. Strategy Pattern
Abstracts AI provider selection:
```csharp
ILlmService service = isOpenAiAvailable 
    ? new OpenAIService(config)
    : new OllamaService(config);
```

## Database Schema

### Core Tables
- `BlogPosts` - Main blog post data
- `SeoMetadatas` - SEO information per post
- `ContentAnalytics` - Performance metrics
- `Comments` - User comments
- `AgentMemories` - AI context memory
- `AgentSettings` - Agent configuration

### Relationships
```
BlogPost (1) ←→ (1) SeoMetadata
BlogPost (1) ←→ (1) ContentAnalytics
BlogPost (1) ←→ (Many) Comments
```

## Configuration Management

Centralized configuration through `appsettings.json`:
- Dependency injection in `Program.cs`
- Options pattern: `IOptions<T>`
- Strongly-typed configuration objects

## Error Handling

### Global Exception Middleware
```csharp
app.UseMiddleware<ErrorHandlingMiddleware>();
```

Handles:
- HTTP exceptions
- Database errors
- API failures
- Validation errors

## Logging

Uses ASP.NET Core built-in logging:
- Console logging (Development)
- File logging (Production)
- Structured logging with Serilog (optional)

## Testing Strategy

### Unit Tests
- Service logic
- Utility functions
- Model validations

### Integration Tests
- API endpoints
- Database operations
- Full request flows

## Performance Considerations

### Caching
- In-memory cache for configuration
- Post cache with TTL
- Analytics aggregation cache

### Database
- Indexed queries
- Pagination for large datasets
- Connection pooling

### API
- Response compression
- Async/await throughout
- Batch operations where possible

## Security Considerations

### Input Validation
- Model validation attributes
- Content sanitization
- SQL injection prevention (EF Core)

### API Protection
- CORS configuration
- Rate limiting (future)
- Request validation

### Data Protection
- Sensitive data in configuration
- Database access control
- HTTPS enforcement (production)

## Extensibility Points

### Add New AI Provider
1. Implement `ILlmService`
2. Register in DI container
3. Update fallback logic

### Add New Service
1. Create interface in Services folder
2. Implement concrete class
3. Register in `Program.cs`
4. Inject into Controller

### Add New Endpoint
1. Create or extend Controller
2. Implement action method
3. Use existing Services
4. Return appropriate response type

## Next Steps

- [Getting Started](./GETTING_STARTED.md)
- [API Documentation](./API.md)
- [Configuration Guide](./CONFIGURATION.md)
