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

        public List<MenuItem> GetMenuItems(MenuItem menuItem)
        {
            //A ternary that checks if CourseCategory equals All, if it is, it gets all menu items, if it isn't, it gets the menu items of the selected course
            return (menuItem.CourseCategory == CourseCategory.All) 
                ? _menuRepository.GetAllMenuItems(menuItem) 
                : _menuRepository.GetMenuItemsByCourse(menuItem);
        }
    }
}
