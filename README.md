# BloggingAgent

An AI-powered blogging platform that leverages Large Language Models (LLMs) to generate, optimize, and manage high-quality blog content with built-in SEO analysis and performance tracking.

## 🚀 Features

### AI-Powered Content Generation
- **Multi-Provider LLM Support**: OpenAI GPT and Ollama integration with automatic fallback
- **Intelligent Content Creation**: Generate blog posts with customizable tone, audience, and word count
- **SEO Optimization**: Automatic keyword integration and content optimization
- **Memory System**: Context-aware content generation with learning capabilities

### Content Management
- **Full CRUD Operations**: Create, read, update, and delete blog posts
- **Markdown Support**: Write in Markdown, publish as HTML
- **Draft System**: Save drafts and publish when ready
- **Content Formatting**: Automatic excerpt generation and content optimization

### SEO & Analytics
- **Real-time SEO Analysis**: Content scoring, keyword density, readability metrics
- **Performance Tracking**: Views, engagement, traffic sources, and conversion metrics
- **Automated Meta Tags**: Generate titles, descriptions, and structured data
- **Export Capabilities**: JSON/CSV export for external analysis

### Developer Experience
- **Clean Architecture**: Well-structured codebase with separation of concerns
- **RESTful API**: Complete API with Swagger documentation
- **Responsive UI**: Modern, mobile-friendly interface
- **Extensible Design**: Easy to add new features and integrations

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 7.0+
- **Database**: SQLite (Entity Framework Core)
- **Frontend**: Razor Pages, Bootstrap 5, JavaScript
- **AI Integration**: OpenAI API, Ollama
- **Caching**: In-memory cache with TTL
- **Logging**: Built-in ASP.NET Core logging

## 📋 Prerequisites

- .NET 7.0 or higher
- SQLite (included with .NET)
- Optional: OpenAI API key for enhanced AI features
- Optional: Ollama for local AI processing

## 🚀 Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/bloggingagent.git
   cd bloggingagent
   ```

2. **Configure AI providers** (optional but recommended)
   - For OpenAI: Add your API key to `bloggingAgent/appsettings.json`
   - For Ollama: Ensure Ollama is running locally on port 11434

3. **Run the application**
   ```bash
   cd bloggingAgent
   dotnet run
   ```

4. **Access the application**
   - Open your browser to `https://localhost:5001`
   - Navigate to `/blog/generate` to create your first AI-generated post

## ⚙️ Configuration

### AI Providers

#### OpenAI Configuration
```json
{
  "OpenAISettings": {
    "ApiKey": "your-openai-api-key-here",
    "Model": "gpt-3.5-turbo",
    "Temperature": 0.7
  }
}
```

#### Ollama Configuration
```json
{
  "LlmSettings": {
    "OllamaEndpoint": "http://localhost:11434",
    "OllamaModel": "llama2"
  }
}
```

### SEO Settings
```json
{
  "SeoSettings": {
    "MinTitleLength": 30,
    "MaxTitleLength": 60,
    "AutoGenerateMetaDescription": true,
    "AutoOptimizeContent": true
  }
}
```

## 📖 API Documentation

The application includes Swagger UI for API documentation. When running, visit `/swagger` to explore available endpoints.

### Key API Endpoints

- `GET /blog` - List blog posts
- `POST /blog/generate` - Generate new blog post
- `GET /analytics` - View analytics dashboard
- `GET /settings` - Application settings

## 🏗️ Architecture

```
BloggingAgent/
├── Agents/              # AI agents and prompt templates
├── Controllers/         # MVC controllers
├── Data/               # Database context and repositories
├── Models/             # Domain models, DTOs, ViewModels
├── Services/           # Business logic services
├── Views/              # Razor views
├── wwwroot/            # Static assets (CSS, JS)
├── Configuration/      # Settings classes
├── Extensions/         # Helper extensions
├── Middleware/         # Custom middleware
└── Utilities/          # Text processing utilities
```

## 🔧 Development

### Running Tests
```bash
dotnet test
```

### Building for Production
```bash
dotnet publish -c Release -o ./publish
```

### Database Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.

## 🙏 Acknowledgments

- OpenAI for providing powerful language models
- Ollama for local AI processing capabilities
- The .NET community for excellent frameworks and tools
- Bootstrap for responsive UI components

## 📞 Support

For support, email support@bloggingagent.com or join our Discord community.

---

**BloggingAgent** - Transform your content creation workflow with the power of AI. Write better, faster, and smarter.