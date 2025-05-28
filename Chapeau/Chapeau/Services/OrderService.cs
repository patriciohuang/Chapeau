using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IKitchenBarDisplayRepository _orderRepository;

        public OrderService(IKitchenBarDisplayRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<DisplayOrder> GetTodaysOrders()
        {
            // Get all orders for today (no status filter)
            return _orderRepository.GetOrders(null);
        }

        public List<DisplayOrder> GetOrdersByStatus(Status status)
        {
            return _orderRepository.GetOrders(status);
        }

        public List<DisplayOrder> GetOrdersByTable(int tableNr)
        {
            // Get all orders and filter by table
            var allOrders = _orderRepository.GetOrders(null);
            return allOrders.Where(o => o.Table_id == tableNr).ToList();
        }

        public DisplayOrder GetOrderById(int orderId)
        {
            // Get all orders and find the specific one
            var allOrders = _orderRepository.GetOrders(null);
            return allOrders.FirstOrDefault(o => o.Order_id == orderId);
        }
    }
}