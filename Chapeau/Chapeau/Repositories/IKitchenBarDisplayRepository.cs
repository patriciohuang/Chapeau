using Chapeau.Enums;
using Chapeau.Models;
namespace Chapeau.Repositories
{
    //pato
    public interface IKitchenBarDisplayRepository
    {
        List<DisplayOrder> GetOrders(Status? status);
/*      void UpdateOrderStatus(int orderId, Status status);
        List<MenuItem> GetMenuItemsForOrder(int orderId);*/
    }
}
