using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BloggingAgent.Models.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public bool IsConfigured => !string.IsNullOrEmpty(_settings.SmtpServer) &&
                                   !string.IsNullOrEmpty(_settings.Username);

        public async Task SendWelcomeEmailAsync(ApplicationUser user)
        {
            var subject = "Welcome to BloggingAgent!";
            var htmlContent = GenerateWelcomeEmailHtml(user);
            var textContent = GenerateWelcomeEmailText(user);

            await SendEmailAsync(user.Email, subject, htmlContent, textContent);
        }

        public async Task SendCommentNotificationAsync(BlogPost post, Comment comment)
        {
            // Send notification to post author
            if (post.Author != comment.AuthorName) // Don't notify if commenting on own post
            {
                var subject = $"New comment on: {post.Title}";
                var htmlContent = GenerateCommentNotificationHtml(post, comment);
                var textContent = GenerateCommentNotificationText(post, comment);

                // In a real app, you'd look up the author's email from the User table
                var authorEmail = $"{post.Author}@bloggingagent.com"; // Placeholder
                await SendEmailAsync(authorEmail, subject, htmlContent, textContent);
            }
        }

        public async Task SendPostPublishedNotificationAsync(BlogPost post)
        {
            // Send notification when a post is published
            var subject = $"Your post has been published: {post.Title}";
            var htmlContent = GeneratePostPublishedHtml(post);
            var textContent = GeneratePostPublishedText(post);

            var authorEmail = $"{post.Author}@bloggingagent.com"; // Placeholder
            await SendEmailAsync(authorEmail, subject, htmlContent, textContent);
        }

        public async Task SendPasswordResetEmailAsync(ApplicationUser user, string resetToken)
        {
            var subject = "Password Reset Request";
            var resetUrl = $"{_settings.BaseUrl}/auth/reset-password?token={resetToken}&email={Uri.EscapeDataString(user.Email)}";
            var htmlContent = GeneratePasswordResetHtml(user, resetUrl);
            var textContent = GeneratePasswordResetText(user, resetUrl);

            await SendEmailAsync(user.Email, subject, htmlContent, textContent);
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent, string textContent = null)
        {
            if (!IsConfigured)
            {
                _logger.LogWarning("Email service is not configured. Skipping email to {To}", to);
                return;
            }

            try
            {
                using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort);
                client.EnableSsl = _settings.UseSsl;
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);

                using var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_settings.FromEmail, _settings.FromName);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = htmlContent;
                mailMessage.IsBodyHtml = true;

                if (!string.IsNullOrEmpty(textContent))
                {
                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(textContent, null, "text/plain"));
                }

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
        }

        private string GenerateWelcomeEmailHtml(ApplicationUser user)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h1 style='color: #0d6efd;'>Welcome to BloggingAgent, {user.FirstName}!</h1>
                    <p>Thank you for joining our AI-powered blogging platform.</p>
                    <p>Here's what you can do:</p>
                    <ul>
                        <li>Generate high-quality blog posts with AI</li>
                        <li>Optimize content for SEO automatically</li>
                        <li>Track performance with detailed analytics</li>
                        <li>Engage with our community through comments</li>
                    </ul>
                    <p>Get started by <a href='{_settings.BaseUrl}/blog/generate' style='color: #0d6efd;'>creating your first post</a>!</p>
                    <p>Best regards,<br>The BloggingAgent Team</p>
                </div>";
        }

        private string GenerateWelcomeEmailText(ApplicationUser user)
        {
            return $@"
Welcome to BloggingAgent, {user.FirstName}!

Thank you for joining our AI-powered blogging platform.

Here's what you can do:
- Generate high-quality blog posts with AI
- Optimize content for SEO automatically
- Track performance with detailed analytics
- Engage with our community through comments

Get started by creating your first post at {_settings.BaseUrl}/blog/generate

Best regards,
The BloggingAgent Team";
        }

        private string GenerateCommentNotificationHtml(BlogPost post, Comment comment)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>New Comment on Your Post</h2>
                    <p><strong>Post:</strong> {post.Title}</p>
                    <p><strong>Comment by:</strong> {comment.AuthorName}</p>
                    <p><strong>Comment:</strong></p>
                    <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                        {comment.Content.Replace("\n", "<br>")}
                    </div>
                    <p><a href='{_settings.BaseUrl}/blog/{post.Slug}#comments' style='background-color: #0d6efd; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Comment</a></p>
                </div>";
        }

        private string GenerateCommentNotificationText(BlogPost post, Comment comment)
        {
            return $@"
New Comment on Your Post

Post: {post.Title}
Comment by: {comment.AuthorName}
Comment: {comment.Content}

View comment: {_settings.BaseUrl}/blog/{post.Slug}#comments";
        }

        private string GeneratePostPublishedHtml(BlogPost post)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>🎉 Your Post Has Been Published!</h2>
                    <p><strong>Title:</strong> {post.Title}</p>
                    <p><strong>Published:</strong> {post.CreatedAt:MMMM dd, yyyy}</p>
                    <p>Your post is now live and available to readers.</p>
                    <p><a href='{_settings.BaseUrl}/blog/{post.Slug}' style='background-color: #0d6efd; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Your Post</a></p>
                </div>";
        }

        private string GeneratePostPublishedText(BlogPost post)
        {
            return $@"
Your Post Has Been Published!

Title: {post.Title}
Published: {post.CreatedAt:MMMM dd, yyyy}

Your post is now live and available to readers.
View your post: {_settings.BaseUrl}/blog/{post.Slug}";
        }

        private string GeneratePasswordResetHtml(ApplicationUser user, string resetUrl)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>Password Reset Request</h2>
                    <p>Hello {user.FirstName},</p>
                    <p>You requested a password reset for your BloggingAgent account.</p>
                    <p><a href='{resetUrl}' style='background-color: #0d6efd; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <p>This link will expire in 24 hours.</p>
                </div>";
        }

        private string GeneratePasswordResetText(ApplicationUser user, string resetUrl)
        {
            return $@"
Password Reset Request

Hello {user.FirstName},

You requested a password reset for your BloggingAgent account.

Reset your password: {resetUrl}

If you didn't request this, please ignore this email.
This link will expire in 24 hours.";
        }
    }
}