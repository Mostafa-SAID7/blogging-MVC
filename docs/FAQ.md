# Frequently Asked Questions (FAQ)

Common questions and answers about BloggingAgent.

## General Questions

### What is BloggingAgent?

BloggingAgent is an AI-powered blogging platform that helps you generate, manage, and optimize blog content. It uses Large Language Models (LLMs) like OpenAI GPT or local Ollama to create high-quality blog posts with built-in SEO analysis and performance tracking.

### Is BloggingAgent free to use?

Yes, BloggingAgent is open source and free to use. However, you may need API keys for AI services:
- **OpenAI**: Paid service (recommended for production)
- **Ollama**: Free local AI (good for development/testing)
- **No AI**: Basic functionality without AI generation

### What makes BloggingAgent different?

- **AI-Powered**: Intelligent content generation with multiple LLM providers
- **SEO Built-in**: Real-time SEO analysis and optimization
- **Developer-Friendly**: Clean API, good documentation, easy deployment
- **Extensible**: Easy to customize and integrate with existing systems
- **Open Source**: No vendor lock-in, community-driven development

---

## Installation & Setup

### What are the system requirements?

**Minimum Requirements:**
- .NET 7.0 or higher
- 2 GB RAM
- 1 GB storage
- Windows, Linux, or macOS

**Recommended for Production:**
- .NET 8.0 or higher
- 4+ GB RAM
- 10+ GB storage
- Linux server
- PostgreSQL database
- Redis for caching

### How do I install BloggingAgent?

1. **Quick Start (5 minutes):**
   ```bash
   git clone https://github.com/Mostafa-SAID7/bloggingAgent.git
   cd bloggingAgent/bloggingAgent
   dotnet run
   ```

2. **Docker Installation:**
   ```bash
   docker build -t blogging-agent .
   docker run -p 5000:80 blogging-agent
   ```

See our [Getting Started Guide](./GETTING_STARTED.md) for detailed instructions.

### Do I need an OpenAI API key?

Not required, but recommended:
- **Without API key**: Basic blogging features work
- **With OpenAI key**: AI content generation enabled
- **With Ollama**: Local AI generation (no API key needed)

### How do I get an OpenAI API key?

1. Go to [OpenAI Platform](https://platform.openai.com)
2. Create an account or sign in
3. Navigate to API Keys section
4. Create a new secret key
5. Add it to your configuration

---

## Configuration & Usage

### How do I configure AI providers?

**OpenAI Configuration:**
```json
{
  "OpenAISettings": {
    "ApiKey": "sk-your-key-here",
    "Model": "gpt-3.5-turbo",
    "Temperature": 0.7
  }
}
```

**Ollama Configuration:**
```json
{
  "LlmSettings": {
    "OllamaEndpoint": "http://localhost:11434",
    "OllamaModel": "llama2"
  }
}
```

See [Configuration Guide](./CONFIGURATION.md) for all options.

### How do I switch between databases?

**SQLite (Default - Development):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=bloggingagent.db"
  }
}
```

**PostgreSQL (Recommended - Production):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=blogging_agent;Username=user;Password=password"
  }
}
```

### Can I use my own AI models?

Currently supported:
- ✅ OpenAI (GPT-3.5, GPT-4, etc.)
- ✅ Ollama (Llama2, Neural Chat, etc.)
- 🔄 Custom providers (via interface implementation)

To add custom providers, implement the `ILlmService` interface.

### How do I backup my data?

**SQLite Backup:**
```bash
cp bloggingagent.db bloggingagent_backup.db
```

**PostgreSQL Backup:**
```bash
pg_dump blogging_agent > backup.sql
```

**Automated Backup Script:**
```bash
# Add to crontab for daily backups
0 2 * * * cp /path/to/bloggingagent.db /backups/blog_$(date +\%Y\%m\%d).db
```

---

## API & Integration

### What API endpoints are available?

**Main Categories:**
- **Blog Management**: CRUD operations for posts
- **AI Generation**: Content generation endpoints  
- **SEO Analysis**: Content optimization tools
- **Analytics**: Performance tracking
- **Settings**: Configuration management

See [API Documentation](./API.md) for complete reference.

### How do I integrate with my existing system?

**Common Integration Points:**
- REST API for content management
- Webhook support (planned)
- Database integration
- Docker deployment
- CI/CD pipeline integration

**Example API Usage:**
```bash
# Generate content
curl -X POST http://localhost:5000/api/blog/generate \
  -H "Content-Type: application/json" \
  -d '{"topic": "Your Topic", "targetWordCount": 800}'
```

### Is there rate limiting?

Yes, basic rate limiting is implemented:
- **General API**: 100 requests/minute per IP
- **Generation**: 10 requests/hour per IP
- **Analytics**: 50 requests/minute per IP

Rate limits can be configured in production deployments.

### Can I integrate with WordPress?

Not directly, but you can:
1. Use the API to generate content
2. Export posts as HTML/Markdown
3. Use scripts to sync with WordPress via WP-CLI or REST API

See [Examples](./EXAMPLES.md) for integration scripts.

---

## Performance & Scaling

### How fast is content generation?

**Typical Generation Times:**
- **OpenAI GPT-3.5**: 5-15 seconds for 800 words
- **OpenAI GPT-4**: 15-45 seconds for 800 words  
- **Ollama (local)**: 30-120 seconds depending on hardware

Performance depends on:
- AI provider and model
- Content length and complexity
- Network latency
- Server resources

### Can BloggingAgent handle high traffic?

**Scaling Options:**
- **Horizontal**: Multiple app instances behind load balancer
- **Vertical**: Increase server resources
- **Database**: PostgreSQL with connection pooling
- **Caching**: Redis for distributed caching
- **CDN**: Static asset distribution

**Production Recommendations:**
- Use PostgreSQL instead of SQLite
- Enable distributed caching
- Deploy with Docker/Kubernetes
- Implement proper monitoring

### What about SEO performance?

**Built-in SEO Features:**
- Real-time content analysis
- Keyword density optimization
- Meta tag generation
- Structured data support
- Readability scoring
- Content optimization suggestions

**SEO Scores:**
- Typical generated content scores 70-85/100
- Manual optimization can achieve 90+/100
- Automatic optimization planned for future versions

---

## Troubleshooting

### The application won't start

**Common Solutions:**

1. **Check .NET version:**
   ```bash
   dotnet --version  # Should be 7.0+
   ```

2. **Port already in use:**
   ```bash
   dotnet run -- --urls "http://localhost:6000"
   ```

3. **Database issues:**
   ```bash
   # Reset database
   rm bloggingagent.db*
   dotnet ef database update
   ```

4. **Dependencies:**
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

### API returns errors

**Check these common issues:**

1. **Invalid API key:**
   - Verify OpenAI key in `appsettings.json`
   - Check key hasn't expired
   - Ensure key has sufficient credits

2. **Request format:**
   - Content-Type must be `application/json`
   - Required fields must be present
   - Check request size limits

3. **Rate limiting:**
   - Wait between requests
   - Check rate limit headers
   - Consider upgrading OpenAI plan

### Content generation is slow

**Optimization Tips:**

1. **Use GPT-3.5 instead of GPT-4** for faster generation
2. **Reduce target word count** for shorter generation time
3. **Check network latency** to AI provider
4. **Enable caching** to avoid regenerating similar content
5. **Use Ollama locally** for consistent performance

### SEO analysis not working

**Troubleshooting Steps:**

1. **Check content length** - must be at least 100 words
2. **Verify title is present** in analysis request
3. **Check for special characters** that might break parsing
4. **Update SEO settings** in configuration
5. **Restart application** after config changes

---

## Development & Customization

### How do I contribute to BloggingAgent?

1. **Fork the repository**
2. **Read [Contributing Guide](../CONTRIBUTING.md)**
3. **Check open issues** for good first contributions
4. **Submit pull requests** with tests and documentation

**Contribution Areas:**
- Bug fixes
- New features
- Documentation improvements  
- Performance optimizations
- Test coverage
- UI/UX enhancements

### Can I customize the UI?

Yes, the UI is built with:
- **Razor Pages** for server-side rendering
- **Bootstrap 5** for responsive design
- **JavaScript** for interactivity
- **Custom CSS** for branding

UI files are located in:
- `Views/` - Razor pages
- `wwwroot/` - Static assets (CSS, JS, images)

### How do I add new AI providers?

1. **Implement `ILlmService` interface:**
   ```csharp
   public class CustomAiService : ILlmService
   {
       public async Task<string> GenerateContentAsync(GenerationRequest request)
       {
           // Your implementation
       }
   }
   ```

2. **Register in DI container:**
   ```csharp
   services.AddScoped<ILlmService, CustomAiService>();
   ```

3. **Add configuration settings** as needed

### Can I modify the database schema?

Yes, using Entity Framework migrations:

1. **Modify entity classes** in `Models/Domain/`
2. **Add migration:**
   ```bash
   dotnet ef migrations add YourMigrationName
   ```
3. **Apply migration:**
   ```bash
   dotnet ef database update
   ```

**Note:** Always backup data before schema changes.

---

## Deployment & Production

### How do I deploy to production?

**Deployment Options:**

1. **Docker (Recommended):**
   ```bash
   docker build -t blogging-agent .
   docker run -p 80:80 -e OPENAI_API_KEY=sk-xxx blogging-agent
   ```

2. **Linux Server:**
   ```bash
   dotnet publish -c Release
   sudo systemctl start blogging-agent
   ```

3. **Cloud Platforms:**
   - Azure App Service
   - AWS Elastic Beanstalk  
   - Google Cloud Run
   - Heroku

See [Deployment Guide](./DEPLOYMENT.md) for detailed instructions.

### What about HTTPS/SSL?

**Development:** HTTP is fine for local testing

**Production:** HTTPS is required, options include:
- **Reverse proxy** (Nginx/Apache) with Let's Encrypt
- **Cloud load balancer** with managed certificates
- **Application-level** HTTPS configuration

### How do I monitor the application?

**Built-in Monitoring:**
- ASP.NET Core logging
- Health check endpoints
- Performance counters

**External Monitoring:**
- Application Performance Monitoring (APM)
- Log aggregation (ELK stack, Splunk)
- Uptime monitoring (Pingdom, StatusCake)
- Error tracking (Sentry, Bugsnag)

---

## Security & Privacy

### Is my data secure?

**Data Protection Measures:**
- No sensitive data logged by default
- API keys stored in configuration (not database)
- SQLite database local by default
- HTTPS encryption in production
- Input validation and sanitization

**Recommendations:**
- Use environment variables for secrets
- Enable HTTPS in production
- Implement proper authentication (planned feature)
- Regular security updates
- Follow [Security Guide](../SECURITY.md)

### What data does BloggingAgent collect?

**Application Data:**
- Blog posts and metadata
- SEO analysis results
- Performance analytics (views, etc.)
- Application logs

**Not Collected:**
- Personal identification information
- Payment information
- User tracking across sites
- Unnecessary telemetry

### Can I run BloggingAgent offline?

**Partially:** 
- ✅ Core blogging features work offline
- ✅ Local database (SQLite)
- ✅ Ollama for local AI (if installed)
- ❌ OpenAI requires internet connection
- ❌ Some external integrations need internet

**Fully Offline Setup:**
1. Use Ollama for AI generation
2. Use SQLite database
3. Disable external integrations
4. Run in local network

---

## Pricing & Licensing

### What does BloggingAgent cost?

**BloggingAgent Software:** Free (MIT License)

**Operational Costs:**
- **OpenAI API**: $0.002 per 1K tokens (~$0.01-0.05 per blog post)
- **Hosting**: Varies by provider ($5-50+/month)
- **Database**: PostgreSQL hosting if needed
- **Domain**: ~$10-15/year

**Total Monthly Cost:** $10-100+ depending on usage and hosting choice

### What is the MIT License?

MIT License means you can:
- ✅ Use commercially
- ✅ Modify the code
- ✅ Distribute copies
- ✅ Use privately
- ✅ Create derivative works

Requirements:
- Include original license notice
- No warranty provided

See [LICENSE.txt](../LICENSE.txt) for full details.

### Can I use this for commercial projects?

**Yes!** The MIT License explicitly allows commercial use. You can:
- Build commercial blogging services
- Integrate into client projects
- Resell as part of larger solutions
- Use for internal company blogging

No attribution required in end-user products (but appreciated).

---

## Future Development

### What features are planned?

**Short Term (Next 3 months):**
- User authentication system
- Advanced caching with Redis
- Performance improvements
- Extended documentation

**Medium Term (3-6 months):**
- Social media integration
- Post scheduling system
- Comment management
- Advanced analytics dashboard

**Long Term (6+ months):**
- Mobile applications
- Real-time collaboration
- AI model fine-tuning
- Marketplace for plugins

See [CHANGELOG.md](../CHANGELOG.md) for detailed roadmap.

### How can I request features?

1. **Check existing requests** in [GitHub Issues](https://github.com/Mostafa-SAID7/bloggingAgent/issues)
2. **Create feature request** using our template
3. **Join discussions** about planned features
4. **Vote** on existing feature requests
5. **Contribute code** for features you need

### Will BloggingAgent always be free?

**Core Platform:** Yes, will remain open source and free

**Potential Commercial Services:**
- Hosted/managed versions
- Premium support
- Custom development
- Enterprise features

The open source version will always include full functionality.

---

## Getting Help

### Where can I get support?

**Free Community Support:**
- 📖 [Documentation](./README.md)
- 🐛 [GitHub Issues](https://github.com/Mostafa-SAID7/bloggingAgent/issues)
- 💬 [GitHub Discussions](https://github.com/Mostafa-SAID7/bloggingAgent/discussions)

**Contact Options:**
- 📧 Email: support@bloggingagent.com
- 🔗 Project: https://github.com/Mostafa-SAID7/bloggingAgent

**Before Asking for Help:**
1. Search existing documentation
2. Check GitHub issues
3. Try troubleshooting steps
4. Provide detailed information about your issue

### How do I report bugs?

1. **Check if it's already reported** in [Issues](https://github.com/Mostafa-SAID7/bloggingAgent/issues)
2. **Use bug report template** 
3. **Include:**
   - Steps to reproduce
   - Expected vs actual behavior
   - Environment details (.NET version, OS)
   - Configuration (without sensitive data)
   - Log output or error messages
   - Screenshots if applicable

### Is there a community?

**Growing Community:**
- GitHub repository with active issues/discussions
- Contributors from around the world
- Regular updates and releases
- Welcoming to new contributors

**Join Us:**
- Star the repository
- Submit issues and feature requests
- Contribute code or documentation
- Help other users in discussions

---

**Still have questions?** 

- 📚 Check our [complete documentation](./README.md)
- 💬 Ask in [GitHub Discussions](https://github.com/Mostafa-SAID7/bloggingAgent/discussions)
- 📧 Email us at support@bloggingagent.com

*Last Updated: January 2024*