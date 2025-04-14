using System.Security.Cryptography;
using System.Text;

namespace DiscordServer.Authentication;
public static class Hash
{
    public static string StringWithSalt(string password, string salt)
    {
        return String(password + ":pepper:" + salt);
    }

    public static string String(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        using var hash = SHA512.Create();
        var hashedInputBytes = hash.ComputeHash(bytes);
        var hashedInputStringBuilder = new StringBuilder(128);
        foreach (var b in hashedInputBytes)
            hashedInputStringBuilder.Append(b.ToString("X2"));
        return hashedInputStringBuilder.ToString();
    }

    public static string GeneratePasswordSalt()
    {
        var random = new Random();
        StringBuilder salt = new StringBuilder();
        var length = random.Next(23, 26);

        for (int i = 0; i < length; i++)
        {
            switch (random.Next(3))
            {
                case 0:
                    salt.Append((char)random.Next('1', '9'));
                    break;
                case 1:
                    salt.Append((char)random.Next('A', 'Z'));
                    break;
                default:
                    salt.Append((char)random.Next('a', 'z'));
                    break;
            }
        }

        return salt.ToString();
    }
}
