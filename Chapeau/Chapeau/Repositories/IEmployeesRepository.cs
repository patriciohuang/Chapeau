namespace Chapeau.Repositories
{
    // Interface defining the contract for employee data access operations
    // Provides abstraction between the service layer and actual database implementation
    // This allows for easy testing (mock implementations) and flexibility to change databases
    public interface IEmployeesRepository
    {
        // Adds a new employee to the data store
        // Used when managers create new employee accounts
        // The password should be hashed before calling this method
        void Add(Models.Employee employee);

        // Retrieves an employee by their unique employee number
        // Used for login authentication and employee lookups
        // Returns null if no employee is found with the given number
        Models.Employee GetByEmployeeNr(int employeeNr);

        // Validates login credentials for an employee
        // Alternative to GetByEmployeeNr + manual password verification
        // Returns true if credentials are valid, false otherwise
        bool ValidateLogin(int employeeNr, string password);

        // Retrieves all employees in the system
        // Used by managers to view the employee list
        // Typically excludes sensitive information like passwords
        IEnumerable<Models.Employee> GetAll();

        // Removes an employee from the system
        // Used when managers need to delete employee accounts
        // Should handle cases where employee doesn't exist
        void Delete(int employeeNr);

        int GetEmployeeId(int employeeNr);
    }
}