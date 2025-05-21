using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Repositories
{
    public interface IMenuRepository
    {
        //fields and properties

        //constructors

        //methods
        List<MenuItem> GetAllCourses(MenuCard menuCard);
        List<MenuItem> GetAllMenuItems(MenuItem menuItem);

        List<MenuItem> GetMenuItemsByCourse(MenuItem menuItem);

    }
}
