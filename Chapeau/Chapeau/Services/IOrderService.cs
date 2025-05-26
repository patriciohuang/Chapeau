using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        Order GetOrder(int orderId);
    }
}
