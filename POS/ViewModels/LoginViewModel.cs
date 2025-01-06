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
    public class LoginViewModel
    {
        public List<EmployeeDataForLogin> Employees { get; set; }
        private PostgresEmployeeDao DAO;
        public LoginViewModel()
        {
            DAO = new PostgresEmployeeDao();
            Employees = DAO.GetAllEmployeesWithAccountData();
            foreach (var employee in Employees)
            {
                employee.UsernameString = EncryptionHelper.DecryptString(employee.Username, (Application.Current as App).aesKey, employee.Username_iv);
                employee.PasswordString = EncryptionHelper.DecryptString(employee.Password, (Application.Current as App).aesKey, employee.Password_iv);
            }
        }
    }
}
