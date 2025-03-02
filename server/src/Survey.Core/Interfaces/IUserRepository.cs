using Survey.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<List<User>> FindByNameAsync(string searchTerm);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> IsRefreshTokenValidAsync(string refreshToken);
        Task<bool> InvalidateAllRefreshTokensAsync();
        Task<bool> InvalidateRefreshTokensForUserAsync(Guid userId);
        Task<int> CleanupExpiredRefreshTokensAsync();
    }
}
