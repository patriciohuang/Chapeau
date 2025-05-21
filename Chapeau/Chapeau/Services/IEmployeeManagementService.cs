using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IEmployeeManagementService
    {
        // Gets all employees from the system
        IEnumerable<Employee> GetAllEmployees();

        // Creates a new employee
        void CreateEmployee(Employee employee);

        // Deletes an employee by their number
        void DeleteEmployee(int employeeNr);

        // Gets employee by their number
        Employee? GetEmployeeByNumber(int employeeNr);
    }
}