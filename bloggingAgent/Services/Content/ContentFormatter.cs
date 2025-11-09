using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.Content
{
    public class ContentFormatter : IContentFormatter
    {
        private readonly ILogger<ContentFormatter> _logger;
        private readonly MarkdownPipeline _markdownPipeline;
        private readonly ContentSettings _settings;

        public ContentFormatter(ILogger<ContentFormatter> logger, IOptions<ContentSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
            _markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseEmojiAndSmiley()
                .UseAutoLinks()
                .UseTaskLists()
                .UseGridTables()
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

            // Use configured excerpt length if available
            maxLength = _settings?.DefaultExcerptLength ?? maxLength;

            // Remove HTML tags and markdown
            var plainText = Regex.Replace(content, @"<[^>]+>", "");
            plainText = Regex.Replace(plainText, @"[#*`~\[\]\(\)]", "");
            plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

            if (plainText.Length <= maxLength)
                return Task.FromResult(plainText);

            var excerpt = plainText.Substring(0, maxLength);
            var lastSpace = excerpt.LastIndexOf(' ');

            if (lastSpace > 0)
                excerpt = excerpt.Substring(0, lastSpace);

            return Task.FromResult(excerpt.TrimEnd() + "...");
        }

        public Task<string> AddImagePlaceholdersAsync(string content)
        {
            if (string.IsNullOrEmpty(content))
                return Task.FromResult(string.Empty);

            // Replace image placeholders with actual img tags
            var result = Regex.Replace(content, @"\[IMAGE:([^\]]+)\]",
                $"<div class=\"image-placeholder\"><img src=\"{_settings?.DefaultImagePath ?? "/images/placeholder.jpg"}\" alt=\"$1\" loading=\"lazy\" class=\"responsive-img\" /></div>");

            // Handle image URLs
            result = Regex.Replace(result, @"!\[([^\]]*)\]\(([^)]+)\)",
                "<div class=\"blog-image\"><img src=\"$2\" alt=\"$1\" loading=\"lazy\" class=\"responsive-img\" /></div>");

            return Task.FromResult(result);
        }

        public Task<string> OptimizeForWebAsync(string content)
        {
            if (string.IsNullOrEmpty(content))
                return Task.FromResult(string.Empty);

            var optimized = content;

            // Add lazy loading to images
            optimized = Regex.Replace(optimized, @"<img([^>]+)>",
                "<img$1 loading=\"lazy\" decoding=\"async\">");

            // Add rel="noopener" to external links
            optimized = Regex.Replace(optimized, @"<a([^>]+href=""(?!#|/)[^""]+""[^>]*)>",
                "<a$1 rel=\"noopener noreferrer\" target=\"_blank\">");

            // Add preload hints for critical resources
            optimized = AddResourceHints(optimized);

            // Optimize heading structure
            optimized = EnsureProperHeadingStructure(optimized);

            // Add schema markup for better SEO
            optimized = AddSchemaMarkup(optimized);

            return Task.FromResult(optimized);
        }

        private string OptimizeHtml(string html)
        {
            // Add classes for styling
            html = Regex.Replace(html, @"<h1([^>]*)>", "<h1 class=\"post-title\"$1>");
            html = Regex.Replace(html, @"<h2([^>]*)>", "<h2 class=\"section-title\"$1>");
            html = Regex.Replace(html, @"<h3([^>]*)>", "<h3 class=\"subsection-title\"$1>");
            html = Regex.Replace(html, @"<p([^>]*)>", "<p class=\"post-content\"$1>");
            html = Regex.Replace(html, @"<ul([^>]*)>", "<ul class=\"post-list\"$1>");
            html = Regex.Replace(html, @"<ol([^>]*)>", "<ol class=\"post-list numbered\"$1>");
            html = Regex.Replace(html, @"<blockquote([^>]*)>", "<blockquote class=\"post-quote\"$1>");
            html = Regex.Replace(html, @"<code([^>]*)>", "<code class=\"inline-code\"$1>");
            html = Regex.Replace(html, @"<pre([^>]*)>", "<pre class=\"code-block\"$1>");

            // Add responsive table wrapper
            html = Regex.Replace(html, @"<table([^>]*)>", "<div class=\"table-responsive\"><table class=\"blog-table\"$1>");
            html = Regex.Replace(html, @"</table>", "</table></div>");

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

            // Ensure proper heading hierarchy (no skipping levels)
            html = FixHeadingHierarchy(html);

            return html;
        }

        private string AddResourceHints(string html)
        {
            // Add preload hints for critical resources
            var hints = new System.Text.StringBuilder();

            // Preload critical CSS
            if (_settings?.CriticalCssPath != null)
            {
                hints.AppendLine($"<link rel=\"preload\" href=\"{_settings.CriticalCssPath}\" as=\"style\" onload=\"this.onload=null;this.rel='stylesheet'\">");
            }

            // Preload critical JS
            if (_settings?.CriticalJsPath != null)
            {
                hints.AppendLine($"<link rel=\"preload\" href=\"{_settings.CriticalJsPath}\" as=\"script\">");
            }

            return hints.ToString() + html;
        }

        private string AddSchemaMarkup(string html)
        {
            // Add basic schema markup for articles
            if (html.Contains("<article") || html.Contains("post-content"))
            {
                var schemaScript = @"<script type=""application/ld+json"">
                {
                  ""@context"": ""https://schema.org"",
                  ""@type"": ""Article"",
                  ""headline"": ""Article Title"",
                  ""author"": {
                    ""@type"": ""Person"",
                    ""name"": ""Author Name""
                  },
                  ""datePublished"": ""2024-01-01"",
                  ""dateModified"": ""2024-01-01""
                }
                </script>";

                html += schemaScript;
            }

            return html;
        }

        private string FixHeadingHierarchy(string html)
        {
            // Simple heading hierarchy fix - ensure no level skipping
            // This is a basic implementation; a more robust solution would parse the DOM
            var headings = new[] { "h1", "h2", "h3", "h4", "h5", "h6" };
            var lastLevel = 0;

            foreach (var heading in headings)
            {
                var pattern = $@"<{heading}([^>]*>.*?)</{heading}>";
                var matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    var level = Array.IndexOf(headings, heading.ToLower()) + 1;
                    if (level > lastLevel + 1 && lastLevel > 0)
                    {
                        // Skip level detected, adjust to previous level + 1
                        var newLevel = Math.Min(lastLevel + 1, 6);
                        var newHeading = $"h{newLevel}";
                        html = html.Replace(match.Value,
                            Regex.Replace(match.Value, $@"</?{heading}", $"<{newHeading}", RegexOptions.IgnoreCase));
                    }
                    lastLevel = level;
                }
            }

            return html;
        }
    }
}