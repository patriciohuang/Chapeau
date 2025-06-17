using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    //pato
    public class KitchenBarDisplayService : IKitchenBarDisplayService
    {
        private readonly IOrderRepository _orderRepository;

        public KitchenBarDisplayService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetOrders(Status status)
        {
            return _orderRepository.GetOrders(status);
        }
        public List<Order> GetOrdersByStatus(List<Status> statuses)
        {
            List<Order> orders = new List<Order>();

            foreach (Status status in statuses)
            {
                orders.AddRange(_orderRepository.GetOrders(status));
            }

            return orders;
        }

        public bool UpdateOrderStatus(int orderId, Status currentStatus, UserRole role)
        {
            try
            {
                Status newStatus;
                if (currentStatus != Status.Ready)
                {
                    newStatus = StatusHelper.NextStatus(currentStatus);
                }
                else
                {
                    newStatus = StatusHelper.PreviousStatus(currentStatus);
                }
                return _orderRepository.UpdateOrderStatus(orderId, newStatus, role);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                return false;
            }
        }
        public bool UpdateOrderCategoryStatus(int orderId, CourseCategory category, Status currentStatus, UserRole role)
        {
            try
            {
                Status newStatus;
                if (currentStatus != Status.Ready)
                {
                    newStatus = StatusHelper.NextStatus(currentStatus);
                }
                else
                {
                    newStatus = StatusHelper.PreviousStatus(currentStatus);
                }
                return _orderRepository.UpdateOrderCategoryStatus(orderId, category, newStatus, role);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                return false;
            }
        }


        public bool UpdateOrderItemStatus(int orderItemId, Status currentStatus, UserRole role)
        {
            try
            {
                Status newStatus;
                if (currentStatus != Status.Ready)
                {
                    newStatus = StatusHelper.NextStatus(currentStatus);
                }
                else
                {
                    newStatus = StatusHelper.PreviousStatus(currentStatus);
                }
                return _orderRepository.UpdateOrderItemStatus(orderItemId, newStatus, role);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                return false;
            }
        }
    }
}
