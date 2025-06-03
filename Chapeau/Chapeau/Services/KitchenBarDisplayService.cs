using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    //pato
    public class KitchenBarDisplayService : IKitchenBarDisplayService
    {
        private readonly IOrderRepository _kitchenBarDisplayRepository;

        public KitchenBarDisplayService(IOrderRepository kitchenBarDisplayRepository)
        {
            _kitchenBarDisplayRepository = kitchenBarDisplayRepository;
        }

        public List<Order> GetOrders(Status status)
        {
            return _kitchenBarDisplayRepository.GetOrders(status);
        }
        public List<Order> GetOrdersByStatus(List<Status> statuses)
        {
            List<Order> orders = new List<Order>();

            foreach (Status status in statuses)
            {
                orders.AddRange(_kitchenBarDisplayRepository.GetOrders(status));
            }

            return orders;
        }

        public bool UpdateOrderStatus(int orderId, Status currentStatus)
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
                return _kitchenBarDisplayRepository.UpdateOrderStatus(orderId, newStatus);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                return false;
            }
        }
    }
}
