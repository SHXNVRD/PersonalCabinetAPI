using System.Security.Cryptography;
using System.Text;

namespace Domain.Helpers
{
    public static class Hasher
    {
        public static string ComputeSha256Hash(string rawData)
        {
            ArgumentNullException.ThrowIfNull(rawData);

            var bytes = SHA256.HashData(Encoding.ASCII.GetBytes(rawData));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}