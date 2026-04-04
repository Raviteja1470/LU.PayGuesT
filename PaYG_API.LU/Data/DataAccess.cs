using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using PaYG_API.LU.Auth;
using PayingG.LU.API.Data.PayingGuest.Core.Models;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PayingG.LU.API.Data
{

    public class DataAccess : IDisposable
    {
        private readonly AppDbContext _db;

        public DataAccess(AppDbContext db)
        {
            _db = db;
        }

        public void Dispose()
        {
           GC.SuppressFinalize(this);
        }

        public async Task<(bool Success, string Message)> RegisterUserAsync(
            string email, string password, string role)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email is required.");

            if (!new EmailAddressAttribute().IsValid(email))
                return (false, "Invalid email format.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return (false, "Password must be at least 8 characters.");

            if (string.IsNullOrWhiteSpace(role))
                return (false, "Role is required.");

            // Check if user exists
            bool exists = await _db.UserAccounts.AnyAsync(u => u.Email == email);
            if (exists)
                return (false, "Email already registered.");

            // Hash password
            string Password = BCrypt.Net.BCrypt.HashPassword(password);

            // Create user
            var user = new UserAccount
            {
                Email = email,
                Password = Password,
                Role = role
            };

            _db.UserAccounts.Add(user);
            await _db.SaveChangesAsync();

            return (true, "User registered successfully.");
        }



        public async Task<UserAccount?> FindUserByEmailAsync(string email)
        {
            return await _db.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddRefreshTokenAsync(string email, string token, DateTime expires)
        {
            var user = await _db.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                throw new Exception("User not found");

            var refreshToken = new RefreshToken
            {
                Token = token,
                Expires = expires,
                Enabled = true,
                GuestId = user.Id   // <-- Correct FK
            };

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();
        }
        public async Task<bool> DisableUserTokenByEmailAsync(string email)
        {
            var userId = await _db.UserAccounts
                .Where(u => u.Email == email)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == 0)
                return false;

            var updated = await _db.RefreshTokens
                .Where(t => t.TokenId == userId && t.Enabled)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.Enabled, false));

            return updated > 0;
        }
        public async Task<bool> DisableUserTokenAsync(string token)
        {
            var updated = await _db.RefreshTokens
                .Where(t => t.Token == token)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.Enabled, false));

            return updated > 0;
        }
        public async Task<bool> IsRefreshTokenValidAsync(string token)
        {
            var count = await _db.RefreshTokens
                .Where(t => t.Token == token &&
                            t.Enabled &&
                            t.Expires >= DateTime.UtcNow)
                .CountAsync();

            return count > 0;
        }

        public async Task<UserAccount?> FindUserByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var refresh = await _db.RefreshTokens
                .AsNoTracking()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);

            return refresh?.User;
        }
    }

}

