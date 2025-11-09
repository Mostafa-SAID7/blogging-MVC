namespace BloggingAgent.Configuration
{
    public class SocialMediaSettings
    {
        public TwitterSettings Twitter { get; set; }
        public LinkedInSettings LinkedIn { get; set; }
        public FacebookSettings Facebook { get; set; }
        public bool AutoPostOnPublish { get; set; } = false;
        public List<string> DefaultPlatforms { get; set; } = new List<string>();
        public int MaxPostLength { get; set; } = 280;
    }

    public class TwitterSettings
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string BearerToken { get; set; }
    }

    public class LinkedInSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccessToken { get; set; }
        public string PersonId { get; set; }
    }

    public class FacebookSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string PageAccessToken { get; set; }
        public string PageId { get; set; }
    }
}