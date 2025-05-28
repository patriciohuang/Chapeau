using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    // Service that handles all authentication-related business logic
    // Separates authentication logic from controllers (following separation of concerns)
    // This service is responsible for login validation and role-based redirections
    public class AuthenticationService : IAuthenticationService
    {
        // Repository for accessing employee data from the database
        private readonly IEmployeesRepository _employeesRepository;

        // Service for password hashing and verification
        private readonly IPasswordHashingService _passwordHasher;

        // Constructor: Receives dependencies through dependency injection
        // This pattern makes the service testable and loosely coupled
        public AuthenticationService(
            IEmployeesRepository employeesRepository,
            IPasswordHashingService passwordHasher)
        {
            _employeesRepository = employeesRepository;
            _passwordHasher = passwordHasher;
        }

        // Attempts to authenticate a user with their credentials
        // Returns the employee object if successful, null if authentication fails
        public Employee? Login(int employeeNr, string password)
        {
            try
            {
                // First, try to find the employee by their employee number
                var employee = _employeesRepository.GetByEmployeeNr(employeeNr);

                // Check if employee exists AND password is correct
                // The password stored in database is hashed, so we use the password hasher to verify
                if (employee != null && _passwordHasher.VerifyPassword(password, employee.Password))
                {
                    // Authentication successful - return the employee object
                    return employee;
                }

                // Authentication failed - either employee doesn't exist or password is wrong
                // We don't distinguish between these cases for security reasons
                return null;
            }
            catch (Exception)
            {
                // If any error occurs during authentication (database error, etc.)
                // Return null to indicate authentication failure
                return null;
            }
        }

        // Checks if an employee has access to a specific role
        // Used for authorization - determining if someone can access certain features
        public bool HasAccess(Employee employee, string requiredRole)
        {
            // If no employee provided, access is denied
            if (employee == null) return false;

            // Compare the employee's role with the required role
            // Case-insensitive comparison for flexibility
            return employee.Role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase);
        }

        // Determines where to redirect a user based on their role
        // Each role has a specific landing page after login
        // Returns a tuple containing controller name and action name
        public (string controller, string action) GetRoleBasedRedirect(string role)
        {
            // Use switch expression to map roles to their respective pages
            return role switch
            {
                // Managers go to the Manager dashboard
                RoleNames.Manager => ("Manager", "Index"),

                // Waiters go directly to the tables view (their main work area)
                RoleNames.Waiter => ("Waiter", "Tables"),

                // Both Kitchen and Bar staff go to the same controller
                // The controller will differentiate between them internally
                RoleNames.Kitchen or RoleNames.Bar => ("KitchenBarDisplay", "Index"),

                // Default case: if role doesn't match any known role, go to login page
                // This should rarely happen due to val
                // ation elsewhere
                _ => ("Auth", "Login")
            };
        }
    }
}