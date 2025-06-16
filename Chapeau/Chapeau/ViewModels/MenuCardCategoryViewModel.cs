using Chapeau.Models.Enums;

namespace Chapeau.ViewModels
{
    public class MenuCardCategoryViewModel
    {
        // Fields and properties
        public int TableNr { get; set; }
        public MenuCard MenuCard { get; set; }
        public List<CourseCategory> CourseCategory { get; set; }


        //constructors
        public MenuCardCategoryViewModel(int tableNr, List<CourseCategory> courseCategory, MenuCard menuCard)
        {
            TableNr = tableNr;
            CourseCategory = courseCategory;
            MenuCard = menuCard;
        }

        //methods
    }
}
