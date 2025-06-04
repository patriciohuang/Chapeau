using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITablesRepository _tableRepository;
        private readonly IMenuRepository _menuRepository;

        public OrderService(IOrderRepository orderRepository, ITablesRepository tableRepository, IMenuRepository menuRepository)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _menuRepository = menuRepository;
        }

        public List<Order> GetTodaysOrders()
        {
            // Get all orders for today (no status filter)
            return _orderRepository.GetOrders(null);
        }

        public List<Order> GetOrdersByStatus(Status status)
        {
            return _orderRepository.GetOrders(status);
        }

        public List<Order> GetOrdersByTable(int tableNr)
        {
            // Get all orders and filter by table
            List<Order> allOrders = _orderRepository.GetOrders(null);
            return allOrders.Where(o => o.Table.TableNr == tableNr).ToList();
        }

        public List<Order> GetActiveOrders()
        {
            List<Order> allOrders = _orderRepository.GetOrders(null);
            return allOrders.Where(o => o.Status != Status.Completed && o.Status != Status.Cancelled).ToList();
        }

        public List<Order> GetReadyOrders()
        {
            List<Order> allOrders = _orderRepository.GetOrders(null);
            return allOrders.Where(o => o.OrderItems.Any(oi => oi.Status == Status.Ready)).ToList();
        }

        public List<Order> GetActiveOrdersByTable(int tableNr)
        {
            List<Order> allOrders = _orderRepository.GetOrders(null);
            return allOrders.Where(o => o.Table.TableNr == tableNr && o.Status != Status.Completed && o.Status != Status.Cancelled).ToList();
        }

        public List<Order> GetReadyOrdersByTable(int tableNr)
        {
            List<Order> allOrders = _orderRepository.GetOrders(null);
            return allOrders.Where(o => o.Table.TableNr == tableNr && o.OrderItems.Any(oi => oi.Status == Status.Ready)).ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _orderRepository.GetOrderById(orderId);
        }

        public int? CheckIfOrderExists(int tableNr)
        {
            return _orderRepository.CheckIfOrderExists(tableNr);
        }

        // Get the tableId and employeeId, use them to create a new order and return its ID
        public int CreateOrder(int tableNr, Employee loggedInEmployee)
        {
            Table selectedTable= _tableRepository.GetTableByNumber(tableNr);

            int orderId = _orderRepository.CreateOrder(selectedTable.TableId, loggedInEmployee.EmployeeId);

            _tableRepository.UpdateTableAvailability(selectedTable.TableNr, false);

            return orderId;
        }

        public void AddItem(int orderId, MenuItem menuItem)
        {
            if (menuItem.Stock <= 0)
            {
                throw new InvalidOperationException("Cannot add item to order: item is out of stock.");
            }
            else
            {
                _orderRepository.AddItem(orderId, menuItem.MenuItemId);

                // Update the stock
                _menuRepository.UpdateStock(menuItem.MenuItemId);
            }
        }

        public void SendOrder(int orderId)
        {
            _orderRepository.SendOrder(orderId);
        }
    }
}