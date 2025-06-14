using Microsoft.Data.SqlClient;
using Chapeau.Models;
using Chapeau.Models.Enums;

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
            int count = (int)reader["count"];
            string comment = reader["comment"] as string ?? string.Empty;
            Status itemStatus = Enum.Parse<Status>((string)reader["item_status"], true);

            MenuItem menuItem = ReadMenuItem(reader);

            return new OrderItem(count, comment, itemStatus, menuItem);
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


        public List<Order> GetOrders(Status? status)
        {
            // Create a dictionary to store orders with their order_id as the key
            Dictionary<int, Order> orders = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all orders for today (optionally filtered by status)
                string sql = @"SELECT   o.order_id, o.date_ordered, o.time_ordered, o.is_paid,
                                        i.count, i.comment, i.status AS item_status,
                                        e.employee_id, e.employee_nr, e.first_name, e.last_name, e.role, e.password,
                                        t.table_id, t.table_nr, t.availability,
                                        m.menu_item_id, m.name, m.price, m.menu_card, m.course_category, m.stock, m.isAlcoholic
                            FROM [order] o
                            JOIN order_item i ON o.order_id = i.order_id
                            JOIN menu_item m ON i.menu_item_id = m.menu_item_id
                            JOIN employee e ON o.employee_id = e.employee_id
                            JOIN [table] t ON t.table_id = o.table_id
                            WHERE CAST(o.date_ordered AS DATE) = CAST(GETDATE() AS DATE)
                            ORDER BY o.time_ordered";

                //CAST(o.date_ordered AS DATE) = CAST(GETDATE() AS DATE) AND            IN THE WHERE

                SqlCommand command = new SqlCommand(sql, connection);/*
                command.Parameters.AddWithValue("@status", $"%{status.ToString()}%");
*/
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int orderId = (int)reader["order_id"];
                    // check if the order already exists in the dictionary
                    if (!orders.ContainsKey(orderId))
                    {
                        var order = ReadOrder(reader);
                        // add the order to the dictionary
                        orders.Add(orderId, order);
                    }

                    AddOrderItemToOrder(orders[orderId], ReadOrderItem(reader));
                }

                reader.Close();
            }
            List<Order> result = orders.Values.ToList();

            if (status.HasValue)
            {
                // After creating the order, the status is calculated based on the items in the order, so now we filter the orders by status
                result = result.Where(o => o.Status == status.Value).ToList();
            }

            return result;
        }

        public Order GetOrderById(int orderId)
        {
            Order? order = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all orders for today (optionally filtered by status)
                string sql = @"SELECT   o.order_id, o.date_ordered, o.time_ordered, o.is_paid,
                                        i.count, i.comment, i.status AS item_status,
                                        e.employee_id, e.employee_nr, e.first_name, e.last_name, e.role, e.password,
                                        t.table_nr, t.table_id, t.availability,
                                        m.menu_item_id, m.name, m.price, m.menu_card, m.course_category, m.stock, m.isAlcoholic
                            FROM [order] o
                            JOIN order_item i ON o.order_id = i.order_id
                            JOIN menu_item m ON i.menu_item_id = m.menu_item_id
                            JOIN employee e ON o.employee_id = e.employee_id
                            JOIN [table] t ON t.table_id = o.table_id
                            WHERE o.order_id = @orderId";

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
            }
            return order;
        }


        // Checks if an order exists for a given table number
        public int? CheckIfOrderExists(int tableNr)
        {
            int? orderId;

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

                orderId = command.ExecuteScalar() as int?;
                return orderId; // Returns the highest order ID for the table if the table is occupied.
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

        public bool UpdateOrderStatus(int orderId, Status status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE [order_item]
                     SET status = @Status
                     WHERE order_id = @OrderId";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@Status", status.ToString());

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }


        public bool UpdateOrderItemStatus(int orderId, int orderItemId, Status status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to update the status of an order
                string sql = "UPDATE [order_item] SET status = @status WHERE order_id = @orderId AND menu_item_id = @orderItemId";
                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@status", status.ToString());
                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@orderItemId", orderItemId);
                connection.Open();
                int affected = command.ExecuteNonQuery();
                return affected > 0;
            }
        }

        //THESE ARE ALL EMPTY AND NOT IMPLEMENTED. REMOVE LATER, WE ALREADY HAVE METHODS THAT DO THIS
        public List<Order> GetAllOrders()
        {
            return GetOrders(null);
        }

        public void AddOrder(Order order)
        {
            throw new NotImplementedException("AddOrder method not implemented yet");
        }

        public void DeleteOrder(int orderId)
        {
            throw new NotImplementedException("DeleteOrder method not implemented yet");
        }

    }
}
