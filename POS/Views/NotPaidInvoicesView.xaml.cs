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
using POS.ViewModels;
using POS.Models;
using System.Diagnostics;
using POS.DTOs;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.VoiceCommands;
using Azure;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotPaidInvoicesView : Page
    {
        public InvoiceViewModel ViewModel { get; set; } = new InvoiceViewModel();
        
        
        public NotPaidInvoicesView()
        {
            this.InitializeComponent();
            ViewModel.GetAllNotPaidInvoices();
            UpdatePagingInfo_bootstrap();
        }
        //================================================================================================
        //Paganation
        //Next and Previous button
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (pagesComboBox.SelectedIndex < ViewModel.TotalPages - 1)
            {
                pagesComboBox.SelectedIndex++;
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentPage > 1)
            {
                pagesComboBox.SelectedIndex--;
            }
        }

        void UpdatePagingInfo_bootstrap()
        {
            var infoList = new List<object>();
            for (int i = 1; i <= ViewModel.TotalPages; i++)
            {
                infoList.Add(new
                {
                    Page = i,
                    Total = ViewModel.TotalPages
                });
            };

            pagesComboBox.ItemsSource = infoList;
            pagesComboBox.SelectedIndex = 0;
        }

        private void pagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic item = pagesComboBox.SelectedItem;
            if (item != null)
            {
                ViewModel.LoadNotPaidInvoices(item.Page);
            }
        }
        //================================================================================================
        private void InvoiceSelectionChanged(object sender, RoutedEventArgs e)
        {
            var gridview = sender as GridView;
            var wholeInvoice = gridview.SelectedItem as WholeInvoice;
            ViewModel.SelectedInvoice = wholeInvoice;

            // Disable OrderMoreDishesButton and PayInvoiceButton if the invoice is paid
            if (wholeInvoice != null)
            {
                PayInvoiceButton.IsEnabled = !wholeInvoice.IsPaid;
            }
        }
        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.searchText = sender.Text;
            ViewModel.LoadNotPaidInvoices(1);
            UpdatePagingInfo_bootstrap();
        }
        //================================================================================================
        //Click handler
        /// <summary>
        /// Sự kiện khi click vào nút order more dishes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PayInvoice_Click(object sender, RoutedEventArgs e)
        {
            var wholeinvoice = ViewModel.SelectedInvoice;
            if (wholeinvoice != null)
            {
                int totalCost = CalculateTotalCost();
                var paymentViewModel = (Application.Current as App).PaymentViewModel;
                var orderItems = new FullObservableCollection<Order>();

                foreach (var item in wholeinvoice.InvoiceDetailsWithProductInfo)
                {
                    var order = new Order(item.ProductInfo, item.InvoiceDetailProperty.Quantity, item.InvoiceDetailProperty.Note)
                    {
                        Price = item.InvoiceDetailProperty.Price
                    };
                    orderItems.Add(order);
                }

                paymentViewModel.SetItems(wholeinvoice.Invoice.InvoiceID,orderItems, totalCost, wholeinvoice.Invoice.CustomerID, wholeinvoice.Invoice.InvoiceID);
                var navigation = (Application.Current as App).navigate;
                navigation.SetCurrentPage(typeof(PaymentView));
            }
        }
        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            var wholeinvoice = ViewModel.SelectedInvoice;
            if (wholeinvoice != null)
            {
                ViewModel.DeleteInvoice();
            }
        }
        //================================================================================================
        private int CalculateTotalCost()
        {
            int totalCost = 0;
            var wholeinvoice = ViewModel.SelectedInvoice;
            if (wholeinvoice != null)
            {
                foreach (var item in wholeinvoice.InvoiceDetailsWithProductInfo)
                {
                    totalCost += item.InvoiceDetailProperty.Price * item.InvoiceDetailProperty.Quantity;
                }
            }
            return totalCost;
        }
        //================================================================================================
        
    }
}
