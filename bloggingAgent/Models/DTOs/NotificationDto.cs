using System;

namespace BloggingAgent.Models.DTOs
{
    public class CreateNotificationRequest
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; }
        public string RelatedEntityType { get; set; }
        public string RelatedEntityId { get; set; }
        public string ActionUrl { get; set; }
    }
}