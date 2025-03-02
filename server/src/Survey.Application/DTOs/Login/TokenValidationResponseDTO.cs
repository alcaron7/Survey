using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.DTOs.Login
{
    public record TokenValidationResponseDTO
    {
        public bool IsValid { get; init; }
        public string UserId { get; init; }
        public string Email { get; init; }
        public DateTime? Expiration { get; init; }

        public TokenValidationResponseDTO(bool isValid, string userId = null, string email = null, DateTime? expiration = null)
        {
            IsValid = isValid;
            UserId = userId;
            Email = email;
            Expiration = expiration;
        }
    }
}
