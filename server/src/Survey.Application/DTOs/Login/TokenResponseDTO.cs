using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.DTOs.Login
{
    public record TokenResponseDTO
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public DateTime Expiration { get; init; }
        public string TokenType { get; init; } = "Bearer";

        public TokenResponseDTO(string accessToken, string refreshToken, DateTime expiration)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expiration = expiration;
        }
    }
}
