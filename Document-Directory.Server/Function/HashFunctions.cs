using System.Text;
using System.Security.Cryptography;

namespace Document_Directory.Server.Function
{
    public class HashFunctions
    {
        static public string GenerationHashPassword(string password)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashValue = SHA256.HashData(messageBytes);
            return Convert.ToHexString(hashValue);
        }
    }
}
