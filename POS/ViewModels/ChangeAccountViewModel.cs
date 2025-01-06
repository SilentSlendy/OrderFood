using Microsoft.UI.Xaml;
using POS.Helpers;
using POS.Models;
using POS.Services.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModels
{
    public class ChangeAccountViewModel
    {
        public List<Account> Accounts { get; set; }
        public int EmployeeId { get; set; }
        private PostgresEmployeeDao DAO;
        public ChangeAccountViewModel()
        {
            DAO = new PostgresEmployeeDao();
            Accounts = DAO.GetAllAccounts();
            foreach (var account in Accounts)
            {
                account.UsernameString = EncryptionHelper.DecryptString(account.Username, (Application.Current as App).aesKey, account.Username_iv);
                account.PasswordString = EncryptionHelper.DecryptString(account.Password, (Application.Current as App).aesKey, account.Password_iv);
            }
            EmployeeId = (Application.Current as App).CurrentEmployee.EmployeeID;
        }
    }
}
