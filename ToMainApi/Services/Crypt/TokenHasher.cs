using System.Security.Cryptography;
using System.Text;

namespace ToMainApi.Services.Crypt
{
    public static class TokenHasher
    {
        public static string HashToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        public static bool VerifyToken(string token, string storedToken)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(storedToken))
                return false;

            var tokenhash = HashToken(token);
            return tokenhash == storedToken;
        }
    }
}
