using System.Text.Json;

namespace Chapeau.Services
{
    // Static class containing extension methods for ASP.NET Core Session
    // These methods allow storing and retrieving complex objects (not just strings) in session
    // Uses JSON serialization to convert objects to/from strings for session storage
    public static class SessionExtensions
    {
        // Extension method: Stores any object in session by serializing it to JSON
        // Generic method - works with any type T (Employee, List<>, etc.)
        // Example usage: HttpContext.Session.SetObject("LoggedInEmployee", employee);
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            // Convert the object to a JSON string using System.Text.Json
            // This allows us to store complex objects in session (which only supports strings)
            string jsonString = JsonSerializer.Serialize(value);

            // Store the JSON string in session using the provided key
            session.SetString(key, jsonString);
        }

        // Extension method: Retrieves an object from session by deserializing it from JSON
        // Generic method - returns the object as the specified type T
        // Returns null if the key doesn't exist or the value is empty
        // Example usage: var employee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
        public static T? GetObject<T>(this ISession session, string key) where T : class
        {
            // Try to get the JSON string from session using the provided key
            string? jsonString = session.GetString(key);

            // Check if the retrieved string is null or empty
            if (string.IsNullOrEmpty(jsonString))
            {
                // No data found - return null
                return null;
            }

            // Deserialize the JSON string back to an object of type T
            // Return the deserialized object
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}