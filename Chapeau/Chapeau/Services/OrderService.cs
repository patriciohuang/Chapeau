using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetTodaysOrders()
        {
            // Get all orders for today (no status filter)
            return _orderRepository.GetOrders(null);
        }

        public List<Order> GetOrdersByStatus(Status status)
        {
            return _orderRepository.GetOrders(status);
        }

        public List<Order> GetOrdersByTable(int tableNr)
        {
            // Get all orders and filter by table
            List<Order> allOrders = _orderRepository.GetOrders(null);
            return allOrders.Where(o => o.Table.TableNr == tableNr).ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _orderRepository.GetOrder(orderId);
        }
    }
}