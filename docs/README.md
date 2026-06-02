# 📚 BloggingAgent Documentation

Complete documentation for BloggingAgent - an AI-powered blogging platform.

## 📖 Documentation Index

### Getting Started
**[Getting Started Guide](./GETTING_STARTED.md)**
- Installation and setup
- Configuration
- Running the application
- Docker deployment
- Troubleshooting

### API Reference
**[API Documentation](./API.md)**
- Complete endpoint reference
- Request/response examples
- Error handling
- Rate limiting
- cURL examples
- Interactive Swagger UI

### Configuration
**[Configuration Guide](./CONFIGURATION.md)**
- Environment variables
- AI provider setup (OpenAI & Ollama)
- Application settings
- Database configuration
- Security settings
- Performance tuning

### System Design
**[Architecture Overview](./ARCHITECTURE.md)**
- Project structure
- Layered architecture
- Data models
- Design patterns
- Request flow
- Database schema
- Extensibility points

### Deployment
**[Deployment Guide](./DEPLOYMENT.md)**
- Local production build
- Docker deployment
- Cloud platforms (Azure, AWS, Google Cloud)
- Linux server setup
- Nginx reverse proxy
- SSL/TLS certificates
- Database backup strategy
- Monitoring and logging

### Examples & Use Cases
**[Examples Guide](./EXAMPLES.md)**
- API usage examples
- Configuration examples
- Real-world use cases
- Integration examples
- Automation scripts

### FAQ & Troubleshooting
**[FAQ](./FAQ.md)**
- Common questions and answers
- Troubleshooting guide
- Performance tips
- Security considerations

---

## 🚀 Quick Navigation

### I want to...

#### **Get Started (First Time)**
→ Start with [Getting Started Guide](./GETTING_STARTED.md)
- Covers installation, configuration, and running the app

#### **Integrate with the API**
→ Read [API Documentation](./API.md)
- Complete endpoint reference with curl examples
- Interactive Swagger docs at `/swagger`

#### **Deploy to Production**
→ Follow [Deployment Guide](./DEPLOYMENT.md)
- Docker, Cloud platforms, and Linux server options

#### **Understand the Code**
→ Review [Architecture Overview](./ARCHITECTURE.md)
- System design, project structure, and patterns

#### **Configure the Application**
→ Check [Configuration Guide](./CONFIGURATION.md)
- All environment variables and settings explained

---

## 🎯 Common Tasks

### Setup & Installation
1. [Quick Start (5 min)](./GETTING_STARTED.md#quick-start-5-minutes)
2. [AI Provider Setup](./GETTING_STARTED.md#configuration)
3. [Running Tests](./GETTING_STARTED.md#running-tests)

### API Development
1. [API Overview](./API.md#overview)
2. [Blog Endpoints](./API.md#blog-endpoints)
3. [Analytics Endpoints](./API.md#analytics-endpoints)
4. [Error Handling](./API.md#error-handling)

### Production Deployment
1. [Production Checklist](./DEPLOYMENT.md#production-checklist)
2. [Docker Setup](./DEPLOYMENT.md#docker-deployment)
3. [Cloud Deployment](./DEPLOYMENT.md#cloud-deployment)
4. [Backup Strategy](./DEPLOYMENT.md#backup-strategy)

### Troubleshooting
1. [Common Issues](./GETTING_STARTED.md#troubleshooting)
2. [Deployment Issues](./DEPLOYMENT.md#troubleshooting-deployment)
3. [Configuration Problems](./CONFIGURATION.md#troubleshooting-configuration)

---

## 📋 Key Concepts

### Architecture
- **Layered Design**: Presentation → Services → Data Access → Database
- **Repository Pattern**: Abstracted data access for testability
- **Dependency Injection**: All services injected via constructor
- **Strategy Pattern**: Abstracted AI provider selection (OpenAI/Ollama)

### Data Models
- **BlogPost**: Core blog post entity with metadata
- **SeoMetadata**: SEO analysis and optimization data
- **ContentAnalytics**: Performance and engagement metrics
- **Comment**: User comments on posts

### API
- **RESTful Design**: Standard HTTP methods and status codes
- **JSON Format**: All requests and responses in JSON
- **Swagger Documentation**: Interactive API docs at `/swagger`
- **Error Responses**: Consistent error format with details

---

## 🔧 Configuration Quick Reference

### Environment Variables
```bash
# Required
OPENAI_API_KEY=sk-your-key-here

# Optional
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:5000
LOG_LEVEL=Information
```

### Settings Files
- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production configuration
- `.env` - Environment variables file

[Full Configuration Reference →](./CONFIGURATION.md)

---

## 🐳 Deployment Quick Reference

### Docker
```bash
# Build
docker build -t blogging-agent:latest .

# Run
docker run -p 5000:80 \
  -e OPENAI_API_KEY=sk-your-key \
  blogging-agent:latest
```

### Linux/Ubuntu
```bash
# Build
dotnet publish -c Release

# Install as service
sudo cp -r publish/* /var/www/blogging-agent/
sudo systemctl start blogging-agent
```

### Cloud Platforms
- **Azure App Service** - Git or zip deployment
- **AWS Elastic Beanstalk** - Automated deployment
- **Google Cloud Run** - Container-based

[Full Deployment Guide →](./DEPLOYMENT.md)

---

## 🧪 Testing

### Run Tests
```bash
# All tests
dotnet test

# Specific project
dotnet test bloggingAgent.Tests/

# With verbosity
dotnet test --verbosity detailed
```

### Test Structure
- Unit tests in `UnitTests/` folder
- Integration tests in `IntegrationTests/` folder
- Mock implementations for services

---

## 📊 API Endpoints Summary

| Method | Endpoint | Purpose |
|--------|----------|---------|
| `GET` | `/api/blog` | List blog posts |
| `POST` | `/api/blog/generate` | Generate new post |
| `GET` | `/api/blog/{slug}` | Get specific post |
| `GET` | `/api/analytics` | Overall analytics |
| `GET` | `/api/analytics/post/{id}` | Post metrics |
| `POST` | `/api/seo/analyze` | SEO analysis |
| `GET` | `/api/settings` | Get settings |
| `POST` | `/api/settings/update` | Update settings |

[Complete API Reference →](./API.md)

---

## 🏗️ Project Structure

```
bloggingAgent/
├── bloggingAgent/
│   ├── Agents/              # AI prompt templates
│   ├── Controllers/         # API endpoints
│   ├── Services/            # Business logic
│   ├── Data/                # Database & repositories
│   ├── Models/              # Entities & DTOs
│   ├── Views/               # UI pages
│   └── Configuration/       # Settings classes
├── bloggingAgent.Tests/     # Unit & integration tests
├── docs/                    # Documentation (you are here!)
└── README.md                # Project overview
```

[Architecture Deep Dive →](./ARCHITECTURE.md)

---

## 📚 External Resources

### Official Documentation
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [OpenAI API](https://platform.openai.com/docs/api-reference)
- [Ollama](https://ollama.ai/library)

### Tools & Services
- [OpenAI Platform](https://platform.openai.com/)
- [Ollama - Local AI](https://ollama.ai)
- [Swagger UI](https://swagger.io/)
- [Bootstrap 5](https://getbootstrap.com/)

---

## ❓ Frequently Asked Questions

**Q: What are the system requirements?**  
A: .NET 7.0 or higher. Optional: OpenAI API key or local Ollama instance.

**Q: Which database systems are supported?**  
A: SQLite (default, development), PostgreSQL (recommended for production).

**Q: How do I switch between OpenAI and Ollama?**  
A: Update configuration in `appsettings.json` - see [Configuration Guide](./CONFIGURATION.md).

**Q: Can I deploy to the cloud?**  
A: Yes! See [Deployment Guide](./DEPLOYMENT.md) for Azure, AWS, and Google Cloud options.

**Q: How do I set up HTTPS?**  
A: Use Let's Encrypt with Nginx reverse proxy - see [Deployment Guide](./DEPLOYMENT.md#ssltls-certificate).

**Q: What's included in the API?**  
A: Blog management, SEO analysis, analytics tracking, and settings configuration - see [API Documentation](./API.md).

---

## 🆘 Getting Help

### Documentation
- 📖 [Full Documentation](../README.md)
- 🏗️ [Architecture & Design](./ARCHITECTURE.md)
- 🚀 [Quick Start Guide](./GETTING_STARTED.md)

### Community
- 🐛 [Report Issues](https://github.com/Mostafa-SAID7/bloggingAgent/issues)
- 💬 [Ask Questions](https://github.com/Mostafa-SAID7/bloggingAgent/discussions)
- 📧 Email: support@bloggingagent.com

### Troubleshooting
- [Getting Started Troubleshooting](./GETTING_STARTED.md#troubleshooting)
- [Deployment Issues](./DEPLOYMENT.md#troubleshooting-deployment)
- [Configuration Problems](./CONFIGURATION.md#troubleshooting-configuration)

---

## 📝 Documentation Versions

| Version | Release Date | Status |
|---------|--------------|--------|
| 1.0 | Jan 2024 | ✅ Current |

---

## 🔄 Continuous Learning

### Recommended Reading Order for New Developers

1. **Start**: [Project README](../README.md) - Overview
2. **Setup**: [Getting Started](./GETTING_STARTED.md) - Installation & first run
3. **Code**: [Architecture](./ARCHITECTURE.md) - Understand the design
4. **Build**: [API Documentation](./API.md) - Learn the interfaces
5. **Config**: [Configuration](./CONFIGURATION.md) - Customize behavior
6. **Deploy**: [Deployment](./DEPLOYMENT.md) - Production readiness

---

**Last Updated**: January 2024  
**Maintained By**: BloggingAgent Team  
**License**: MIT

---

← [Back to Main README](../README.md)
