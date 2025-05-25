using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.Models;
using Chapeau.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        // This method runs before every action in this controller. It adds an additional role check specifically for Waiters
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // First run the base authentication check (from BaseController)
            base.OnActionExecuting(context);

            // If already redirecting due to no authentication, don't continue
            if (context.Result != null) return;

            // Check if user has the Waiter role, if they don't: redirect them to unauthorized page
            if (CurrentEmployee == null || !CurrentEmployee.Role.Equals(RoleNames.Waiter, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = RedirectToAction("Unauthorized", "Auth");
            }
        }

        //This displays a list of all courses/types of drinks in a specific menu card
        [HttpGet]
        public IActionResult Index(string? id)
        {
            try
            {
                // Double-check if the user has Waiter role access
                var authResult = CheckAccess(UserRole.Waiter);
                if (authResult != null) return authResult;

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

        //This displays a list of items belonging to a specific course/type of drink in a specific menu card
        [HttpGet]
        public IActionResult Course(string course, string card)
        {
            try
            {
                // Double-check if the user has Waiter role access
                var authResult = CheckAccess(UserRole.Waiter);
                if (authResult != null) return authResult;

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
