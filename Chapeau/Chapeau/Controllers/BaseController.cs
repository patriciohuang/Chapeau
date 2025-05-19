using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Models;
using Chapeau.Services;

namespace Chapeau.Controllers
{
    public abstract class BaseController : Controller
    {
        protected Employee CurrentEmployee => HttpContext.Session.GetObject<Employee>("LoggedInEmployee");

        // This runs automatically for every action in controllers that inherit from BaseController
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Check if user is authenticated
            if (CurrentEmployee == null)
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }
        }

        // For explicit role checks in specific actions
        protected IActionResult CheckAccess(string requiredRole)
        {
            if (CurrentEmployee == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (requiredRole != null && !CurrentEmployee.Role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Unauthorized", "Auth");
            }

            return null;
        }

        // Overloaded method using enum for type safety
        protected IActionResult CheckAccess(UserRole requiredRole)
        {
            return CheckAccess(RoleNames.GetRoleName(requiredRole));
        }
    }
}