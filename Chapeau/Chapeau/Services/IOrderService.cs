using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        // Get all orders for today
        List<Order> GetTodaysOrders();

        // Get orders by status
        List<Order> GetOrdersByStatus(Status status);

        List<Order> GetOrdersByTable(int tableNr);

        List<Order> GetOrdersByFilter(string filter);
        List<Order> GetOrdersByTableAndFilter(int tableNr, string filter);

        // Gets all active orders (orders that are not completed or cancelled)
        List<Order> GetActiveOrders();

        // Gets all orders that have at least one order item with status "Ready"
        List<Order> GetReadyOrders();

        // Gets active orders for a specific table
        List<Order> GetActiveOrdersByTable(int tableNr);

        // Gets ready orders for a specific table
        List<Order> GetReadyOrdersByTable(int tableNr);

        Order GetOrderById(int orderId);

        int? CheckIfOrderExists(int tableNr);

        int CreateOrder(int tableNr, Employee loggedInEmployee);

        void AddItem(int orderId, MenuItem menuItem);

        void SendOrder(int orderId);
        void MarkOrderAsServed(int orderId);
        void MarkOrderItemAsServed(int orderId, int orderItemId);
        void RecalculateOrderStatus(int orderId);
    }
}