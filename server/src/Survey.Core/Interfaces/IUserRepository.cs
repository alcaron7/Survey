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
        Task<User> AddAsync(User user);
        Task<bool> EmailAlreadyExists(string email);
        Task<User> GetByEmailAsync(string email);
    }
}
