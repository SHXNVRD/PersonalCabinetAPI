using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers
{
    public static class Hasher
    {
        public static async Task<string> ComputeSha256HashAsync(string rawData)
        {
            if (rawData == null)
                throw new ArgumentNullException(nameof(rawData));

            using var sha256 = SHA256.Create();
            var bytes = await Task.Run(() => sha256.ComputeHash(Encoding.ASCII.GetBytes(rawData)));
            return Convert.ToHexString(bytes);
        }
    }
}