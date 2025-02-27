using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Exceptions
{
    public abstract class CoreException : Exception
    {
        public string ErrorCode { get; }

        protected CoreException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
