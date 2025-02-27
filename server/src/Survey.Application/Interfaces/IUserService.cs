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
    }
}
