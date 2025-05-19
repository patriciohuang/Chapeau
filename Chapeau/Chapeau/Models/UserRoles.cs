namespace Chapeau.Models
{
    // Enum for type-safe role handling throughout the application
    // This prevents typos and provides IntelliSense support when working with roles
    // Each enum value represents a different employee role in the restaurant
    public enum UserRole
    {
        Manager,    // Restaurant manager - has access to employee management, reports, etc.
        Waiter,     // Waiter staff - handles tables, orders, customer service
        Kitchen,    // Kitchen staff - prepares food orders
        Bar         // Bar staff - prepares drink orders
    }

    // Static class containing string constants for role names
    // We use both enums AND strings because:
    // - Database stores roles as strings
    // - Views and forms work with strings
    // - Enums provide type safety in code
    // This class bridges between the two approaches
    public static class RoleNames
    {
        // String constants matching the database values exactly
        // These must match what's stored in the employee table
        public const string Manager = "Manager";
        public const string Waiter = "Waiter";
        public const string Kitchen = "Kitchen";
        public const string Bar = "Bar";

        // Helper array containing all valid role names
        // Useful for validation and dropdown lists in forms
        public static readonly string[] AllRoles = { Manager, Waiter, Kitchen, Bar };

        // Helper method: Converts UserRole enum to string
        // Use this when you have an enum value and need the corresponding string
        // Example: RoleNames.GetRoleName(UserRole.Manager) returns "Manager"
        public static string GetRoleName(UserRole role)
        {
            return role switch // Hyper optimized switch statement, actually its a switch expression.
            {
                UserRole.Manager => Manager,   // Manager enum → "Manager" string
                UserRole.Waiter => Waiter,     // Waiter enum → "Waiter" string
                UserRole.Kitchen => Kitchen,   // Kitchen enum → "Kitchen" string
                UserRole.Bar => Bar,           // Bar enum → "Bar" string
                _ => throw new ArgumentException($"Unknown role: {role}") // Safety check
            };
        }

        // Helper method: Converts string to UserRole enum
        // Use this when you have a string from database/form and need the enum
        // Example: RoleNames.GetRoleEnum("Manager") returns UserRole.Manager
        public static UserRole GetRoleEnum(string roleName)
        {
            return roleName switch // Hyper optimized switch statement, actually its a switch expression.
            {
                Manager => UserRole.Manager,   // "Manager" string → Manager enum
                Waiter => UserRole.Waiter,     // "Waiter" string → Waiter enum
                Kitchen => UserRole.Kitchen,   // "Kitchen" string → Kitchen enum
                Bar => UserRole.Bar,           // "Bar" string → Bar enum
                _ => throw new ArgumentException($"Unknown role name: {roleName}") // Safety check
            };
        }
    }
}