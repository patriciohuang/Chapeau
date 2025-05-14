namespace Chapeau.Models
{
    public class Employee
    {
        public int EmpNr { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }

        public Employee()
        {
            EmpNr = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            Role = string.Empty;
            Password = string.Empty;
        }
        

        public Employee(int empNr, string firstName, string lastName, string role, string password)
        {
            EmpNr = empNr;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            Password = password;
        }
    }
}
