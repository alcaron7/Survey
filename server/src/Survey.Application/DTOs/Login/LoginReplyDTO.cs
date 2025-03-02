using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.DTOs.Login
{
    public record LoginReplyDTO
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public DateTime Expiration { get; init; }
        public string TokenType { get; init; } = "Bearer";

        public LoginReplyDTO(string accessToken, string refreshToken)
            : this(accessToken, refreshToken, DateTime.Now.AddMinutes(60))
        {
        }

        public LoginReplyDTO(string accessToken, string refreshToken, DateTime expiration)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expiration = expiration;
        }
    }
}
