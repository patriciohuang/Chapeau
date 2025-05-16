using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPasswordHashingService _passwordHasher;

        public AccountController(
            IEmployeesRepository employeesRepository,
            IPasswordHashingService passwordHasher)
        {
            _employeesRepository = employeesRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if already logged in
            var loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            if (loggedInEmployee != null)
            {
                // Redirect based on role
                return RedirectBasedOnRole(loggedInEmployee.Role);
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(int employeeNr, string password)
        {
            var employee = _employeesRepository.GetByEmployeeNr(employeeNr);

            if (employee != null && _passwordHasher.VerifyPassword(password, employee.Password))
            {
                // Store the entire employee object in session
                HttpContext.Session.SetObject("LoggedInEmployee", employee);

                // Redirect based on role
                return RedirectBasedOnRole(employee.Role);
            }

            // Invalid login
            ViewBag.ErrorMessage = "Invalid employee number or password";
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Redirect to login page
            return RedirectToAction("Login");
        }

        public IActionResult Unauthorized()
        {
            return View();
        }

        private IActionResult RedirectBasedOnRole(string role)
        {
            switch (role.ToLower())
            {
                case "manager":
                    return RedirectToAction("Index", "Manager");
                case "waiter":
                    return RedirectToAction("Tables", "Waiter");
                case "kitchen":
                case "bar":
                    return RedirectToAction("Index", "KitchenAndBar");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
    }
}
