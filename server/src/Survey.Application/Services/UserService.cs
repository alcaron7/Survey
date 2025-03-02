using Survey.Application.DTOs.User;
using Survey.Application.Interfaces;
using Survey.Core.Entities;
using Survey.Core.Exceptions.User;
using Survey.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> CreateUser(CreateUserDTO userDto)
        {
            // Check if email already exists
            if (await _repository.ExistsByEmailAsync(userDto.Email))
            {
                throw new EmailAlreadyExistsException(userDto.Email);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
                Password = _passwordHasher.HashPassword(userDto.Password),
                CreatedAt = DateTime.UtcNow,
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.MinValue
            };

            return await _repository.CreateAsync(user);
        }

        public async Task<UserDTO> GetUserById(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new UserNotFoundException(id);
            }

            return MapUserToDTO(user);
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new UserNotFoundException(email);
            }

            return MapUserToDTO(user);
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(MapUserToDTO).ToList();
        }

        public async Task<UserDTO> UpdateUser(Guid id, UpdateUserDTO userDto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new UserNotFoundException(id);
            }

            if (userDto.Email != null && userDto.Email != user.Email &&
                await _repository.ExistsByEmailAsync(userDto.Email))
            {
                throw new EmailAlreadyExistsException(userDto.Email);
            }

            if (!string.IsNullOrEmpty(userDto.Name))
            {
                user.Name = userDto.Name;
            }

            if (!string.IsNullOrEmpty(userDto.Email))
            {
                user.Email = userDto.Email;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(user);

            return MapUserToDTO(user);
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new UserNotFoundException(id);
            }

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ChangePassword(Guid id, ChangePasswordDTO passwordDto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new UserNotFoundException(id);
            }

            if (!_passwordHasher.VerifyPassword(passwordDto.CurrentPassword, user.Password))
            {
                throw new InvalidPasswordException();
            }

            user.Password = _passwordHasher.HashPassword(passwordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            await _repository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> InvalidateUserRefreshTokens(Guid userId)
        {
            return await _repository.InvalidateRefreshTokensForUserAsync(userId);
        }

        public async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            return await _repository.IsRefreshTokenValidAsync(refreshToken);
        }

        public async Task<int> CleanupExpiredTokens()
        {
            return await _repository.CleanupExpiredRefreshTokensAsync();
        }

        public async Task<bool> VerifyUserCredentials(string email, string password)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            return _passwordHasher.VerifyPassword(password, user.Password);
        }

        public async Task<bool> CheckUserExists(string email)
        {
            return await _repository.ExistsByEmailAsync(email);
        }

        private UserDTO MapUserToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                HasActiveRefreshToken = user.RefreshToken != null && user.RefreshTokenExpiryTime > DateTime.UtcNow
            };
        }
    }
}
