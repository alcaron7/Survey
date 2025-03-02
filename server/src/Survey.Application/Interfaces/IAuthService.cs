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
        Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request);
        Task<bool> RevokeTokenAsync(RevokeTokenRequestDTO request);
        Task<bool> RevokeAllUserTokensAsync(string userId);
        Task<TokenValidationResponseDTO> ValidateTokenAsync(string token);
        Task<string> CreateHashSync(string password);
        string CreateTokenAsync(User user);
        Task<string> GenerateRefreshTokenAsync(User user);
    }
}
