using System.Threading.Tasks;

namespace BloggingAgent.Services.SocialMedia
{
    public interface ISocialMediaService
    {
        Task<bool> PostToTwitterAsync(string content, string imageUrl = null);
        Task<bool> PostToLinkedInAsync(string content, string imageUrl = null);
        Task<bool> PostToFacebookAsync(string content, string imageUrl = null);
        Task<bool> SchedulePostAsync(string platform, string content, System.DateTime scheduledTime, string imageUrl = null);
        bool IsPlatformConfigured(string platform);
        System.Collections.Generic.List<string> GetConfiguredPlatforms();
    }
}