using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BloggingAgent.Utilities
{
    public static class WordCounter
    {
        public static int CountWords(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // Split by whitespace and filter out empty entries
            var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        public static int CountCharacters(string text, bool includeSpaces = true)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return includeSpaces ? text.Length : text.Count(c => !char.IsWhiteSpace(c));
        }

        public static int CountSentences(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // Split by sentence-ending punctuation
            var sentences = Regex.Split(text, @"[.!?]+");
            return sentences.Count(s => !string.IsNullOrWhiteSpace(s));
        }

        public static int CountParagraphs(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // Split by double newlines (paragraph breaks)
            var paragraphs = Regex.Split(text, @"\n\s*\n");
            return paragraphs.Count(p => !string.IsNullOrWhiteSpace(p));
        }

        public static int CountLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return text.Split('\n').Length;
        }

        public static Dictionary<string, int> GetReadingStats(string text)
        {
            var stats = new Dictionary<string, int>
            {
                ["Words"] = CountWords(text),
                ["Characters"] = CountCharacters(text),
                ["CharactersNoSpaces"] = CountCharacters(text, false),
                ["Sentences"] = CountSentences(text),
                ["Paragraphs"] = CountParagraphs(text),
                ["Lines"] = CountLines(text)
            };

            // Calculate estimated reading time (words per minute)
            const int wordsPerMinute = 200;
            var readingTimeMinutes = stats["Words"] / wordsPerMinute;
            stats["EstimatedReadingTimeMinutes"] = readingTimeMinutes;

            return stats;
        }

        public static string GetReadingTimeString(string text)
        {
            var wordCount = CountWords(text);
            const int wordsPerMinute = 200;

            var minutes = wordCount / wordsPerMinute;
            var seconds = (wordCount % wordsPerMinute) * 60 / wordsPerMinute;

            if (minutes == 0)
                return $"{seconds} second{(seconds == 1 ? "" : "s")}";

            if (seconds == 0)
                return $"{minutes} minute{(minutes == 1 ? "" : "s")}";

            return $"{minutes} minute{(minutes == 1 ? "" : "s")} {seconds} second{(seconds == 1 ? "" : "s")}";
        }

        public static bool IsWithinWordLimit(string text, int minWords, int maxWords)
        {
            var wordCount = CountWords(text);
            return wordCount >= minWords && wordCount <= maxWords;
        }

        public static double GetWordsPerSentence(string text)
        {
            var wordCount = CountWords(text);
            var sentenceCount = CountSentences(text);

            return sentenceCount > 0 ? (double)wordCount / sentenceCount : 0;
        }

        public static double GetCharactersPerWord(string text)
        {
            var wordCount = CountWords(text);
            var charCount = CountCharacters(text, false);

            return wordCount > 0 ? (double)charCount / wordCount : 0;
        }
    }
}