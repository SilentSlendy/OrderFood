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
using POS.Models;
using POS.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views.UserControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrdersUserControl : UserControl
    {
        public EmployeeDataForLogin CurrentEmployee { get; set; }
        public OrderDetailViewModel ViewModel { get; set; } = new OrderDetailViewModel();

        public void AddToOrder(Product info, int quanlity, string note)
        {
            ViewModel.Add(info, quanlity, note);
        }
        public OrdersUserControl()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            ViewModel.GetVATValue();
            CurrentEmployee = (Application.Current as App).CurrentEmployee;
        }
        private void SaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Items.Count == 0 )
            {
                EmptyCartSavingErrorDialog("Vui lòng chọn món trước khi lưu hóa đơn.");
            }
            else
            {int newID = ViewModel.SaveToDatabase(ViewModel.InvoiceID, ViewModel.CustomerID);
            resetCart();
                DisplayIDInvoiceDialog(newID);
            }
        }
        private void PayInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Items.Count == 0)
            {
                EmptyCartSavingErrorDialog("Vui lòng chọn món trước khi thanh toán hóa đơn.");
            }
            else
            {
                int payFromMenuInvoiceId = -1; // flag to indicate that this payment is from menu page
                // Pass data to PaymentViewModel
                var paymentViewModel = (Application.Current as App).PaymentViewModel;
                paymentViewModel.SetItems(ViewModel.InvoiceID,ViewModel.Items, ViewModel.SubTotal, ViewModel.CustomerID, payFromMenuInvoiceId);

                // Navigate to PaymentView
                var navigation = (Application.Current as App).navigate;
                navigation.SetCurrentPage(typeof(PaymentView));
            }    
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as Order;
            ViewModel.Remove(item);
        }
        //================================================================================================
        //Notification
        private void ShowSaveSuccessTeachingTip()
        {
            SaveSuccessTeachingTip.IsOpen = true;

            // Auto close after 3s
            _ = Task.Delay(3000).ContinueWith(_ =>
            {
                DispatcherQueue.TryEnqueue(() => SaveSuccessTeachingTip.IsOpen = false);
            });
        }
        //Notification
        private async void DisplayIDInvoiceDialog(int newID)
        {
            ContentDialog IDinvoiceDialog = new ContentDialog()
            {
                Title = "Lưu hóa đơn thành công!",
                Content = $"Mã hóa đơn: {newID}",
                CloseButtonText = "Đóng",
                XamlRoot = this.XamlRoot
            };
            ContentDialogResult result = await IDinvoiceDialog.ShowAsync();
        }

        private async void EmptyCartSavingErrorDialog(string msg)
        {
            ContentDialog Dialog = new ContentDialog()
            {
                Title = "Lỗi",
                Content = msg,
                CloseButtonText = "Đóng",
                XamlRoot = this.XamlRoot
            };
            ContentDialogResult result = await Dialog.ShowAsync();
        }
        //================================================================================================
        private void resetCart()
        {
            ViewModel.Items.Clear();
            ViewModel.Total = 0;
            ViewModel.SubTotal = 0;
            ViewModel.Tax = 0;
            ViewModel.InvoiceID = -1;
            ViewModel.InvoiceDate = DateTime.Now;
        }
        //================================================================================================
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing,
            // otherwise assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var fiteredCustomers = ViewModel.AllCustomers.
                    Where(p => p.Name.ToLower().Contains(sender.Text.ToLower())).ToList();
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = dataset;
                    { sender.ItemsSource = fiteredCustomers; }
            }
        }


        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = (args.SelectedItem as Customer).Name;
            ViewModel.CustomerID = (args.SelectedItem as Customer).CustomerID;
        }


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                var customer = args.ChosenSuggestion as Customer;
                sender.Text = customer?.Name;
            }
            else
            {
                // Use args.QueryText to determine what to do.
           
            }
        }
    }
}


    
