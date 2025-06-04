using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Payment
    {
        public decimal TotalAmount { get; set; } // Total amount for the payment
        public decimal Tip { get; set; } // Tip amount
        public int VatValue { get; set; } // VAT value for the payment
        public PaymentMethod PaymentMethod { get; set; } // Payment method used
        public string FeedBack { get; set; } // Feedback from the customer regarding the payment

        public Payment(decimal totalAmount, decimal tip, int vatValue, PaymentMethod paymentMethod, string feedBack)
        {
            TotalAmount = totalAmount;
            Tip = tip;
            VatValue = vatValue;
            PaymentMethod = paymentMethod;
            FeedBack = feedBack;
        }

        // Property to get total amount including tip
        public decimal GrandTotal => TotalAmount + Tip;
    }
} 