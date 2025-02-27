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

        public async Task<User> AddAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> EmailAlreadyExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
