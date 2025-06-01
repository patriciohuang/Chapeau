using Chapeau.Repositories;
using Chapeau.Models;

namespace Chapeau.Services
{
    public class TableService : ITableService
    {
        private readonly ITablesRepository _tablesRepository;

        public TableService(ITablesRepository tablesRepository)
        {
            _tablesRepository = tablesRepository;
        }

        public IEnumerable<Table> GetAllTables()
        {
            return _tablesRepository.GetAllTables();

        }

        public Table GetTableByNumber(int tableNr)
        {
            return _tablesRepository.GetTableByNumber(tableNr);
        }

        public void UpdateTableAvailability(int tableNr, bool available)
        {
            _tablesRepository.UpdateTableAvailability(tableNr, available);
        }

    }
}
