using Survey.Core.Entities;
using Survey.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SurveyDbContext _context;

        public UserRepository(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<List<User>> FindByNameAsync(string searchTerm)
        {
            return await _context.Users
                .Where(u => u.Name.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
        {
            var user = await GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> InvalidateAllRefreshTokensAsync()
        {
            var allUsers = await _context.Users.ToListAsync();
            foreach (var user in allUsers)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InvalidateRefreshTokensForUserAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CleanupExpiredRefreshTokensAsync()
        {
            var expiredUsers = await _context.Users
                .Where(u => u.RefreshTokenExpiryTime < DateTime.Now && u.RefreshToken != null)
                .ToListAsync();

            foreach (var user in expiredUsers)
            {
                user.RefreshToken = null;
            }

            await _context.SaveChangesAsync();
            return expiredUsers.Count;
        }
    }
}
