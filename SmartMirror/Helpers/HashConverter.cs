using System.Security.Cryptography;
using System.Text;

namespace SmartMirror.Helpers;

public static class HashConverter
{
    public static string GetMD5FromString(string input)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);

        var hashBytes = MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}
