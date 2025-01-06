using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.Views;

namespace POS.Shells
{
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(LoginPage));
        }
    }
}
