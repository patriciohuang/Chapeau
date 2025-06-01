using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IPaymentService
    {
        PaymentDetailsViewModel GetOrderForPayment(int orderId);
    }
}
