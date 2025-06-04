using Chapeau.Models;
using Chapeau.Models.Enums;
namespace Chapeau.Repositories
{
    //pato
    public interface IOrderRepository
    {
        List<Order> GetOrders(Status? status);

        Order GetOrderById(int orderId);

        int? CheckIfOrderExists(int tableNr);

        int CreateOrder(int tableNr, int employeeId);

        void AddItem(int orderId, int menuItemId);

        void SendOrder(int orderId);

        void UpdateOrder(Order order);


        bool UpdateOrderStatus(int orderId, Status status);

        //THESE ARE ALL EMPTY AND NOT IMPLEMENTED. REMOVE LATER, WE ALREADY HAVE METHODS THAT DO THIS
        List<Order> GetAllOrders();

        void AddOrder(Order order);

        void DeleteOrder(int orderId);
    }
}
