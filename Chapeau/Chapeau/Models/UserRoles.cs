namespace Chapeau.Models
{
    // Enum for type-safe role handling
    public enum UserRole
    {
        Manager,
        Waiter,
        Kitchen,
        Bar
    }

    // String constants for database/view compatibility
    public static class RoleNames
    {
        public const string Manager = "Manager";
        public const string Waiter = "Waiter";
        public const string Kitchen = "Kitchen";
        public const string Bar = "Bar";

        // Helper method to get all role names
        public static readonly string[] AllRoles = { Manager, Waiter, Kitchen, Bar };

        // Convert enum to string
        public static string GetRoleName(UserRole role)
        {
            return role switch
            {
                UserRole.Manager => Manager,
                UserRole.Waiter => Waiter,
                UserRole.Kitchen => Kitchen,
                UserRole.Bar => Bar,
                _ => throw new ArgumentException($"Unknown role: {role}")
            };
        }

        // Convert string to enum
        public static UserRole GetRoleEnum(string roleName)
        {
            return roleName switch
            {
                Manager => UserRole.Manager,
                Waiter => UserRole.Waiter,
                Kitchen => UserRole.Kitchen,
                Bar => UserRole.Bar,
                _ => throw new ArgumentException($"Unknown role name: {roleName}")
            };
        }
    }
}