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
        private bool IsItemForCurrentRole(OrderItem orderItem)
        {
            // Assuming the issue is that 'MenuItems' is not a property of 'OrderItem',
            // we need to replace it with the correct property or collection.

            // If 'MenuItem' is the intended property, adjust the logic accordingly:
            if (CurrentEmployee.Role == RoleNames.Kitchen &&
                (orderItem.MenuItem.MenuCard == MenuCard.Lunch ||
                 orderItem.MenuItem.MenuCard == MenuCard.Dinner ||
                 orderItem.MenuItem.MenuCard == MenuCard.LunchAndDinner))
            {
                return true;
            }
            else if (CurrentEmployee.Role == RoleNames.Bar &&
                     orderItem.MenuItem.MenuCard == MenuCard.Drinks)
            {
                return true;
            }

            return false;
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
            List<Order> orders = _kitchenBarDisplaySevice.GetOrders(status);

            foreach (var order in orders)
            {
                order.OrderItems = order.OrderItems
                    .Where(item => IsItemForCurrentRole(item))
                    .ToList();
            }

            // Also remove orders with no matching items
            orders = orders.Where(o => o.OrderItems.Any()).ToList();

            ViewBag.Rol = CurrentEmployee.Role;

            // Pass the filtered orders to the view
            return View(orders);
        }
    }
}
