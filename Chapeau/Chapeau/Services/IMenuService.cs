using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuService
    {
        //fields and properties

        //constructors

        //methods
        List<MenuItem> GetAllCourses(MenuCard menuCard);

        MenuCard GetMenuCardByTime();

        List<MenuItem> GetMenuItemsByCourse(MenuItem menuItem);

    }
}
