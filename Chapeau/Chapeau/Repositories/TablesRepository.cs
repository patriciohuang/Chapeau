using Chapeau.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Chapeau.Repositories
{
    // Repository for accessing table data from the database
    // Implements ITablesRepository interface for dependency injection
    // Handles all database operations related to restaurant tables
    public class TablesRepository : ITablesRepository
    {
        // Database connection string retrieved from configuration
        // Used to establish connections to the SQL Server database
        private readonly string _connectionString;

        // Constructor: Receives configuration through dependency injection
        // Gets connection string from appsettings.json
        public TablesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        // Retrieves all tables from the database with their current availability status
        // Used by waiters to see the restaurant layout and table statuses
        public IEnumerable<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all tables ordered by table number
                // This ensures consistent display order in the UI
                string query = "SELECT table_nr, availability FROM [table] ORDER BY table_nr";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Read each table record from the database
                    while (reader.Read())
                    {
                        // Create Table object from database record
                        Table table = new Table
                        {
                            // Get table number from database
                            TableNr = reader.GetInt32(reader.GetOrdinal("table_nr")),

                            // Get availability status (true = available, false = occupied)
                            Available = reader.GetBoolean(reader.GetOrdinal("availability"))
                        };

                        // Calculate grid position for UI display
                        // This determines where the table appears on the visual layout
                        CalculateGridPosition(table);

                        tables.Add(table);
                    }
                }
            }

            return tables;
        }

        // Retrieves a specific table by its table number
        // Used for individual table operations
        public Table GetTableByNumber(int tableNr)
        {
            Table table = null; // Initialize as null (table not found)

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to find specific table by number
                string query = "SELECT table_nr, availability FROM [table] WHERE table_nr = @TableNr";

                SqlCommand command = new SqlCommand(query, connection);
                // Use parameter to prevent SQL injection
                command.Parameters.AddWithValue("@TableNr", tableNr);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Check if table was found
                    if (reader.Read())
                    {
                        // Create Table object from database record
                        table = new Table
                        {
                            TableNr = reader.GetInt32(reader.GetOrdinal("table_nr")),
                            Available = reader.GetBoolean(reader.GetOrdinal("availability"))
                        };

                        // Calculate grid position for UI display
                        CalculateGridPosition(table);
                    }
                }
            }

            return table; // Returns null if table not found
        }

        // Updates the availability status of a specific table
        // Called when waiters mark tables as occupied or available
        public void UpdateTableAvailability(int tableNr, bool available)
        {
            // TODO: This method needs to be implemented
            // It should execute an UPDATE SQL statement to change the availability
            // Example implementation:
            /*
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [table] SET availability = @Available WHERE table_nr = @TableNr";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Available", available);
                command.Parameters.AddWithValue("@TableNr", tableNr);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
            */
        }

        // Private helper method: Calculates the grid position for tables in the UI
        // This determines where each table appears in the visual restaurant layout
        private void CalculateGridPosition(Table table)
        {
            // For a 2-column grid layout (like shown in the UI)
            // Tables are arranged in rows of 2

            // Calculate which row this table should be in
            // Table 1 & 2 = Row 0, Table 3 & 4 = Row 1, etc.
            table.Row = (table.TableNr - 1) / 2;

            // Calculate which column this table should be in  
            // Table 1, 3, 5 = Column 0 (left), Table 2, 4, 6 = Column 1 (right)
            table.Column = (table.TableNr - 1) % 2;
        }
    }
}