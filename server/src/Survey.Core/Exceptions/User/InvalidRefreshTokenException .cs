using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Exceptions.User
{
    public class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException()
            : base("The refresh token is invalid or expired.")
        {
        }
    }
}
