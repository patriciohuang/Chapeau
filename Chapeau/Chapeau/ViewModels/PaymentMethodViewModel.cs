using Chapeau.Models.Enums;

namespace Chapeau.ViewModels
{
    public class PaymentMethodViewModel
    {
        public PaymentMethod Method { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
    }
} 