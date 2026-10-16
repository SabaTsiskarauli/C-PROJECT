using System;

namespace FitZone.Services.Security
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "Password is required.",
                    nameof(password)
                );
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(
            string password,
            string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(
                    password,
                    passwordHash
                );
            }
            catch
            {
                return false;
            }
        }
    }
}