using System;

namespace Chapeau.ViewModels
{
    public class OrderItemViewModel
    {
        private decimal _amount;

        public string Name { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int VATRate { get; set; }

        public decimal Amount
        {
            get => Price * Quantity;
            set => _amount = value;
        }

        public string FormattedVAT => $"{VATRate}%";
        public string FormattedPrice => $"€{Price:0.00}";
    }
} 