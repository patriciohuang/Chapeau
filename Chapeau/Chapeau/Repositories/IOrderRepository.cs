using Chapeau.Models;
using Chapeau.Models.Enums;
namespace Chapeau.Repositories
{
    //pato
    public interface IOrderRepository
    {
        List<Order> GetOrders(Status? status);
        List<Order> GetTodaysOrders();
        List<Order> GetActiveOrders();
        List<Order> GetReadyOrders();
        List<Order> GetOrdersByTable(int tableNr);
        List<Order> GetActiveOrdersByTable(int tableNr);
        List<Order> GetReadyOrdersByTable(int tableNr);

        Order GetOrderById(int orderId);

        int? CheckIfOrderExists(int tableNr);

        int CreateOrder(int tableNr, int employeeId);

        void AddItem(int orderId, int menuItemId);

        public OrderItem GetOrderItem(int orderItemId);

        public void EditOrderItem(OrderItem item);

        public void DeleteOrderItem(int orderItemId);

        public int DeleteUnsentOrderItems(int orderId);

        void SendOrder(int orderId);

        void UpdateOrder(Order order);

        bool UpdateOrderStatus(int orderId, Status status, UserRole role);
        bool UpdateOrderItemStatus(int orderItemId, Status status, UserRole role);
        void UpdateAllReadyItemsToServed(int orderId);

        bool UpdateOrderCategoryStatus(int orderId, CourseCategory courseCategory, Status status, UserRole role);



    }
}
