using Chapeau.Models;
using Chapeau.Models.Enums;
namespace Chapeau.Repositories
{
    //pato
    public interface IOrderRepository
    {
        List<Order> GetOrders(Status? status);
        bool UpdateOrderStatus(int orderId, Status status);
        Order GetOrder(int orderId);
    }
}
