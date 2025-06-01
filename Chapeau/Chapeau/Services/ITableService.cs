using Chapeau.Models;

namespace Chapeau.Services
{
    public interface ITableService
    {
        IEnumerable<Table> GetAllTables();

        Table GetTableByNumber(int tableNr);

        void UpdateTableAvailability(int tableNr, bool available);
    }
}
