using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITablesRepository _tableRepository;
        private readonly IEmployeesRepository _employeeRepository;
        private readonly IMenuRepository _menuRepository;

        public OrderService(IOrderRepository orderRepository, ITablesRepository tableRepository, IEmployeesRepository employeeRepository, IMenuRepository menuRepository)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _employeeRepository = employeeRepository;
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

        public Order GetOrderById(int orderId)
        {
            return _orderRepository.GetOrder(orderId);
        }

        public int? CheckIfOrderExists(int tableNr)
        {
            return _orderRepository.CheckIfOrderExists(tableNr);
        }

        // Get the tableId and employeeId, use them to create a new order and return its ID
        public int CreateOrder(int tableNr, Employee loggedInEmployee)
        {
            int tableId = _tableRepository.GetTableId(tableNr);
            int employeeId = _employeeRepository.GetEmployeeId(loggedInEmployee.EmployeeNr);

            _tableRepository.UpdateTableAvailability(tableNr, false);

            return _orderRepository.CreateOrder(tableId, employeeId);
        }

        public void AddItem(int orderId, MenuItem menuItem)
        {
            int menuItemId = _menuRepository.GetMenuItemId(menuItem.Name);

            _orderRepository.AddItem(orderId, menuItemId);
        }
    }
}