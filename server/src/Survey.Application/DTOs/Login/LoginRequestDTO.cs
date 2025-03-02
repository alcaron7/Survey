using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.DTOs.Login
{
    public record LoginRequestDTO
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
