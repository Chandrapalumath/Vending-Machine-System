using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Exceptions
{
    internal class UserConflictException : Exception
    {
        public UserConflictException(string message) : base(message) { }
    }
}
