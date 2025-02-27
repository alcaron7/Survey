using Microsoft.AspNetCore.Identity.Data;
using Survey.Application.DTOs.Login;
using Survey.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginReplyDTO> LoginAsync(LoginRequestDTO request);
        string CreateTokenAsync(User user);
        Task<string> CreateHashSync(string password);
    }
}
