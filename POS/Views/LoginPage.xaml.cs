using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS;
using POS.Helpers;
using POS.Login;
using POS.Views;
using System.Text;
using System;
using POS.Shells;
using Windows.ApplicationModel.Store;
using System.Diagnostics;
using System.IO;
using POS.ViewModels;

namespace POS.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; set;}

        public LoginPage()
        {
            this.InitializeComponent();
            ViewModel = new LoginViewModel();
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            string inputUsername = UsernameTextBox.Text;
            string inputPassword = PasswordBox.Password;
            foreach (var employee in ViewModel.Employees)
            {
                if (inputUsername == employee.UsernameString && inputPassword == employee.PasswordString)
                {
                    (Application.Current as App).CurrentEmployee = employee;
                    Shell window = new Shell();
                    (Application.Current as App).navigate = window;
                    window.Activate();
                    // Set the title for the app
                    window.Title = "POS HCMUS";
                    (Application.Current as App).m_window = window;
                    (Application.Current as App).m_window2.Close();
                    return;
                }
            }
            if (ViewModel.Employees.Count == 0)
            {
                ShowNoAccountDialog();
            }
            else
            {
                ShowLoginFailedDialog();
            }
           
        }
        private void ShowLoginFailedDialog()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Đăng nhập thất bại",
                Content = "Tên đăng nhập hoặc mật khẩu không đúng.",
                CloseButtonText = "Đóng",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
        private void ShowNoAccountDialog()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Đăng nhập thất bại",
                Content = "Chưa có tài khoản trên vào hệ thống.",
                CloseButtonText = "Đóng",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
}
