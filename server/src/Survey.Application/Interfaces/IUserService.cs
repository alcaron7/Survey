using Survey.Application.DTOs.User;
using Survey.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUser(CreateUserDTO userDto);
        Task<UserDTO> GetUserById(Guid id);
        Task<UserDTO> GetUserByEmail(string email);
        Task<List<UserDTO>> GetAllUsers();
        Task<UserDTO> UpdateUser(Guid id, UpdateUserDTO userDto);
        Task<bool> DeleteUser(Guid id);
        Task<bool> ChangePassword(Guid id, ChangePasswordDTO passwordDto);
        Task<bool> InvalidateUserRefreshTokens(Guid userId);
        Task<bool> ValidateRefreshToken(string refreshToken);
        Task<int> CleanupExpiredTokens();
        Task<bool> VerifyUserCredentials(string email, string password);
        Task<bool> CheckUserExists(string email);
    }
}
