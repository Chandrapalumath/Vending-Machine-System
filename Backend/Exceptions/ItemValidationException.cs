using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Exceptions
{
    public class ItemValidationException : Exception
    {
        public ItemValidationException(string message) : base(message) { }
    }
}
