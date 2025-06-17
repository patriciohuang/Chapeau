using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Models;
using Chapeau.Repositories;
using System.Collections.Generic;
using Chapeau.Models.Enums;
using Chapeau.Services;
using Chapeau.ViewModels;

namespace Chapeau.Controllers
{
    // Controller for Waiter-specific operations
    // Inherits from BaseController to get automatic authentication checking
    // Only users with "Waiter" role can access these actions
    public class WaiterController : BaseController
    {
        // Repository that handles table data from the database
        private readonly ITableService _tableService;
        // Service that handles order operations
        private readonly IOrderService _orderService;

        // Constructor: Receives tables repository through dependency injection
        public WaiterController(ITableService tableService, IOrderService orderService)
        {
            _tableService = tableService;
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

            // Check if user has the Waiter role specifically
            if (CurrentEmployee == null || !CurrentEmployee.Role.Equals(RoleNames.Waiter, StringComparison.OrdinalIgnoreCase))
            {
                // User doesn't have Waiter role - redirect to unauthorized page
                context.Result = RedirectToAction("Unauthorized", "Auth");
            }
        }

        // GET: /Waiter/Tables
        // Shows visual layout of all restaurant tables with their current status
        // The main page for waiters rn, and it will stay like that
        public IActionResult Tables()
        {
            try
            {
                // Get all tables from the database with their current availability status
                var tables = _tableService.GetAllTables();

                // Pass tables to view for visual display
                return View(tables);
            }
            catch (Exception ex)
            {
                // If error loading tables, show error and empty list
                TempData["ErrorMessage"] = "Failed to load tables: " + ex.Message;
                return View(new List<Table>());
            }
        }

        // Table Actions thing
        public IActionResult TableActions(int tableNr)
        {
            try
            {
                // Get the current table status from the database
                // This ensures we always have the most up-to-date information
                var table = _tableService.GetTableByNumber(tableNr);

                if (table == null)
                {
                    TempData["ErrorMessage"] = $"Table {tableNr} not found.";
                    return RedirectToAction("Tables");
                }

                var viewModel = CreateTableActionsViewModel(tableNr, table.Available); // Create view model for table actions
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load table information: " + ex.Message;
                return RedirectToAction("Tables");
            }
        }

        // GET: /Waiter/Orders
        // Shows all orders for the waiter to manage
        public IActionResult Orders(string filter = "active", int? tableNr = null)
        {
            try
            {
                var orders = GetFilteredOrders(filter, tableNr);
                SetOrderViewData(filter, tableNr, orders);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load orders: " + ex.Message;
                return View(new List<Order>());
            }
        }
        private List<Order> GetFilteredOrders(string filter, int? tableNr)
        {
            // Get base orders
            // This works for some mysterious reason
            var orders = tableNr.HasValue
                ? _orderService.GetOrdersByTable(tableNr.Value)
                : _orderService.GetTodaysOrders();

            // Apply filter and sort
            var filteredOrders = filter.ToLower() switch
            {
                "active" => orders.Where(o => o.Status != Status.Completed && o.Status != Status.Cancelled),
                "ready" => orders.Where(o => o.OrderItems.Any(item => item.Status == Status.Ready)),
                _ => orders
            };

            return filteredOrders.OrderByDescending(o => o.Time_ordered).ToList();
        }

        [HttpPost]
        public IActionResult UpdateTableAvailability(int tableNr, bool available)
        {
            try
            {
                // Validates that table can be changed
                ValidateTableStatusChange(tableNr, available);
                _tableService.UpdateTableAvailability(tableNr, available);
                TempData["SuccessMessage"] = $"Table {tableNr} updated successfully to {(available ? "available" : "unavailable")}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update table status: " + ex.Message;
            }

            return RedirectToAction("Tables");
        }

        // Technically, TECHNICALLY right, you could just mark all orderitems as served, but just in case, i added this
        [HttpPost]
        public IActionResult MarkOrderAsServed(int orderId)
        {
            try
            {
                _orderService.MarkOrderAsServed(orderId);
                TempData["SuccessMessage"] = "Order marked as served successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update order status: " + ex.Message;
            }

            return RedirectToAction("Orders");
        }

        // use this. click the checkmark
        [HttpPost]
        public IActionResult MarkOrderItemAsServed(int orderId, int orderItemId)
        {
            try
            {
                _orderService.MarkOrderItemAsServed(orderId, orderItemId);
                TempData["SuccessMessage"] = "Order item marked as served successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update order item status: " + ex.Message;
            }

            return RedirectToAction("Orders");
        }

        // well we need to get data somewhere, right?
        private void SetOrderViewData(string filter, int? tableNr, List<Order> orders)
        {
            ViewBag.CurrentFilter = filter;
            ViewBag.FilteredByTable = tableNr;
            ViewBag.AvailableTableNumbers = GetAvailableTableNumbers(orders);
        }


        private void ValidateTableStatusChange(int tableNr, bool available)
        {
            if (available && HasActiveOrders(tableNr))
            {
                throw new InvalidOperationException(
                    $"Cannot mark Table {tableNr} as available. Active orders must be completed first.");
            }
        }

        private bool HasActiveOrders(int tableNr)
        {
            var activeOrders = _orderService.GetActiveOrdersByTable(tableNr);
            return activeOrders.Any();
        }

        private List<int> GetAvailableTableNumbers(List<Order> orders)
        {
            return orders.Select(o => o.Table.TableNr).Distinct().OrderBy(t => t).ToList();
        }

        // thought i would dip my toes in the view of models
        private TableActionsViewModel CreateTableActionsViewModel(int tableNr, bool available)
        {
            return new TableActionsViewModel(tableNr, available);
        }
    }
}