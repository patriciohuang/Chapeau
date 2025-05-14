using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Models;

namespace Chapeau.Controllers
{
    public class KitchenAndBarController : BaseController
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // If already redirecting, don't continue
            if (context.Result != null) return;

            // Check if user has Kitchen or Bar role
            if (CurrentEmployee == null ||
                (CurrentEmployee.Role.ToLower() != "kitchen" &&
                 CurrentEmployee.Role.ToLower() != "bar"))
            {
                context.Result = RedirectToAction("Unauthorized", "Account");
            }
        }

        public IActionResult Index()
        {
            // Set view data based on role
            ViewBag.Role = CurrentEmployee.Role;
            ViewBag.FullName = $"{CurrentEmployee.FirstName} {CurrentEmployee.LastName}";

            // You could load different data based on role
            if (CurrentEmployee.Role.ToLower() == "kitchen")
            {
                ViewBag.OrderType = "Food";
                ViewBag.PrepArea = "Kitchen";
                // Load food orders...
            }
            else // "bar"
            {
                ViewBag.OrderType = "Drink";
                ViewBag.PrepArea = "Bar";
                // Load drink orders...
            }

            return View();
        }

        public IActionResult Orders()
        {
            ViewBag.Role = CurrentEmployee.Role;
            ViewBag.FullName = $"{CurrentEmployee.FirstName} {CurrentEmployee.LastName}";

            // Load orders based on role
            if (CurrentEmployee.Role.ToLower() == "kitchen")
            {
                // Load food orders
                ViewBag.OrderType = "Food";
            }
            else // "bar"
            {
                // Load drink orders
                ViewBag.OrderType = "Drink";
            }

            return View();
        }

        public IActionResult CompleteOrder(int orderId)
        {
            // Mark order as complete
            // ...

            return RedirectToAction("Orders");
        }
    }
}