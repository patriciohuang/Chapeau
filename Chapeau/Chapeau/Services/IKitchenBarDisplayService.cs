using Chapeau.Models;
using Chapeau.Models.Enums;
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
