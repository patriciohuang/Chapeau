using Chapeau.Models.Enums;
namespace Chapeau.Models
{
    public static class StatusHelper
    {
        public static Status NextStatus(Status currentStatus)
        {
            return currentStatus switch
            {
                Status.Ordered => Status.Preparing,
                Status.Preparing => Status.Ready,
                _ => currentStatus
            };
        }

        public static Status PreviousStatus(Status currentStatus)
        {
            return currentStatus switch
            {
                Status.Ready => Status.Preparing,
                Status.Preparing => Status.Ordered,
                _ => currentStatus
            };
        }

        public static Status AggregateStatus(IEnumerable<OrderItem> items, UserRole? role = null)
        {
            IEnumerable<OrderItem> filteredOrders = items;
            // Optionally filter items based on role
            if (role != null)
            {
                filteredOrders = items.Where(item =>
                    (role == UserRole.Kitchen && item.MenuItem.MenuCard != MenuCard.Drinks) ||
                    (role == UserRole.Bar && item.MenuItem.MenuCard == MenuCard.Drinks)
                );
            }

            // Aggregation logic
            if (filteredOrders.Any(i => i.Status == Status.Unordered)) return Status.Unordered;
            if (filteredOrders.Any(i => i.Status == Status.Ordered)) return Status.Ordered;
            if (filteredOrders.Any(i => i.Status == Status.Preparing)) return Status.Preparing;
            if (filteredOrders.Any(i => i.Status == Status.Ready)) return Status.Ready;
            if (filteredOrders.All(i => i.Status == Status.Served)) return Status.Served;
            if (filteredOrders.All(i => i.Status == Status.Completed)) return Status.Completed;

            return Status.Cancelled;
        }

    }

}