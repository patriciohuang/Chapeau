using Chapeau.Models;

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
        public int Vat {get; set; }

        //constructors
        
        //empty constructor
        public MenuItem()
        {

        }

        public MenuItem(CourseCategory courseCategory)
        {
            CourseCategory = courseCategory;
        }

        //constructor with all properties filled
        public MenuItem(int itemId, string name, decimal price, MenuCard menuCard, CourseCategory courseCategory, int stock, int vat)
        {
            this.ItemId = itemId;
            this.Name = name;
            this.Price = price;
            this.MenuCard = menuCard;
            this.CourseCategory = courseCategory;
            this.Stock = stock;
            this.Vat = vat;
        }

        //methods
    }
}
