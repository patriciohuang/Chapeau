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
        public List<Order> GetOrders(Status? status = null)
        {
            List<Order> incommingorders = _kitchenBarDisplayRepository.GetOrders(status);
            return incommingorders;
        }
/*        public void UpdateOrderStatus(int orderId, Status status)
        {
            _kitchenBarDisplayRepository.UpdateOrderStatus(orderId, status);
        }
        public List<MenuItem> GetMenuItemsForOrder(int orderId)
        {
            return _kitchenBarDisplayRepository.GetMenuItemsForOrder(orderId);
        }*/
    }
}
