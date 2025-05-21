using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Repositories
{
    public interface IPaymentRepository
    {
        List<PaymentItemModel> GetPaymentSummaryForTable(int orderId);
    }
}

