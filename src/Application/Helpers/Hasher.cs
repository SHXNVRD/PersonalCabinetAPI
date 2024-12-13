using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers
{
    public static class Hasher
    {
        public static async Task<string> ComputeSha256HashAsync(string rawData)
        {
            ArgumentNullException.ThrowIfNull(rawData);
            
            var bytes = await Task.Run(() => SHA256.HashData(Encoding.ASCII.GetBytes(rawData)));
            return Convert.ToHexString(bytes);
        }
    }
}