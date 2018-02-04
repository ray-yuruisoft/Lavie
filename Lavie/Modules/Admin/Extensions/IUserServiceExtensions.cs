using System;
using System.Threading.Tasks;
using Lavie.Extensions;
using Lavie.Utilities.Cryptography;
using Lavie.Modules.Admin.Services;
using Lavie.Modules.Admin.Models;
using System.Text.RegularExpressions;

namespace Lavie.Modules.Admin.Extensions
{
    public static class IUserServiceExtensions
    {
        public static async Task<UserInfo> GetNormalUser(this IUserService userService, string account, string password)
        {
            if (account.IsNullOrWhiteSpace() || password.IsNullOrWhiteSpace()) return null;

            // ^(([a-zA-Z][a-zA-Z0-9-_]*)|(1\d{10}))|([\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?)$
            UserInfo userInfo = null;
            if (IsValidUsername(account))
            {
                userInfo = await userService.GetItemByUsernameAsync(account, UserStatus.Normal);
            }
            else if (IsValidMobile(account))
            {
                userInfo = await userService.GetItemByMobileAsync(account, UserStatus.Normal);
            }
            else if (IsValidEmail(account))
            {
                userInfo = await userService.GetItemByEmailAsync(account, UserStatus.Normal);
            }

            if (userInfo == null || userInfo.Password.IsNullOrWhiteSpace()) return null;

            string[] splitData = userInfo.Password.Split('|');
            if (splitData.Length != 2) throw new InvalidOperationException("Password and PasswordSalt could not be read from module data");

            string userPasswordSalt = splitData[0];
            string userPassword = splitData[1];

            return userPassword == SHA256.Encrypt(password, userPasswordSalt) ? userInfo : null;
        }
        public static Boolean VerifyPassword(this UserInfo userInfo, string password)
        {
            if (userInfo == null || password.IsNullOrWhiteSpace()) return false;
            if (userInfo.Password.IsNullOrWhiteSpace()) return false;

            string[] splitData = userInfo.Password.Split('|');
            if (splitData.Length != 2) throw new InvalidOperationException("Password and PasswordSalt could not be read from module data");

            string userPasswordSalt = splitData[0];
            string userPassword = splitData[1];

            return userPassword == SHA256.Encrypt(password, userPasswordSalt) ? true : false;

        }
        private static bool IsValidEmail(string source)
        {
            return Regex.IsMatch(source, @"^([\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?)$");
        }
        private static bool IsValidMobile(string source)
        {
            return Regex.IsMatch(source, @"^(1\d{10})$");
        }
        private static bool IsValidUsername(string source)
        {
            return Regex.IsMatch(source, @"^([a-zA-Z][a-zA-Z0-9-_]*)$");
        }
    }
}
