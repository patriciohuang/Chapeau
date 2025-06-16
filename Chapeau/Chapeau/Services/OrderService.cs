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

        public List<Order> GetOrdersByFilter(string filter)
        {
            return filter.ToLower() switch
            {
                "active" => _orderRepository.GetActiveOrders(),
                "ready" => _orderRepository.GetReadyOrders(),
                "all" => _orderRepository.GetTodaysOrders(),
                _ => _orderRepository.GetTodaysOrders()
            };
        }

        public List<Order> GetOrdersByTableAndFilter(int tableNr, string filter)
        {
            return filter.ToLower() switch
            {
                "active" => _orderRepository.GetActiveOrdersByTable(tableNr),
                "ready" => _orderRepository.GetReadyOrdersByTable(tableNr),
                "all" => _orderRepository.GetOrdersByTable(tableNr),
                _ => _orderRepository.GetOrdersByTable(tableNr)
            };
        }

        public List<Order> GetActiveOrders()
        {
            return _orderRepository.GetActiveOrders();
        }

        public List<Order> GetReadyOrders()
        {
            return _orderRepository.GetReadyOrders();
        }

        public List<Order> GetActiveOrdersByTable(int tableNr)
        {
            return _orderRepository.GetActiveOrdersByTable(tableNr);
        }

        public List<Order> GetReadyOrdersByTable(int tableNr)
        {
            return _orderRepository.GetReadyOrdersByTable(tableNr);
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
            if (menuItem.Stock < menuItem.Stock)
            {
                throw new InvalidOperationException($"Cannot add {menuItem.Name} to order: item is out of stock.");
            }
            else
            {
                _orderRepository.AddItem(orderId, menuItem.MenuItemId);

                // Update the stock
                _menuRepository.UpdateStock(menuItem.MenuItemId, -1);
            }
        }

        public OrderItem GetOrderItem(int orderItemId)
        {
            return _orderRepository.GetOrderItem(orderItemId);
        }

        public void EditOrderItem(OrderItem item)
        {
            _orderRepository.EditOrderItem(item);
        }

        public void DeleteOrderItem(int orderItemId)
        {
            OrderItem orderItem = GetOrderItem(orderItemId);

            if(orderItem.Status == Status.Ready || orderItem.Status == Status.Served)
            {
                throw new InvalidOperationException("Cannot delete an order item that is already ready or served.");
            }

            _orderRepository.DeleteOrderItem(orderItemId);

            _menuRepository.UpdateStock(orderItem.MenuItem.MenuItemId, orderItem.Count);
        }

        public void DeleteUnsentOrderItems(int orderId)
        {
            _orderRepository.DeleteUnsentOrderItems(orderId);
        }

        public void SendOrder(int orderId)
        {
            _orderRepository.SendOrder(orderId);
        }

        public void MarkOrderItemAsServed(int orderId, int orderItemId)
        {
            _orderRepository.UpdateOrderItemStatus(orderItemId, Status.Served);
            RecalculateOrderStatus(orderId);
        }

        public void MarkOrderAsServed(int orderId)
        {
            _orderRepository.UpdateAllReadyItemsToServed(orderId);
            RecalculateOrderStatus(orderId);
        }

        public void RecalculateOrderStatus(int orderId)
        {
            var order = GetOrderById(orderId);
            var calculatedStatus = CalculateOrderStatusFromItems(order);

            if (order.Status != calculatedStatus)
            {
                _orderRepository.UpdateOrderStatus(orderId, calculatedStatus);
            }
        }

        private Status CalculateOrderStatusFromItems(Order order)
        {
            if (!order.OrderItems.Any())
            {
                return Status.Unordered;
            }

            var statusPriority = GetStatusPriorityOrder();
            var itemStatuses = order.OrderItems.Select(item => item.Status).Distinct();

            return GetEarliestStatus(itemStatuses, statusPriority);
        }

        private List<Status> GetStatusPriorityOrder()
        {
            return new List<Status>
            {
                Status.Unordered,
                Status.Ordered,
                Status.Preparing,
                Status.Ready,
                Status.Served,
                Status.Completed,
                Status.Cancelled
            };
        }

        private Status GetEarliestStatus(IEnumerable<Status> statuses, List<Status> priorityOrder)
        {
            foreach (var status in priorityOrder)
            {
                if (statuses.Contains(status))
                {
                    return status;
                }
            }

            return Status.Unordered;
        }
    }
}