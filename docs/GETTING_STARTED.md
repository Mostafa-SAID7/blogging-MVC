# Getting Started

## Prerequisites

- **.NET 7.0** or higher ([Download](https://dotnet.microsoft.com/download))
- **Git** for version control
- **Optional:** [OpenAI API key](https://platform.openai.com/api-keys) for enhanced AI features
- **Optional:** [Ollama](https://ollama.ai) for local AI processing

## Quick Start (5 minutes)

### 1. Clone the Repository

```bash
git clone https://github.com/Mostafa-SAID7/bloggingAgent.git
cd bloggingAgent
```

### 2. Configure Environment (Optional)

Copy the example environment file and update with your settings:

```bash
cp .env.example .env
```

Edit `.env` and add your API keys if desired. The app works without them (uses demo mode).

### 3. Run the Application

```bash
cd bloggingAgent
dotnet run
```

The application will:
- Build the project
- Initialize the SQLite database
- Seed sample data
- Start the server on `http://localhost:5000`

### 4. Access the Application

- **Web UI:** Open [http://localhost:5000](http://localhost:5000) in your browser
- **API Docs:** Visit [http://localhost:5000/swagger](http://localhost:5000/swagger)
- **Generate Post:** Navigate to `/blog/generate`

## Configuration

### AI Provider Setup

#### Using OpenAI (Recommended)

1. Get your API key from [OpenAI Platform](https://platform.openai.com)
2. Update `bloggingAgent/appsettings.json`:

```json
{
  "OpenAISettings": {
    "ApiKey": "sk-your-key-here",
    "Model": "gpt-3.5-turbo",
    "Temperature": 0.7
  }
}
```

Or set via environment variable:
```bash
OPENAI_API_KEY=sk-your-key-here
```

#### Using Ollama (Local)

1. Install [Ollama](https://ollama.ai)
2. Start Ollama: `ollama serve`
3. Pull a model: `ollama pull llama2`
4. Update `bloggingAgent/appsettings.json`:

```json
{
  "LlmSettings": {
    "OllamaEndpoint": "http://localhost:11434",
    "OllamaModel": "llama2"
  }
}
```

## Running Tests

```bash
# All tests
dotnet test

# Specific test project
dotnet test bloggingAgent.Tests/bloggingAgent.Tests.csproj

# With verbose output
dotnet test --verbosity detailed
```

## Building for Production

```bash
# Build release version
dotnet publish -c Release -o ./publish

# Run published version
cd publish
./bloggingAgent.exe  # or dotnet bloggingAgent.dll on Linux/Mac
```

## Docker Deployment

### Using Docker

```bash
# Build image
docker build -t blogging-agent:latest .

# Run container
docker run -p 5000:5000 blogging-agent:latest
```

### Using Docker Compose

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

## Database Configuration

### SQL Server (Recommended for Production)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server.databaseasp.net; Database=your-db; User Id=your-user; Password=your-password; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"
  }
}
```

**Connection String Parameters:**
- `Server` - Your SQL Server host/endpoint
- `Database` - Database name
- `User Id` - Username for authentication
- `Password` - Password for authentication
- `Encrypt=True` - Enable connection encryption
- `TrustServerCertificate=True` - Trust self-signed certificates (production: False)
- `MultipleActiveResultSets=True` - Allow multiple queries

### SQLite (Default - Development Only)

## Troubleshooting

### Port Already in Use

If port 5000 is already in use:

```bash
# Specify different port
dotnet run -- --urls "http://localhost:6000"
```

### Database Connection Issues

- Check if `bloggingagent.db` exists in the project root
- Verify file permissions allow read/write
- Clear SQLite cache: Delete `.db-shm` and `.db-wal` files

### API Key Errors

- Verify your API key is correctly set in `appsettings.json` or `.env`
- Check API key hasn't expired or been revoked
- Ensure OPENAI_API_KEY environment variable is set correctly

### Build Failures

```bash
# Clean previous builds
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

## Project Structure

```
bloggingAgent/
├── bloggingAgent/              # Main application
│   ├── Agents/                # AI agents & prompts
│   ├── Controllers/           # API endpoints
│   ├── Services/              # Business logic
│   ├── Data/                  # Database & repositories
│   ├── Models/                # Domain models
│   ├── Views/                 # UI pages
│   ├── wwwroot/               # Static assets
│   └── Program.cs             # App configuration
├── bloggingAgent.Tests/        # Unit & integration tests
├── docs/                       # Documentation
└── README.md                   # Project overview
```

## Next Steps

- Read the [API Documentation](./API.md)
- Check [Configuration Guide](./CONFIGURATION.md)
- Review [Architecture Overview](./ARCHITECTURE.md)
- Explore example requests in `/examples`

## Support

- 📖 [Full Documentation](./README.md)
- 🐛 [Report Issues](https://github.com/Mostafa-SAID7/bloggingAgent/issues)
- 💬 [Discussions](https://github.com/Mostafa-SAID7/bloggingAgent/discussions)
