using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Application.Interfaces
{
    public interface IPasswordHashing
    {
        string HashPassword(string password);
        bool VerifyBcryptHashPassword(string password, string passwordHash);
        bool PasswordNeedsRehash(string passwordHash);
        void CreatePassword(string password, out byte[] passwordHash, out byte[] passwordSolt);
        bool Verifypasswordhash(string password, byte[] passwordSolt, byte[] passwordHash);
    }
}
