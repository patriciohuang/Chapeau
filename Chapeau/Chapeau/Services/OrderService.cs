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
            return _orderRepository.GetOrders(null, null);
        }

        public List<Order> GetOrdersByStatus(Status status)
        {
            return _orderRepository.GetOrders(status, null);
        }

        public List<Order> GetOrdersByTable(int tableNr)
        {
            // Get all orders and filter by table
            List<Order> allOrders = _orderRepository.GetOrders(null, null);
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

        // This retrieves a single FULL order from the database by its ID
        public Order GetOrderById(int orderId)
        {
            return _orderRepository.GetOrderById(orderId);
        }

        //This is used to check if an order already exists, it is used to check if an order needs to be created/an error message needs to be shown (empty orders aren't allowed to exist according to the ERD)
        public int? CheckIfOrderExists(int tableNr)
        {
            return _orderRepository.CheckIfOrderExists(tableNr);
        }

        // Get the tableId and employeeId, use them to create a new order and return its ID
        public int CreateOrder(int tableNr, Employee loggedInEmployee)
        {
            Table selectedTable = _tableRepository.GetTableByNumber(tableNr);

            int orderId = _orderRepository.CreateOrder(selectedTable.TableId, loggedInEmployee.EmployeeId);

            _tableRepository.UpdateTableAvailability(selectedTable.TableNr, false);

            return orderId;
        }

        //This checks if the order item already exists (same orderId, menuItemId, comment and status)
        //adds it to the order if it doesn't, or updates the count if it does
        public void AddItem(int orderId, MenuItem menuItem, string? comment)
        {
            if (menuItem.Stock < 1)
            {
                throw new InvalidOperationException($"Cannot add {menuItem.Name} to order: item is out of stock.");
            }

            comment = null ?? ""; // Ensure comment is not null
            Status status = Status.Unordered;

            int? orderIdFromDatabase = _orderRepository.CheckIfOrderItemExists(orderId, menuItem.MenuItemId, comment, status);


            if (orderIdFromDatabase == null)
            {

                _orderRepository.AddItem(orderId, menuItem.MenuItemId);
            }
            else
            {
                // If the item already exists in the order, we can just update the count
                OrderItem existingOrderItem = _orderRepository.GetOrderItem((int)orderIdFromDatabase);

                existingOrderItem.Count++;

                _orderRepository.EditOrderItem(existingOrderItem);
            }

            // Update the stock
            _menuRepository.UpdateStock(menuItem.MenuItemId, -1);

        }

        //This retrieves a single order item from the database by its ID
        public OrderItem GetOrderItem(int orderItemId)
        {
            return _orderRepository.GetOrderItem(orderItemId);
        }

        // This is to change the count and notes of an existing order item
        public void EditOrderItem(OrderItem newOrderItem)
        {
            if (newOrderItem.Count < 1 || newOrderItem.Count > 100)
            {
                throw new InvalidOperationException("Cannot edit order item: Amount ordered must be at least 1.");
            }

            OrderItem existingOrderItem = _orderRepository.GetOrderItem(newOrderItem.OrderItemId);

            //Calculate how much needs to be added or removed from the stock
            int differenceInStock = existingOrderItem.Count - newOrderItem.Count;

            if (-differenceInStock > existingOrderItem.MenuItem.Stock)
            {
                throw new InvalidOperationException($"Cannot add {-differenceInStock} {existingOrderItem.MenuItem.Name} to order: there are only {existingOrderItem.MenuItem.Stock} left.");
            }

            _orderRepository.EditOrderItem(newOrderItem);

            _menuRepository.UpdateStock(existingOrderItem.MenuItem.MenuItemId, differenceInStock);
        }

        // This is to delete a single order item, deletes multiple if it has a count greater than 1, then adds them back to the stock
        public void DeleteOrderItem(int orderItemId, int tableNr, int orderId)
        {
            //Get a list of all order items to check if you'll end up with an empty order
            List<OrderItem> orderItems = _orderRepository.GetOrderItems(orderId);

            OrderItem? orderItem = orderItems.FirstOrDefault(item => item.OrderItemId == orderItemId);

            if (orderItem == null)
            {
                throw new InvalidOperationException("Order item not found.");
            }

            if (orderItem.Status == Status.Ready || orderItem.Status == Status.Served || orderItem.Status == Status.Completed)
            {
                throw new InvalidOperationException("Cannot delete an order item that is already ready or served.");
            }

            _orderRepository.DeleteOrderItem(orderItemId);

            _menuRepository.UpdateStock(orderItem.MenuItem.MenuItemId, orderItem.Count);

            if (orderItems.Count <= 1)
            {
                // If that was the last item in the order, delete the order
                _orderRepository.DeleteOrder(orderId);
                _tableRepository.UpdateTableAvailability(tableNr, true);
            }
        }

        // This is to delete all order items that have not been sent yet, and updates the stock accordingly
        public void CancelUnsentOrder(int orderId, int tableNr)
        {
            //Get a list of all order items to check if you'll end up with an empty order
            List<OrderItem> orderItems = _orderRepository.GetOrderItems(orderId);

            //Need a list of unsent order items and their stock
            List<OrderItem> unorderedOrderItems = orderItems.Where(item => item.Status == Status.Unordered).ToList();

            if (_orderRepository.DeleteUnsentOrderItems(orderId) == 0)
            {
                throw new InvalidOperationException("No unsent order items found to cancel for this order.");
            }

            //Update the stock for each unordered item
            foreach (OrderItem item in unorderedOrderItems)
            {
                _menuRepository.UpdateStock(item.MenuItem.MenuItemId, item.Count);
            }

            //Prevent an empty order from existing (delete the order if removing unordered items would leave an empty order
            if (orderItems.Count == unorderedOrderItems.Count)
            {
                _orderRepository.DeleteOrder(orderId);

                _tableRepository.UpdateTableAvailability(tableNr, true);
            }
        }

        // This is to send the order to the kitchen, updates the status of all items to Ordered
        public void SendOrder(int orderId)
        {
            _orderRepository.SendOrder(orderId);
        }

        public void MarkOrderItemAsServed(int orderId, int orderItemId)
        {
            _orderRepository.UpdateOrderItemStatus(orderItemId, Status.Served, UserRole.Waiter);
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
                _orderRepository.UpdateOrderStatus(orderId, calculatedStatus, UserRole.Waiter);

                if (calculatedStatus == Status.Completed)
                {
                    _orderRepository.UpdateAllOrderItemsStatus(orderId, Status.Completed);
                }
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
            var calculatedStatus = GetEarliestStatus(itemStatuses, statusPriority);

            // If the order is paid and the status is Served, we consider it Completed
            if (order.IsPaid && calculatedStatus == Status.Served)
            {
                return Status.Completed;
            }

            return calculatedStatus;
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

        // This is the helper to calculate the overall orders status, based on the "earliest" statused orderitem in said order
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