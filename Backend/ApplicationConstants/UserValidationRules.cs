using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.ApplicationConstants
{
    public static class UserValidationRules
    {
        public const int MinUserNameLength = 3;
        public const int MaxUserNameLength = 30;
        public const int MinPasswordLength = 12;
        public const int MaxPasswordLength = 20;
    }
}
