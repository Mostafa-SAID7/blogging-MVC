using System;
using System.Collections.Generic;
using System.Linq;
using BloggingAgent.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Services.Content
{
    public class SyndicationService : ISyndicationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SyndicationService> _logger;

        public SyndicationService(IHttpContextAccessor httpContextAccessor, ILogger<SyndicationService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private string GetBaseUrl()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null)
            {
                _logger.LogWarning("HttpContext is null when generating syndication content; defaulting to '/'.");
                return string.Empty;
            }
            return $"{ctx.Request.Scheme}://{ctx.Request.Host}";
        }

        public string GenerateRssFeed(List<BlogPost> posts)
        {
            var baseUrl = GetBaseUrl();
            var rss = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<channel>
<title>BloggingAgent Blog</title>
<description>AI-powered blog content</description>
<link>{baseUrl}</link>
<atom:link href=""{baseUrl}/blog/rss"" rel=""self"" type=""application/rss+xml"" />
<language>en-us</language>
<lastBuildDate>{DateTime.UtcNow:R}</lastBuildDate>
";

            foreach (var post in posts)
            {
                var excerpt = post.Excerpt ?? (post.Content?.Substring(0, Math.Min(200, post.Content.Length)) ?? string.Empty);
                rss += $@"
<item>
<title><![CDATA[{post.Title}]]></title>
<description><![CDATA[{excerpt}]]></description>
<link>{baseUrl}/blog/{post.Slug}</link>
<guid>{baseUrl}/blog/{post.Slug}</guid>
<pubDate>{post.CreatedAt:R}</pubDate>
<author>{post.Author}</author>
</item>";
            }

            rss += "\n</channel>\n</rss>";
            return rss;
        }

        public string GenerateSitemap(List<BlogPost> posts)
        {
            var baseUrl = GetBaseUrl();
            var sitemap = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
";

            // Add main pages
            sitemap += $@"
<url>
<loc>{baseUrl}/</loc>
<priority>1.0</priority>
<changefreq>daily</changefreq>
</url>
<url>
<loc>{baseUrl}/blog</loc>
<priority>0.9</priority>
<changefreq>daily</changefreq>
</url>";

            foreach (var post in posts)
            {
                sitemap += $@"
<url>
<loc>{baseUrl}/blog/{post.Slug}</loc>
<priority>0.8</priority>
<changefreq>weekly</changefreq>
<lastmod>{post.UpdatedAt:yyyy-MM-dd}</lastmod>
</url>";
            }

            sitemap += "\n</urlset>";
            return sitemap;
        }
    }
}
