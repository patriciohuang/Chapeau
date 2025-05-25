using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    //pato
    public class OrderItem
    {
        public int OrderItemId { get; set; } // Unique identifier for the order item
        public int MenuItemId { get; set; } // Foreign key to the menu item
        public int OrderId { get; set; } // Foreign key to the order this item belongs to
        public int Count { get; set; } // Number of items ordered
        public string Comment { get; set; } = string.Empty; // Comment for the order item
        public Status Status { get; set; } = Status.Preparing; // Default status is 'Preparing'

        public MenuItem MenuItem { get; set; } = new MenuItem(); // Initialize to avoid null

        public OrderItem(int orderItemId, int menuItemId, int orderId, int count, string comment, Status status, MenuItem menuItem)
        {
            OrderItemId = orderItemId;
            MenuItemId = menuItemId;
            OrderId = orderId;
            Count = count;
            Comment = comment;
            Status = status;
            MenuItem = menuItem;
        }

        public OrderItem() { }
    }
}
