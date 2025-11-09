using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.Content
{
    public class ContentFormatter : IContentFormatter
    {
        private readonly ILogger<ContentFormatter> _logger;
        private readonly MarkdownPipeline _markdownPipeline;

        public ContentFormatter(ILogger<ContentFormatter> logger)
        {
            _logger = logger;
            _markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
        }

        public Task<string> FormatAsHtmlAsync(string markdownContent)
        {
            if (string.IsNullOrEmpty(markdownContent))
                return Task.FromResult(string.Empty);

            try
            {
                var html = Markdown.ToHtml(markdownContent, _markdownPipeline);
                return Task.FromResult(OptimizeHtml(html));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting markdown to HTML");
                return Task.FromResult(markdownContent);
            }
        }

        public Task<string> FormatAsMarkdownAsync(string htmlContent)
        {
            // Simple HTML to Markdown conversion
            // In a real implementation, you'd use a proper HTML to Markdown converter
            if (string.IsNullOrEmpty(htmlContent))
                return Task.FromResult(string.Empty);

            var markdown = htmlContent
                .Replace("<strong>", "**")
                .Replace("</strong>", "**")
                .Replace("<em>", "*")
                .Replace("</em>", "*")
                .Replace("<p>", "")
                .Replace("</p>", "\n\n")
                .Replace("<br>", "\n")
                .Replace("<br/>", "\n")
                .Replace("<br />", "\n");

            // Remove other HTML tags
            markdown = Regex.Replace(markdown, @"<[^>]+>", "");

            return Task.FromResult(markdown.Trim());
        }

        public Task<string> ExtractExcerptAsync(string content, int maxLength = 150)
        {
            if (string.IsNullOrEmpty(content))
                return Task.FromResult(string.Empty);

            // Remove HTML tags and markdown
            var plainText = Regex.Replace(content, @"<[^>]+>", "");
            plainText = Regex.Replace(plainText, @"[#*`]", "");

            if (plainText.Length <= maxLength)
                return Task.FromResult(plainText);

            var excerpt = plainText.Substring(0, maxLength);
            var lastSpace = excerpt.LastIndexOf(' ');

            if (lastSpace > 0)
                excerpt = excerpt.Substring(0, lastSpace);

            return Task.FromResult(excerpt + "...");
        }

        public Task<string> AddImagePlaceholdersAsync(string content)
        {
            // Replace image placeholders with actual img tags
            var result = Regex.Replace(content, @"\[IMAGE:([^\]]+)\]", 
                "<div class=\"image-placeholder\"><img src=\"/images/placeholder.jpg\" alt=\"$1\" loading=\"lazy\" /></div>");

            return Task.FromResult(result);
        }

        public Task<string> OptimizeForWebAsync(string content)
        {
            if (string.IsNullOrEmpty(content))
                return Task.FromResult(string.Empty);

            var optimized = content;

            // Add lazy loading to images
            optimized = Regex.Replace(optimized, @"<img([^>]+)>", 
                "<img$1 loading=\"lazy\">");

            // Add rel="noopener" to external links
            optimized = Regex.Replace(optimized, @"<a([^>]+href=""(?!#|/)[^""]+""[^>]*)>", 
                "<a$1 rel=\"noopener noreferrer\">");

            // Optimize heading structure
            optimized = EnsureProperHeadingStructure(optimized);

            return Task.FromResult(optimized);
        }

        private string OptimizeHtml(string html)
        {
            // Add classes for styling
            html = Regex.Replace(html, @"<h1([^>]*)>", "<h1 class=\"post-title\"$1>");
            html = Regex.Replace(html, @"<h2([^>]*)>", "<h2 class=\"section-title\"$1>");
            html = Regex.Replace(html, @"<p([^>]*)>", "<p class=\"post-content\"$1>");
            html = Regex.Replace(html, @"<ul([^>]*)>", "<ul class=\"post-list\"$1>");
            html = Regex.Replace(html, @"<ol([^>]*)>", "<ol class=\"post-list\"$1>");
            html = Regex.Replace(html, @"<blockquote([^>]*)>", "<blockquote class=\"post-quote\"$1>");

            return html;
        }

        private string EnsureProperHeadingStructure(string html)
        {
            // Ensure H1 is only used once at the top
            var h1Count = Regex.Matches(html, @"<h1[^>]*>").Count;
            if (h1Count > 1)
            {
                // Convert additional H1s to H2s
                html = Regex.Replace(html, @"<h1([^>]*)>(.*?)</h1>", 
                    "<h2$1>$2</h2>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }

            return html;
        }
    }
}