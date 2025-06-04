using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuItemsAndOrder
    {
        // Fields and properties
        public int OrderId { get; set; }
        public List<MenuItem> MenuItems { get; set; }


        //constructors
        public MenuItemsAndOrder(int orderId, List<MenuItem> menuItems)
        {
            OrderId = orderId;
            MenuItems = menuItems;
        }

        //methods
    }
}
