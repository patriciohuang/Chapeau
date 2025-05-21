using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{
    //pato
    public class KitchenBarDisplayController : BaseController
    {
        private readonly IKitchenBarDisplayService _kitchenBarDisplaySevice;

        public KitchenBarDisplayController(IKitchenBarDisplayService kitchenBarDisplayServices)
        {
            _kitchenBarDisplaySevice = kitchenBarDisplayServices;
        }

        // This method runs before every action in this controller
        // It adds an additional role check specifically for Kitchen and Bar staff
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // First run the base authentication check (from BaseController)
            base.OnActionExecuting(context);

            // If already redirecting due to no authentication, don't continue
            if (context.Result != null) return;

            // Check if user has either Kitchen or Bar role
            if (CurrentEmployee == null ||
                (CurrentEmployee.Role != RoleNames.Kitchen && CurrentEmployee.Role != RoleNames.Bar))
            {
                // User doesn't have Kitchen or Bar role - redirect to unauthorized page
                context.Result = RedirectToAction("Unauthorized", "Auth");
            }
        }

        public IActionResult Index(Status? status)
        {
            // Check access using the CheckAccess method for both roles
            var kitchenAccess = CheckAccess(UserRole.Kitchen);
            var barAccess = CheckAccess(UserRole.Bar);

            // If user doesn't have either Kitchen or Bar role, deny access
            // (User needs at least one of the roles, not both)
            if (kitchenAccess != null && barAccess != null)
            {
                return kitchenAccess; // Return the unauthorized redirect
            }

            // Get the orders from the service with optional status filter
            List<DisplayOrder> orders = _kitchenBarDisplaySevice.GetOrders(status);

            // Pass the filtered orders to the view
            return View(orders);
        }
    }
}
