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
            int orderId = (int)reader["order_id"];
            int tableId = (int)reader["table_id"];
            int employeeId = (int)reader["employee_id"];
            Status status = Enum.Parse<Status>((string)reader["order_status"], true);
            DateOnly dateOrdered = DateOnly.FromDateTime((DateTime)reader["date_ordered"]);
            TimeOnly timeOrdered = TimeOnly.FromTimeSpan((TimeSpan)reader["time_ordered"]);
            int employeeNr = (int)reader["employee_nr"];
            string firstName = (string)reader["first_name"];
            string lastName = (string)reader["last_name"];
            string role = (string)reader["role"];
            string password = (string)reader["password"];
            int tableNr = (int)reader["table_nr"];


            // Read employee
            Employee employee = new Employee(employeeNr, firstName, lastName, role, password);

            // Read table
            Table table = new Table(tableId, tableNr);

            return new Order(orderId, status, dateOrdered, timeOrdered, table, employee);
        }
        /// Read order item from the database
        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            int orderItemId = (int)reader["order_item_id"];
            int menuItemId = (int)reader["menu_item_id"];
            int orderId = (int)reader["order_id"];
            int count = (int)reader["count"];
            string comment = reader["comment"] as string ?? string.Empty;
            Status itemStatus = Enum.Parse<Status>((string)reader["item_status"], true);

            // Read menu item
            string name = (string)reader["name"];
            decimal price = (decimal)reader["price"];
            MenuCard menuCard = Enum.Parse<MenuCard>((string)reader["menu_card"], true);
            CourseCategory courseCategory = Enum.Parse<CourseCategory>((string)reader["course_category"], true);
            int stock = (int)reader["stock"];
            bool isAlcoholic = (bool)reader["isAlcoholic"];
            MenuItem menuItem = new MenuItem(menuItemId, name, price, menuCard, courseCategory, stock, isAlcoholic);

            return new OrderItem(orderItemId, menuItemId, orderId, count, comment, itemStatus, menuItem);
        }


        public List<Order> GetOrders(Status? status)
        {
            // Create a dictionary to store orders with their order_id as the key
            Dictionary<int, Order> orders = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all orders for today (optionally filtered by status)
                string sql = @"SELECT   o.order_id, o.table_id, o.employee_id, o.status AS order_status, o.date_ordered, o.time_ordered,
                                        i.order_item_id, i.menu_item_id, i.count, i.comment, i.status AS item_status,
                                        e.employee_nr, e.first_name, e.last_name, e.role, e.password,
                                        t.table_nr, t.table_id,
                                        m.name, m.price, m.menu_card, m.course_category, m.stock, m.isAlcoholic
                            FROM [order] o
                            JOIN order_item i ON o.order_id = i.order_id
                            JOIN menu_item m ON i.menu_item_id = m.menu_item_id
                            JOIN employee e ON o.employee_id = e.employee_id
                            JOIN [table] t ON t.table_id = o.table_id
                            WHERE CAST(o.date_ordered AS DATE) = CAST(GETDATE() AS DATE)";

                // Add a WHERE clause if status is provided
                if (status != null)
                {
                    sql += " AND o.status = @status";
                }

                SqlCommand command = new SqlCommand(sql, connection);
                // Add the status parameter if provided
                if (status != null)
                {
                    command.Parameters.AddWithValue("@status", status.ToString());
                }

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

                    var order_item = ReadOrderItem(reader);
                    // add the item to the existing order
                    orders[orderId].OrderItems.Add(order_item);
                }  
            }
            // return the list of orders
            return orders.Values.ToList();
        }

        // TODO, get a method that returns a (filled) order object when an orderId parameter is entered
        public Order GetOrder(int orderId)
        {
            Order order = new Order();

            return order;
        }




    }
}
