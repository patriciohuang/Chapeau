using System.Text.Json;

namespace Chapeau.Services
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            string jsonString = JsonSerializer.Serialize(value); // Serialize the object to a JSON string
            session.SetString(key, jsonString); // Store the JSON string in the session
        }

        public static T? GetObject<T>(this ISession session, string key) where T : class
        {
            string? jsonString = session.GetString(key); // Retrieve the JSON string from the session
            if (string.IsNullOrEmpty(jsonString)) // Check if the string is null or empty
            {
                return null; // Return null if the string is empty
            }
            return JsonSerializer.Deserialize<T>(jsonString); // Deserialize the JSON string back to an object of type T
        }
    }
}
