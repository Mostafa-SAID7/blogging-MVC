using System.Collections.Generic;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Services.Content
{
    public interface ISyndicationService
    {
        string GenerateRssFeed(List<BlogPost> posts);
        string GenerateSitemap(List<BlogPost> posts);
    }
}
