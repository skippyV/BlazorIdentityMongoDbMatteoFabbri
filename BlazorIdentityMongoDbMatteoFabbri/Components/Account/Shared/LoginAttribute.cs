using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BlazorIdentityMongoDbMatteoFabbri.Components.Account.Shared
{
    public class LoginAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            string? inputString = value as string;

            if (String.IsNullOrEmpty(inputString))
            {
                return false;
            }

            if (LoginHelper.IsValidEmail(inputString))  // test for email
            {
                return true;
            }
            else  // test for username
            {
                Regex regex = new Regex(@"^[a-zA-Z0-9_]+$");

                Match match = regex.Match(inputString);

                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class LoginHelper
    {
        public static bool IsValidEmail(string email)
        {
            MailAddress? resultMailAddress = null;

            if (MailAddress.TryCreate(email, null, out resultMailAddress))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
