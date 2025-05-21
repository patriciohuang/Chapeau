using Chapeau.Enums;
using Chapeau.Models;
namespace Chapeau.Services
{
    //pato
    public interface IKitchenBarDisplayService
    {
        List<DisplayOrder> GetOrders(Status? status);
        /*  
        void UpdateOrderStatus(int orderId, Status status);  
        List<MenuItem> GetMenuItemsForOrder(int orderId);  
        */
    }
}
