using Chapeau.Enums;

namespace Chapeau.Models
{
    //pato
    public class DisplayOrder
    {
        // Basic order info
        public int Order_id { get; set; }
        public int Table_id { get; set; }

        public string Employee_name { get; set; }

        public Status Status { get; set; }
        public DateOnly Date_ordered { get; set; }
        public TimeOnly Time_ordered { get; set; }

        //List of items in the order
        public List<DisplayOrderItem> OrderItems { get; set; } = new();

        // Constructors
        public DisplayOrder() { }

        public DisplayOrder(int order_id, int table_id, string employee_name, Status status, DateOnly date_ordered, TimeOnly time_ordered)
        {
            Order_id = order_id;
            Table_id = table_id;
            Employee_name = employee_name;
            Status = status;
            Date_ordered = date_ordered;
            Time_ordered = time_ordered;
        }

        //Elapsed time since order placed (in minutes)
        public int MinutesSinceOrdered =>(int)(DateTime.Today.Add(TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan()) - Date_ordered.ToDateTime(Time_ordered)).TotalMinutes;
    }
}
