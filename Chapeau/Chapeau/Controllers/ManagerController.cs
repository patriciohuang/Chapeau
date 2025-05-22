using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{
    // Controller for Manager-specific operations
    // Inherits from BaseController to get automatic authentication checking
    // Only users with "Manager" role can access these actions
    public class ManagerController : BaseController
    {
        // Service that handles employee management business logic
        private readonly IEmployeeManagementService _employeeService;

        // Constructor: Receives employee service through dependency injection
        public ManagerController(IEmployeeManagementService employeeService)
        {
            _employeeService = employeeService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // First run the base authentication check
            base.OnActionExecuting(context);

            // If already redirecting due to no authentication, don't continue
            if (context.Result != null) return;

            // Check if user has Manager role
            if (CurrentEmployee == null || !CurrentEmployee.Role.Equals(RoleNames.Manager, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = RedirectToAction("Unauthorized", "Auth");
            }
        }

        // GET: /Manager/Index
        // Shows list of all employees in the system (Manager dashboard)
        public IActionResult Index()
        {
            try
            {
                // Get all employees from the service layer
                var employees = _employeeService.GetAllEmployees();

                // Pass the employee list to the view for display
                return View(employees);
            }
            catch (Exception ex)
            {
                // If error loading employees, show error message and empty list
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Employee>());
            }
        }

        // GET: /Manager/Create
        // Shows the form for creating a new employee
        [HttpGet]
        public IActionResult Create()
        {
            // Show empty form for creating new employee
            return View();
        }

        // POST: /Manager/Create
        // Processes the submitted form to create a new employee
        [HttpPost]
        public IActionResult Create(Employee employee)
        {

            // Check if the submitted form data is valid (required fields, etc.)
            if (!ModelState.IsValid)
            {
                // If validation failed, show the form again with error messages
                return View(employee);
            }

            try
            {
                // Use service to create the employee (includes validation and database save)
                _employeeService.CreateEmployee(employee);

                // Success - set success message and redirect to employee list
                TempData["SuccessMessage"] = "Employee added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // If error creating employee, show error message and return to form
                ModelState.AddModelError("", ex.Message);
                return View(employee);
            }
        }

        // POST: /Manager/Delete
        // Deletes an employee from the system
        [HttpPost]
        public IActionResult Delete(int employeeNr)
        {
            try
            {
                // Use service to delete the employee from database
                _employeeService.DeleteEmployee(employeeNr);

                // Success - set success message
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            catch (Exception ex)
            {
                // If error deleting employee, set error message
                TempData["ErrorMessage"] = ex.Message;
            }

            // Always redirect back to employee list (whether success or error)
            return RedirectToAction("Index");
        }
    }
}