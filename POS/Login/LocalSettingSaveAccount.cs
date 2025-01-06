using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace POS.Login
{
    public static class LocalSettingSaveAccount
    {
        public const string UsernameKey = "UsernameInBase64";
        public const string PasswordKey = "PasswordInBase64";
        public const string EntropyUsernameKey = "UsernameEntropyInBase64";
        public const string EntropyPasswordKey = "PasswordEntropyInBase64";

        public static void ClearAccount()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove(UsernameKey);
            localSettings.Values.Remove(PasswordKey);
            localSettings.Values.Remove(EntropyPasswordKey);
            localSettings.Values.Remove(EntropyUsernameKey);
        }
        static public void SaveAccount(string usernameRaw = "admin", string passwordRaw = "password")
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey(UsernameKey))
            {
                var passwordInBytes = Encoding.UTF8.GetBytes(passwordRaw);
                var entropyInBytes = new byte[20];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(entropyInBytes);
                }

                var encryptedPasswordInBytes = ProtectedData.Protect(
                    passwordInBytes,
                    entropyInBytes,
                    DataProtectionScope.CurrentUser);

                var encryptedPasswordBase64 = Convert.ToBase64String(encryptedPasswordInBytes);
                var entropyInBase64 = Convert.ToBase64String(entropyInBytes);

                // Mã hóa username
                var usernameInBytes = Encoding.UTF8.GetBytes(usernameRaw);

                // Tạo entropy cho username
                var usernameEntropyInBytes = new byte[20];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(usernameEntropyInBytes);
                }

                var encryptedUsernameInBytes = ProtectedData.Protect(
                    usernameInBytes,
                    usernameEntropyInBytes,
                    DataProtectionScope.CurrentUser);

                var usernameEntropyBase64 = Convert.ToBase64String(usernameEntropyInBytes);
                var encryptedUsernameBase64 = Convert.ToBase64String(encryptedUsernameInBytes);

                localSettings.Values["PasswordInBase64"] = encryptedPasswordBase64;
                localSettings.Values["PasswordEntropyInBase64"] = entropyInBase64;
                localSettings.Values["UsernameInBase64"] = encryptedUsernameBase64;
                localSettings.Values["UsernameEntropyInBase64"] = usernameEntropyBase64;
            }
        }

        public static (string username, string password) GetAccount()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("UsernameInBase64"))
            {
                var encryptedUsernameBase64 = (string)localSettings.Values["UsernameInBase64"];
                var usernameEntropyBase64 = (string)localSettings.Values["UsernameEntropyInBase64"];
                var encryptedUsernameInBytes = Convert.FromBase64String(encryptedUsernameBase64);
                var usernameEntropyInBytes = Convert.FromBase64String(usernameEntropyBase64);
                var usernameInBytes = ProtectedData.Unprotect(
                    encryptedUsernameInBytes,
                    usernameEntropyInBytes,
                    DataProtectionScope.CurrentUser);

                var usernameRaw = Encoding.UTF8.GetString(usernameInBytes);

                if (localSettings.Values.ContainsKey("PasswordInBase64"))
                {
                    var encryptedPasswordBase64 = (string)localSettings.Values["PasswordInBase64"];
                    var entropyBase64 = (string)localSettings.Values["PasswordEntropyInBase64"];

                    var encryptedPasswordInBytes = Convert.FromBase64String(encryptedPasswordBase64);
                    var entropyInBytes = Convert.FromBase64String(entropyBase64);

                    var passwordInBytes = ProtectedData.Unprotect(
                        encryptedPasswordInBytes,
                        entropyInBytes,
                        DataProtectionScope.CurrentUser);

                    var passwordRaw = Encoding.UTF8.GetString(passwordInBytes);
                    return (usernameRaw, passwordRaw);
                }
                else
                {
                    throw new Exception("Password not found in local settings.");
                }
            }
            else
            {
                throw new Exception("Username not found in local settings.");
            }
        }
    }
    
}
