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

        public static Status AggregateStatus(IEnumerable<OrderItem> items)
        {
            if (items.Any(i => i.Status == Status.Unordered)) return Status.Unordered;
            if (items.Any(i => i.Status == Status.Ordered)) return Status.Ordered;
            if (items.Any(i => i.Status == Status.Preparing)) return Status.Preparing;
            if (items.All(i => i.Status == Status.Ready)) return Status.Ready;
            if (items.All(i => i.Status == Status.Served)) return Status.Served;
            if (items.All(i => i.Status == Status.Completed)) return Status.Completed;

            return Status.Cancelled;
        }
    }

}