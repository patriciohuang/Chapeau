using Microsoft.Data.SqlClient;
using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Repositories
{
    //pato
    public class DbKitchenBarDisplayRepository : IKitchenBarDisplayRepository
    {
        private readonly string? _connectionString;

        public DbKitchenBarDisplayRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("chapeaudatabase");
        }
        /// Read order from the database
        private DisplayOrder ReadOrder(SqlDataReader reader)
        {
            int orderId = (int)reader["order_id"];
            int tableNr = (int)reader["table_nr"];
            string employee_name = (string)reader["first_name"];
            Status status = Enum.Parse<Status>((string)reader["status"], true);
            DateOnly dateOrdered = DateOnly.FromDateTime((DateTime)reader["date_ordered"]);
            TimeOnly timeOrdered = TimeOnly.FromTimeSpan((TimeSpan)reader["time_ordered"]);

            return new DisplayOrder(orderId, tableNr, employee_name, status, dateOrdered, timeOrdered);
        }
        /// Read order item from the database
        private DisplayOrderItem ReadOrderItem(SqlDataReader reader)
        {
            string itemName = (string)reader["item_name"];
            MenuCard menuCard= Enum.Parse<MenuCard>((string)reader["menu_card"], true);
            int count = (int)reader["count"];
            CourseCategory courseCategory = Enum.Parse<CourseCategory>((string)reader["course_category"], true);

            return new DisplayOrderItem(itemName, menuCard, count, courseCategory);
        }

        public List<DisplayOrder> GetOrders(Status? status)
        {
            // Create a dictionary to store orders with their order_id as the key
            Dictionary<int, DisplayOrder> orders = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all orders with status 'Preparing'
                string sql = @"
                    SELECT o.order_id, t.table_nr, e.first_name, o.status, o.date_ordered, o.time_ordered,
                           i.count, m.menu_card, m.name AS item_name, m.course_category
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
                    var menu_item = ReadOrderItem(reader);
                }  
            }
            // return the list of orders
            return orders.Values.ToList();
        }
    }
}
