using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BloggingAgent.Controllers;
using BloggingAgent.Data;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.Cache;
using BloggingAgent.Services.SEO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BloggingAgent.Tests.IntegrationTests
{
    public class BlogControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public BlogControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace database with in-memory database for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });

                    // Ensure database is created
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureCreated();
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetBlogIndex_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/blog");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetBlogDetails_ExistingPost_ReturnsSuccess()
        {
            // Arrange - Create a test post
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var testPost = new BlogPost
            {
                Title = "Test Post",
                Slug = "test-post",
                Content = "Test content",
                Author = "Test Author",
                CreatedAt = DateTime.UtcNow,
                IsPublished = true
            };
            db.BlogPosts.Add(testPost);
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/blog/test-post");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Post", content);
        }

        [Fact]
        public async Task GetBlogDetails_NonExistingPost_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/blog/non-existing-post");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GeneratePost_ValidRequest_ReturnsRedirect()
        {
            // Arrange
            var request = new GeneratePostRequest
            {
                Topic = "Integration Test Topic",
                Keywords = "test, integration",
                TargetWordCount = 100,
                Tone = "professional",
                TargetAudience = "developers"
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/blog/generate", content);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/blog/", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task GeneratePost_InvalidRequest_ReturnsViewWithErrors()
        {
            // Arrange
            var request = new GeneratePostRequest
            {
                Topic = "", // Invalid - empty topic
                TargetWordCount = 100
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/blog/generate", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Returns to form with validation errors
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("generateForm", responseContent); // Should return the form
        }

        [Fact]
        public async Task PublishPost_ExistingPost_ReturnsRedirect()
        {
            // Arrange - Create a test post
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var testPost = new BlogPost
            {
                Title = "Draft Post",
                Slug = "draft-post",
                Content = "Draft content",
                Author = "Test Author",
                CreatedAt = DateTime.UtcNow,
                IsPublished = false
            };
            db.BlogPosts.Add(testPost);
            await db.SaveChangesAsync();

            // Act
            var response = await _client.PostAsync($"/blog/publish/{testPost.Id}", null);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/blog/draft-post", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task PublishPost_NonExistingPost_ReturnsNotFound()
        {
            // Act
            var response = await _client.PostAsync("/blog/publish/99999", null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAnalytics_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/analytics");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task ExportAnalytics_JsonFormat_ReturnsJsonFile()
        {
            // Act
            var response = await _client.GetAsync("/analytics/export?format=json");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.Contains("attachment", response.Content.Headers.ContentDisposition.ToString());
        }

        [Fact]
        public async Task ExportAnalytics_CsvFormat_ReturnsCsvFile()
        {
            // Act
            var response = await _client.GetAsync("/analytics/export?format=csv");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.ToString());
            Assert.Contains("attachment", response.Content.Headers.ContentDisposition.ToString());
        }

        [Fact]
        public async Task GetSettings_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/settings");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task UpdateSettings_ValidData_ReturnsRedirect()
        {
            // Arrange
            var formData = new MultipartFormDataContent
            {
                { new StringContent("AI Assistant"), "Settings.DefaultAuthor" },
                { new StringContent("5000"), "Settings.MaxPostLength" },
                { new StringContent("blog, ai-generated"), "Settings.DefaultTags" },
                { new StringContent("false"), "Settings.AutoPublish" }
            };

            // Act
            var response = await _client.PostAsync("/settings/update", formData);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/settings", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task ApiEndpoints_ReturnJsonResponses()
        {
            // Test SEO analysis endpoint
            var seoRequest = new { content = "Test content", title = "Test Title" };
            var seoJson = JsonSerializer.Serialize(seoRequest);
            var seoContent = new StringContent(seoJson, Encoding.UTF8, "application/json");

            var seoResponse = await _client.PostAsync("/seo/analyze", seoContent);
            seoResponse.EnsureSuccessStatusCode();
            Assert.Equal("application/json", seoResponse.Content.Headers.ContentType.MediaType);

            // Test keyword suggestions endpoint
            var keywordRequest = new { content = "Test content for keywords", count = 3 };
            var keywordJson = JsonSerializer.Serialize(keywordRequest);
            var keywordContent = new StringContent(keywordJson, Encoding.UTF8, "application/json");

            var keywordResponse = await _client.PostAsync("/seo/keywords", keywordContent);
            keywordResponse.EnsureSuccessStatusCode();
            Assert.Equal("application/json", keywordResponse.Content.Headers.ContentType.MediaType);
        }
    }
}