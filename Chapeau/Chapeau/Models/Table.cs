namespace Chapeau.Models
{
    public class Table
    {
        public int TableNr { get; set; }
        public bool Available { get; set; } // Boolean field for availability

        // For UI display only
        public int Row { get; set; } // Row position in grid
        public int Column { get; set; } // Column position in grid

        public Table()
        {
            Available = true; // Default to available
        }
        public Table(int tableNr, bool available)
        {
            TableNr = tableNr;
            Available = available;
        }
    }
}