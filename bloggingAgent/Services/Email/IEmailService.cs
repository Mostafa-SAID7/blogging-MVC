using System.Threading.Tasks;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Services.Email
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(ApplicationUser user);
        Task SendCommentNotificationAsync(BlogPost post, Comment comment);
        Task SendPostPublishedNotificationAsync(BlogPost post);
        Task SendPasswordResetEmailAsync(ApplicationUser user, string resetToken);
        Task SendEmailAsync(string to, string subject, string htmlContent, string textContent = null);
        bool IsConfigured { get; }
    }
}