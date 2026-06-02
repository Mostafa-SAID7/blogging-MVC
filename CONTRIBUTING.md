# Contributing to BloggingAgent

First off, thank you for considering contributing to BloggingAgent! It's people like you that make BloggingAgent such a great tool.

## Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the issue list as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible:

* **Use a clear and descriptive title**
* **Describe the exact steps which reproduce the problem**
* **Provide specific examples to demonstrate the steps**
* **Describe the behavior you observed after following the steps**
* **Explain which behavior you expected to see instead and why**
* **Include screenshots and animated GIFs if possible**
* **Include your environment details** (OS, .NET version, etc.)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

* **Use a clear and descriptive title**
* **Provide a step-by-step description of the suggested enhancement**
* **Provide specific examples to demonstrate the steps**
* **Describe the current behavior and the suggested behavior**
* **Explain why this enhancement would be useful**

### Pull Requests

* Fill in the required template
* Follow the styleguides
* Include appropriate test cases
* Update documentation as needed
* End all files with a newline

## Development Setup

### Prerequisites

- .NET 7.0 or higher
- Git
- Visual Studio Code or Visual Studio (recommended)

### Local Development

1. **Fork and Clone**
   ```bash
   git clone https://github.com/yourusername/bloggingAgent.git
   cd bloggingAgent
   ```

2. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Install Dependencies**
   ```bash
   cd bloggingAgent
   dotnet restore
   ```

4. **Run Application**
   ```bash
   dotnet run
   ```

5. **Run Tests**
   ```bash
   dotnet test
   ```

## Styleguides

### Git Commit Messages

* Use the present tense ("Add feature" not "Added feature")
* Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
* Limit the first line to 72 characters or less
* Reference issues and pull requests liberally after the first line

Example:
```
Add support for custom AI providers

- Implement ILlmService abstraction
- Add OllamaService implementation
- Update configuration documentation

Fixes #123
```

### C# Code Style

* Use meaningful variable and method names
* Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
* Use async/await for all I/O operations
* Add XML documentation comments to public members
* Keep methods small and focused (single responsibility)

Example:
```csharp
/// <summary>
/// Generates a blog post using the configured LLM service.
/// </summary>
/// <param name="request">The generation request parameters</param>
/// <returns>The generated blog post</returns>
public async Task<BlogPost> GeneratePostAsync(GenerationRequest request)
{
    // Implementation
}
```

### File Organization

```
bloggingAgent/
├── Controllers/      # API endpoints
├── Services/         # Business logic
├── Models/           # Data models
├── Data/             # Data access
├── Views/            # UI pages
└── [Feature]/        # Feature-specific folders
```

### Comments

* Use `//` for single-line comments
* Use `/* */` for multi-line comments
* Use `///` for documentation comments
* Explain WHY, not WHAT (code explains what)

### Testing

All contributions should include tests:

```csharp
[Fact]
public async Task GeneratePost_WithValidRequest_ReturnsPost()
{
    // Arrange
    var request = new GenerationRequest { Topic = "AI" };
    var service = new BlogService(_mockLlm, _mockRepository);
    
    // Act
    var result = await service.GeneratePostAsync(request);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("AI", result.Title.Contains("AI"));
}
```

## Testing Requirements

* Write unit tests for new services/utilities
* Write integration tests for new endpoints
* Maintain or improve code coverage
* Run tests before submitting PR: `dotnet test`

## Documentation

* Update relevant documentation files
* Add comments to complex logic
* Update README.md if adding new features
* Update API documentation if changing endpoints

## Pull Request Process

1. **Before Submitting**
   ```bash
   # Make sure code builds
   dotnet clean
   dotnet build
   
   # Run all tests
   dotnet test
   
   # Format code
   dotnet format (if available)
   ```

2. **Create Pull Request**
   - Use descriptive title
   - Reference related issues
   - Describe changes in detail
   - Include before/after screenshots if UI changes

3. **PR Template**
   ```markdown
   ## Description
   Brief description of changes
   
   ## Type of Change
   - [ ] Bug fix
   - [ ] New feature
   - [ ] Documentation update
   
   ## Changes Made
   - Change 1
   - Change 2
   
   ## Testing
   - [ ] Unit tests added
   - [ ] Integration tests added
   - [ ] Manual testing completed
   
   ## Screenshots
   If applicable, add screenshots
   
   ## Related Issues
   Closes #123
   ```

4. **Review Process**
   - At least one approval required
   - Address review comments
   - Keep commits clean and organized

## Additional Notes

### Issue and Pull Request Labels

* `bug` - Something isn't working
* `enhancement` - New feature or request
* `documentation` - Improvements or additions to documentation
* `good first issue` - Good for newcomers
* `help wanted` - Extra attention is needed
* `question` - Further information is requested
* `wontfix` - This will not be worked on

### Recognition

Contributors will be recognized in:
- [CONTRIBUTORS.md](./CONTRIBUTORS.md)
- GitHub contributors page
- Release notes

## Questions?

Feel free to:
- Open a discussion on GitHub
- Email support@bloggingagent.com
- Check existing documentation

## License

By contributing to BloggingAgent, you agree that your contributions will be licensed under its MIT License.

---

Thank you for contributing! 🚀
