using POS.ViewModels;
using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using POS.Interfaces;
using POS.Models;
using System.Linq;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;
using POS.Shells;


namespace POS.Views
{
    /// <summary>
    /// Giao diện thanh toán.
    /// </summary>
    public sealed partial class PaymentView : Page
    {
        bool isPaidInvoice = false;
        /// <summary>
        /// ViewModel quản lý logic và dữ liệu của giao diện thanh toán.
        /// </summary>
        public PaymentViewModel ViewModel { get; set; }

        /// <summary>
        /// Khởi tạo giao diện thanh toán.
        /// </summary>
        public PaymentView()
        {
            this.InitializeComponent();
            ViewModel = (Application.Current as App).PaymentViewModel;
            this.DataContext = ViewModel;
            ViewModel.GetValuesInLocalSettings();
            //ViewModel.initCount += 1;
            //// Nếu lần đầu khởi tạo trang thì gán các giá trị cho ViewModel từ LocalSettings
            //if (ViewModel.initCount == 1)
            //{
            //    ViewModel.LoadLocalSettings();
            //}    
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút thanh toán.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSubmitPayment(object sender, RoutedEventArgs e)
        {
            if (ViewModel.TotalCost <= 0) 
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Không thể thanh toán khi chưa có đơn hàng.",
                    CloseButtonText = "Đóng",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
                return;
            }

            if (ViewModel.SelectedPaymentMethod == "Tiền mặt")
            {
                {
                    ViewModel.InvoiceId = ViewModel.SaveToDB();
                }

                // Delete used discount code
                if (ViewModel.DiscountCode != null)
                {
                    ViewModel.DeleteUsedDiscountCode();
                }

                // Show invoice dialog
                ShowInvoiceDialog();
            }
            else if (ViewModel.SelectedPaymentMethod == "Momo")
            {
                string payUrl = await ViewModel.RequestMoMoPayment();

                if (payUrl != null)
                {
                    // Chuyển đến trang thanh toán MoMo
                    Uri uri = new Uri(payUrl);
                    await Windows.System.Launcher.LaunchUriAsync(uri);

                    // Hiển thị dialog xác nhận
                    ShowPaymentConfirmDialog();
                }
                else
                {
                    // Show error dialog
                    ShowErrorDialog();
                }
            }
        }

        /// <summary>
        /// Hiển thị dialog xác nhận thanh toán qua MoMo.
        /// </summary>
        private async void ShowPaymentConfirmDialog()
        {
            ContentDialog paymentConfirmDialog = new ContentDialog
            {
                Title = "Xác nhận đã thanh toán qua Momo",
                PrimaryButtonText = "Xác nhận",
                CloseButtonText = "Hủy",
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await paymentConfirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                {
                    ViewModel.InvoiceId = ViewModel.SaveToDB();
                }
                // Delete used discount code
                if (ViewModel.DiscountCode != null)
                {
                    ViewModel.DeleteUsedDiscountCode();
                }

                // Show Invoice dialog
                ShowInvoiceDialog();
            }
            else
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Hiển thị dialog lỗi.
        /// </summary>
        private async void ShowErrorDialog()
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Lỗi",
                Content = "Chức năng này đang gặp lỗi. Bạn vui lòng thử lại sau.",
                CloseButtonText = "Đóng",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }


        /// <summary>
        /// Xử lý sự kiện khi thay đổi số tiền nhận được.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivedAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReceivedAmountTextBox.Text))
            {
                ViewModel.ReceivedAmount = 0;
            }
        }

        /// <summary>
        /// Hiển thị dialog hóa đơn.
        /// </summary>
        private async void ShowInvoiceDialog()
        {
            // Lấy tên khách hàng từ DB và lưu vào ViewModel
            ViewModel.GetCustomerName();

            // Hiển thị dialog hóa đơn
            ContentDialogResult result = await InvoiceDialog.ShowAsync();
        }


        /// <summary>
        /// Xử lý sự kiện khi nhấn nút đóng dialog hóa đơn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCloseInvoiceDialog(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Reset lại dữ liệu
            ViewModel.ResetData();
            // Về trang Menu
            var navigation = (Application.Current as App).navigate;
            var festivalItem = navigation.GetNavigationViewItems(typeof(Menu)).First();
            navigation.SetCurrentNavigationViewItem(festivalItem);
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút mở account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Lấy NavigationViewItem từ sender
            var menuItem = sender as NavigationViewItem;

            // Kiểm tra xem menuItem có hợp lệ không
            if (menuItem != null)
            {
                var accountWindow = new ShellWindow();
                accountWindow.Activate();
            }
        }
    }
}
