using System.Threading.Tasks;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.SEO;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BloggingAgent.Tests.UnitTests
{
    public class SeoServiceTests
    {
        private readonly Mock<ILlmConnector> _llmConnectorMock;
        private readonly Mock<ILogger<SeoService>> _loggerMock;
        private readonly SeoService _seoService;

        public SeoServiceTests()
        {
            _llmConnectorMock = new Mock<ILlmConnector>();
            _loggerMock = new Mock<ILogger<SeoService>>();
            _seoService = new SeoService(_llmConnectorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AnalyzeContentAsync_ValidContent_ReturnsAnalysisResult()
        {
            // Arrange
            var content = "<h1>Test Title</h1><p>This is a test content with some keywords.</p><img src='test.jpg' alt='Test image'>";
            var title = "Test Title";

            // Act
            var result = await _seoService.AnalyzeContentAsync(content, title);

            // Assert
            result.Should().NotBeNull();
            result.Score.Should().BeGreaterThanOrEqualTo(0);
            result.Score.Should().BeLessThanOrEqualTo(100);
            result.Checks.Should().ContainKey("HasTitle");
            result.Checks.Should().ContainKey("ContentLength");
            result.Checks.Should().ContainKey("HasHeadings");
            result.Checks.Should().ContainKey("HasImages");
        }

        [Fact]
        public async Task AnalyzeContentAsync_ContentWithAllElements_HighScore()
        {
            // Arrange
            var content = "<h1>Perfect SEO Title</h1><h2>Subtitle</h2><p>This is comprehensive content with over 300 words. " +
                         "It includes proper headings, images, and links. The content is well-structured and provides value.</p>" +
                         "<img src='image.jpg' alt='Descriptive alt text'><a href='/link'>Internal Link</a>";
            var title = "Perfect SEO Title";

            // Act
            var result = await _seoService.AnalyzeContentAsync(content, title);

            // Assert
            result.Score.Should().BeGreaterThan(70);
            result.Checks["HasTitle"].Should().BeTrue();
            result.Checks["ContentLength"].Should().BeTrue();
            result.Checks["HasHeadings"].Should().BeTrue();
            result.Checks["HasImages"].Should().BeTrue();
            result.Checks["HasLinks"].Should().BeTrue();
        }

        [Fact]
        public async Task AnalyzeContentAsync_ContentMissingElements_LowScore()
        {
            // Arrange
            var content = "Short content without proper structure.";
            var title = "";

            // Act
            var result = await _seoService.AnalyzeContentAsync(content, title);

            // Assert
            result.Score.Should().BeLessThan(50);
            result.Checks["HasTitle"].Should().BeFalse();
            result.Checks["ContentLength"].Should().BeFalse();
            result.Suggestions.Should().Contain(s => s.Contains("title"));
            result.Suggestions.Should().Contain(s => s.Contains("content"));
        }

        [Fact]
        public async Task GenerateMetaDescriptionAsync_CallsLlmConnector()
        {
            // Arrange
            var content = "Test content for meta description generation";
            var expectedDescription = "This is a compelling meta description that captures the essence of the content.";

            _llmConnectorMock.Setup(x => x.GenerateContentAsync(It.IsAny<string>(), It.IsAny<int>()))
                           .ReturnsAsync(expectedDescription);

            // Act
            var result = await _seoService.GenerateMetaDescriptionAsync(content);

            // Assert
            result.Should().Be(expectedDescription);
            _llmConnectorMock.Verify(x => x.GenerateContentAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task SuggestKeywordsAsync_CallsLlmConnector()
        {
            // Arrange
            var content = "Test content for keyword suggestions";
            var expectedKeywords = new[] { "test", "content", "keyword", "suggestions" };

            _llmConnectorMock.Setup(x => x.GenerateContentAsync(It.IsAny<string>(), It.IsAny<int>()))
                           .ReturnsAsync("test, content, keyword, suggestions");

            // Act
            var result = await _seoService.SuggestKeywordsAsync(content, 4);

            // Assert
            result.Should().BeEquivalentTo(expectedKeywords);
            _llmConnectorMock.Verify(x => x.GenerateContentAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CalculateReadabilityScoreAsync_ValidContent_ReturnsScore()
        {
            // Arrange
            var content = "This is a simple sentence. It contains basic words that are easy to read. The readability should be quite high.";

            // Act
            var score = await _seoService.CalculateReadabilityScoreAsync(content);

            // Assert
            score.Should().BeGreaterThanOrEqualTo(0);
            score.Should().BeLessThanOrEqualTo(100);
        }

        [Fact]
        public async Task CalculateReadabilityScoreAsync_ComplexContent_LowerScore()
        {
            // Arrange
            var content = "The utilization of sophisticated lexical constructs and convoluted syntactical arrangements inherently diminishes the accessibility quotient for the preponderance of readership demographics, consequently engendering a diminution in overall comprehensibility metrics.";

            // Act
            var score = await _seoService.CalculateReadabilityScoreAsync(content);

            // Assert
            score.Should().BeGreaterThanOrEqualTo(0);
            score.Should().BeLessThanOrEqualTo(100);
            // Complex content should generally have lower readability
            score.Should().BeLessThan(50);
        }

        [Fact]
        public async Task CalculateReadabilityScoreAsync_EmptyContent_ReturnsZero()
        {
            // Arrange
            var content = "";

            // Act
            var score = await _seoService.CalculateReadabilityScoreAsync(content);

            // Assert
            score.Should().Be(0);
        }

        [Fact]
        public async Task AnalyzeContentAsync_IncludesKeywordAnalysis()
        {
            // Arrange
            var content = "SEO is important for content marketing. Good SEO practices help improve search rankings. Content with proper SEO performs better.";
            var title = "SEO Best Practices";

            // Act
            var result = await _seoService.AnalyzeContentAsync(content, title);

            // Assert
            result.KeywordOccurrences.Should().NotBeNull();
            result.KeywordOccurrences.Should().ContainKey("seo");
            result.KeywordOccurrences["seo"].Should().BeGreaterThan(0);
            result.KeywordOccurrences.Should().ContainKey("content");
            result.KeywordOccurrences["content"].Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AnalyzeContentAsync_GeneratesSuggestions()
        {
            // Arrange
            var content = "Short content.";
            var title = "A";

            // Act
            var result = await _seoService.AnalyzeContentAsync(content, title);

            // Assert
            result.Suggestions.Should().NotBeEmpty();
            result.Suggestions.Should().Contain(s => s.Contains("title") || s.Contains("content") || s.Contains("length"));
        }
    }
}