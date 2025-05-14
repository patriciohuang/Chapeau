using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuService
    {
        //fields and properties

        //constructors

        //methods
        List<MenuItem> GetAllCourses(MenuCard? menuCard);

        List<MenuItem> GetMenuItemsByCourse(string course);

    }
}
