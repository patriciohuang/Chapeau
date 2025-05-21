using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Services
{
    public interface IMenuService
    {
        //fields and properties

        //constructors

        //methods
        List<MenuItem> GetAllCourses(MenuCard menuCard);

        MenuCard GetMenuCardByTime();

        List<MenuItem> GetMenuItems(MenuItem menuItem);

    }
}
