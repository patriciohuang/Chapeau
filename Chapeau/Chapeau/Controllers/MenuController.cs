using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{

    //TODO: SHOULD I RENAME THIS CONTROLLER TO ORDERCONTROLLER?
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


        [HttpGet]
        public IActionResult Overview(int tableNr)
        {
            try
            {
                int? orderId = _orderService.CheckIfOrderExists(tableNr);

                //instead of an error message, should I just create a new order?
                if (!orderId.HasValue)
                {
                    throw new Exception("No order found for this table. Please add a menu item to it first.");
                }

                Order order = _orderService.GetOrderById((int)orderId);

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

                return RedirectToAction("Tables", "Waiter");
            }
        }

        //Should I do this with just the tableNr, it's an extra database query to get the orderId, but it makes it impossible to enter your own orderId in the form
        [HttpPost]
        public IActionResult SendOrder (int orderId, int tableNr)
        {
            try
            {
                _orderService.SendOrder(orderId);

                TempData["Success"] = "Order sent successfully!";

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

            }

            return RedirectToAction("Overview", "Menu", new { tableNr });
            }
            //
            [HttpGet]
        public IActionResult Card(string? card, int tableNr)
        {
            // Fill the menu card enum (this is used to filter by card) and get a list of all course categories in the current menu card, then send that to the view
            try
            {
                // Writing an if else statement as a ternary operator saves A LOT of space, but this is basically just an if else statement
                // If id is not null, parse it to a MenuCard enum, otherwise get the menu card by time through the service
                MenuCard menuCard = (card != null) ? (MenuCard)Enum.Parse(typeof(MenuCard), card) : _menuService.GetMenuCardByTime();

                List<CourseCategory> menuCourses = _menuService.GetAllCourses(menuCard);

                MenuCardCategory Menu = new MenuCardCategory(tableNr, menuCourses, menuCard);

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
        public IActionResult Course(int tableNr, string course, string card)
        {
            try
            {
                CourseCategory courseCategory = (CourseCategory)Enum.Parse(typeof(CourseCategory), course.ToString());
                MenuCard menuCard = (MenuCard)Enum.Parse(typeof(MenuCard), card.ToString());

                List<MenuItem> menuItems = _menuService.GetMenuItems(courseCategory, menuCard);

                MenuItemsAndTableNr menuItemsAndOrder = new MenuItemsAndTableNr(tableNr, menuCard, menuItems);

                return View(menuItemsAndOrder);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

                return RedirectToAction("Tables", "Waiter");
            }
        }

        [HttpPost]
        public IActionResult Course(int tableNr, string card, MenuItem menuItem)
        {
            try
            {
                int? orderId = _orderService.CheckIfOrderExists(tableNr);
                MenuCard menuCard = (MenuCard)Enum.Parse(typeof(MenuCard), card.ToString());

                //If the orderId is empty, that means there is no order for that table yet, this creates an order for the table AND saves the orderId
                if (!orderId.HasValue)
                {
                    Employee loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
                    
                    //Create a new order
                    orderId = _orderService.CreateOrder(tableNr, loggedInEmployee);
                }

                _orderService.AddItem((int)orderId, menuItem);

                TempData["Success"] = $"{menuItem.Name} added to order successfully!";
                
                return RedirectToAction("Course", new {tableNr, card = menuCard, course = menuItem.CourseCategory.ToString() });

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";

                return RedirectToAction("Tables", "Waiter");
            }
        }

    }
}
