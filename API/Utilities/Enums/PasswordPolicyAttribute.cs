﻿using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace API.Utilities.Enums
{
    public class PasswordPolicyAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var password = (string)value;
            if (password is null)
            {
                return false;
            }
            var hasMinimum6Chars = new Regex(@".{6,}");
            var hasNumber = new Regex(@"[0-9]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            var hasUpperCase = new Regex(@"[A-Z]+");
            var hasLowerCase = new Regex(@"[a-z]+");

            var isValidated = hasMinimum6Chars.IsMatch(password) &&
                      hasNumber.IsMatch(password) &&
                      hasSymbols.IsMatch(password) &&
                      hasUpperCase.IsMatch(password) &&
                      hasLowerCase.IsMatch(password);

            ErrorMessage = "password must contain at least 6 characters, " +
                "1 number, 1 symbol, 1 upper case, and 1 lower case";

            return isValidated;
        }
        

    }
}
