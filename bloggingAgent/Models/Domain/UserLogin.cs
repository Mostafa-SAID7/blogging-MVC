using System;

namespace BloggingAgent.Models.Domain
{
    public class UserLogin : BaseEntity
    {
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
