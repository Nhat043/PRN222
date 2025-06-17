using System.Security.Cryptography;
using System.Text;

namespace BLL.Util
{
    public static class PasswordHashingHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            return HashPassword(enteredPassword) == hashedPassword;
        }
    }
}
