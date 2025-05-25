using Chapeau.Models;
using Chapeau.Models.Enums;
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
        public MenuCardCategory GetAllCourses(MenuCard menuCard)
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

        public List<MenuItem> GetMenuItems(CourseCategory courseCategory, MenuCard menuCard)
        {
            //A ternary that checks if CourseCategory equals All, if it is, it gets all menu items, if it isn't, it gets the menu items of the selected course
            return (courseCategory == CourseCategory.All) 
                ? _menuRepository.GetAllMenuItems(menuCard) 
                : _menuRepository.GetMenuItemsByCourse(courseCategory, menuCard);
        }
    }
}
