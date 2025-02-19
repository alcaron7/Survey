using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestDev.Core.Exceptions.User
{
    public class EmailAlreadyExistsException : CoreException
    {
        public string Email { get; }

        public EmailAlreadyExistsException(string email)
            : base($"Email {email} is already in use", "EMAIL_ALREADY_EXISTS")
        {
            Email = email;
        }
    }
}
