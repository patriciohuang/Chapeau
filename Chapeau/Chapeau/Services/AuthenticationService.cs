using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPasswordHashingService _passwordHasher;

        public AuthenticationService(
            IEmployeesRepository employeesRepository,
            IPasswordHashingService passwordHasher)
        {
            _employeesRepository = employeesRepository;
            _passwordHasher = passwordHasher;
        }

        public Employee? Login(int employeeNr, string password)
        {
            try
            {
                var employee = _employeesRepository.GetByEmployeeNr(employeeNr);

                if (employee != null && _passwordHasher.VerifyPassword(password, employee.Password))
                {
                    return employee;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool HasAccess(Employee employee, string requiredRole)
        {
            if (employee == null) return false;

            return employee.Role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase);
        }

        public (string controller, string action) GetRoleBasedRedirect(string role)
        {
            return role switch
            {
                RoleNames.Manager => ("Manager", "Index"),
                RoleNames.Waiter => ("Waiter", "Tables"),
                RoleNames.Kitchen or RoleNames.Bar => ("KitchenAndBar", "Index"),
                _ => ("Home", "Index")
            };
        }
    }
}