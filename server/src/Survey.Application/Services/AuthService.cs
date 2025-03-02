using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Survey.Application.DTOs.Login;
using Survey.Application.Interfaces;
using Survey.Core.Entities;
using Survey.Core.Exceptions.User;
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
                throw new InvalidPasswordException();
            }

            var accessToken = CreateTokenAsync(user);

            var refreshToken = await GenerateRefreshTokenAsync(user);

            var expiration = DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes);

            return new LoginReplyDTO(accessToken, refreshToken, expiration);
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

            if (user == null)
            {
                throw new InvalidRefreshTokenException();
            }

            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new InvalidRefreshTokenException();
            }

            var accessToken = CreateTokenAsync(user);

            var refreshToken = await GenerateRefreshTokenAsync(user);

            var expiration = DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes);

            return new TokenResponseDTO(accessToken, refreshToken, expiration);
        }

        public async Task<bool> RevokeTokenAsync(RevokeTokenRequestDTO request)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

            if (user == null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> RevokeAllUserTokensAsync(string userId)
        {
            return await _userRepository.InvalidateRefreshTokensForUserAsync(Guid.Parse(userId));
        }

        public async Task<TokenValidationResponseDTO> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecurityKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                    var expiration = jwtSecurityToken.ValidTo;

                    return new TokenValidationResponseDTO(true, userId, email, expiration);
                }

                return new TokenValidationResponseDTO(false);
            }
            catch
            {
                return new TokenValidationResponseDTO(false);
            }
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

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

            await _userRepository.UpdateAsync(user);

            return refreshToken;
        }
    }
}
