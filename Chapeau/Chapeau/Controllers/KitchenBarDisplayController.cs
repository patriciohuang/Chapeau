using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chapeau.Controllers
{
    //pato
    public class KitchenBarDisplayController : BaseController
    {
        private readonly IKitchenBarDisplayService _kitchenBarDisplaySevice;
        private readonly IHubContext<OrderHub> _hubContext;

        public KitchenBarDisplayController(IKitchenBarDisplayService kitchenBarDisplayServices, IHubContext<OrderHub> hubContext)
        {
            _kitchenBarDisplaySevice = kitchenBarDisplayServices;
            _hubContext = hubContext;
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

        private List<Status> ParseStatuses(string status)
        {
            List<Status> result = new List<Status>();

            foreach (string s in status.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                // TryParse will return false if the string cannot be parsed to a Status enum
                // out Status parsed: This is where the resulting parsed enum will be stored if parsing succeeds. So I can use it later to add to the list.
                if (Enum.TryParse(s.Trim(), true, out Status parsed))
                {
                    result.Add(parsed);
                }
            }

            return result;
        }
        private List<Order> GetOrdersByStatus(string status)
        {
            List<Status> statusList = ParseStatuses(status);
            List<Order> orders = _kitchenBarDisplaySevice.GetOrdersByStatus(statusList);
            FilterOrdersByRole(orders);

            ViewBag.Role = CurrentEmployee.Role;
            return orders;
        }

        public void FilterOrdersByRole(List<Order> orders)
        {
            // Filter the orders based on the current role
            foreach (Order order in orders)
            {
                order.OrderItems = order.OrderItems
                    .Where(item => IsItemForCurrentRole(item))
                    .ToList();
            }
            // Remove orders with no matching items
            orders.RemoveAll(o => !o.OrderItems.Any());
        }


        public IActionResult Index(string status)
        {
            // If the status parameter is null or empty, default to "Preparing,Ordered" so the first page shows orders that are either preparing or ordered
            if (string.IsNullOrEmpty(status))
            {
                return RedirectToAction("Index", new { status = "Preparing,Ordered" });
            }

            List<Order> orders = GetOrdersByStatus(status);
            // Pass the filtered orders to the view
            return View(orders);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, Status currentStatus)
        {
            bool result = _kitchenBarDisplaySevice.UpdateOrderStatus(orderId, currentStatus);
            if (result)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveOrders");
                return Ok();
            }
            else
            {
                return Problem();
            }
        }
        public IActionResult OrdersPartial(string status)
        {
            List<Order> orders = GetOrdersByStatus(status);
            return PartialView("_OrdersPartial", orders); // Make sure this partial view exists
        }
    }
}
