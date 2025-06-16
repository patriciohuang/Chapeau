using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class OrderItemEditViewModel
    {
        public OrderItem OrderItem { get; set; }

        public int TableNr { get; set; }

        public OrderItemEditViewModel(OrderItem orderItem, int tableNr)
        {
            OrderItem = orderItem;
            TableNr = tableNr;
        }
    }
}
