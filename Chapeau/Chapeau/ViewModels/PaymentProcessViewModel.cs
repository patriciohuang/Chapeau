using Chapeau.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Chapeau.ViewModels
{
    public class PaymentProcessViewModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public decimal TipAmount { get; set; }

        [Required]
        public decimal VatValues { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public string? Feedback { get; set; }

        // Optional properties for specific payment methods
        public decimal? CashAmount { get; set; }
        public string? GiftCardNumber { get; set; }
    }
} 