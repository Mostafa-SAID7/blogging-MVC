using System.Threading.Tasks;
using BloggingAgent.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingAgent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(IEmailService emailService, ILogger<NotificationController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> SendTestEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email address is required");
            }

            try
            {
                await _emailService.SendEmailAsync(
                    email,
                    "Test Email from BloggingAgent",
                    "<h1>Test Email</h1><p>This is a test email from BloggingAgent.</p>",
                    "Test Email\nThis is a test email from BloggingAgent."
                );

                return Ok(new { message = "Test email sent successfully" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to send test email");
                return StatusCode(500, new { error = "Failed to send test email", details = ex.Message });
            }
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                emailConfigured = _emailService.IsConfigured,
                timestamp = System.DateTime.UtcNow
            });
        }
    }
}