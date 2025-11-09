using System;
using System.Linq;
using System.Threading.Tasks;
using BloggingAgent.Data.Repositories;
using BloggingAgent.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAgent.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Subscription> _subscriptionRepository;
        private readonly IRepository<Payment> _paymentRepository;

        public BillingController(
            UserManager<ApplicationUser> userManager,
            IRepository<Subscription> subscriptionRepository,
            IRepository<Payment> paymentRepository)
        {
            _userManager = userManager;
            _subscriptionRepository = subscriptionRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var subscription = await GetUserSubscriptionAsync(user.Id);

            var model = new BillingViewModel
            {
                CurrentSubscription = subscription,
                PaymentHistory = await GetPaymentHistoryAsync(user.Id),
                AvailablePlans = GetAvailablePlans(),
                IsSubscribed = subscription?.IsActive ?? false
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upgrade(string planName)
        {
            var user = await _userManager.GetUserAsync(User);
            var plan = GetAvailablePlans().FirstOrDefault(p => p.Name == planName);

            if (plan == null)
            {
                TempData["Error"] = "Invalid plan selected.";
                return RedirectToAction("Index");
            }

            // In a real implementation, this would integrate with Stripe
            // For now, we'll simulate the upgrade process

            var subscription = await GetUserSubscriptionAsync(user.Id) ?? new Subscription
            {
                UserId = user.Id,
                PlanName = "Free",
                Status = "active",
                CreatedAt = DateTime.UtcNow
            };

            // Update subscription
            subscription.PlanName = plan.Name;
            subscription.Amount = plan.Price;
            subscription.BillingInterval = plan.Period == "month" ? "month" : "year";
            subscription.Status = "active";
            subscription.CurrentPeriodStart = DateTime.UtcNow;
            subscription.CurrentPeriodEnd = plan.Period == "month" ?
                DateTime.UtcNow.AddMonths(1) : DateTime.UtcNow.AddYears(1);
            subscription.UpdatedAt = DateTime.UtcNow;

            // Set plan limits
            SetPlanLimits(subscription, plan.Name);

            if (subscription.Id == 0)
            {
                await _subscriptionRepository.AddAsync(subscription);
            }
            else
            {
                await _subscriptionRepository.UpdateAsync(subscription);
            }

            // Create payment record
            var payment = new Payment
            {
                UserId = user.Id,
                Amount = plan.Price,
                Currency = "USD",
                Description = $"{plan.Name} Plan - {plan.Period}ly subscription",
                Status = "succeeded",
                PlanName = plan.Name,
                BillingInterval = subscription.BillingInterval,
                ProcessedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);

            TempData["Success"] = $"Successfully upgraded to {plan.Name} plan!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel()
        {
            var user = await _userManager.GetUserAsync(User);
            var subscription = await GetUserSubscriptionAsync(user.Id);

            if (subscription == null || !subscription.IsActive)
            {
                TempData["Error"] = "No active subscription found.";
                return RedirectToAction("Index");
            }

            subscription.Status = "canceled";
            subscription.CanceledAt = DateTime.UtcNow;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _subscriptionRepository.UpdateAsync(subscription);

            TempData["Success"] = "Subscription canceled successfully. You'll continue to have access until the end of your billing period.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(int paymentId)
        {
            var user = await _userManager.GetUserAsync(User);
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null || payment.UserId != user.Id)
            {
                return NotFound();
            }

            var model = new InvoiceViewModel
            {
                Payment = payment,
                Subscription = payment.Subscription,
                User = user
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePaymentMethod()
        {
            // In a real implementation, this would integrate with Stripe to update payment method
            TempData["Success"] = "Payment method updated successfully.";
            return RedirectToAction("Index");
        }

        private async Task<Subscription> GetUserSubscriptionAsync(string userId)
        {
            var subscriptions = await _subscriptionRepository.FindAsync(s => s.UserId == userId);
            return subscriptions.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
        }

        private async Task<List<Payment>> GetPaymentHistoryAsync(string userId)
        {
            var payments = await _paymentRepository.FindAsync(p => p.UserId == userId);
            return payments.OrderByDescending(p => p.CreatedAt).ToList();
        }

        private List<PricingPlan> GetAvailablePlans()
        {
            return new List<PricingPlan>
            {
                new PricingPlan
                {
                    Name = "Free",
                    Price = 0,
                    Period = "forever",
                    Description = "Perfect for getting started",
                    Features = new List<string> { "5 posts/month", "Basic AI features", "Community access" }
                },
                new PricingPlan
                {
                    Name = "Pro",
                    Price = 29,
                    Period = "month",
                    Description = "For serious content creators",
                    Features = new List<string> { "Unlimited posts", "Advanced AI", "Analytics", "Priority support" }
                },
                new PricingPlan
                {
                    Name = "Enterprise",
                    Price = 99,
                    Period = "month",
                    Description = "For teams and businesses",
                    Features = new List<string> { "Everything in Pro", "Team collaboration", "White-label", "API access" }
                }
            };
        }

        private void SetPlanLimits(Subscription subscription, string planName)
        {
            switch (planName)
            {
                case "Free":
                    subscription.MaxPostsPerMonth = 5;
                    subscription.MaxAiTokensPerMonth = 10000;
                    subscription.HasAdvancedAnalytics = false;
                    subscription.HasPrioritySupport = false;
                    subscription.HasWhiteLabel = false;
                    subscription.HasApiAccess = false;
                    subscription.MaxTeamMembers = 1;
                    break;
                case "Pro":
                    subscription.MaxPostsPerMonth = -1; // Unlimited
                    subscription.MaxAiTokensPerMonth = 100000;
                    subscription.HasAdvancedAnalytics = true;
                    subscription.HasPrioritySupport = true;
                    subscription.HasWhiteLabel = false;
                    subscription.HasApiAccess = true;
                    subscription.MaxTeamMembers = 1;
                    break;
                case "Enterprise":
                    subscription.MaxPostsPerMonth = -1; // Unlimited
                    subscription.MaxAiTokensPerMonth = 1000000;
                    subscription.HasAdvancedAnalytics = true;
                    subscription.HasPrioritySupport = true;
                    subscription.HasWhiteLabel = true;
                    subscription.HasApiAccess = true;
                    subscription.MaxTeamMembers = 10;
                    break;
            }
        }
    }

}