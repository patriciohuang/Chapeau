using Chapeau.Models;
using System.Collections.Generic;

namespace Chapeau.Repositories
{
    // Interface defining the contract for table data access operations  
    // Provides abstraction for restaurant table management functionality
    // Allows for different implementations (database, file, memory, etc.)
    public interface ITablesRepository
    {
        // Retrieves all tables in the restaurant with their current status
        // Used by waiters to see the complete restaurant layout
        // Returns tables with availability and position information
        IEnumerable<Table> GetAllTables();

        // Retrieves a specific table by its table number
        // Used for individual table operations and status checks
        // Returns null if table with given number doesn't exist
        Table GetTableByNumber(int tableNr);

        // Updates the availability status of a specific table
        // Used when waiters seat customers or clear tables
        // Parameters: tableNr - which table to update, available - new status
        void UpdateTableAvailability(int tableNr, bool available);

        int GetTableId(int tableNr);
    }
}