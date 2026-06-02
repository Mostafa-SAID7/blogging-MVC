# 🚀 BloggingAgent

> **AI-powered blogging platform** with intelligent content generation, real-time SEO analysis, and performance tracking.

Transform your content creation workflow with advanced AI integration and built-in optimization tools.

![Status](https://img.shields.io/badge/status-active-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-7.0+-blue)
![Platform](https://img.shields.io/badge/platform-windows%20%7C%20linux%20%7C%20macos-lightgrey)

---

## ✨ Key Features

### 🤖 AI-Powered Content Generation
- **Multi-Provider LLM Support** - OpenAI GPT and Ollama with automatic fallback
- **Customizable Content** - Adjustable tone, audience, word count, and keywords
- **Context-Aware** - Memory system for intelligent, coherent generation
- **Batch Operations** - Generate multiple posts efficiently

### 📝 Content Management
- **Full CRUD Operations** - Create, read, update, delete blog posts
- **Draft System** - Save drafts and publish when ready
- **Markdown Editor** - Native markdown support with HTML conversion
- **Auto-Optimization** - Automatic excerpt and content formatting

### 🔍 SEO & Analytics
- **Real-time SEO Analysis** - Scoring, keyword density, readability metrics
- **Performance Tracking** - Views, engagement, traffic sources, conversions
- **Automated Meta Tags** - AI-generated titles, descriptions, structured data
- **Export Tools** - JSON/CSV data export for external analysis

### 🎨 Developer Experience
- **Clean Architecture** - Layered design with clear separation of concerns
- **RESTful API** - Complete API with interactive Swagger documentation
- **Responsive UI** - Modern interface built with Bootstrap 5 and JavaScript
- **Extensible** - Easy to add features, integrations, and custom providers

---

## 🛠️ Technology Stack

| Category | Tech |
|----------|------|
| **Runtime** | .NET 7.0+ / .NET 10.0 |
| **Database** | SQLite (default) / PostgreSQL (production) |
| **Frontend** | Razor Pages, Bootstrap 5, JavaScript |
| **AI** | OpenAI API, Ollama (local) |
| **Caching** | In-memory cache, Redis support |
| **Logging** | ASP.NET Core built-in |

---

## 🚀 Quick Start

### Prerequisites
- .NET 7.0 or higher
- Git
- Optional: [OpenAI API key](https://platform.openai.com/api-keys)
- Optional: [Ollama](https://ollama.ai) for local AI

### 30-Second Setup

```bash
# Clone repository
git clone https://github.com/Mostafa-SAID7/bloggingAgent.git
cd bloggingAgent

# Run application
cd bloggingAgent
dotnet run
```

Open [http://localhost:5000](http://localhost:5000) in your browser.

---

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| [**Getting Started**](./docs/GETTING_STARTED.md) | Installation, configuration, first steps |
| [**API Documentation**](./docs/API.md) | Complete API reference with examples |
| [**Configuration Guide**](./docs/CONFIGURATION.md) | Environment setup and settings |
| [**Architecture**](./docs/ARCHITECTURE.md) | System design and code structure |
| [**Deployment**](./docs/DEPLOYMENT.md) | Production deployment options |

---

## 🎯 API Endpoints

### Blog Management
```
GET    /api/blog                    # List posts
POST   /api/blog/generate           # Generate new post
GET    /api/blog/{slug}             # Get specific post
POST   /api/blog/publish/{id}       # Publish post
POST   /api/blog/unpublish/{id}     # Unpublish post
```

### Analytics
```
GET    /api/analytics               # Overall analytics
GET    /api/analytics/post/{id}     # Post-specific metrics
GET    /api/analytics/export        # Export data (JSON/CSV)
```

### SEO Tools
```
POST   /api/seo/analyze             # Analyze content
POST   /api/seo/meta-description    # Generate meta description
POST   /api/seo/keywords            # Get keyword suggestions
```

### Settings
```
GET    /api/settings                # Get configuration
POST   /api/settings/update         # Update settings
POST   /api/settings/reset          # Reset to defaults
```

**Interactive Docs:** Visit `/swagger` when app is running.

---

## ⚙️ Configuration

### AI Provider Selection

**OpenAI (Recommended for Production)**
```json
{
  "OpenAISettings": {
    "ApiKey": "sk-your-key-here",
    "Model": "gpt-3.5-turbo",
    "Temperature": 0.7
  }
}
```

**Ollama (Local Development)**
```json
{
  "LlmSettings": {
    "OllamaEndpoint": "http://localhost:11434",
    "OllamaModel": "llama2"
  }
}
```

[Full Configuration Guide →](./docs/CONFIGURATION.md)

---

## 🐳 Docker Deployment

```bash
# Build image
docker build -t blogging-agent:latest .

# Run container
docker run -p 5000:80 \
  -e OPENAI_API_KEY=sk-your-key \
  blogging-agent:latest

# Using Docker Compose
docker-compose up -d
```

[Detailed Deployment Guide →](./docs/DEPLOYMENT.md)

---

## 🏗️ Project Architecture

```
bloggingAgent/
├── Controllers/          # API endpoints
├── Services/             # Business logic
├── Data/                 # Database & repositories
├── Models/               # Domain entities
├── Views/                # UI pages
├── Agents/               # AI prompt templates
├── Configuration/        # Settings classes
├── Utilities/            # Helper functions
└── wwwroot/              # Static assets
```

[Architecture Deep Dive →](./docs/ARCHITECTURE.md)

---

## 🧪 Development

### Running Tests
```bash
dotnet test
dotnet test --verbosity detailed
```

### Building for Production
```bash
dotnet publish -c Release -o ./publish
```

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName

# Apply changes
dotnet ef database update

# View history
dotnet ef migrations list
```

---

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| **Port 5000 in use** | `dotnet run -- --urls "http://localhost:6000"` |
| **API key not working** | Verify in `appsettings.json` or `.env` |
| **Database errors** | Delete `.db-shm` and `.db-wal` files, restart app |
| **Build fails** | `dotnet clean && dotnet restore && dotnet build` |

[More Troubleshooting →](./docs/GETTING_STARTED.md#troubleshooting)

---

## 🤝 Contributing

Contributions are welcome! Here's how:

1. **Fork** the repository
2. **Create** feature branch: `git checkout -b feature/amazing-feature`
3. **Commit** changes: `git commit -m 'Add amazing feature'`
4. **Push** to branch: `git push origin feature/amazing-feature`
5. **Open** a Pull Request

---

## 📋 Project Status

- ✅ Core blogging functionality
- ✅ AI content generation
- ✅ SEO analysis
- ✅ Analytics dashboard
- ✅ RESTful API
- 🔄 User authentication (in progress)
- 🔄 Advanced caching (planned)
- 🔄 Social media integration (planned)

---

## 📝 License

This project is licensed under the **MIT License** - see [LICENSE.txt](./LICENSE.txt)

---

## 📞 Support & Community

- 📖 [Documentation](./docs/)
- 🐛 [Report Issues](https://github.com/Mostafa-SAID7/bloggingAgent/issues)
- 💬 [Discussions](https://github.com/Mostafa-SAID7/bloggingAgent/discussions)
- 📧 Email: support@bloggingagent.com

---

## 🙏 Acknowledgments

- **OpenAI** - Advanced language models
- **Ollama** - Local AI processing
- **.NET Community** - Excellent frameworks and tools
- **Bootstrap** - Responsive UI components

---

## 🎯 Quick Links

| Link | Purpose |
|------|---------|
| [Setup Guide](./docs/GETTING_STARTED.md) | First-time setup |
| [API Reference](./docs/API.md) | Complete endpoint documentation |
| [Config Options](./docs/CONFIGURATION.md) | All settings explained |
| [System Design](./docs/ARCHITECTURE.md) | Technical overview |
| [Deploy Guide](./docs/DEPLOYMENT.md) | Production deployment |
| [Example ENV](./env.example) | Environment variables template |

---

**Built with ❤️ to transform content creation**

Transform your content creation workflow with the power of AI. Write better, faster, and smarter. 🚀
