using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Models;
using Chapeau.Repositories;
using System.Collections.Generic;
using Chapeau.Models.Enums;
using Chapeau.Services;

namespace Chapeau.Controllers
{
    // Controller for Waiter-specific operations
    // Inherits from BaseController to get automatic authentication checking
    // Only users with "Waiter" role can access these actions
    public class WaiterController : BaseController
    {
        // Repository that handles table data from the database
        private readonly ITablesRepository _tablesRepository;
        // Service that handles order operations
        private readonly IOrderService _orderService;

        // Constructor: Receives tables repository through dependency injection
        public WaiterController(ITablesRepository tablesRepository, IOrderService orderService)
        {
            _tablesRepository = tablesRepository;
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

        // GET: /Waiter/Index
        // Shows the waiter dashboard with table statistics and navigation options
        // -- This is completely irrelevant now
        // Since we literally got rid of Waiter/Index and replaced it with Waiter/Tables -- I wanna take it out just was too lazy
        // Lol
        public IActionResult Index()
        {

            try
            {
                // Get all tables to calculate statistics
                var tables = _tablesRepository.GetAllTables();

                // Count free tables (available = true)
                ViewBag.FreeTablesCount = tables.Count(t => t.Available);

                // Count occupied tables (available = false)
                ViewBag.OccupiedTablesCount = tables.Count(t => !t.Available);

                // TODO: These will be implemented when order system is added
                ViewBag.PendingOrdersCount = 0;   // Placeholder for pending orders
                ViewBag.CompletedOrdersCount = 0; // Placeholder for completed orders

                return View();
            }
            catch (Exception ex)
            {
                // If error loading dashboard data, show error and set default values
                TempData["ErrorMessage"] = "Failed to load dashboard: " + ex.Message;

                // Set default values so the view doesn't break
                ViewBag.FreeTablesCount = 0;
                ViewBag.OccupiedTablesCount = 0;
                ViewBag.PendingOrdersCount = 0;
                ViewBag.CompletedOrdersCount = 0;

                return View();
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
                IEnumerable<Table> tables = _tablesRepository.GetAllTables();

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

        // POST: /Waiter/UpdateTableAvailability
        // Updates the availability status of a specific table
        // This doesn't work yet, since the update table availability method in the repository is not implemented :3
        [HttpPost]
        public IActionResult UpdateTableAvailability(int tableNr, bool available)
        {
            try
            {
                // Update the table availability in the database
                _tablesRepository.UpdateTableAvailability(tableNr, available);

                // Success - set success message
                TempData["SuccessMessage"] = $"Table {tableNr} updated successfully";
            }
            catch (Exception ex)
            {
                // If error updating table, set error message
                TempData["ErrorMessage"] = "Failed to update table: " + ex.Message;
            }

            // Always redirect back to tables view (whether success or error)
            return RedirectToAction("Tables");
        }

        // GET: /Waiter/Orders
        // Shows all orders for the waiter to manage
        // Jeroen's note: Psycho code, don't use Viewbag
        public IActionResult Orders(Status? status = null, int? tableNr = null)
        {
            try
            {
                List<Order> orders;

                // Filter by table if specified
                if (tableNr.HasValue)
                {
                    orders = _orderService.GetOrdersByTable(tableNr.Value);
                    ViewBag.FilteredByTable = tableNr.Value;
                }
                // Filter by status if specified
                else if (status.HasValue)
                {
                    orders = _orderService.GetOrdersByStatus(status.Value);
                    ViewBag.FilteredByStatus = status.Value;
                }
                // Otherwise get all today's orders
                else
                {
                    orders = _orderService.GetTodaysOrders();
                }

                // Sort orders by time (most recent first)
                orders = orders.OrderByDescending(o => o.Time_ordered).ToList();

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load orders: " + ex.Message;
                return View(new List<Order>());
            }
        }
    }
}