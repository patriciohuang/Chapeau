using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;

        public MenuController(IMenuService menuService, IOrderService orderService)
        {
            _menuService = menuService;
            _orderService = orderService;
        }

        // This method runs before every action in this controller
        // It adds an additional role check specifically for Waiters
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // First run the base authentication check (from BaseController)
            base.OnActionExecuting(context);

            // If already redirecting due to no authentication, don't continue
            if (context.Result != null) return;

            // Check if user has Waiter role
            if (CurrentEmployee == null || !CurrentEmployee.Role.Equals(RoleNames.Waiter, StringComparison.OrdinalIgnoreCase))
            {
                // User doesn't have Waiter role - redirect to unauthorized page
                context.Result = RedirectToAction("Unauthorized", "Auth");
            }
        }

        // 
        [HttpGet]
        public IActionResult Index(string? card, int orderId)
        {
            // Fill the menu card enum (this is used to filter by card) and get a list of all course categories in the current menu card, then send that to the view
            try
            {
                // Writing an if else statement as a ternary operator saves A LOT of space, but this is basically just an if else statement
                // If id is not null, parse it to a MenuCard enum, otherwise get the menu card by time through the service
                MenuCard menuCard = (card != null) ? (MenuCard)Enum.Parse(typeof(MenuCard), card) : _menuService.GetMenuCardByTime();

                List<CourseCategory> menuCourses = _menuService.GetAllCourses(menuCard);

                MenuCardCategory Menu = new MenuCardCategory(orderId, menuCourses, menuCard);

                return View(Menu);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

                return RedirectToAction("Tables", "Waiter");
            }
        }

        //change name to something referring the fact that you display things
        [HttpGet]
        public IActionResult Course(int orderId, string course, string card)
        {
            try
            {
                CourseCategory courseCategory = (CourseCategory)Enum.Parse(typeof(CourseCategory), course.ToString());
                MenuCard menuCard = (MenuCard)Enum.Parse(typeof(MenuCard), card.ToString());

                List<MenuItem> menuItems = _menuService.GetMenuItems(courseCategory, menuCard);

                MenuItemsAndOrder menuItemsAndOrder = new MenuItemsAndOrder(orderId, menuItems);    //TODO give this a better name

                return View(menuItemsAndOrder);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

                return RedirectToAction("Index", "Menu");
            }
        }

        [HttpPost]
        public IActionResult Course(int orderId, MenuItem menuItem)
        {
            try
            {
                _orderService.AddItem(orderId, menuItem);

                TempData["Success"] = $"{menuItem.Name} added to order successfully!";

                return RedirectToAction("Course", new { orderId, course = menuItem.CourseCategory, card = menuItem.MenuCard });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

                return RedirectToAction("Index", "Menu");
            }
        }

    }
}
