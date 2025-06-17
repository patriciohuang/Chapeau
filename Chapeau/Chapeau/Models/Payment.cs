using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Payment
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; } // Total amount for the payment
        public decimal Tip { get; set; } // Tip amount for the payment
        public decimal VatValue { get; set; } // VAT value for the payment
        public PaymentMethod PaymentMethod { get; set; } // ViewModel for the payment details
        public string FeedBack { get; set; } // Feedback from the customer regarding the payment
        public int PaymentId {  get; set; }

        // Property to get total amount including tip
        public decimal GrandTotal => TotalAmount + Tip;

        public Payment(int orderId, decimal totalAmount, decimal tip, decimal vatValue, PaymentMethod paymentMethod, string feedBack)
        {
            TotalAmount = totalAmount;
            Tip = tip;
            VatValue = vatValue;
            PaymentMethod = paymentMethod;
            FeedBack = feedBack;
            OrderId = orderId;
        }

        public Payment(int orderId, decimal totalAmount, decimal tip, decimal vatValue, PaymentMethod paymentMethod, string feedBack, int paymentId): this(orderId, totalAmount, tip, vatValue, paymentMethod, feedBack)
        {
            this.PaymentId = paymentId;
        }



    }
}
