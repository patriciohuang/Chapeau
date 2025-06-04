using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Repositories
{
    public interface IPaymentRepository
    {
        List<Payment> GetPaymentSummaryForTable(int orderId);
        void SavePayment(Payment payment, int orderId);
    }
}

