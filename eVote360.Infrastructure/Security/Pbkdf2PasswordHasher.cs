using System.Security.Cryptography;
using System.Text;
using eVote360.Application.Common.Security;

namespace eVote360.Infrastructure.Security;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20);

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        byte[] hashBytes = Convert.FromBase64String(hash);
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hashToCompare = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hashToCompare[i])
                return false;
        }

        return true;
    }
}
