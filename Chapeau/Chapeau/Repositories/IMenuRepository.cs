using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IMenuRepository
    {
        //fields and properties

        //constructors

        //methods
        List<MenuItem> GetAllCourses(MenuCard menuCard);

        List<MenuItem> GetMenuItemsByCourse(string course);
    }
}
