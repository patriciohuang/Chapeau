using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class MenuItem
    {
        //fields and properties
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public MenuCard MenuCard { get; set; }
        public CourseCategory CourseCategory { get; set; }
        public int Stock { get; set; }
        public bool IsAlcoholic {get; set; }

        //constructors
        
        //empty constructor
        public MenuItem()
        {

        }

        public MenuItem(CourseCategory courseCategory, MenuCard menuCard)
        {
            CourseCategory = courseCategory;
            MenuCard = menuCard;
        }

        //constructor with all properties filled
        public MenuItem(int itemId, string name, decimal price, MenuCard menuCard, CourseCategory courseCategory, int stock, bool isAlcoholic)
        {
            this.ItemId = itemId;
            this.Name = name;
            this.Price = price;
            this.MenuCard = menuCard;
            this.CourseCategory = courseCategory;
            this.Stock = stock;
            this.IsAlcoholic = isAlcoholic;
        }

        //methods
    }
}
