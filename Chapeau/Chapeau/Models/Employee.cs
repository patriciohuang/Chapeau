using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Employee
    {
        public int EmployeeNr { get; set; } // Employee number, can be used for login or identification
        public string FirstName { get; set; } // First name of the employee
        public string LastName { get; set; } // Last name of the employee
        public string Role { get; set; } // Role of the employee (e.g., waiter, chef, manager)
        public string Password { get; set; } // Password for the employee, used for authentication

        public Employee()
        {
            EmployeeNr = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            Role = string.Empty;
            Password = string.Empty;
        }
        

        public Employee(int employeeNr, string firstName, string lastName, string role, string password)
        {
            EmployeeNr = employeeNr;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            Password = password;
        }
    }
}
