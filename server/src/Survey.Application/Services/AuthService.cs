using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Survey.Application.DTOs.Login;
using Survey.Application.Interfaces;
using Survey.Core.Entities;
using Survey.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IOptions<JwtSettings> jwtSettings,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _jwtSettings = jwtSettings.Value;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginReplyDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                throw new Exception("Invalid email or password");
            }

            var token = CreateTokenAsync(user);

            return new LoginReplyDTO(token, "");
        }

        public async Task<string> CreateHashSync(string password)
        {
            return await Task.Run(() => _passwordHasher.HashPassword(password));
        }

        public string CreateTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.Name}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
