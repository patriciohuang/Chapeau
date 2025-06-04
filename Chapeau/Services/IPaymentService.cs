using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IPaymentService
    {
        PaymentDetailsViewModel GetOrderForPayment(int orderId);
        PaymentDetailsViewModel GetPaymentDetails(int orderId);
        bool ProcessPayment(PaymentProcessViewModel model);
    }
} 