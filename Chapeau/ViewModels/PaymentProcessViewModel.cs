using Chapeau.Models.Enums;

namespace Chapeau.ViewModels
{
    public class PaymentProcessViewModel
    {
        public int OrderId { get; set; }
        public decimal TipAmount { get; set; }
        public bool IsTipPercentage { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Feedback { get; set; }
    }
} 