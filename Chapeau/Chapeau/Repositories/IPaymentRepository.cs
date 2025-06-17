using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Repositories
{
    public interface IPaymentRepository
    {
        List<Payment> GetPaymentsForOrder(int orderId);
        void SavePayment(Payment payment);
    }
}

