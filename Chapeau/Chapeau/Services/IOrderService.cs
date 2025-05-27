using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        // Get all orders for today
        List<DisplayOrder> GetTodaysOrders();

        // Get orders by status
        List<DisplayOrder> GetOrdersByStatus(Status status);

        // Get orders for a specific table
        List<DisplayOrder> GetOrdersByTable(int tableNr);

        // Get a specific order with all details
        DisplayOrder GetOrderById(int orderId);
    }
}