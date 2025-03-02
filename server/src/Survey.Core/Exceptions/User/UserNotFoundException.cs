using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Exceptions.User
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid id)
            : base($"User with ID {id} was not found.")
        {
        }

        public UserNotFoundException(string email)
            : base($"User with email {email} was not found.")
        {
        }
    }
}
