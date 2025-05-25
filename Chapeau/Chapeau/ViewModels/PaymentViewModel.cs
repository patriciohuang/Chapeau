using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {
        public decimal TotalAmount { get { return PaymentItems.Sum(i => i.TotalAmount); } } // Updated to use 'TotalAmount' property
        public List<Payment> PaymentItems { get; set; }

        public PaymentViewModel(List<Payment> paymentItems)
        {
            PaymentItems = paymentItems;
        }
    }
}
