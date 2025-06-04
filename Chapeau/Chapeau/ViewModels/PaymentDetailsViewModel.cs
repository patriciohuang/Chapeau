using Chapeau.Models;
using Chapeau.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Chapeau.ViewModels
{
    public class PaymentDetailsViewModel
    {
        private decimal _subtotal;
        private decimal _totalVAT;
        private decimal _tipAmount;

        public int TableNr { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
        public decimal TotalLowVAT { get; set; }
        public decimal TotalHighVAT { get; set; }
        
        public decimal Subtotal
        {
            get => _subtotal;
            set => _subtotal = value;
        }

        public decimal TotalVAT
        {
            get => _totalVAT;
            set => _totalVAT = value;
        }

        public decimal TipAmount
        {
            get => _tipAmount;
            set => _tipAmount = value;
        }

        public decimal GrandTotal { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public PaymentMethod? SelectedPaymentMethod { get; set; }
        public List<PaymentMethodViewModel> AvailablePaymentMethods => GetAvailablePaymentMethods();

        private List<PaymentMethodViewModel> GetAvailablePaymentMethods()
        {
            return Enum.GetValues<PaymentMethod>()
                .Select(method => new PaymentMethodViewModel 
                { 
                    Method = method,
                    DisplayName = GetDisplayName(method),
                    IconClass = GetIconClass(method)
                })
                .ToList();
        }

        private string GetDisplayName(PaymentMethod method) => method switch
        {
            PaymentMethod.Cash => "Cash",
            PaymentMethod.CreditCard => "Credit Card",
            PaymentMethod.DebitCard => "Debit Card",
            PaymentMethod.GiftCard => "Gift Card",
            _ => method.ToString()
        };

        private string GetIconClass(PaymentMethod method) => method switch
        {
            PaymentMethod.Cash => "bi-cash",
            PaymentMethod.CreditCard => "bi-credit-card",
            PaymentMethod.DebitCard => "bi-credit-card",
            PaymentMethod.GiftCard => "bi-gift",
            _ => "bi-credit-card"
        };

        public void CalculateTotals()
        {
            Subtotal = Items.Sum(i => i.Price * i.Quantity);
            TotalVAT = TotalLowVAT + TotalHighVAT;
            GrandTotal = Subtotal + TotalVAT + TipAmount;
        }

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

        public class PaymentMethodViewModel
        {
            public PaymentMethod Method { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public string IconClass { get; set; } = string.Empty;
        }

        public string FormattedLowVAT => $"€{TotalLowVAT:0.00}";
        public string FormattedHighVAT => $"€{TotalHighVAT:0.00}";
        public string FormattedTip => $"€{TipAmount:0.00}";
        public string FormattedTotal => $"€{GrandTotal:0.00}";
        public string FormattedSubtotal => $"€{Subtotal:0.00}";
    }
} 