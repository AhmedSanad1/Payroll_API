using PayRollApi.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Infrastructure.Services
{
    public class PasswordHashing : IPasswordHashing
    {
        private const int CurrentWorkFactor = 12;

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, CurrentWorkFactor);
        }

        public bool VerifyBcryptHashPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public bool PasswordNeedsRehash(string passwordHash)
        {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(passwordHash, CurrentWorkFactor);
        }

        // Old HMAC scheme, kept only so nothing breaks — don't use it for new hashes.
        [Obsolete("User HashPassword Function instead")]
        public void CreatePassword(string password, out byte[] passwordHash, out byte[] passwordSolt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSolt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        [Obsolete("User VerifyBcryptHashPassword Function instead")]
        public bool Verifypasswordhash(string password, byte[] passwordSolt, byte[] passwordHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSolt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }
    }
}
