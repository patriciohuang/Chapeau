using Chapeau.Models;
using Chapeau.Models.Enums;
namespace Chapeau.Services
{
    //pato
    public interface IKitchenBarDisplayService
    {
        List<Order> GetOrders(Status status);
        List<Order> GetOrdersByStatus(List<Status> statuses);
        bool UpdateOrderStatus(int orderId, Status currentStatus);
    }
}
