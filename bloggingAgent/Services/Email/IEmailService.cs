using System.Threading.Tasks;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Services.Email
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(User user);
        Task SendCommentNotificationAsync(BlogPost post, Comment comment);
        Task SendPostPublishedNotificationAsync(BlogPost post);
        Task SendPasswordResetEmailAsync(User user, string resetToken);
        Task SendEmailAsync(string to, string subject, string htmlContent, string textContent = null);
        bool IsConfigured { get; }
    }
}