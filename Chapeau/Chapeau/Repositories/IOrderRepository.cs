using Chapeau.Models;
using Chapeau.Models.Enums;
namespace Chapeau.Repositories
{
    //pato
    public interface IOrderRepository
    {
        List<Order> GetOrders(Status? status, UserRole? role);
        List<Order> GetTodaysOrders();
        List<Order> GetActiveOrders();
        List<Order> GetReadyOrders();
        List<Order> GetOrdersByTable(int tableNr);
        List<Order> GetActiveOrdersByTable(int tableNr);
        List<Order> GetReadyOrdersByTable(int tableNr);

        Order GetOrderById(int orderId);



        int? CheckIfOrderExists(int tableNr);

        int CreateOrder(int tableNr, int employeeId);

        void DeleteOrder(int orderId);

        void AddItem(int orderId, int menuItemId);

        int? CheckIfOrderItemExists(int orderId, int menuItemId, string comment, Status status);

        OrderItem GetOrderItem(int orderItemId);

        List<OrderItem> GetOrderItems(int OrderId);

        void EditOrderItem(OrderItem item);

        void DeleteOrderItem(int orderItemId);

        int DeleteUnsentOrderItems(int orderId);

        void SendOrder(int orderId);

        void UpdateOrder(Order order);

        bool UpdateOrderStatus(int orderId, Status status, UserRole role);
        bool UpdateOrderItemStatus(int orderItemId, Status status, UserRole role);
        void UpdateAllReadyItemsToServed(int orderId);

        bool UpdateOrderCategoryStatus(int orderId, CourseCategory courseCategory, Status status, UserRole role);



    }
}
