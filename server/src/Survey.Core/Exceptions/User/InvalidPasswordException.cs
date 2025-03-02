using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Exceptions.User
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException()
            : base("The provided password is incorrect.")
        {
        }
    }
}
