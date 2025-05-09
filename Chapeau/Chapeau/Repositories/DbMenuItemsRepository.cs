using Chapeau.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace Chapeau.Repositories
{
    public class DbMenuItemsRepository : IMenuItemsRepository
    {
        //fields and properties
        private readonly string? _connectionString;

        //constructors
        public DbMenuItemsRepository(IConfiguration configuration)
        {
            //get (database) connectionstring from appsettings
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        //methods
        public List<MenuItem> GetAllMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM menu_item";
                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MenuItem menuItem = ReadMenuItem(reader);
                    menuItems.Add(menuItem);
                }
                reader.Close();
            }
            return menuItems;
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            int itemId = (int)reader["item_id"];
            string name = (string)reader["name"];
            decimal price = (decimal)reader["price"];
            string menuCard = (string)reader["menu_card"];
            string courseCategory = (string)reader["course_category"];
            int stock = (int)reader["stock"];
            int vat = (int)reader["vat"];

            return new MenuItem(itemId, name, price, menuCard, courseCategory, stock, vat);
        }
    }
}
