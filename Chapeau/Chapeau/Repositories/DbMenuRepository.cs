using Chapeau.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace Chapeau.Repositories
{
    public class DbMenuRepository : IMenuRepository
    {
        //fields and properties
        private readonly string? _connectionString;

        //constructors
        public DbMenuRepository(IConfiguration configuration)
        {
            //get (database) connectionstring from appsettings
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        //methods
        //This method returns a list of all courses in the menu, depending on the card selected
        public List<MenuItem> GetAllCourses(MenuCard menuCard)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT DISTINCT course_category FROM menu_item where [menu_card] IN (@menuCard,'Lunch/Dinner','Drinks');";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@menuCard", menuCard.ToString());
                
                command.Connection.Open();
                
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MenuItem menuItem = new MenuItem( (CourseCategory)Enum.Parse(typeof(CourseCategory), (string)reader["course_category"]) );
                    menuItems.Add(menuItem);
                }
                reader.Close();
            }
            return menuItems;
        }

        public List<MenuItem> GetMenuItemsByCourse(string course)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM menu_item WHERE course_category = @course";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@course", course);

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


        //
        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            int itemId = (int)reader["item_id"];
            string name = (string)reader["name"];
            decimal price = (decimal)reader["price"];
            MenuCard menuCard = (MenuCard)Enum.Parse(typeof(MenuCard), (string)reader["menu_card"]);
            CourseCategory courseCategory = (CourseCategory)Enum.Parse(typeof(CourseCategory), (string)reader["course_category"]);
            int stock = (int)reader["stock"];
            int vat = (int)reader["vat"];

            return new MenuItem(itemId, name, price, menuCard, courseCategory, stock, vat);
        }
    }
}
