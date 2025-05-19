using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    // Handles all authentication-related operations for the restaurant system
    // Including login, logout, and access control redirections
    public class AuthController : Controller
    {
        // Dependency injection: Service that handles authentication logic
        private readonly IAuthenticationService _authenticationService;

        // Constructor: Receives authentication service through dependency injection
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        // GET: /Auth/Login
        // Displays the login page or redirects if user is already logged in
        [HttpGet]
        public IActionResult Login()
        {
            // Check if a user is already logged in by looking for employee data in session
            var loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");

            // If user is already authenticated, redirect them to their role-specific page
            if (loggedInEmployee != null)
            {
                return RedirectBasedOnRole(loggedInEmployee.Role);
            }

            // No user logged in, show the login form
            return View();
        }

        // POST: /Auth/Login
        // Processes login form submission and authenticates the user
        [HttpPost]
        public IActionResult Login(int employeeNr, string password)
        {
            try
            {
                // Attempt to authenticate user using the authentication service
                var employee = _authenticationService.Login(employeeNr, password);

                // If authentication successful, employee object is returned
                if (employee != null)
                {
                    // Store the authenticated employee in the session for future requests
                    HttpContext.Session.SetObject("LoggedInEmployee", employee);

                    // Redirect user to their role-specific landing page
                    return RedirectBasedOnRole(employee.Role);
                }

                // Authentication failed - set error message for the view to display
                ViewBag.ErrorMessage = "Invalid employee number or password";
                return View();
            }
            catch (Exception)
            {
                // Handle any unexpected errors during login process
                ViewBag.ErrorMessage = "An error occurred during login. Please try again.";
                return View();
            }
        }

        // POST: /Auth/Logout
        // Logs out the current user by clearing their session
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear all session data to log the user out
            HttpContext.Session.Clear();

            // Redirect back to login page
            return RedirectToAction("Login");
        }

        // GET: /Auth/Unauthorized
        // Displays error page when user tries to access a page they don't have permission for
        public IActionResult Unauthorized()
        {
            return View();
        }

        // Helper method: Redirects user to appropriate page based on their role
        // Uses the authentication service to determine the correct destination
        private IActionResult RedirectBasedOnRole(string role)
        {
            // Get the appropriate controller and action for this role from the service
            var (controller, action) = _authenticationService.GetRoleBasedRedirect(role);

            // Redirect to the determined controller and action
            return RedirectToAction(action, controller);
        }
    }
}