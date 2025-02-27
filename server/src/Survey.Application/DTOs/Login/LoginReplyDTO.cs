using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.DTOs.Login
{
    public class LoginReplyDTO
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

        public LoginReplyDTO(string jwtToken, string refreshToken)
        {
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
