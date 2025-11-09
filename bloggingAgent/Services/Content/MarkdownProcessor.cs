using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.Content
{
    public class MarkdownProcessor
    {
        private readonly ILogger<MarkdownProcessor> _logger;
        private readonly MarkdownPipeline _pipeline;
        private readonly ContentSettings _settings;

        public MarkdownProcessor(ILogger<MarkdownProcessor> logger, IOptions<ContentSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseYamlFrontMatter()
                .UseEmojiAndSmiley()
                .UseAutoLinks()
                .UseTaskLists()
                .UseGridTables()
                .UseFootnotes()
                .UseCitations()
                .Build();
        }

        public async Task<string> ProcessAsync(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            try
            {
                // Extract front matter if present
                var frontMatter = ExtractFrontMatter(markdown);
                var cleanMarkdown = RemoveFrontMatter(markdown);

                // Process markdown to HTML
                var html = Markdown.ToHtml(cleanMarkdown, _pipeline);

                // Apply custom processing
                html = await ApplyCustomProcessingAsync(html);

                return html;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing markdown");
                return markdown;
            }
        }

        public Dictionary<string, string> ExtractFrontMatter(string markdown)
        {
            var frontMatter = new Dictionary<string, string>();
            var frontMatterRegex = new Regex(@"^---\s*\n(.*?)\n---\s*\n", RegexOptions.Singleline);

            var match = frontMatterRegex.Match(markdown);
            if (match.Success)
            {
                var yamlContent = match.Groups[1].Value;
                var lines = yamlContent.Split('\n');

                foreach (var line in lines)
                {
                    if (line.Contains(':'))
                    {
                        var parts = line.Split(':', 2);
                        if (parts.Length == 2)
                        {
                            frontMatter[parts[0].Trim()] = parts[1].Trim().Trim('"');
                        }
                    }
                }
            }

            return frontMatter;
        }

        public string RemoveFrontMatter(string markdown)
        {
            return Regex.Replace(markdown, @"^---\s*\n.*?\n---\s*\n", "", RegexOptions.Singleline);
        }

        public List<string> ExtractHeadings(string markdown)
        {
            var headings = new List<string>();
            var headingRegex = new Regex(@"^(#{1,6})\s+(.+)$", RegexOptions.Multiline);

            foreach (Match match in headingRegex.Matches(markdown))
            {
                headings.Add(match.Groups[2].Value.Trim());
            }

            return headings;
        }

        public List<string> ExtractLinks(string markdown)
        {
            var links = new List<string>();
            var linkRegex = new Regex(@"\[([^\]]+)\]\(([^)]+)\)");

            foreach (Match match in linkRegex.Matches(markdown))
            {
                links.Add(match.Groups[2].Value);
            }

            return links;
        }

        public int CountWords(string markdown)
        {
            var cleanText = Regex.Replace(markdown, @"[#*`~\[\]\(\)]", "");
            cleanText = Regex.Replace(cleanText, @"!\[.*?\]\(.*?\)", "");
            cleanText = Regex.Replace(cleanText, @"^---\s*\n.*?\n---\s*\n", "", RegexOptions.Singleline);

            var words = cleanText.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        public async Task<string> AddTableOfContentsAsync(string markdown)
        {
            var headings = ExtractHeadings(markdown);
            if (!headings.Any())
                return markdown;

            var toc = new List<string> { "## Table of Contents\n" };

            for (int i = 0; i < headings.Count; i++)
            {
                var level = GetHeadingLevel(markdown, headings[i]);
                var indent = new string(' ', (level - 1) * 2);
                var slug = GenerateSlug(headings[i]);
                toc.Add($"{indent}- [{headings[i]}](#{slug})");
            }

            toc.Add("\n");

            // Insert TOC after the first heading or at the beginning
            var firstHeadingIndex = markdown.IndexOf("# ");
            if (firstHeadingIndex >= 0)
            {
                var insertIndex = markdown.IndexOf('\n', firstHeadingIndex) + 1;
                return markdown.Insert(insertIndex, string.Join("\n", toc));
            }

            return string.Join("\n", toc) + markdown;
        }

        public async Task<string> OptimizeMarkdownAsync(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return markdown;

            // Fix common markdown issues
            markdown = FixMarkdownFormatting(markdown);

            // Add automatic TOC if enabled in settings
            if (_settings?.EnableAutoToc ?? false)
            {
                markdown = await AddTableOfContentsAsync(markdown);
            }

            // Optimize images
            markdown = OptimizeImages(markdown);

            // Add reading time estimate
            markdown = AddReadingTimeEstimate(markdown);

            return markdown;
        }

        public Dictionary<string, int> AnalyzeMarkdown(string markdown)
        {
            return new Dictionary<string, int>
            {
                ["wordCount"] = CountWords(markdown),
                ["headingCount"] = ExtractHeadings(markdown).Count,
                ["linkCount"] = ExtractLinks(markdown).Count,
                ["imageCount"] = Regex.Matches(markdown, @"!\[.*?\]\(.*?\)").Count,
                ["codeBlockCount"] = Regex.Matches(markdown, @"```.*?```", RegexOptions.Singleline).Count,
                ["readingTimeMinutes"] = CalculateReadingTime(markdown)
            };
        }

        private async Task<string> ApplyCustomProcessingAsync(string html)
        {
            // Add syntax highlighting classes
            html = Regex.Replace(html, @"<code([^>]*)>", "<code class=\"language-text\"$1>");

            // Add responsive classes to tables
            html = Regex.Replace(html, @"<table([^>]*)>", "<div class=\"table-responsive\"><table class=\"blog-table\"$1>");
            html = Regex.Replace(html, @"</table>", "</table></div>");

            // Process custom shortcodes
            html = ProcessShortcodes(html);

            // Add image optimization
            if (_settings?.EnableImageOptimization ?? true)
            {
                html = OptimizeImagesHtml(html);
            }

            // Add accessibility attributes
            html = AddAccessibilityAttributes(html);

            return html;
        }

        private string ProcessShortcodes(string html)
        {
            // YouTube shortcode
            html = Regex.Replace(html, @"\[youtube:([^\]]+)\]",
                "<div class=\"video-wrapper\"><iframe src=\"https://www.youtube.com/embed/$1\" frameborder=\"0\" allowfullscreen loading=\"lazy\"></iframe></div>");

            // Twitter shortcode
            html = Regex.Replace(html, @"\[tweet:([^\]]+)\]",
                "<div class=\"tweet-embed\"><blockquote class=\"twitter-tweet\"><a href=\"https://twitter.com/x/status/$1\"></a></blockquote></div>");

            // CodePen shortcode
            html = Regex.Replace(html, @"\[codepen:([^\]]+)\]",
                "<div class=\"codepen-wrapper\"><iframe src=\"https://codepen.io/$1\" frameborder=\"0\" allowfullscreen loading=\"lazy\"></iframe></div>");

            // GitHub Gist shortcode
            html = Regex.Replace(html, @"\[gist:([^\]]+)\]",
                "<div class=\"gist-wrapper\"><script src=\"https://gist.github.com/$1.js\"></script></div>");

            return html;
        }

        private string OptimizeImagesHtml(string html)
        {
            // Add responsive image attributes
            html = Regex.Replace(html, @"<img([^>]+)>", match =>
            {
                var imgTag = match.Value;
                if (!imgTag.Contains("srcset=") && _settings?.EnableResponsiveImages == true)
                {
                    // Add basic responsive attributes
                    imgTag = Regex.Replace(imgTag, @"<img([^>]+)>", "<img$1 sizes=\"(max-width: 768px) 100vw, 50vw\">");
                }
                return imgTag;
            });

            return html;
        }

        private string AddAccessibilityAttributes(string html)
        {
            // Add alt text placeholders for images without alt
            html = Regex.Replace(html, @"<img([^>]+)(?!alt=)[^>]*>", "<img$1 alt=\"Image\">");

            // Add aria labels for external links
            html = Regex.Replace(html, @"<a([^>]+href=""(?!#|/)[^""]+""[^>]*)>", "<a$1 aria-label=\"External link\">");

            return html;
        }

        private string FixMarkdownFormatting(string markdown)
        {
            // Fix common markdown issues
            markdown = Regex.Replace(markdown, @"^#{1,6}\s*$", "", RegexOptions.Multiline); // Remove empty headers
            markdown = Regex.Replace(markdown, @"(\r\n|\r|\n){3,}", "\n\n"); // Normalize line breaks
            markdown = Regex.Replace(markdown, @"^(\s{4,}|\t+)", "    ", RegexOptions.Multiline); // Normalize indentation

            return markdown;
        }

        private string OptimizeImages(string markdown)
        {
            // Optimize image syntax
            markdown = Regex.Replace(markdown, @"!\[([^\]]*)\]\(([^)]+)\)",
                match => $"![{match.Groups[1].Value}]({match.Groups[2].Value} \"{match.Groups[1].Value}\")");

            return markdown;
        }

        private string AddReadingTimeEstimate(string markdown)
        {
            var wordCount = CountWords(markdown);
            var readingTime = CalculateReadingTime(markdown);

            if (readingTime > 0)
            {
                var readingTimeText = $"<!-- Reading time: {readingTime} minutes ({wordCount} words) -->\n";
                return readingTimeText + markdown;
            }

            return markdown;
        }

        private int CalculateReadingTime(string markdown)
        {
            var wordCount = CountWords(markdown);
            const int wordsPerMinute = 200; // Average reading speed
            return (int)Math.Ceiling((double)wordCount / wordsPerMinute);
        }

        private int GetHeadingLevel(string markdown, string heading)
        {
            var lines = markdown.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains(heading) && line.TrimStart().StartsWith("#"))
                {
                    return line.TrimStart().TakeWhile(c => c == '#').Count();
                }
            }
            return 1;
        }

        private string GenerateSlug(string text)
        {
            return text.ToLower()
                      .Replace(" ", "-")
                      .Replace(".", "")
                      .Replace(",", "")
                      .Replace("?", "")
                      .Replace("!", "")
                      .Replace(":", "")
                      .Replace(";", "");
        }
    }
}