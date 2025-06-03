using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.ViewModels
{
    public class MenuItemsAndTableNr
    {
        // Fields and properties
        public int TableNr { get; set; }
        public MenuCard MenuCard { get; set; }
        public List<MenuItem> MenuItems { get; set; }


        //constructors
        public MenuItemsAndTableNr(int tableNr, MenuCard menuCard, List<MenuItem> menuItems)
        {
            TableNr = tableNr;
            MenuCard = menuCard;
            MenuItems = menuItems;
        }

        //methods
    }
}
