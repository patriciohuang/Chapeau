using Chapeau.Models.Enums;

namespace Chapeau.ViewModels
{
    public class MenuCardCategory
    {
        // Fields and properties
        public int OrderId { get; set; }
        public MenuCard MenuCard { get; set; }
        public List<CourseCategory> CourseCategory { get; set; }


        //constructors
        public MenuCardCategory(int orderId, List<CourseCategory> courseCategory, MenuCard menuCard)
        {
            OrderId = orderId;
            CourseCategory = courseCategory;
            MenuCard = menuCard;
        }

        //methods
    }
}
