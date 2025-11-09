using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using BloggingAgent.Services.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAgent.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly ICacheService _cacheService;

        public NotificationController(
            UserManager<ApplicationUser> userManager,
            IRepository<Notification> notificationRepository,
            ICacheService cacheService)
        {
            _userManager = userManager;
            _notificationRepository = notificationRepository;
            _cacheService = cacheService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var user = await _userManager.GetUserAsync(User);
            var cacheKey = $"notifications_{user.Id}_{page}_{pageSize}";

            var cachedNotifications = await _cacheService.GetAsync<List<Notification>>(cacheKey);
            if (cachedNotifications != null)
            {
                return View(cachedNotifications);
            }

            var notifications = await _notificationRepository.FindAsync(n =>
                n.UserId == user.Id && !n.IsExpired);

            var paginatedNotifications = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, paginatedNotifications, TimeSpan.FromMinutes(5));

            return View(paginatedNotifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var user = await _userManager.GetUserAsync(User);
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null || notification.UserId != user.Id)
            {
                return NotFound();
            }

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);

            // Clear cache
            await ClearUserNotificationCache(user.Id);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsUnread(int notificationId)
        {
            var user = await _userManager.GetUserAsync(User);
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null || notification.UserId != user.Id)
            {
                return NotFound();
            }

            notification.MarkAsUnread();
            await _notificationRepository.UpdateAsync(notification);

            // Clear cache
            await ClearUserNotificationCache(user.Id);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            var unreadNotifications = await _notificationRepository.FindAsync(n =>
                n.UserId == user.Id && !n.IsRead && !n.IsExpired);

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
            }

            await _notificationRepository.UpdateRangeAsync(unreadNotifications);

            // Clear cache
            await ClearUserNotificationCache(user.Id);

            return Json(new { success = true, count = unreadNotifications.Count() });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int notificationId)
        {
            var user = await _userManager.GetUserAsync(User);
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null || notification.UserId != user.Id)
            {
                return NotFound();
            }

            await _notificationRepository.DeleteAsync(notificationId);

            // Clear cache
            await ClearUserNotificationCache(user.Id);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var user = await _userManager.GetUserAsync(User);
            var cacheKey = $"unread_count_{user.Id}";

            var cachedCount = await _cacheService.GetAsync<int>(cacheKey);
            if (cachedCount > 0)
            {
                return Json(new { count = cachedCount });
            }

            var unreadCount = await _notificationRepository.CountAsync(n =>
                n.UserId == user.Id && !n.IsRead && !n.IsExpired);

            // Cache for 1 minute
            await _cacheService.SetAsync(cacheKey, unreadCount, TimeSpan.FromMinutes(1));

            return Json(new { count = unreadCount });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecent(int count = 5)
        {
            var user = await _userManager.GetUserAsync(User);
            var cacheKey = $"recent_notifications_{user.Id}_{count}";

            var cachedNotifications = await _cacheService.GetAsync<List<Notification>>(cacheKey);
            if (cachedNotifications != null)
            {
                return Json(cachedNotifications);
            }

            var notifications = await _notificationRepository.FindAsync(n =>
                n.UserId == user.Id && !n.IsExpired);

            var recentNotifications = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToList();

            // Cache for 2 minutes
            await _cacheService.SetAsync(cacheKey, recentNotifications, TimeSpan.FromMinutes(2));

            return Json(recentNotifications);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTestNotification()
        {
            var user = await _userManager.GetUserAsync(User);

            var notification = new Notification
            {
                UserId = user.Id,
                Title = "Test Notification",
                Message = "This is a test notification to demonstrate the system.",
                Type = "system",
                Priority = "normal",
                ActionUrl = "/Dashboard",
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _notificationRepository.AddAsync(notification);

            // Clear cache
            await ClearUserNotificationCache(user.Id);

            return Json(new { success = true, notificationId = notification.Id });
        }

        // Static method for creating notifications from other controllers
        public static async Task CreateNotificationAsync(
            IRepository<Notification> notificationRepository,
            ICacheService cacheService,
            CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                Priority = request.Priority ?? "normal",
                RelatedEntityType = request.RelatedEntityType,
                RelatedEntityId = request.RelatedEntityId,
                ActionUrl = request.ActionUrl,
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Default 30 days
            };

            await notificationRepository.AddAsync(notification);

            // Clear cache for the user
            await ClearUserNotificationCacheStatic(cacheService, request.UserId);
        }

        private async Task ClearUserNotificationCache(string userId)
        {
            // Clear all notification-related caches for this user
            await _cacheService.RemoveAsync($"notifications_{userId}_*");
            await _cacheService.RemoveAsync($"unread_count_{userId}");
            await _cacheService.RemoveAsync($"recent_notifications_{userId}_*");
        }

        private static async Task ClearUserNotificationCacheStatic(ICacheService cacheService, string userId)
        {
            // Static version for use from other controllers
            await cacheService.RemoveAsync($"notifications_{userId}_*");
            await cacheService.RemoveAsync($"unread_count_{userId}");
            await cacheService.RemoveAsync($"recent_notifications_{userId}_*");
        }
    }

}