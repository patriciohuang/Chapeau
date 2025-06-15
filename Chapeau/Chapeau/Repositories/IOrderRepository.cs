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

        void SendOrder(int orderId);

        void UpdateOrder(Order order);

        bool UpdateOrderStatus(int orderId, Status status);
        void UpdateOrderItemStatus(int orderItemId, Status status);
        void UpdateAllReadyItemsToServed(int orderId);

        //THESE ARE ALL EMPTY AND NOT IMPLEMENTED. REMOVE LATER, WE ALREADY HAVE METHODS THAT DO THIS
        List<Order> GetAllOrders();

        void AddOrder(Order order);

        void DeleteOrder(int orderId);
    }
}
