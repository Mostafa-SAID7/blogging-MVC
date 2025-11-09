using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BloggingAgent.Agents;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.Content;
using BloggingAgent.Services.LLM;
using BloggingAgent.Services.Memory;
using BloggingAgent.Services.SEO;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BloggingAgent.Tests.UnitTests
{
    public class BloggingAgentTests
    {
        private readonly Mock<ILlmConnector> _llmConnectorMock;
        private readonly Mock<ISeoService> _seoServiceMock;
        private readonly Mock<IMemoryService> _memoryServiceMock;
        private readonly Mock<IContentFormatter> _contentFormatterMock;
        private readonly Mock<ILogger<BloggingAgent.Agents.BloggingAgent>> _loggerMock;
        private readonly IOptions<AgentSettings> _settings;

        public BloggingAgentTests()
        {
            _llmConnectorMock = new Mock<ILlmConnector>();
            _seoServiceMock = new Mock<ISeoService>();
            _memoryServiceMock = new Mock<IMemoryService>();
            _contentFormatterMock = new Mock<IContentFormatter>();
            _loggerMock = new Mock<ILogger<BloggingAgent.Agents.BloggingAgent>>();

            _settings = Options.Create(new AgentSettings
            {
                DefaultAuthor = "AI Assistant",
                MaxPostLength = 5000,
                AutoPublish = false
            });
        }

        [Fact]
        public async Task GeneratePostAsync_ValidRequest_ReturnsBlogPostDto()
        {
            // Arrange
            var request = new GeneratePostRequest
            {
                Topic = "Test Topic",
                Keywords = "test, keyword",
                TargetWordCount = 500,
                Tone = "professional",
                TargetAudience = "developers"
            };

            var expectedContent = "# Test Content\n\nThis is generated content.";
            var expectedHtml = "<h1>Test Content</h1><p>This is generated content.</p>";
            var expectedExcerpt = "This is generated content.";

            _llmConnectorMock.Setup(x => x.GenerateContentAsync(It.IsAny<string>(), It.IsAny<int>()))
                           .ReturnsAsync(expectedContent);
            _contentFormatterMock.Setup(x => x.FormatAsHtmlAsync(expectedContent))
                               .ReturnsAsync(expectedHtml);
            _contentFormatterMock.Setup(x => x.ExtractExcerptAsync(expectedContent))
                               .ReturnsAsync(expectedExcerpt);
            _memoryServiceMock.Setup(x => x.StoreAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(Task.CompletedTask);

            var agent = new BloggingAgent.Agents.BloggingAgent(
                _llmConnectorMock.Object,
                _seoServiceMock.Object,
                _memoryServiceMock.Object,
                _contentFormatterMock.Object,
                _loggerMock.Object,
                _settings);

            // Act
            var result = await agent.GeneratePostAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(request.Topic);
            result.Content.Should().Be(expectedHtml);
            result.Excerpt.Should().Be(expectedExcerpt);
            result.Author.Should().Be(_settings.Value.DefaultAuthor);
            result.IsPublished.Should().Be(_settings.Value.AutoPublish);
        }

        [Fact]
        public async Task GeneratePostAsync_TopicNotRelevant_ThrowsException()
        {
            // Arrange
            var request = new GeneratePostRequest
            {
                Topic = "Irrelevant Topic"
            };

            _memoryServiceMock.Setup(x => x.RetrieveAsync(It.IsAny<string>()))
                            .ReturnsAsync("false"); // Topic marked as not relevant

            var agent = new BloggingAgent.Agents.BloggingAgent(
                _llmConnectorMock.Object,
                _seoServiceMock.Object,
                _memoryServiceMock.Object,
                _contentFormatterMock.Object,
                _loggerMock.Object,
                _settings);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                agent.GeneratePostAsync(request));
        }

        [Fact]
        public async Task AnalyzePostAsync_CallsSeoService()
        {
            // Arrange
            var content = "Test content";
            var title = "Test Title";
            var expectedResult = new SeoAnalysisResult { Score = 85 };

            _seoServiceMock.Setup(x => x.AnalyzeContentAsync(content, title))
                         .ReturnsAsync(expectedResult);

            var agent = new BloggingAgent.Agents.BloggingAgent(
                _llmConnectorMock.Object,
                _seoServiceMock.Object,
                _memoryServiceMock.Object,
                _contentFormatterMock.Object,
                _loggerMock.Object,
                _settings);

            // Act
            var result = await agent.AnalyzePostAsync(content, title);

            // Assert
            result.Should().Be(expectedResult);
            _seoServiceMock.Verify(x => x.AnalyzeContentAsync(content, title), Times.Once);
        }

        [Fact]
        public async Task OptimizeContentAsync_CallsLlmConnector()
        {
            // Arrange
            var content = "Original content";
            var keywords = new[] { "test", "keyword" };
            var expectedOptimized = "Optimized content with test and keyword";

            _llmConnectorMock.Setup(x => x.GenerateContentAsync(It.IsAny<string>(), It.IsAny<int>()))
                           .ReturnsAsync(expectedOptimized);

            var agent = new BloggingAgent.Agents.BloggingAgent(
                _llmConnectorMock.Object,
                _seoServiceMock.Object,
                _memoryServiceMock.Object,
                _contentFormatterMock.Object,
                _loggerMock.Object,
                _settings);

            // Act
            var result = await agent.OptimizeContentAsync(content, keywords);

            // Assert
            result.Should().Be(expectedOptimized);
            _llmConnectorMock.Verify(x => x.GenerateContentAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task SuggestTagsAsync_CallsSeoService()
        {
            // Arrange
            var content = "Test content for tagging";
            var expectedTags = new[] { "test", "content", "tagging" };

            _seoServiceMock.Setup(x => x.SuggestKeywordsAsync(content, 8))
                         .ReturnsAsync(expectedTags);

            var agent = new BloggingAgent.Agents.BloggingAgent(
                _llmConnectorMock.Object,
                _seoServiceMock.Object,
                _memoryServiceMock.Object,
                _contentFormatterMock.Object,
                _loggerMock.Object,
                _settings);

            // Act
            var result = await agent.SuggestTagsAsync(content);

            // Assert
            result.Should().BeEquivalentTo(expectedTags);
            _seoServiceMock.Verify(x => x.SuggestKeywordsAsync(content, 8), Times.Once);
        }

        [Fact]
        public async Task IsTopicRelevantAsync_NoMemory_ReturnsTrue()
        {
            // Arrange
            var topic = "New Topic";

            _memoryServiceMock.Setup(x => x.RetrieveAsync(It.IsAny<string>()))
                            .ReturnsAsync((string)null);
            _memoryServiceMock.Setup(x => x.StoreAsync(It.IsAny<string>(), "true", "topic_relevance"))
                            .Returns(Task.CompletedTask);

            var agent = new BloggingAgent.Agents.BloggingAgent(
                _llmConnectorMock.Object,
                _seoServiceMock.Object,
                _memoryServiceMock.Object,
                _contentFormatterMock.Object,
                _loggerMock.Object,
                _settings);

            // Act
            var result = await agent.IsTopicRelevantAsync(topic);

            // Assert
            result.Should().BeTrue();
            _memoryServiceMock.Verify(x => x.StoreAsync(It.IsAny<string>(), "true", "topic_relevance"), Times.Once);
        }

        [Fact]
        public async Task IsTopicRelevantAsync_WithMemory_ReturnsStoredValue()
        {
            // Arrange
            var topic = "Existing Topic";

            _memoryServiceMock.Setup(x => x.RetrieveAsync(It.IsAny<string>()))
                            .ReturnsAsync("false");

            var agent = new BloggingAgent.Agents.BloggingAgent(
                _llmConnectorMock.Object,
                _seoServiceMock.Object,
                _memoryServiceMock.Object,
                _contentFormatterMock.Object,
                _loggerMock.Object,
                _settings);

            // Act
            var result = await agent.IsTopicRelevantAsync(topic);

            // Assert
            result.Should().BeFalse();
            _memoryServiceMock.Verify(x => x.StoreAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}