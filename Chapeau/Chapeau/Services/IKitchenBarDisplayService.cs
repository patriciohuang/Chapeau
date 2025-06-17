using Chapeau.Models;
using Chapeau.Models.Enums;
namespace Chapeau.Services
{
    //pato
    public interface IKitchenBarDisplayService
    {
        List<Order> GetOrders(Status status);
        List<Order> GetOrdersByStatus(List<Status> statuses);
        bool UpdateOrderItemStatus(int orderItemId, Status currentStatus, UserRole role);
        bool UpdateOrderCategoryStatus(int orderId, CourseCategory category, Status currentStatus, UserRole role);
        bool UpdateOrderStatus(int orderId, Status currentStatus, UserRole role);
    }
}
