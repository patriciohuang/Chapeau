using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Services
{
    public interface IMenuService
    {
        //fields and properties

        //constructors

        //methods
        MenuCardCategory GetAllCourses(MenuCard menuCard);

        MenuCard GetMenuCardByTime();

        List<MenuItem> GetMenuItems(CourseCategory courseCategory, MenuCard menuCard);

    }
}
