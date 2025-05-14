using Chapeau.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Chapeau.Repositories
{
    public class TablesRepository : ITablesRepository
    {
        private readonly string _connectionString;

        public TablesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public IEnumerable<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT table_nr, availability FROM [table] ORDER BY table_nr";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Table table = new Table
                        {
                            TableNr = reader.GetInt32(reader.GetOrdinal("table_nr")),
                            Available = reader.GetBoolean(reader.GetOrdinal("availability"))
                        };

                        // Set grid position based on table number
                        CalculateGridPosition(table);

                        tables.Add(table);
                    }
                }
            }

            return tables;
        }

        public Table GetTableByNumber(int tableNr)
        {
            Table table = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT table_nr, availability FROM [table] WHERE table_nr = @TableNr";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableNr", tableNr);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        table = new Table
                        {
                            TableNr = reader.GetInt32(reader.GetOrdinal("table_nr")),
                            Available = reader.GetBoolean(reader.GetOrdinal("availability"))
                        };

                        // Set grid position
                        CalculateGridPosition(table);
                    }
                }
            }

            return table;
        }

        public void UpdateTableAvailability(int tableNr, bool available)
        {
            // To be implemented
        }

        // Helper method to calculate grid position for tables
        private void CalculateGridPosition(Table table)
        {
            // For a 2-column grid layout like in the image
            table.Row = (table.TableNr - 1) / 2;
            table.Column = (table.TableNr - 1) % 2;
        }
    }
}