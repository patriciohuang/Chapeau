using Chapeau.Models.Enums;
namespace Chapeau.Models
{
    internal class StatusHelper
    {
        public static Status NextStatus (Status currentStatus)
        {
            Status nextStatus = currentStatus switch
            {
                Status.Ordered => Status.Preparing,
                Status.Preparing => Status.Ready,
                _ => currentStatus
            };
            return nextStatus;
        }
        public static Status PreviousStatus(Status currentStatus)
        {
            Status previousStatus = currentStatus switch
            {
                Status.Preparing => Status.Ordered,
                Status.Ready => Status.Preparing,
                _ => currentStatus
            };
            return previousStatus;
        }
    }
}