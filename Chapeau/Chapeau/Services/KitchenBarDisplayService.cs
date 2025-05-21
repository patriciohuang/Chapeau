using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    //pato
    public class KitchenBarDisplayService : IKitchenBarDisplayService
    {
        private readonly IKitchenBarDisplayRepository _kitchenBarDisplayRepository;
        public KitchenBarDisplayService(IKitchenBarDisplayRepository kitchenBarDisplayRepository)
        {
            _kitchenBarDisplayRepository = kitchenBarDisplayRepository;
        }
        public List<DisplayOrder> GetOrders(Status? status = null)
        {
            List<DisplayOrder> incommingorders = _kitchenBarDisplayRepository.GetOrders(status);
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
