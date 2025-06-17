using Microsoft.Data.SqlClient;
using Chapeau.Models;
using Chapeau.Models.Enums;
using System.Data;

namespace Chapeau.Repositories
{
    //pato
    public class OrderRepository : IOrderRepository
    {
        private readonly string? _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("chapeaudatabase");
        }

        /// Read order item from the database
        private Order ReadOrder(SqlDataReader reader)
        {
            // Read order details
            int orderId = (int)reader["order_id"];
            DateOnly dateOrdered = DateOnly.FromDateTime((DateTime)reader["date_ordered"]);
            TimeOnly timeOrdered = TimeOnly.FromTimeSpan((TimeSpan)reader["time_ordered"]);
            bool isPaid = (bool)reader["is_paid"];


            // Read employee
            Employee employee = ReadEmployee(reader);

            // Read table
            Table table = ReadTable(reader);

            return new Order(orderId, dateOrdered, timeOrdered, isPaid, table, employee);
        }

        private Employee ReadEmployee(SqlDataReader reader)
        {
            int employeeId = (int)reader["employee_id"];
            int employeeNr = (int)reader["employee_nr"];
            string firstName = (string)reader["first_name"];
            string lastName = (string)reader["last_name"];
            string role = (string)reader["role"];
            string password = (string)reader["password"];

            return new Employee(employeeId, employeeNr, firstName, lastName, role, password);
        }

        private Table ReadTable(SqlDataReader reader)
        {
            int tableId = (int)reader["table_id"];
            int tableNr = (int)reader["table_nr"];
            bool availability = (bool)reader["availability"];

            return new Table(tableId, tableNr, availability);
        }

        /// Read order item from the database
        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            int orderItemId = (int)reader["order_item_id"];
            int count = (int)reader["count"];
            string comment = reader["comment"] as string ?? string.Empty;
            Status itemStatus = Enum.Parse<Status>((string)reader["item_status"], true);

            MenuItem menuItem = ReadMenuItem(reader);

            return new OrderItem(orderItemId, count, comment, itemStatus, menuItem);
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            // Read menu item
            int menuItemId = (int)reader["menu_item_id"];
            string name = (string)reader["name"];
            decimal price = (decimal)reader["price"];
            MenuCard menuCard = Enum.Parse<MenuCard>((string)reader["menu_card"], true);
            CourseCategory courseCategory = Enum.Parse<CourseCategory>((string)reader["course_category"], true);
            int stock = (int)reader["stock"];
            bool isAlcoholic = (bool)reader["isAlcoholic"];

            return new MenuItem(menuItemId, name, price, menuCard, courseCategory, stock, isAlcoholic);
        }

        private void AddOrderItemToOrder(Order order, OrderItem newOrderItem)
        {
            // Check if the item already exists in the order by comparing the Name, Status, and Comment
            OrderItem? existingOrderItem = order.OrderItems.FirstOrDefault(i => i.MenuItem.Name == newOrderItem.MenuItem.Name && i.Status == newOrderItem.Status && i.Comment == newOrderItem.Comment);
            if (existingOrderItem != null)
            {
                // If it exists, increment the count
                existingOrderItem.Count++;
            }
            else
            {
                // If it doesn't exist, add the new item to the order
                order.OrderItems.Add(newOrderItem);
            }
        }


        public List<Order> GetOrders(Status? status, UserRole? role)
        {
            // Create a dictionary to store orders with their order_id as the key
            Dictionary<int, Order> orders = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all orders for today (optionally filtered by status)
                string sql = @"SELECT   o.order_id, o.date_ordered, o.time_ordered, o.is_paid,
                                        i.order_item_id, i.count, i.comment, i.status AS item_status,
                                        e.employee_id, e.employee_nr, e.first_name, e.last_name, e.role, e.password,
                                        t.table_id, t.table_nr, t.availability,
                                        m.menu_item_id, m.name, m.price, m.menu_card, m.course_category, m.stock, m.isAlcoholic
                            FROM [order] o
                            JOIN order_item i ON o.order_id = i.order_id
                            JOIN menu_item m ON i.menu_item_id = m.menu_item_id
                            JOIN employee e ON o.employee_id = e.employee_id
                            JOIN [table] t ON t.table_id = o.table_id
                            WHERE CAST(o.date_ordered AS DATE) = CAST(GETDATE() AS DATE)
                            ";
                if (role == UserRole.Bar || role == UserRole.Kitchen)
                {
                    sql += " AND i.status != 'Served'";
                }

                sql += " ORDER BY o.time_ordered";

                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int orderId = (int)reader["order_id"];
                    // check if the order already exists in the dictionary
                    if (!orders.ContainsKey(orderId))
                    {
                        Order order = ReadOrder(reader);
                        // add the order to the dictionary
                        orders.Add(orderId, order);
                    }
                    AddOrderItemToOrder(orders[orderId], ReadOrderItem(reader));
                }

                reader.Close();
            }

            foreach (var order in orders.Values)
            {
                if (order.IsPaid && order.OrderItems.All(item => item.Status == Status.Served))
                {
                    UpdateAllOrderItemsStatus(order.OrderId, Status.Completed);
                    // Update the in-memory order items
                    foreach (var item in order.OrderItems)
                    {
                        item.Status = Status.Completed;
                    }
                }
            }


            if (status.HasValue && role.HasValue)
            {
                return orders.Values.Where(order => order.GetStatusForRole(role.Value) == status.Value).ToList();
            }
            else
            {
                return orders.Values.ToList(); // If status is null, return all orders
            }
        }

        public Order GetOrderById(int orderId)
        {
            Order? order = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all orders for today (optionally filtered by status)
                string sql = @"SELECT   o.order_id, o.date_ordered, o.time_ordered, o.is_paid,
                                        i.order_item_id, i.count, i.comment, i.status AS item_status,
                                        e.employee_id, e.employee_nr, e.first_name, e.last_name, e.role, e.password,
                                        t.table_nr, t.table_id, t.availability,
                                        m.menu_item_id, m.name, m.price, m.menu_card, m.course_category, m.stock, m.isAlcoholic
                            FROM [order] o
                            JOIN order_item i ON o.order_id = i.order_id
                            JOIN menu_item m ON i.menu_item_id = m.menu_item_id
                            JOIN employee e ON o.employee_id = e.employee_id
                            JOIN [table] t ON t.table_id = o.table_id
                            WHERE o.order_id = @orderId
                            ORDER BY m.name";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@orderId", orderId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //null-coalescing assignment (fancy if statement) that checks if the order is null, if it is, read the order using the ReadOrder method. If it is not, move on. 
                    //IT IS ALWAYS NULL ON THE FIRST PASS
                    order ??= ReadOrder(reader);

                    AddOrderItemToOrder(order, ReadOrderItem(reader));
                }
                reader.Close();

                if (order != null && order.IsPaid && order.OrderItems.All(item => item.Status == Status.Served))
                {
                    UpdateAllOrderItemsStatus(orderId, Status.Completed);
                    // Reload the order to get updated item statuses
                    return GetOrderById(orderId);
                }
            }
            return order;
        }


        // Checks if an order exists for a given table number
        public int? CheckIfOrderExists(int tableNr)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to check if an order exists for the given table number
                string sql = @"Select MAX([order_id]) AS order_id
                                FROM [order] AS ord
                                JOIN [table] AS tab ON ord.table_id = tab.table_id
                                WHERE table_nr = @tableNr AND [is_paid] = 0";
                //TODO see if MAX order_id can become a regular order_id

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@tableNr", tableNr);

                connection.Open();

                return command.ExecuteScalar() as int?; // Returns the highest order ID for the table if the table is occupied.
            }
        }

        // Creates a new order and returns the order ID
        public int CreateOrder(int tableId, int employeeId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to insert a new order with default status 'In Progress'
                string sql = @"INSERT INTO [order] (table_id, employee_id , date_ordered, time_ordered)
                               VALUES (@tableId, @employeeId, @dateOrdered, @timeOrdered);
                               SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@tableId", tableId);
                command.Parameters.AddWithValue("@employeeId", employeeId);
                command.Parameters.AddWithValue("@dateOrdered", DateTime.Now.Date);
                command.Parameters.AddWithValue("@timeOrdered", DateTime.Now.TimeOfDay);

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void DeleteOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to delete an order and its items
                string sql = @"DELETE FROM [order_item] WHERE order_id = @orderId;
                               DELETE FROM [order] WHERE order_id = @orderId;";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@orderId", orderId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public int? CheckIfOrderItemExists(int orderId, int menuItemId, string comment, Status status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                const string sql = @"SELECT order_item_id
                               FROM order_item
                               WHERE order_id = @orderId AND menu_item_id = @menuItemId AND comment = @comment AND status = @status";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@menuItemId", menuItemId);
                command.Parameters.AddWithValue("@comment", comment);
                command.Parameters.AddWithValue("@status", status.ToString());

                connection.Open();

                return command.ExecuteScalar() as int?;
            }
        }

        public void AddItem(int orderId, int menuItemId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to insert a new order item
                string sql = @"INSERT INTO order_item (order_id, menu_item_id, comment, status)
                               VALUES (@orderId, @menuItemId, @comment, @status)";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@menuItemId", menuItemId);
                command.Parameters.AddWithValue("@comment", string.Empty); // Default comment is empty
                command.Parameters.AddWithValue("@status", Status.Unordered.ToString());

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public OrderItem GetOrderItem(int orderItemId)
        {
            OrderItem item = new OrderItem();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to insert a new order item
                const string sql = @"SELECT i.order_item_id, i.count, i.comment, i.status AS item_status,
                                            m.menu_item_id, m.name, m.price, m.menu_card, m.course_category, m.stock, m.isAlcoholic 
                                    FROM order_item AS i
                                    JOIN menu_item AS m ON i.menu_item_id = m.menu_item_id
                                    WHERE order_item_id = @orderItemId";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@orderItemId", orderItemId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    item = ReadOrderItem(reader);
                }
                reader.Close();
            }
            return item;
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            List<OrderItem> item = new List<OrderItem>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to insert a new order item
                const string sql = @"SELECT i.order_item_id, i.count, i.comment, i.status AS item_status,
                                            m.menu_item_id, m.name, m.price, m.menu_card, m.course_category, m.stock, m.isAlcoholic 
                                    FROM order_item AS i
                                    JOIN menu_item AS m ON i.menu_item_id = m.menu_item_id
                                    WHERE order_id = @orderId";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@orderId", orderId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    item.Add(ReadOrderItem(reader));
                }
                reader.Close();
            }
            return item;
        }

        public void EditOrderItem(OrderItem item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to update an order item
                string sql = @"UPDATE order_item
                               SET count = @count, 
                                   comment = @comment
                               WHERE order_item_id = @orderItemId";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@orderItemId", item.OrderItemId);
                command.Parameters.AddWithValue("@count", item.Count);
                if (!string.IsNullOrEmpty(item.Comment)){
                    command.Parameters.AddWithValue("@comment", item.Comment);
                }
                else
                {
                    command.Parameters.AddWithValue("@comment", "");
                }

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteOrderItem(int orderItemId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to delete an order item
                string sql = @"DELETE FROM order_item 
                               WHERE order_item_id = @orderItemId";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@orderItemId", orderItemId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public int DeleteUnsentOrderItems(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to delete all order items with status 'Unordered' for a specific order
                string sql = @"DELETE FROM order_item
                               WHERE order_id = @orderId AND status = 'Unordered'";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@orderId", orderId);

                connection.Open();
                return command.ExecuteNonQuery(); // Returns the number of rows affected
            }
        }

        public void SendOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to update the order status to 'In Progress'
                string sql = @"UPDATE [order_item]
                               SET status = @status
                               WHERE order_id = @orderId AND status = 'Unordered'";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@status", Status.Ordered.ToString());

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public bool UpdateOrderStatus(int orderId, Status status, UserRole role)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = GetUpdateOrderStatusQuery(role);
            var command = CreateCommand(connection, sql);
            command.Parameters.AddWithValue("@status", status.ToString());
            command.Parameters.AddWithValue("@orderId", orderId);
            command.Parameters.AddWithValue("@role", role.ToString());
            AddMenuCardParameters(command, role);

            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public void UpdateOrderPaid(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE [order]
             SET is_paid = @IsPaid 
             WHERE order_id = @OrderId";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@OrderId", order.OrderId);
                command.Parameters.AddWithValue("@IsPaid", order.IsPaid);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception($"Order with ID {order.OrderId} not found or could not be updated");
                }
            }
        }


        public bool UpdateOrderItemStatus(int orderItemId, Status status, UserRole role)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = GetUpdateOrderItemStatusQuery(role);
            var command = CreateCommand(connection, sql);
            command.Parameters.AddWithValue("@orderItemId", orderItemId);
            command.Parameters.AddWithValue("@status", status.ToString());
            command.Parameters.AddWithValue("@role", role.ToString());
            AddMenuCardParameters(command, role);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            ValidateOrderItemUpdateResult(rowsAffected, orderItemId);
            return rowsAffected > 0;
        }
        public bool UpdateOrderCategoryStatus(int orderId, CourseCategory category, Status status, UserRole role)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var sql = GetUpdateOrderCategoryStatusQuery(role);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@Status", status.ToString());
                command.Parameters.AddWithValue("@Category", category.ToString());
                command.Parameters.AddWithValue("@role", role.ToString());
                AddMenuCardParameters(command, role);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        public void UpdateAllReadyItemsToServed(int orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = GetUpdateAllReadyItemsQuery();
            var command = CreateCommand(connection, sql);
            command.Parameters.AddWithValue("@orderId", orderId);
            command.Parameters.AddWithValue("@fromStatus", Status.Ready.ToString());
            command.Parameters.AddWithValue("@toStatus", Status.Served.ToString());

            connection.Open();
            command.ExecuteNonQuery();
        }

        private List<Order> ExecuteOrderQuery(string sql, string statusFilter)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = CreateCommand(connection, sql);
            command.Parameters.AddWithValue("@status", $"%{statusFilter}%");

            connection.Open();
            return ProcessOrderResults(command.ExecuteReader());
        }

        private List<Order> ExecuteOrderQueryWithTable(string sql, string statusFilter, int tableNr)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = CreateCommand(connection, sql);
            command.Parameters.AddWithValue("@status", $"%{statusFilter}%");
            command.Parameters.AddWithValue("@tableNr", tableNr);

            connection.Open();
            return ProcessOrderResults(command.ExecuteReader());
        }

        // Why is this here
        private List<Order> ExecuteOrderQueryWithId(string sql, int orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = CreateCommand(connection, sql);
            command.Parameters.AddWithValue("@orderId", orderId);

            connection.Open();
            return ProcessOrderResults(command.ExecuteReader());
        }

        public void UpdateAllOrderItemsStatus(int orderId, Status status)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "UPDATE order_item SET status = @status WHERE order_id = @orderId";
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@orderId", orderId);
            command.Parameters.AddWithValue("@status", status.ToString());

            connection.Open();
            command.ExecuteNonQuery();
        }

        private SqlCommand CreateCommand(SqlConnection connection, string sql)
        {
            return new SqlCommand(sql, connection);
        }
        private List<Order> ProcessOrderResults(SqlDataReader reader)
        {
            var orders = new Dictionary<int, Order>();

            while (reader.Read())
            {
                var orderId = (int)reader["order_id"];
                ProcessOrderRow(orders, orderId, reader);
            }

            reader.Close();
            return orders.Values.ToList();
        }

        private void ProcessOrderRow(Dictionary<int, Order> orders, int orderId, SqlDataReader reader)
        {
            if (!orders.ContainsKey(orderId))
            {
                orders.Add(orderId, ReadOrder(reader));
            }

            AddOrderItemToOrder(orders[orderId], ReadOrderItem(reader));
        }

        private void ValidateOrderItemUpdateResult(int rowsAffected, int orderItemId)
        {
            if (rowsAffected == 0)
            {
                throw new Exception($"Order item with ID {orderItemId} not found or could not be updated");
            }
        }


        public List<Order> GetTodaysOrders()
        {
            return ExecuteOrderQuery(GetBaseOrderQuery() + GetTodayFilter(), "%");
        }

        public List<Order> GetActiveOrders()
        {
            var sql = GetBaseOrderQuery() + GetTodayFilter() + GetActiveOrderFilter();
            return ExecuteOrderQuery(sql, "%");
        }

        public List<Order> GetReadyOrders()
        {
            var sql = GetBaseOrderQuery() + GetTodayFilter() + GetReadyItemsFilter();
            return ExecuteOrderQuery(sql, "%");
        }

        public List<Order> GetOrdersByTable(int tableNr)
        {
            var sql = GetBaseOrderQuery() + GetTodayFilter() + GetTableFilter();
            return ExecuteOrderQueryWithTable(sql, "%", tableNr);
        }

        public List<Order> GetActiveOrdersByTable(int tableNr)
        {
            var sql = GetBaseOrderQuery() + GetTodayFilter() + GetActiveOrderFilter() + GetTableFilter();
            return ExecuteOrderQueryWithTable(sql, "%", tableNr);
        }

        public List<Order> GetReadyOrdersByTable(int tableNr)
        {
            var sql = GetBaseOrderQuery() + GetTodayFilter() + GetReadyItemsFilter() + GetTableFilter();
            return ExecuteOrderQueryWithTable(sql, "%", tableNr);
        }

        private string GetBaseOrderQuery()
        {
            return @"SELECT o.order_id, o.date_ordered, o.time_ordered, o.is_paid,
                            i.order_item_id, i.count, i.comment, i.status AS item_status,
                            e.employee_id, e.employee_nr, e.first_name, e.last_name, e.role, e.password,
                            t.table_id, t.table_nr, t.availability,
                            m.menu_item_id, m.name, m.price, m.menu_card, m.course_category, m.stock, m.isAlcoholic
                     FROM [order] o
                     JOIN order_item i ON o.order_id = i.order_id
                     JOIN menu_item m ON i.menu_item_id = m.menu_item_id
                     JOIN employee e ON o.employee_id = e.employee_id
                     JOIN [table] t ON t.table_id = o.table_id ";
        }

        private string GetTodayAndStatusFilter()
        {
            return "WHERE CAST(o.date_ordered AS DATE) = CAST(GETDATE() AS DATE) AND i.status LIKE @status AND i.status != 'Unordered' ORDER BY o.time_ordered";
        }

        private string GetTodayFilter()
        {
            return "WHERE CAST(o.date_ordered AS DATE) = CAST(GETDATE() AS DATE) AND i.status != 'Unordered' ";
        }

        private string GetActiveOrderFilter()
        {
            return "AND i.status NOT IN ('Completed', 'Cancelled') ";
        }

        private string GetReadyItemsFilter()
        {
            return "AND o.order_id IN (SELECT DISTINCT order_id FROM order_item WHERE status = 'Ready') ";
        }

        private string GetTableFilter()
        {
            return "AND t.table_nr = @tableNr ";
        }

        private void AddMenuCardParameters(SqlCommand command, UserRole role)
        {
            switch (role)
            {
                case UserRole.Bar:
                    command.Parameters.AddWithValue("@menuCard", "Drinks");
                    break;

                case UserRole.Kitchen:
                    command.Parameters.AddWithValue("@menuCard1", "Lunch");
                    command.Parameters.AddWithValue("@menuCard2", "Dinner");
                    command.Parameters.AddWithValue("@menuCard3", "LunchAndDinner");
                    break;

                default: // Admin or other roles
                    command.Parameters.AddWithValue("@menuCard1", "Drinks");
                    command.Parameters.AddWithValue("@menuCard2", "Lunch");
                    command.Parameters.AddWithValue("@menuCard3", "Dinner");
                    command.Parameters.AddWithValue("@menuCard4", "LunchAndDinner");
                    break;
            }
        }

        // SQL Query Methods
        private string GetUpdateOrderStatusQuery(UserRole role)
        {
            string baseQuery = @"
                UPDATE oi
                SET status = @status
                FROM [Order_item] AS oi
                INNER JOIN menu_item AS mi ON mi.menu_item_id = oi.menu_item_id
                WHERE oi.order_id = @orderId
                AND (
                    (@role = 'Bar' AND mi.menu_card = 'Drinks') OR
                    (@role = 'Kitchen' AND mi.menu_card IN ('Lunch', 'Dinner', 'LunchAndDinner')) OR
                    (@role NOT IN ('Bar', 'Kitchen'))
                )";

            // Add extra condition if role is Bar or Kitchen
            if (role == UserRole.Bar || role == UserRole.Kitchen)
            {
                baseQuery += " AND oi.status != 'Served'";
            }

            return baseQuery;
        }

        private string GetUpdateOrderItemStatusQuery(UserRole role)
        {
            return @"UPDATE oi 
                    SET status = @Status
                    FROM [Order_item] AS oi
                        INNER JOIN menu_item AS mi ON
                            mi.menu_item_id = oi.menu_item_id
                            WHERE oi.order_item_id = @orderItemId AND 
                            ((@role = 'Bar' AND mi.menu_card = 'Drinks') OR
                            (@role = 'Kitchen' AND mi.menu_card IN ('Lunch', 'Dinner', 'LunchAndDinner')) OR
                            (@role NOT IN ('Bar', 'Kitchen')))";
        }
        private string GetUpdateOrderCategoryStatusQuery(UserRole role)
        {
            return @"UPDATE oi
                    SET status = @Status
                    FROM [Order_item] AS oi
                        INNER JOIN menu_item AS mi ON
                            mi.menu_item_id = oi.menu_item_id
		                    WHERE mi.course_category = @Category AND oi.order_id = @OrderId AND 
                            ((@role = 'Bar' AND mi.menu_card = 'Drinks') OR
                            (@role = 'Kitchen' AND mi.menu_card IN ('Lunch', 'Dinner', 'LunchAndDinner')) OR
                            (@role NOT IN ('Bar', 'Kitchen')))";
        }
        private string GetUpdateAllReadyItemsQuery() => @"UPDATE order_item SET status = @toStatus WHERE order_id = @orderId AND status = @fromStatus";
    }
}
