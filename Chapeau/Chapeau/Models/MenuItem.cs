namespace Chapeau.Models
{
    public class MenuItem
    {
        //fields and properties
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string MenuCard { get; set; }
        public string CourseCategory { get; set; }
        public int Stock { get; set; }
        public int Vat {get; set; }

        //constructors
        
        //empty constructor
        public MenuItem()
        {

        }

        public MenuItem(int itemId, string name, decimal price, string menuCard, string courseCategory, int stock, int vat)
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
