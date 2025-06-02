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
    }
}
