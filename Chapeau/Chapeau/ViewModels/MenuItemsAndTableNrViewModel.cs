using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.ViewModels
{
    public class MenuItemsAndTableNrViewModel
    {
        // Fields and properties
        public int TableNr { get; set; }
        public MenuCard MenuCard { get; set; }
        public List<MenuItem> MenuItems { get; set; }


        //constructors
        public MenuItemsAndTableNrViewModel(int tableNr, MenuCard menuCard, List<MenuItem> menuItems)
        {
            TableNr = tableNr;
            MenuCard = menuCard;
            MenuItems = menuItems;
        }

        //methods
    }
}
