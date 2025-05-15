using Microsoft.Data.SqlClient;
using Chapeau.Models;
using Chapeau.Services;
using System.Collections.Generic;

namespace Chapeau.Repositories
{
    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly string _connectionString;
        private readonly IPasswordHashingService _passwordHasher;
        
        public EmployeesRepository(IConfiguration configuration, IPasswordHashingService passwordHasher) // Dependency Injection
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
            _passwordHasher = passwordHasher;
        }
        
        public void Add(Employee employee) // Add method to add a new employee
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Hash the password before storing
                string hashedPassword = _passwordHasher.HashPassword(employee.Password);
                string query = "INSERT INTO employee (first_name, last_name, role, employee_nr, password) " +
                               "VALUES (@FirstName, @LastName, @Role, @EmployeeNumber, @Password); " +
                               "SELECT SCOPE_IDENTITY();";
                
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@Role", employee.Role);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employee.EmpNr);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                
                connection.Open();
                int employeeId = Convert.ToInt32(cmd.ExecuteScalar());
                // Note: We're retrieving the ID but not using it since the Employee object
                // already has an EmpNr that serves as the identifier in the application
            }
        }

        public void Delete(int employeeNr) // Delete method to remove an employee
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM employee WHERE employee_nr = @EmployeeNr";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EmployeeNr", employeeNr);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception("Employee not found.");
                }
            }
        }

        public Employee GetByEmployeeNr(int employeeNr)
        {
            Employee employee = null;
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT employee_id, first_name, last_name, role, employee_nr, password " +
                               "FROM employee WHERE employee_nr = @EmployeeNr";
                
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EmployeeNr", employeeNr);
                
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                
                if (reader.Read())
                {
                    employee = new Employee
                    {
                        EmpNr = reader.GetInt32(reader.GetOrdinal("employee_nr")),
                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                        LastName = reader.GetString(reader.GetOrdinal("last_name")),
                        Role = reader.GetString(reader.GetOrdinal("role")),
                        Password = reader.GetString(reader.GetOrdinal("password")) // This is the hashed password
                    };
                }
                
                reader.Close();
            }
            
            return employee;
        }
        
        public bool ValidateLogin(int employeeNr, string password)
        {
            Employee employee = GetByEmployeeNr(employeeNr);
            
            if (employee == null)
            {
                return false;
            }
            
            // Verify password
            return _passwordHasher.VerifyPassword(password, employee.Password);
        }
        
        public IEnumerable<Employee> GetAll()
        {
            List<Employee> employees = new List<Employee>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT employee_id, first_name, last_name, role, employee_nr FROM employee";
                
                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();
                
                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    Employee employee = new Employee
                    {
                        EmpNr = reader.GetInt32(reader.GetOrdinal("employee_nr")),
                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                        LastName = reader.GetString(reader.GetOrdinal("last_name")),
                        Role = reader.GetString(reader.GetOrdinal("role")),
                        // Note: We don't include the password here for security reasons
                        Password = string.Empty
                    };
                    
                    employees.Add(employee);
                }
                
                reader.Close();
            }
            
            return employees;
        }
    }
}