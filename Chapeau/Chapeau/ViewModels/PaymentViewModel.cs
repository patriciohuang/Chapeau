using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {

        public bool IsAlcoholic { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }

        public PaymentViewModel(bool isAlcoholic, decimal price, int count)
        {
            IsAlcoholic = isAlcoholic;
            Price = price;
            Count = count;
        }
        public decimal TotalAmount { get { return PaymentItems.Sum(i => i.TotalAmount); } } // Updated to use 'TotalAmount' property
        public List<Payment> PaymentItems { get; set; }
        public decimal Amount
        {
            get
            {
                return Price * Count;
            }
        }

        public int VATPercentage
        {
            get
            {
                return IsAlcoholic ? 1 : 0;
            }

        }

        public PaymentViewModel(List<Payment> paymentItems)
        {
            PaymentItems = paymentItems;
        }
    }
}
