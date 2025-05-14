using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Models;
using Chapeau.Repositories;
using System.Collections.Generic;

namespace Chapeau.Controllers
{
    public class WaiterController : BaseController
    {
        private readonly ITablesRepository _tablesRepository;

        public WaiterController(ITablesRepository tablesRepository)
        {
            _tablesRepository = tablesRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // If already redirecting, don't continue
            if (context.Result != null) return;

            // Check if user has Waiter role
            if (CurrentEmployee == null || !CurrentEmployee.Role.Equals("Waiter", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = RedirectToAction("Unauthorized", "Account");
            }
        }

        public IActionResult Index()
        {
            // Check access
            var authResult = CheckAccess("Waiter");
            if (authResult != null)
            {
                return authResult;
            }

            // Get table statistics
            var tables = _tablesRepository.GetAllTables();
            ViewBag.FreeTablesCount = tables.Count(t => t.Available);
            ViewBag.OccupiedTablesCount = tables.Count(t => !t.Available);

            // Placeholder values for order statistics
            ViewBag.PendingOrdersCount = 0;
            ViewBag.CompletedOrdersCount = 0;

            return View();
        }

        public IActionResult Tables()
        {
            // Get all tables from repository
            IEnumerable<Table> tables = _tablesRepository.GetAllTables();

            return View(tables);
        }

        [HttpPost]
        public IActionResult UpdateTableAvailability(int tableNr, bool available)
        {
            // To be done
            return View("Tables");
        }
    }
}