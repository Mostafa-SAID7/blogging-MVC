using System.Threading.Tasks;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.ViewModels;
using BloggingAgent.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggingAgent.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<SettingsController> _logger;
        private readonly AgentSettings _settings;

        public SettingsController(
            ICacheService cacheService,
            ILogger<SettingsController> logger,
            IOptions<AgentSettings> settings)
        {
            _cacheService = cacheService;
            _logger = logger;
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            var model = new SettingsViewModel
            {
                Settings = _settings,
                SaveSuccess = TempData["SaveSuccess"] as bool? ?? false,
                ErrorMessage = TempData["ErrorMessage"] as string
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Settings = _settings;
                return View("Index", model);
            }

            try
            {
                // In a real application, you'd save these settings to a database or configuration file
                // For now, we'll just update the in-memory settings and clear relevant caches

                // Update settings (this would typically be persisted)
                _settings.DefaultAuthor = model.Settings.DefaultAuthor;
                _settings.MaxPostLength = model.Settings.MaxPostLength;
                _settings.DefaultTags = model.Settings.DefaultTags;
                _settings.AutoPublish = model.Settings.AutoPublish;
                _settings.Theme = model.Settings.Theme;
                _settings.CustomSettings = model.Settings.CustomSettings;

                // Clear caches that might be affected by settings changes
                await _cacheService.RemoveAsync("blog_index_*");
                await _cacheService.RemoveAsync("blog_details_*");

                TempData["SaveSuccess"] = true;
                _logger.LogInformation("Settings updated successfully");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating settings");
                TempData["ErrorMessage"] = "Error saving settings. Please try again.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetToDefaults()
        {
            try
            {
                // Reset to default values
                _settings.DefaultAuthor = "AI Assistant";
                _settings.MaxPostLength = 5000;
                _settings.DefaultTags = new System.Collections.Generic.List<string> { "blog", "ai-generated" };
                _settings.AutoPublish = false;
                _settings.Theme = "default";
                _settings.CustomSettings = new System.Collections.Generic.Dictionary<string, object>();

                // Clear caches
                await _cacheService.RemoveAsync("blog_index_*");
                await _cacheService.RemoveAsync("blog_details_*");

                TempData["SaveSuccess"] = true;
                _logger.LogInformation("Settings reset to defaults");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting settings");
                TempData["ErrorMessage"] = "Error resetting settings. Please try again.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Export()
        {
            var settingsJson = System.Text.Json.JsonSerializer.Serialize(_settings, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            return File(System.Text.Encoding.UTF8.GetBytes(settingsJson), "application/json", "blog_settings.json");
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile settingsFile)
        {
            if (settingsFile == null || settingsFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid settings file.";
                return RedirectToAction("Index");
            }

            try
            {
                using var reader = new System.IO.StreamReader(settingsFile.OpenReadStream());
                var jsonContent = await reader.ReadToEndAsync();

                var importedSettings = System.Text.Json.JsonSerializer.Deserialize<AgentSettings>(jsonContent);
                if (importedSettings == null)
                {
                    TempData["ErrorMessage"] = "Invalid settings file format.";
                    return RedirectToAction("Index");
                }

                // Update current settings
                _settings.DefaultAuthor = importedSettings.DefaultAuthor;
                _settings.MaxPostLength = importedSettings.MaxPostLength;
                _settings.DefaultTags = importedSettings.DefaultTags;
                _settings.AutoPublish = importedSettings.AutoPublish;
                _settings.Theme = importedSettings.Theme;
                _settings.CustomSettings = importedSettings.CustomSettings;

                // Clear caches
                await _cacheService.RemoveAsync("blog_index_*");
                await _cacheService.RemoveAsync("blog_details_*");

                TempData["SaveSuccess"] = true;
                _logger.LogInformation("Settings imported successfully");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing settings");
                TempData["ErrorMessage"] = "Error importing settings. Please check the file format.";
                return RedirectToAction("Index");
            }
        }
    }
}