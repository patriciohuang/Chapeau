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
        public List<MenuItem> GetAllCourses(MenuCard menuCard)
        {
            return _menuRepository.GetAllCourses((MenuCard)menuCard);
        }

        public MenuCard GetMenuCardByTime()
        {
            MenuCard menuCard = new MenuCard();

            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
            if (currentTime.Hour < 16)
            {
                menuCard = MenuCard.Lunch;
            }
            else
            {
                menuCard = MenuCard.Dinner;
            }

            return menuCard;
        }

        public List<MenuItem> GetMenuItemsByCourse(MenuItem menuItem)
        {
            return _menuRepository.GetMenuItemsByCourse(menuItem);
        }
    }
}
