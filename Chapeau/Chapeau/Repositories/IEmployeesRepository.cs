namespace Chapeau.Repositories
{
    public interface IEmployeesRepository
    {
        void Add(Models.Employee employee);
        Models.Employee GetByEmployeeNr(int employeeNr);
        bool ValidateLogin(int employeeNr, string password);
        IEnumerable<Models.Employee> GetAll();
        void Delete(int employeeNr);
    }
}