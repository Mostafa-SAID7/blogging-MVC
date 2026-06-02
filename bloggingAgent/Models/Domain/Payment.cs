using System;

namespace BloggingAgent.Models.Domain
{
    public class Payment : BaseEntity
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Stripe Payment Information
        public string StripePaymentIntentId { get; set; }
        public string StripeChargeId { get; set; }
        public string Status { get; set; } // "succeeded", "pending", "failed", "canceled"

        // Payment Details
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Description { get; set; }
        public string ReceiptEmail { get; set; }

        // Subscription Context
        public Guid? SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }
        public string PlanName { get; set; }
        public string BillingInterval { get; set; }

        // Payment Method
        public string PaymentMethodType { get; set; } // "card", "bank_account", etc.
        public string Last4 { get; set; } // Last 4 digits of card
        public string CardBrand { get; set; } // "visa", "mastercard", etc.

        // Refund Information
        public bool IsRefunded { get; set; } = false;
        public decimal? RefundedAmount { get; set; }
        public string RefundReason { get; set; }
        public DateTime? RefundedAt { get; set; }

        // Metadata
        public DateTime? ProcessedAt { get; set; }
        public string FailureReason { get; set; }
        public string Metadata { get; set; } // JSON string for additional data

        // Computed Properties
        public bool IsSuccessful => Status == "succeeded";
        public bool IsPending => Status == "pending";
        public bool IsFailed => Status == "failed";
        public decimal NetAmount => IsRefunded ? Amount - (RefundedAmount ?? 0) : Amount;
        public string FormattedAmount => $"{Amount:C} {Currency.ToUpper()}";

        // Methods
        public void MarkAsSuccessful(DateTime processedAt)
        {
            Status = "succeeded";
            ProcessedAt = processedAt;
        }

        public void MarkAsFailed(string reason)
        {
            Status = "failed";
            FailureReason = reason;
            ProcessedAt = DateTime.UtcNow;
        }

        public void ProcessRefund(decimal refundAmount, string reason)
        {
            IsRefunded = true;
            RefundedAmount = refundAmount;
            RefundReason = reason;
            RefundedAt = DateTime.UtcNow;
        }
    }
}