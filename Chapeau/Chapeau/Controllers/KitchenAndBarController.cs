using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Models;

namespace Chapeau.Controllers
{
    // Controller for Kitchen and Bar staff operations
    // Inherits from BaseController to get automatic authentication checking
    // Only users with "Kitchen" or "Bar" roles can access these actions
    public class KitchenAndBarController : BaseController
    {
        // This method runs before every action in this controller
        // It adds an additional role check specifically for Kitchen and Bar staff
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // First run the base authentication check (from BaseController)
            base.OnActionExecuting(context);

            // If already redirecting due to no authentication, don't continue
            if (context.Result != null) return;

            // Check if user has either Kitchen or Bar role
            // This controller serves both Kitchen and Bar staff since they have similar workflows
            if (CurrentEmployee == null ||
                (CurrentEmployee.Role != RoleNames.Kitchen &&
                 CurrentEmployee.Role != RoleNames.Bar))
            {
                // User doesn't have Kitchen or Bar role - redirect to unauthorized page
                context.Result = RedirectToAction("Unauthorized", "Auth");
            }
        }

        // GET: /KitchenAndBar/Index
        // Shows the dashboard for Kitchen or Bar staff with role-specific information
        public IActionResult Index()
        {
            try
            {
                // Set view data based on the employee's specific role
                ViewBag.Role = CurrentEmployee.Role;
                ViewBag.FullName = $"{CurrentEmployee.FirstName} {CurrentEmployee.LastName}";

                // Customize the dashboard content based on whether user is Kitchen or Bar staff
                if (CurrentEmployee.Role == RoleNames.Kitchen)
                {
                    // Kitchen staff see food-related information
                    ViewBag.OrderType = "Food";        // What type of orders they handle
                    ViewBag.PrepArea = "Kitchen";      // Where they work
                    // TODO: Load food orders from database when order system is implemented
                }
                else // Bar staff
                {
                    // Bar staff see drink-related information
                    ViewBag.OrderType = "Drink";       // What type of orders they handle
                    ViewBag.PrepArea = "Bar";          // Where they work
                    // TODO: Load drink orders from database when order system is implemented
                }

                return View();
            }
            catch (Exception ex)
            {
                // If error loading dashboard data, show error and set default values
                TempData["ErrorMessage"] = "Failed to load dashboard: " + ex.Message;

                // Set safe default values so the view doesn't break
                ViewBag.Role = CurrentEmployee?.Role ?? "Unknown";
                ViewBag.FullName = "Unknown User";
                ViewBag.OrderType = "Items";
                ViewBag.PrepArea = "Area";

                return View();
            }
        }

        // GET: /KitchenAndBar/Orders
        // Shows the list of orders that need to be prepared (food for Kitchen, drinks for Bar)
        public IActionResult Orders()
        {
            try
            {
                // Set view data for the orders page
                ViewBag.Role = CurrentEmployee.Role;
                ViewBag.FullName = $"{CurrentEmployee.FirstName} {CurrentEmployee.LastName}";

                // Filter orders based on staff role
                if (CurrentEmployee.Role == RoleNames.Kitchen)
                {
                    // Kitchen staff only see food orders
                    ViewBag.OrderType = "Food";
                    // TODO: Load pending food orders from database when order system is implemented
                }
                else // Bar staff
                {
                    // Bar staff only see drink orders
                    ViewBag.OrderType = "Drink";
                    // TODO: Load pending drink orders from database when order system is implemented
                }

                return View();
            }
            catch (Exception ex)
            {
                // If error loading orders, show error and set default values
                TempData["ErrorMessage"] = "Failed to load orders: " + ex.Message;

                // Set safe default values so the view doesn't break
                ViewBag.Role = CurrentEmployee?.Role ?? "Unknown";
                ViewBag.FullName = "Unknown User";
                ViewBag.OrderType = "Items";

                return View();
            }
        } 
    }
}