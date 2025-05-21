using Chapeau.Models;
using Chapeau.Models.Enums;
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
        //This method returns a list of all courses in a specific menu, depending on the card selected
        public List<MenuItem> GetAllCourses(MenuCard menuCard)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT DISTINCT course_category, menu_card FROM menu_item where [menu_card] LIKE @menuCard ORDER BY course_category DESC;";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@menuCard", menuCard.ToString());
                
                command.Connection.Open();
                
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //This monstrosity is needed to parse from the database into two enums (course_category and menu_card), then makes a menuItem object out of it
                    MenuItem menuItem = new MenuItem( (CourseCategory)Enum.Parse(typeof(CourseCategory), (string)reader["course_category"]), (MenuCard)Enum.Parse(typeof(MenuCard), (string)reader["menu_card"]) );
                    menuItems.Add(menuItem);
                }
                reader.Close();
            }
            return menuItems;
        }

        //This method returns a list of ALL MenuItems belonging to a specific menu card
        public List<MenuItem> GetAllMenuItems(MenuItem menuItem)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM menu_item WHERE menu_card LIKE @menuCard ORDER BY course_category DESC;";
                SqlCommand command = new SqlCommand(query, connection);

                //The parameter is like this (having two %'s) because of the database column being poorly normalized, 
                //HOWEVER, I ASKED DAN AND HE SAID IT WOULD NOT COST ME ANY POINTS. SO LIKE THIS IT STAYS, TOO BAD!
                command.Parameters.AddWithValue("@menuCard", $"%{menuItem.MenuCard.ToString()}%");

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MenuItem newMenuItem = ReadMenuItem(reader);
                    menuItems.Add(newMenuItem);
                }
                reader.Close();
            }
            return menuItems;
        }

        //This method returns a list of MenuItems belonging to a specific course/type of drink belonging to a specific menu card
        public List<MenuItem> GetMenuItemsByCourse(MenuItem menuItem)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM menu_item WHERE course_category LIKE @course AND menu_card LIKE @menuCard ORDER BY name ASC;";
                SqlCommand command = new SqlCommand(query, connection);

                //The parameter is like this (having two %'s) because of the database column being poorly normalized, 
                //HOWEVER, I ASKED DAN AND HE SAID IT WOULD NOT COST ME ANY POINTS. SO LIKE THIS IT STAYS, TOO BAD!
                command.Parameters.AddWithValue("@course", menuItem.CourseCategory.ToString());
                command.Parameters.AddWithValue("@menuCard", $"%{menuItem.MenuCard.ToString()}%");

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MenuItem newMenuItem = ReadMenuItem(reader);
                    menuItems.Add(newMenuItem);
                }
                reader.Close();
            }
            return menuItems;
        }



        //Here are reusable private methods

        //This method reads the data received by the reader and parses it into a MenuItem object
        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            int itemId = (int)reader["menu_item_id"];
            string name = (string)reader["name"];
            decimal price = (decimal)reader["price"];
            MenuCard menuCard = (MenuCard)Enum.Parse(typeof(MenuCard), (string)reader["menu_card"]);
            CourseCategory courseCategory = (CourseCategory)Enum.Parse(typeof(CourseCategory), (string)reader["course_category"]);
            int stock = (int)reader["stock"];
            bool isAlcoholic = (bool)reader["isAlcoholic"];

            return new MenuItem(itemId, name, price, menuCard, courseCategory, stock, isAlcoholic);
        }

    }
}
