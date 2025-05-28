using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Payment
    {
        public int TotalAmount { get; set; } // Total amount for the payment
        public int Tip { get; set; } // Tip amount for the payment
        public int VatValue { get; set; } // VAT value for the payment
        public PaymentMethod PaymentMethod { get; set; } // ViewModel for the payment details
        public string FeedBack { get; set; } // Feedback from the customer regarding the payment

        public Payment(int totalAmount, int tip, int vatValue, PaymentMethod paymentMethod, string feedBack)
        {
            TotalAmount = totalAmount;
            Tip = tip;
            VatValue = vatValue;
            PaymentMethod = paymentMethod;
            FeedBack = feedBack;
        }
/*        //calculates the amount for a menu item
        public decimal Amount
        {
            get
            {
                return Price * Count;
            }
        }
        //VAT percentage based on the bool value
        public int VATPercent
        {
            get
            {
                return IsAlcoholic ? 21 : 9;
            }
        }*/

    }
}
