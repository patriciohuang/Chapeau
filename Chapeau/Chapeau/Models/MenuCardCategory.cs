using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class MenuCardCategory
    {
        // Fields and properties
        public MenuCard MenuCard { get; set; }
        public List<CourseCategory> CourseCategory { get; set; }


        //constructors
        public MenuCardCategory(List<CourseCategory> courseCategory, MenuCard menuCard)
        {
            CourseCategory = courseCategory;
            MenuCard = menuCard;
        }

        //methods
    }
}
