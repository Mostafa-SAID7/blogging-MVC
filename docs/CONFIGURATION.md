# Configuration Guide

## Environment Setup

### Development Environment

Create an `.env` file in the project root:

```bash
cp .env.example .env
```

See `.env.example` for all available configuration options.

## AI Providers

### OpenAI Configuration (Recommended)

1. Get API key from [OpenAI Platform](https://platform.openai.com/api-keys)
2. Update `bloggingAgent/appsettings.json`:

```json
{
  "OpenAISettings": {
    "ApiKey": "sk-your-key-here",
    "Model": "gpt-3.5-turbo",
    "Temperature": 0.7,
    "MaxTokens": 1000
  }
}
```

Or use environment variable:
```bash
export OPENAI_API_KEY=sk-your-key-here
```

**Parameters:**
- `ApiKey` - Your OpenAI API key
- `Model` - Model to use (gpt-3.5-turbo, gpt-4, etc.)
- `Temperature` - Response creativity (0-1, default: 0.7)
- `MaxTokens` - Maximum response length (default: 1000)

### Ollama Configuration (Local AI)

1. Install [Ollama](https://ollama.ai)
2. Start Ollama service: `ollama serve`
3. Pull model: `ollama pull llama2`
4. Update `bloggingAgent/appsettings.json`:

```json
{
  "LlmSettings": {
    "OllamaEndpoint": "http://localhost:11434",
    "OllamaModel": "llama2"
  }
}
```

**Parameters:**
- `OllamaEndpoint` - Ollama service URL
- `OllamaModel` - Model name (llama2, neural-chat, etc.)

## Application Settings

### Core Settings

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Database Configuration

SQLite (default):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=bloggingagent.db"
  }
}
```

PostgreSQL (alternative):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=bloggingagent;Username=user;Password=password"
  }
}
```

### Content Settings

```json
{
  "ContentSettings": {
    "DefaultAuthor": "AI Assistant",
    "MaxPostLength": 5000,
    "DefaultTags": ["blog", "ai-generated"],
    "AutoPublish": false,
    "AutoOptimizeContent": true
  }
}
```

### SEO Settings

```json
{
  "SeoSettings": {
    "MinTitleLength": 30,
    "MaxTitleLength": 60,
    "MinDescriptionLength": 120,
    "MaxDescriptionLength": 160,
    "AutoGenerateMetaDescription": true,
    "AutoOptimizeContent": true,
    "MinReadingTimeMinutes": 2,
    "MaxReadingTimeMinutes": 20
  }
}
```

### Cache Settings

```json
{
  "CacheSettings": {
    "ExpirationMinutes": 30,
    "EnableDistributedCache": false,
    "RedisConnectionString": "localhost:6379"
  }
}
```

### Security Settings

```json
{
  "SecuritySettings": {
    "JwtSecret": "your-secret-key-min-32-chars-long",
    "JwtExpiryHours": 24,
    "EnableCors": true,
    "CorsOrigins": ["http://localhost:3000", "https://yourdomain.com"]
  }
}
```

## Environment Variables

### Development

```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001
LOG_LEVEL=Information
```

### Production

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:80;https://0.0.0.0:443
LOG_LEVEL=Warning
```

## appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=bloggingagent.db"
  },
  "OpenAISettings": {
    "ApiKey": "",
    "Model": "gpt-3.5-turbo",
    "Temperature": 0.7,
    "MaxTokens": 1000
  },
  "LlmSettings": {
    "OllamaEndpoint": "http://localhost:11434",
    "OllamaModel": "llama2"
  },
  "ContentSettings": {
    "DefaultAuthor": "AI Assistant",
    "MaxPostLength": 5000,
    "DefaultTags": ["blog"],
    "AutoPublish": false
  },
  "SeoSettings": {
    "MinTitleLength": 30,
    "MaxTitleLength": 60
  },
  "CacheSettings": {
    "ExpirationMinutes": 30
  }
}
```

## Development vs Production

### Development Mode

```bash
dotnet run
```

Features:
- Detailed error pages
- Verbose logging
- Database resets on startup
- Hot reload enabled

### Production Mode

```bash
dotnet publish -c Release -o ./publish
cd publish
./bloggingAgent
```

Features:
- Minimal error information
- Warning level logging
- Production database handling
- Performance optimized

## Docker Environment Variables

Set environment variables when running Docker:

```bash
docker run -e OPENAI_API_KEY=sk-xxx \
           -e ASPNETCORE_ENVIRONMENT=Production \
           -e ASPNETCORE_URLS=http://0.0.0.0:80 \
           -p 5000:80 \
           blogging-agent:latest
```

## Configuration Priority

Settings are loaded in this order (later overrides earlier):
1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. Environment variables
4. User secrets (development only)

## User Secrets (Development)

Store sensitive data without committing to repository:

```bash
# Initialize secrets storage
dotnet user-secrets init

# Set a secret
dotnet user-secrets set "OpenAISettings:ApiKey" "sk-your-key"

# List all secrets
dotnet user-secrets list

# Remove a secret
dotnet user-secrets remove "OpenAISettings:ApiKey"
```

## Troubleshooting Configuration

### API Key Not Recognized

- Verify key is set in correct location (appsettings.json or .env)
- Check environment variable name exactly matches
- Restart application after changes
- Check for trailing spaces in keys

### Database Connection Failed

- Verify `ConnectionStrings:DefaultConnection` is correct
- Check file permissions for SQLite database
- Ensure database directory exists
- For PostgreSQL: verify server is running and accessible

### Cache Not Working

- Check `CacheSettings:ExpirationMinutes` is set
- For distributed cache: verify Redis connection string
- Restart application to clear cache

### SEO Analysis Failing

- Verify `SeoSettings` values are reasonable
- Check minimum/maximum lengths aren't conflicting
- Ensure content meets minimum length requirements

## Performance Tuning

### For Large Deployments

```json
{
  "CacheSettings": {
    "ExpirationMinutes": 60,
    "EnableDistributedCache": true,
    "RedisConnectionString": "your-redis-server:6379"
  },
  "OpenAISettings": {
    "MaxTokens": 500,
    "Temperature": 0.5
  }
}
```

### For High Traffic

- Enable distributed cache with Redis
- Increase cache expiration time
- Optimize database queries
- Use production build (`-c Release`)

## Next Steps

- [Getting Started](./GETTING_STARTED.md)
- [Deployment Guide](./DEPLOYMENT.md)
