namespace Chapeau.Models
{
    public class Table
    {
        public int TableId { get; set; } // Unique identifier for the table
        public int TableNr { get; set; }
        public bool Available { get; set; } // Boolean field for availability

        // For UI display only
        public int Row { get; set; } // Row position in grid
        public int Column { get; set; } // Column position in grid

        public Table()
        {
            Available = true; // Default to available
        }
        public Table(int tableId, int tableNr)
        {
            TableId = tableId;
            TableNr = tableNr;
        }
    }
}