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
            List<Table> tables = new List<Table>(); // Initialize empty list to hold table objects

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // SQL query to get all tables ordered by table number
                // This ensures consistent display order in the UI
                string query = "SELECT table_id, table_nr, availability FROM [table] ORDER BY table_nr";

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
                            // Get table ID from database (primary key)
                            TableId = reader.GetInt32(reader.GetOrdinal("table_id")),
                            // Get table number from database
                            TableNr = reader.GetInt32(reader.GetOrdinal("table_nr")),

                            // Get availability status (true = available, false = occupied)
                            Available = reader.GetBoolean(reader.GetOrdinal("availability"))
                        };
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
                string query = "SELECT table_id, table_nr, availability FROM [table] WHERE table_nr = @TableNr";

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
                            TableId = reader.GetInt32(reader.GetOrdinal("table_id")),
                            TableNr = reader.GetInt32(reader.GetOrdinal("table_nr")),
                            Available = reader.GetBoolean(reader.GetOrdinal("availability"))
                        };
                    }
                }
            }

            return table; // Returns null if table not found
        }




        // Updates the availability status of a specific table
        // Called when waiters mark tables as unavailable or available
        public void UpdateTableAvailability(int tableNr, bool available)
        {   
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [table] SET availability = @available WHERE table_nr = @tableNr";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@available", available);
                command.Parameters.AddWithValue("@tableNr", tableNr);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public int GetTableId(int tableNr){
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT table_id FROM [table] WHERE table_nr = @tableNr";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@tableNr", tableNr);

                connection.Open();

                return (int)command.ExecuteScalar(); // Returns the table ID for the given table number
            }
        }

    }
}