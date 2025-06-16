namespace Chapeau.ViewModels
{
    public class TableActionsViewModel
    {
        public int TableNr { get; set; }
        public bool Available { get; set; }

        public TableActionsViewModel()
        {
        }

        public TableActionsViewModel(int tableNr, bool available)
        {
            TableNr = tableNr;
            Available = available;
        }
    }
}
