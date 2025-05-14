using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class MenuService : IMenuService
    {
        //fields and properties
        private readonly IMenuRepository _menuRepository;

        //constructors
        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        //methods
        public List<MenuItem> GetAllCourses(MenuCard? menuCard)
        {
            if (menuCard == null)
            {
                TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
                if (currentTime.Hour < 16)
                {
                    menuCard = MenuCard.Lunch;
                }
                else
                {
                    menuCard = MenuCard.Dinner;
                }   
            }

            return _menuRepository.GetAllCourses((MenuCard)menuCard);
        }

        public List<MenuItem> GetMenuItemsByCourse(string course)
        {
            return _menuRepository.GetMenuItemsByCourse(course);
        }
    }
}
