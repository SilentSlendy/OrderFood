using Microsoft.UI.Xaml;
using POS.Helpers;
using POS.Models;
using POS.Services.DAO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Login
{
    internal class AccountCreator
    {
        public static (byte[] encryptedUsername, byte[] iv_username, byte[] encryptedPassword, byte[] iv_password) CreateAccount(string username, string password, int employeeID)
        {
            byte[] key = (Application.Current as App).aesKey;
            //Username encryption
            byte[] iv_username = KeyGenerator.GenerateRandomKey(16); // 16 bytes
            // Encrypt
            byte[] encryptedUsername = EncryptionHelper.EncryptString(username, key, iv_username);
            //=======
            //Password encryption
            byte[] iv_password = KeyGenerator.GenerateRandomKey(16); // 16 bytes
            // Encrypt
            byte[] encryptedPassword = EncryptionHelper.EncryptString(password, key, iv_password);
            //=======
            PostgresEmployeeDao.updateAccount(employeeID, encryptedUsername, iv_username, encryptedPassword, iv_password);
            //Decrypt
            //string decrypted = EncryptionHelper.DecryptString(encrypted, key, iv);
            //Debug.WriteLine("Decrypted: " + decrypted);
            //LocalSettingSaveAccount.SaveAccount();
            //(storedUsername, storedPassword) = LocalSettingSaveAccount.GetAccount();
            return (encryptedUsername, iv_username, encryptedPassword, iv_password);
        }
        public static void GenarateBase64AccountData(string Username, string Password)
        {
            byte[] key = (Application.Current as App).aesKey;
            //Username encryption
            byte[] iv_username = KeyGenerator.GenerateRandomKey(16); // 16 bytes
            // Encrypt
            byte[] encryptedUsername = EncryptionHelper.EncryptString(Username, key, iv_username);
            //=======
            //Password encryption
            byte[] iv_password = KeyGenerator.GenerateRandomKey(16); // 16 bytes
            // Encrypt
            byte[] encryptedPassword = EncryptionHelper.EncryptString(Password, key, iv_password);
            //=======
            string base64_username = Convert.ToBase64String(encryptedUsername);
            string base64_iv_username = Convert.ToBase64String(iv_username);
            string base64_password = Convert.ToBase64String(encryptedPassword);
            string base64_iv_password = Convert.ToBase64String(iv_password);

            Debug.WriteLine("Encrypted username: " + base64_username);
            Debug.WriteLine("IV username: " + base64_iv_username);
            Debug.WriteLine("Encrypted password: " + base64_password);
            Debug.WriteLine("IV password: " + base64_iv_password);
        }
    }
}
