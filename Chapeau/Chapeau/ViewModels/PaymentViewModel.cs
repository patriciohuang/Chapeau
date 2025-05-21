using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {
        public decimal TotalAmount { get { return PaymentItems.Sum(i => i.Amount); } }
        public List<PaymentItemModel> PaymentItems { get; set; }

        public PaymentViewModel(List<PaymentItemModel> paymentItems)
        {
            PaymentItems = paymentItems;
            
        }
    }
}
