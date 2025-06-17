using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    //pato
    public class Order
    {
        // Basic order info  
        public int OrderId { get; set; }
        public Status StoredStatus { get; set; }
        public Status Status => StatusHelper.AggregateStatus(OrderItems);
        public DateOnly Date_ordered { get; set; }
        public TimeOnly Time_ordered { get; set; }
        public bool IsPaid { get; set; }
        public Table Table { get; set; }
        public Employee Employee { get; set; }

        // List of items in the order  
        public List<OrderItem> OrderItems { get; set; }

        // Total cost of the order  
        public decimal TotalCost => OrderItems?.Sum(item => item.MenuItem.Price * item.Count) ?? 0;

        // VAT calculations  
        public decimal TotalHighVAT => OrderItems?
            .Where(item => item.MenuItem.IsAlcoholic)
            .Sum(item => item.MenuItem.Price * item.Count * 0.21m) ?? 0;

        public decimal TotalLowVAT => OrderItems?
            .Where(item => !item.MenuItem.IsAlcoholic)
            .Sum(item => item.MenuItem.Price * item.Count * 0.09m) ?? 0;

        public decimal TotalVAT => TotalHighVAT + TotalLowVAT;

        // Elapsed time since order placed (in minutes)  
        public int MinutesSinceOrdered => (int)(DateTime.Today.Add(TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan()) - Date_ordered.ToDateTime(Time_ordered)).TotalMinutes;

        // Constructors  
        public Order() { }

        public Order(int orderId, DateOnly date_ordered, TimeOnly time_ordered, bool isPaid, Table table, Employee employee)
        {
            OrderId = orderId;
            Date_ordered = date_ordered;
            Time_ordered = time_ordered;
            IsPaid = isPaid;
            Table = table;
            Employee = employee;
            OrderItems = new List<OrderItem>();
        }
    }
}
