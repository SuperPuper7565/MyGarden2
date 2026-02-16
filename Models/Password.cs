using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.Models
{
    public class Password
    {
        public static string GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        // Получение хэша пароля с использованием соли
        public static string GetPasswordHash(string password, string saltBase64)
        {
            byte[] salt = Convert.FromBase64String(saltBase64);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(pbkdf2.GetBytes(32)); // Хэш размером 32 байта
            }
        }

        // Проверка пароля — сравниваем новый хэш с сохраненным
        public static bool VerifyPassword(string enteredPassword, string savedPasswordHash, string savedSaltBase64)
        {
            string newHash = GetPasswordHash(enteredPassword, savedSaltBase64);
            byte[] savedHashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] newHashBytes = Convert.FromBase64String(newHash);
            return newHashBytes.SequenceEqual(savedHashBytes);
        }
    }
}
