namespace Chapeau.Services
{
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly int _workFactor; // The work factor determines the computational cost of hashing - - higher is more secure but slower - - Work factor 12 = 2^12 = 4,096 iterations

        public PasswordHashingService(int workFactor = 12)
        {
            _workFactor = workFactor;
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, _workFactor); // Hash the password with the specified work factor
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash); // Verify the password against the stored hash
        }
    }
}
