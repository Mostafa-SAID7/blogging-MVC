using System.Collections.Generic;
using BloggingAgent.Models.Domain;

namespace BloggingAgent.Models.DTOs
{
    public class BillingViewModel
    {
        public Subscription CurrentSubscription { get; set; }
        public List<Payment> PaymentHistory { get; set; } = new List<Payment>();
        public List<PricingPlan> AvailablePlans { get; set; } = new List<PricingPlan>();
        public bool IsSubscribed { get; set; }
    }

    public class InvoiceViewModel
    {
        public Payment Payment { get; set; }
        public Subscription Subscription { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class PricingPlan
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Period { get; set; }
        public string Description { get; set; }
        public List<string> Features { get; set; } = new List<string>();
    }
}