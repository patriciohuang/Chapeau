namespace Chapeau.Models
{
    public class Table
    {
        public int TableId { get; set; }
        public int TableNr { get; set; }
        public bool Available { get; set; } // Boolean field for availability

        public Table()
        {
            Available = true; // Default to available
        }

        public Table(int tableId, int tableNr, bool available)
        {
            TableId = tableId;
            TableNr = tableNr;
            Available = available;
        }
    }
}