using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    //pato
    public class OrderItem
    {
        //This class contains items that have been ordered from the menu (it contains a menu item class)

        public int Count { get; set; } // Number of items ordered
        public string Comment { get; set; } = string.Empty; // Comment for the order item
        public Status Status { get; set; } = Status.Preparing; // Default status is 'Preparing'

        public MenuItem MenuItem { get; set; } = new MenuItem(); // Initialize to avoid null


        //Payment stuff

        public decimal TotalCost 
        { 
            get
            {
                return Count * MenuItem.Price;
            }
        }

        public decimal TotalHighVAT { get; set; }

        public decimal TotalLowVAT { get; set; }

        public OrderItem(int count, string comment, Status status, MenuItem menuItem)
        {
            Count = count;
            Comment = comment;
            Status = status;
            MenuItem = menuItem;
        }

        public OrderItem() { }
    }
}
