using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IAuthenticationService
    {
        // Tries to login with employee number and password
        // Returns the employee if successful, null if failed
        Employee? Login(int employeeNr, string password);

        // Checks if employee has the required role
        bool HasAccess(Employee employee, string requiredRole);

        // Gets where to redirect based on employee role
        (string controller, string action) GetRoleBasedRedirect(string role);
    }
}