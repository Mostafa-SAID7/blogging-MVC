using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.Content
{
    public class MarkdownProcessor
    {
        private readonly ILogger<MarkdownProcessor> _logger;
        private readonly MarkdownPipeline _pipeline;

        public MarkdownProcessor(ILogger<MarkdownProcessor> logger)
        {
            _logger = logger;
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseYamlFrontMatter()
                .UseEmojiAndSmiley()
                .UseAutoLinks()
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

        private async Task<string> ApplyCustomProcessingAsync(string html)
        {
            // Add syntax highlighting classes
            html = Regex.Replace(html, @"<code([^>]*)>", "<code class=\"language-text\"$1>");

            // Add responsive classes to tables
            html = Regex.Replace(html, @"<table([^>]*)>", "<div class=\"table-responsive\"><table class=\"table\"$1>");
            html = Regex.Replace(html, @"</table>", "</table></div>");

            // Process custom shortcodes (example)
            html = Regex.Replace(html, @"\[youtube:([^\]]+)\]", 
                "<div class=\"video-wrapper\"><iframe src=\"https://www.youtube.com/embed/$1\" frameborder=\"0\" allowfullscreen></iframe></div>");

            return html;
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