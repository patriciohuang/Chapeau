using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class PaymentDetailsViewModel
    {
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
        public decimal Subtotal { get; set; }
        public decimal TotalLowVAT { get; set; }
        public decimal TotalHighVAT { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TipAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public class OrderItemViewModel
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal Amount { get; set; }
            public string VATRate { get; set; } = string.Empty;
        }
    }
} 