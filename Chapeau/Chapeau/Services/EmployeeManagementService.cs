using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    // Service that handles all employee management business logic
    // Provides a layer between controllers and repositories for employee operations
    // Includes validation, error handling, and business rules for employee data
    public class EmployeeManagementService : IEmployeeManagementService
    {
        // Repository for accessing employee data from the database
        private readonly IEmployeesRepository _employeesRepository;

        // Constructor: Receives repository through dependency injection
        public EmployeeManagementService(IEmployeesRepository employeesRepository)
        {
            _employeesRepository = employeesRepository;
        }

        // Retrieves all employees from the system
        // Used by managers to view the employee list
        public IEnumerable<Employee> GetAllEmployees()
        {
            try
            {
                // Delegate to repository to get all employees from database
                return _employeesRepository.GetAll();
            }
            catch (Exception ex)
            {
                // Wrap any repository exceptions with more context
                // This helps with debugging and provides clearer error messages
                throw new Exception("Failed to retrieve employees", ex);
            }
        }

        // Creates a new employee in the system
        // Includes validation before saving to database
        public void CreateEmployee(Employee employee)
        {
            try
            {
                // First validate the employee data (business rules)
                ValidateEmployee(employee);

                // If validation passes, save to database via repository
                _employeesRepository.Add(employee);
            }
            catch (Exception ex)
            {
                // Wrap any exceptions with context about the operation
                throw new Exception("Failed to create employee", ex);
            }
        }

        // Deletes an employee from the system
        // Only managers can perform this operation
        public void DeleteEmployee(int employeeNr)
        {
            try
            {
                // Delegate to repository to remove employee from database
                _employeesRepository.Delete(employeeNr);
            }
            catch (Exception ex)
            {
                // Wrap any repository exceptions with more context
                throw new Exception("Failed to delete employee", ex);
            }
        }

        // Retrieves a specific employee by their employee number
        // Used for lookups and profile viewing
        public Employee? GetEmployeeByNumber(int employeeNr)
        {
            try
            {
                // Delegate to repository to find employee in database
                return _employeesRepository.GetByEmployeeNr(employeeNr);
            }
            catch (Exception ex)
            {
                // Wrap any repository exceptions with more context
                throw new Exception("Failed to retrieve employee", ex);
            }
        }

        // Private helper method: Validates employee data according to business rules
        // Ensures all required fields are present and valid before database operations
        private void ValidateEmployee(Employee employee)
        {
            // Check if employee object itself is null
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            // Validate first name is provided and not just whitespace
            if (string.IsNullOrWhiteSpace(employee.FirstName))
                throw new ArgumentException("First name is required");

            // Validate last name is provided and not just whitespace
            if (string.IsNullOrWhiteSpace(employee.LastName))
                throw new ArgumentException("Last name is required");

            // Validate role is provided and not just whitespace
            if (string.IsNullOrWhiteSpace(employee.Role))
                throw new ArgumentException("Role is required");

            // Validate role is one of the allowed roles from our constants
            // This prevents invalid roles from being saved to database
            if (!RoleNames.AllRoles.Contains(employee.Role))
                throw new ArgumentException($"Invalid role. Must be one of: {string.Join(", ", RoleNames.AllRoles)}");

            // Validate password is provided (it will be hashed before saving)
            if (string.IsNullOrWhiteSpace(employee.Password))
                throw new ArgumentException("Password is required");

            // Validate employee number is positive (business rule)
            if (employee.EmpNr <= 0)
                throw new ArgumentException("Employee number must be positive");
        }
    }
}