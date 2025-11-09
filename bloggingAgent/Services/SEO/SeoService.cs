using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BloggingAgent.Models.DTOs;
using BloggingAgent.Services.LLM;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.SEO
{
    public class SeoService : ISeoService
    {
        private readonly ILlmConnector _llmConnector;
        private readonly ILogger<SeoService> _logger;

        public SeoService(ILlmConnector llmConnector, ILogger<SeoService> logger)
        {
            _llmConnector = llmConnector;
            _logger = logger;
        }

        public async Task<SeoAnalysisResult> AnalyzeContentAsync(string content, string title = null)
        {
            var result = new SeoAnalysisResult();
            var checks = new Dictionary<string, bool>();

            // Basic SEO checks
            checks["HasTitle"] = !string.IsNullOrEmpty(title);
            checks["TitleLength"] = title?.Length >= 30 && title?.Length <= 60;
            checks["ContentLength"] = content.Length >= 300;
            checks["HasHeadings"] = Regex.IsMatch(content, @"<h[1-6][^>]*>.*?</h[1-6]>", RegexOptions.IgnoreCase);
            checks["HasImages"] = content.Contains("<img");
            checks["HasLinks"] = content.Contains("<a href");

            result.Checks = checks;
            result.Score = checks.Count(c => c.Value) * 100 / checks.Count;

            // Generate suggestions
            var suggestions = new List<string>();
            if (!checks["HasTitle"]) suggestions.Add("Add a compelling title");
            if (!checks["TitleLength"]) suggestions.Add("Title should be 30-60 characters");
            if (!checks["ContentLength"]) suggestions.Add("Content should be at least 300 words");
            if (!checks["HasHeadings"]) suggestions.Add("Add headings (H1-H6) to structure content");
            if (!checks["HasImages"]) suggestions.Add("Consider adding relevant images");
            if (!checks["HasLinks"]) suggestions.Add("Add internal/external links for better SEO");

            result.Suggestions = suggestions;

            // Keyword analysis
            var keywords = await SuggestKeywordsAsync(content, 10);
            result.KeywordOccurrences = AnalyzeKeywordDensity(content, keywords);

            return result;
        }

        public async Task<string> GenerateMetaDescriptionAsync(string content)
        {
            var prompt = $"Generate a compelling meta description (150-160 characters) for this content:\n\n{content.Substring(0, Math.Min(500, content.Length))}";
            return await _llmConnector.GenerateContentAsync(prompt, 200);
        }

        public async Task<string[]> SuggestKeywordsAsync(string content, int count = 5)
        {
            var prompt = $"Extract {count} relevant keywords from this content:\n\n{content.Substring(0, Math.Min(1000, content.Length))}\n\nReturn only the keywords separated by commas.";
            var response = await _llmConnector.GenerateContentAsync(prompt, 100);
            return response.Split(',').Select(k => k.Trim()).Take(count).ToArray();
        }

        public Task<int> CalculateReadabilityScoreAsync(string content)
        {
            // Simple readability calculation (Flesch Reading Ease)
            var sentences = Regex.Split(content, @"[.!?]+").Length;
            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var syllables = CountSyllables(content);

            if (sentences == 0 || words == 0) return Task.FromResult(0);

            var score = 206.835 - 1.015 * (words / (double)sentences) - 84.6 * (syllables / (double)words);
            return Task.FromResult((int)Math.Max(0, Math.Min(100, score)));
        }

        private Dictionary<string, int> AnalyzeKeywordDensity(string content, string[] keywords)
        {
            var result = new Dictionary<string, int>();
            var words = Regex.Split(content.ToLower(), @"\W+");

            foreach (var keyword in keywords)
            {
                var count = words.Count(w => w.Contains(keyword.ToLower()));
                result[keyword] = count;
            }

            return result;
        }

        private int CountSyllables(string text)
        {
            var words = text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var totalSyllables = 0;

            foreach (var word in words)
            {
                totalSyllables += CountSyllablesInWord(word);
            }

            return totalSyllables;
        }

        private int CountSyllablesInWord(string word)
        {
            word = word.ToLower();
            var syllables = 0;
            var vowels = "aeiouy";

            if (word.Length == 0) return 0;

            if (vowels.Contains(word[0])) syllables++;

            for (var i = 1; i < word.Length; i++)
            {
                if (vowels.Contains(word[i]) && !vowels.Contains(word[i - 1]))
                {
                    syllables++;
                }
            }

            if (word.EndsWith("e")) syllables--;

            return Math.Max(1, syllables);
        }
    }
}