using GestDev.Application.DTOs.User;
using GestDev.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestDev.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUser(CreateUserDTO userDto);
    }
}
