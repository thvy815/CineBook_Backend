using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public enum LoginIdentifierType
    {
        Email,
        Phone,
        Username
    }

    public static class LoginIdentifierHelper
    {
        public static LoginIdentifierType Detect(string input)
        {
            if (input.Contains("@"))
                return LoginIdentifierType.Email;

            if (input.All(char.IsDigit))
                return LoginIdentifierType.Phone;

            return LoginIdentifierType.Username;
        }
    }
}
