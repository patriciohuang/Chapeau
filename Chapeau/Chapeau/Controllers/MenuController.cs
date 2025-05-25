using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        // 
        [HttpGet]
        public IActionResult Index(string? id)
        {
            // Fill the menu card enum (this is used to filter by card) and get a list of all course categories in the current menu card, then send that to the view
            try
            {
                // Writing an if else statement as a ternary operator saves A LOT of space, but this is basically just an if else statement
                // If id is not null, parse it to a MenuCard enum, otherwise get the menu card by time through the service
                MenuCard menuCard = (id != null) ? (MenuCard)Enum.Parse(typeof(MenuCard), id) : _menuService.GetMenuCardByTime();

                MenuCardCategory menuCourses = _menuService.GetAllCourses(menuCard);

                return View(menuCourses);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

                return RedirectToAction("Tables", "Waiter");
            }
        }

        //change name to something referring the fact that you display things
        [HttpGet]
        public IActionResult Course(string course, string card)
        {
            try
            {
                CourseCategory courseCategory = (CourseCategory)Enum.Parse(typeof(CourseCategory), course.ToString());
                MenuCard menuCard = (MenuCard)Enum.Parse(typeof(MenuCard), card.ToString());

                List<MenuItem> menuItems = _menuService.GetMenuItems(courseCategory, menuCard);

                return View(menuItems);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

                return RedirectToAction("Index", "Menu");
            }
        }


    }
}
