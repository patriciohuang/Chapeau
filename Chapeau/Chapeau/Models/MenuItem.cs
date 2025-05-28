using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class MenuItem
    {
        //This class contains the items that are on the menu

        //fields and properties
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public MenuCard MenuCard { get; set; }
        public CourseCategory CourseCategory { get; set; }
        public int Stock { get; set; }
        public bool IsAlcoholic { get; set; }

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
