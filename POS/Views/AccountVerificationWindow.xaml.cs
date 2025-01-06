using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.VoiceCommands;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountVerificationWindow : Window
    {
        public AccountVerificationWindow()
        {
            this.InitializeComponent();
          
        }
        /// <summary>
        /// Verify the account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVerifyClick(object sender, RoutedEventArgs e)
        {
            string username = (Application.Current as App).CurrentEmployee.UsernameString;
            string password = (Application.Current as App).CurrentEmployee.PasswordString;
            if (usernameTextBox.Text == username && passwordTextBox.Password == password)
            {
                ContentFrame.Navigate(typeof(ChangeAccountPage));
            }
            else
            {
               showInvalidDialog();
            }
        }
        /// <summary>
        /// Show invalid dialog
        /// </summary>
        private void showInvalidDialog()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Đăng nhập thất bại",
                Content = "Tên đăng nhập hoặc mật khẩu không đúng.",
                CloseButtonText = "Đóng",
                XamlRoot = this.Content.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
}
