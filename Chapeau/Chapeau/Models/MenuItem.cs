using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class MenuItem
    {
        //fields and properties
        public int MenuItemId { get; set; } // Unique identifier for the menu item
        public string Name { get; set; } // Name of the menu item
        public decimal Price { get; set; } // Price of the menu item
        public MenuCard MenuCard { get; set; } // Menu card to which the item belongs (e.g., Food, Drinks)
        public CourseCategory CourseCategory { get; set; } // Category of the course (e.g., Appetizer, Main Course, Dessert, Drink)
        public int Stock { get; set; } // Stock quantity of the menu item, used to track availability
        public bool IsAlcoholic { get; set; } // Indicates if the menu item is alcoholic (true) or non-alcoholic (false)

        //constructors

        //empty constructor
        public MenuItem()
        {

        }

        //constructor with all properties filled
        public MenuItem(int menuItemId, string name, decimal price, MenuCard menuCard, CourseCategory courseCategory, int stock, bool isAlcoholic)
        {
            MenuItemId = menuItemId;
            Name = name;
            Price = price;
            MenuCard = menuCard;
            CourseCategory = courseCategory;
            Stock = stock;
            IsAlcoholic = isAlcoholic;
        }

        //methods
    }
}
