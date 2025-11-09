namespace BloggingAgent.Configuration
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; } = 587;
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool UseSsl { get; set; } = true;
        public string BaseUrl { get; set; }
        public bool EnableNotifications { get; set; } = true;
        public bool EnableWelcomeEmails { get; set; } = true;
        public bool EnableCommentNotifications { get; set; } = true;
        public bool EnablePostPublishedNotifications { get; set; } = true;
    }
}