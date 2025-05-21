using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Models.Enums;

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
            try
            {
                // Check if user is a manager
                if (CurrentEmployee == null || !CurrentEmployee.Role.Equals(RoleNames.Manager, StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Unauthorized", "Auth");
                }

                // Manager is authenticated - either show manager view or redirect to Manager controller
                return View(); // Or: return RedirectToAction("Index", "Manager");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load page: " + ex.Message;
                return View();
            }
        }

        // For other actions that might be accessible to all roles
        public IActionResult Privacy()
        {
            try
            {
                // No role check here - any authenticated user can access
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load privacy page: " + ex.Message;
                return View();
            }
        }

        // Error page should be accessible to all
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                // Even error page failed - return basic error response
                return Content("An error occurred and the error page could not be loaded.");
            }
        }
    }
}