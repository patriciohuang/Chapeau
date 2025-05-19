using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly IEmployeesRepository _employeesRepository;

        public EmployeeManagementService(IEmployeesRepository employeesRepository)
        {
            _employeesRepository = employeesRepository;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            try
            {
                return _employeesRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve employees", ex);
            }
        }

        public void CreateEmployee(Employee employee)
        {
            try
            {
                ValidateEmployee(employee);
                _employeesRepository.Add(employee);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create employee", ex);
            }
        }

        public void DeleteEmployee(int employeeNr)
        {
            try
            {
                _employeesRepository.Delete(employeeNr);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete employee", ex);
            }
        }

        public Employee? GetEmployeeByNumber(int employeeNr)
        {
            try
            {
                return _employeesRepository.GetByEmployeeNr(employeeNr);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve employee", ex);
            }
        }

        private void ValidateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrWhiteSpace(employee.FirstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrWhiteSpace(employee.LastName))
                throw new ArgumentException("Last name is required");

            if (string.IsNullOrWhiteSpace(employee.Role))
                throw new ArgumentException("Role is required");

            // Validate role is one of the allowed roles
            if (!RoleNames.AllRoles.Contains(employee.Role))
                throw new ArgumentException($"Invalid role. Must be one of: {string.Join(", ", RoleNames.AllRoles)}");

            if (string.IsNullOrWhiteSpace(employee.Password))
                throw new ArgumentException("Password is required");

            if (employee.EmpNr <= 0)
                throw new ArgumentException("Employee number must be positive");
        }
    }
}