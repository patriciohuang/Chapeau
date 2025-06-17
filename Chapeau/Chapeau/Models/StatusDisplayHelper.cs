// Helpers/StatusDisplayHelper.cs
using Chapeau.Models.Enums;

public static class StatusDisplayHelper
{
    public static string GetStatusColor(Status status) => status switch
    {
        Status.Ready => "secondary",
        Status.Preparing => "success",
        Status.Ordered => "primary",
        _ => "dark"
    };

    public static string ChangeStatusName(Status status) => status switch
    {
        Status.Ready => "UNDO",
        Status.Preparing => "DONE",
        Status.Ordered => "START PREPARING",
        _ => ""
    };
}
