using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using Microsoft.Extensions.Logging;

namespace Chapeau.Services
{
    //pato
    public class KitchenBarDisplayService : IKitchenBarDisplayService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<KitchenBarDisplayService> _logger;

        public KitchenBarDisplayService(IOrderRepository orderRepository, ILogger<KitchenBarDisplayService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public List<Order> GetOrders(Status status, UserRole role)
        {
            try
            {
                return _orderRepository.GetOrders(status, role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to retrieve orders. Status: {Status}, Role: {Role}", status, role);
                return new List<Order>();
            }
        }

        public List<Order> GetOrdersByStatus(List<Status> statuses, UserRole role)
        {
            try
            {

                List<Order> orders = new List<Order>();

                foreach (Status status in statuses)
                {
                    orders.AddRange(_orderRepository.GetOrders(status, role));
                }

                return orders;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to retrieve orders by status. Statuses: {Statuses}, Role: {Role}", statuses, role);
                return new List<Order>();
            }
        }
        public bool UpdateOrderStatus(int orderId, Status currentStatus, UserRole role)
        {
            try
            {
                Status newStatus = GetNextOrPreviousStatus(currentStatus);
                return _orderRepository.UpdateOrderStatus(orderId, newStatus, role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating order status. OrderId: {OrderId}, Role: {Role}", orderId, role);
                return false;
            }
        }
        public bool UpdateOrderCategoryStatus(int orderId, CourseCategory category, Status currentStatus, UserRole role)
        {
            try
            {
                Status newStatus = GetNextOrPreviousStatus(currentStatus);
                return _orderRepository.UpdateOrderCategoryStatus(orderId, category, newStatus, role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating category status. OrderId: {OrderId}, Category: {Category}, Role: {Role}", orderId, category, role);
                return false;
            }
        }


        public bool UpdateOrderItemStatus(int orderItemId, Status currentStatus, UserRole role)
        {
            try
            {
                Status newStatus = GetNextOrPreviousStatus(currentStatus);
                return _orderRepository.UpdateOrderItemStatus(orderItemId, newStatus, role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating item status. OrderItemId: {OrderItemId}, Role: {Role}", orderItemId, role);
                return false;
            }
        }
        private Status GetNextOrPreviousStatus(Status currentStatus)
        {
            return currentStatus != Status.Ready
                ? StatusHelper.NextStatus(currentStatus)
                : StatusHelper.PreviousStatus(currentStatus);
        }

    }
}
