using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BloggingAgent.Utilities
{
    public static class TextAnalyzer
    {
        private static readonly string[] CommonWords = {
            "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by",
            "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "do", "does",
            "did", "will", "would", "could", "should", "may", "might", "must", "can", "shall"
        };

        public static Dictionary<string, int> GetWordFrequency(string text, bool excludeCommonWords = true)
        {
            if (string.IsNullOrEmpty(text))
                return new Dictionary<string, int>();

            var words = Regex.Split(text.ToLower(), @"\W+")
                            .Where(w => !string.IsNullOrWhiteSpace(w))
                            .Where(w => w.Length > 2);

            if (excludeCommonWords)
            {
                words = words.Where(w => !CommonWords.Contains(w));
            }

            return words.GroupBy(w => w)
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        public static string[] ExtractKeywords(string text, int count = 10)
        {
            var wordFrequency = GetWordFrequency(text);
            return wordFrequency.OrderByDescending(kvp => kvp.Value)
                               .Take(count)
                               .Select(kvp => kvp.Key)
                               .ToArray();
        }

        public static double CalculateReadabilityScore(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var sentences = Regex.Split(text, @"[.!?]+").Length;
            var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var syllables = CountSyllables(text);

            if (sentences == 0 || words == 0)
                return 0;

            // Flesch Reading Ease formula
            var score = 206.835 - 1.015 * (words / (double)sentences) - 84.6 * (syllables / (double)words);
            return Math.Max(0, Math.Min(100, score));
        }

        public static int CountWords(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static int CountSentences(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return Regex.Split(text, @"[.!?]+").Length;
        }

        public static int CountParagraphs(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return Regex.Split(text, @"\n\s*\n").Length;
        }

        public static TimeSpan EstimateReadingTime(string text, int wordsPerMinute = 200)
        {
            var wordCount = CountWords(text);
            var minutes = wordCount / (double)wordsPerMinute;
            return TimeSpan.FromMinutes(minutes);
        }

        public static double CalculateKeywordDensity(string text, string keyword)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(keyword))
                return 0;

            var wordCount = CountWords(text);
            if (wordCount == 0)
                return 0;

            var keywordCount = Regex.Matches(text, $@"\b{Regex.Escape(keyword)}\b", RegexOptions.IgnoreCase).Count;
            return (keywordCount / (double)wordCount) * 100;
        }

        private static int CountSyllables(string text)
        {
            var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var totalSyllables = 0;

            foreach (var word in words)
            {
                totalSyllables += CountSyllablesInWord(word);
            }

            return totalSyllables;
        }

        private static int CountSyllablesInWord(string word)
        {
            word = word.ToLower();
            var syllables = 0;
            var vowels = "aeiouy";

            if (word.Length == 0)
                return 0;

            if (vowels.Contains(word[0]))
                syllables++;

            for (var i = 1; i < word.Length; i++)
            {
                if (vowels.Contains(word[i]) && !vowels.Contains(word[i - 1]))
                {
                    syllables++;
                }
            }

            if (word.EndsWith("e"))
                syllables--;

            return Math.Max(1, syllables);
        }
    }
}