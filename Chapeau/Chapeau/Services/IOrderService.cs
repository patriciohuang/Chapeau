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

        // Get orders for a specific table
        List<Order> GetOrdersByTable(int tableNr);

        // Get a specific order with all details
        Order GetOrderById(int orderId);
    }
}