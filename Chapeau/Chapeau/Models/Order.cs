using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    //pato
    public class Order
    {
        // Basic order info
        public int Order_id { get; set; } // Unique identifier for the order
        public Status Status { get; set; } // Status of the order (e.g., Preparing, Ready, Served, etc.)
        public DateOnly Date_ordered { get; set; } // Date when the order was placed
        public TimeOnly Time_ordered { get; set; } // Time when the order was placed
        public Table Table { get; set; }
        public Employee Employee { get; set; }

        //Stuff for payment
        

        // List of items in the order
        public List<OrderItem> OrderItems { get; set; }

        // Constructors
        public Order() { }

        public Order(int order_id, Status status, DateOnly date_ordered, TimeOnly time_ordered, Table table, Employee employee)
        {
            Order_id = order_id;
            Status = status;
            Date_ordered = date_ordered;
            Time_ordered = time_ordered;
            Table = table;
            Employee = employee;
            OrderItems = new List<OrderItem>();
        }

        // Elapsed time since order placed (in minutes)
        public int MinutesSinceOrdered => (int)(DateTime.Today.Add(TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan()) - Date_ordered.ToDateTime(Time_ordered)).TotalMinutes;
    }
}
