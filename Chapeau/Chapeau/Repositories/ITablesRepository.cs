using Chapeau.Models;
using System.Collections.Generic;

namespace Chapeau.Repositories
{
    public interface ITablesRepository
    {
        IEnumerable<Table> GetAllTables();
        Table GetTableByNumber(int tableNr);
        void UpdateTableAvailability(int tableNr, bool available);
    }
}