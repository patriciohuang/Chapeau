using Microsoft.Data.SqlClient;
using Chapeau.Models;
using Chapeau.Services;
using System.Collections.Generic;

namespace Chapeau.Repositories
{
    // Repository for accessing employee data from the database
    // Implements IEmployeesRepository interface for dependency injection
    // Handles all database operations related to employees (CRUD operations)
    public class EmployeesRepository : IEmployeesRepository
    {
        // Database connection string retrieved from configuration
        // Used to establish connections to the SQL Server database
        private readonly string _connectionString;

        // Password hashing service for securing passwords before storing them
        private readonly IPasswordHashingService _passwordHasher;

        // Constructor: Receives configuration and password service through dependency injection
        // Configuration provides access to appsettings.json for connection string
        public EmployeesRepository(IConfiguration configuration, IPasswordHashingService passwordHasher)
        {
            // Get the connection string from appsettings.json
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
            _passwordHasher = passwordHasher;
        }

        // Adds a new employee to the database
        // Hashes the password before storing for security
        public void Add(Employee employee)
        {
            // Use using statement to ensure proper disposal of database connection
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Hash the password before storing it in database (security best practice)
                string hashedPassword = _passwordHasher.HashPassword(employee.Password);

                // SQL query to insert new employee record
                // Uses parameters to prevent SQL injection attacks
                string query = "INSERT INTO employee (first_name, last_name, role, employee_nr, password) " +
                               "VALUES (@FirstName, @LastName, @Role, @EmployeeNumber, @Password); ";

                // Create SQL command with parameterized query
                SqlCommand cmd = new SqlCommand(query, connection);

                // Add parameters to prevent SQL injection
                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@Role", employee.Role);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employee.EmployeeNr);
                cmd.Parameters.AddWithValue("@Password", hashedPassword); // Store hashed password

                // Open connection and execute the query
                connection.Open();

                // Execute query
                cmd.ExecuteNonQuery();

            }
        }

        // Deletes an employee from the database by their employee number
        // Only managers should be able to call this method
        public void Delete(int employeeNr)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to delete employee by employee number
                string query = "DELETE FROM employee WHERE employee_nr = @EmployeeNr";

                SqlCommand cmd = new SqlCommand(query, connection);
                // Use parameter to prevent SQL injection
                cmd.Parameters.AddWithValue("@EmployeeNr", employeeNr);

                connection.Open();

                // Execute the delete command and get number of affected rows
                int rowsAffected = cmd.ExecuteNonQuery();

                // Check if any rows were actually deleted
                if (rowsAffected == 0)
                {
                    // No employee found with this number - throw exception
                    throw new Exception("Employee not found.");
                }
            }
        }

        // Retrieves a specific employee by their employee number
        // Used for login authentication and employee lookups
        public Employee GetByEmployeeNr(int employeeNr)
        {
            Employee employee = null; // Initialize as null (employee not found)

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to find employee by employee number
                // Selects all necessary fields including the hashed password for authentication
                string query = "SELECT first_name, last_name, role, employee_nr, password " +
                               "FROM employee WHERE employee_nr = @EmployeeNr";

                SqlCommand cmd = new SqlCommand(query, connection);
                // Use parameter to prevent SQL injection
                cmd.Parameters.AddWithValue("@EmployeeNr", employeeNr);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // Check if a record was found
                if (reader.Read())
                {
                    // Create Employee object from database record
                    employee = new Employee
                    {
                        EmployeeNr = reader.GetInt32(reader.GetOrdinal("employee_nr")),
                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                        LastName = reader.GetString(reader.GetOrdinal("last_name")),
                        Role = reader.GetString(reader.GetOrdinal("role")),
                        Password = reader.GetString(reader.GetOrdinal("password")) // This is the hashed password
                    };
                }

                // Always close the reader when done
                reader.Close();
            }

            // Return the employee object (or null if not found)
            return employee;
        }

        // Validates login credentials for an employee
        // Alternative method to GetByEmployeeNr + password verification
        // Returns true if credentials are valid, false otherwise
        public bool ValidateLogin(int employeeNr, string password)
        {
            // First, get the employee record
            Employee employee = GetByEmployeeNr(employeeNr);

            // If employee doesn't exist, login fails
            if (employee == null)
            {
                return false;
            }

            // Verify the provided password against the stored hash
            return _passwordHasher.VerifyPassword(password, employee.Password);
        }

        // Retrieves all employees from the database
        // Used by managers to view the complete employee list
        // Excludes passwords for security reasons
        public IEnumerable<Employee> GetAll()
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all employees (excluding passwords for security)
                string query = "SELECT first_name, last_name, role, employee_nr FROM employee";

                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                // Read each employee record from the database
                while (reader.Read())
                {
                    Employee employee = new Employee
                    {
                        EmployeeNr = reader.GetInt32(reader.GetOrdinal("employee_nr")),
                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                        LastName = reader.GetString(reader.GetOrdinal("last_name")),
                        Role = reader.GetString(reader.GetOrdinal("role")),
                        // Security: We don't include the password here for security reasons
                        Password = string.Empty
                    };

                    employees.Add(employee);
                }

                reader.Close();
            }

            return employees;
        }

        public int GetEmployeeId(int employeeNr)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get the employee ID by employee number
                string query = "SELECT employee_id FROM employee WHERE employee_nr = @EmployeeNr";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EmployeeNr", employeeNr);

                connection.Open();

                // Execute the command and return the employee number
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}