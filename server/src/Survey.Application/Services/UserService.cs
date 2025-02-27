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

        public UserService(IUserRepository repository, IPasswordHasher password)
        {
            _repository = repository;
            _passwordHasher = password;
        }

        public async Task<User> CreateUser(CreateUserDTO userDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
                Password = _passwordHasher.HashPassword(userDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            if(await _repository.EmailAlreadyExists(user.Email))
            {
                throw new EmailAlreadyExistsException(user.Email);
            }

            return await _repository.AddAsync(user);
        }
    }
}
