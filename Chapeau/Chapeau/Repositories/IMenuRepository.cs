using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Repositories
{
    public interface IMenuRepository
    {
        //fields and properties

        //constructors

        //methods
        List<CourseCategory> GetAllCourses(MenuCard menuCard);
        List<MenuItem> GetAllMenuItems(MenuCard menuCard);

        List<MenuItem> GetMenuItemsByCourse(CourseCategory courseCategory, MenuCard menuCard);

        void UpdateStock(int menuItemId);

    }
}
