namespace Chapeau.Services
{
    // Service responsible for securely hashing passwords and verifying them
    // Uses BCrypt algorithm for strong, salt-based password hashing
    // This ensures passwords are never stored in plain text in the database
    public class PasswordHashingService : IPasswordHashingService
    {
        // The work factor determines the computational cost of hashing
        // Higher values = more secure but slower
        // Work factor 12 = 2^12 = 4,096 iterations
        // This is a good balance between security and performance as of 2025
        private readonly int _workFactor;

        // Constructor: Sets the work factor for BCrypt hashing
        // Default work factor of 12 is recommended for most applications
        public PasswordHashingService(int workFactor = 12)
        {
            _workFactor = workFactor;
        }

        // Hashes a plain text password into a secure hash
        // This method is called when creating new employees or changing passwords
        // The resulting hash includes the salt, so each hash is unique even for the same password
        public string HashPassword(string password)
        {
            // Use BCrypt to hash the password with the specified work factor
            // BCrypt automatically generates a random salt for each password
            // The salt is included in the returned hash string
            return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
        }

        // Verifies if a plain text password matches a stored hash
        // This method is called during login to check if the entered password is correct
        // BCrypt handles extracting the salt from the hash and comparing it properly
        public bool VerifyPassword(string password, string passwordHash)
        {
            // Use BCrypt to verify the password against the stored hash
            // Returns true if the password matches, false if it doesn't
            // This method is safe against timing attacks due to BCrypt's implementation
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}