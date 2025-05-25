using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    //pato
    public class Order
    {
        // Basic order info
        public int Order_id { get; set; } // Unique identifier for the order
        public int Table_id { get; set; } // Foreign key to the table this order belongs to
        public int Employee_Id { get; set; } // Foreign key to the employee who took the order
        public Status Status { get; set; } // Status of the order (e.g., Preparing, Ready, Served, etc.)
        public DateOnly Date_ordered { get; set; } // Date when the order was placed
        public TimeOnly Time_ordered { get; set; } // Time when the order was placed
        public Table Table { get; set; } = null!; // Non-nullable property initialized with null-forgiving operator
        public Employee Employee { get; set; } = null!; // Non-nullable property initialized with null-forgiving operator
        public Payment? Payment { get; set; } // Nullable payment, as it may not be processed yet

        // List of items in the order
        public List<OrderItem> OrderItems { get; set; } = new();

        // Constructors
        public Order() { }

        public Order(int order_id, int table_id, int employee_Id, Status status, DateOnly date_ordered, TimeOnly time_ordered, Table table, Employee employee)
        {
            Order_id = order_id;
            Table_id = table_id;
            Employee_Id = employee_Id;
            Status = status;
            Date_ordered = date_ordered;
            Time_ordered = time_ordered;
            Table = table; // Ensure Table is initialized
            Employee = employee; // Ensure Employee is initialized
        }

        // Elapsed time since order placed (in minutes)
        public int MinutesSinceOrdered => (int)(DateTime.Today.Add(TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan()) - Date_ordered.ToDateTime(Time_ordered)).TotalMinutes;
    }
}
