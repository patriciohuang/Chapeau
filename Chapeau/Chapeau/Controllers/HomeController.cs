using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{
    public class HomeController : BaseController
    {
        // This will check general authentication
        public override void OnActionExecuting(ActionExecutingContext context) 
        {
            base.OnActionExecuting(context); // Call the base method to check authentication

            // If already redirecting, don't continue
            if (context.Result != null) return;
        }

        // Index action is manager-only
        public IActionResult Index()
        {
            // Check if user is a manager
            if (CurrentEmployee == null || !CurrentEmployee.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            // Manager is authenticated - either show manager view or redirect to Manager controller
            return View(); // Or: return RedirectToAction("Index", "Manager");
        }

        // For other actions that might be accessible to all roles
        public IActionResult Privacy()
        {
            // No role check here - any authenticated user can access
            return View();
        }

        // Error page should be accessible to all
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}