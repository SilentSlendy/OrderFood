using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using POS.ViewModels;


namespace POS.Views
{
    /// <summary>
    /// Trang cài đặt
    /// </summary>
    public sealed partial class SettingView : Page
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public SettingsViewModel ViewModel { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingView()
        {
            this.InitializeComponent();

            ViewModel = new SettingsViewModel();

            // Gán ViewModel vào DataContext để binding
            DataContext = ViewModel;

            // Khởi tạo giao diện với giá trị từ ViewModel
            InitializeFields();
        }

        /// <summary>
        /// Khởi tạo giao diện với giá trị từ ViewModel
        /// </summary>
        private void InitializeFields()
        {
            VatTextBox.Text = ViewModel.VAT.ToString();
            AccessKeyPasswordBox.Password = ViewModel.AccessKey;
            SecretKeyPasswordBox.Password = ViewModel.SecretKey;
            IpnUrlTextBox.Text = ViewModel.IpnUrl;
        }

        /// <summary>
        /// Sự kiện click nút lưu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            // Cập nhật dữ liệu vào ViewModel
            if (float.TryParse(VatTextBox.Text, out float vat))
            {
                ViewModel.VAT = vat;
            }
            else
            {
                ShowError("Vui lòng nhập số hợp lệ cho Thuế VAT.");
                return;
            }
            ViewModel.AccessKey = AccessKeyPasswordBox.Password;
            ViewModel.SecretKey = SecretKeyPasswordBox.Password;
            ViewModel.IpnUrl = IpnUrlTextBox.Text;

            ViewModel.PaymentLoadLocalSettings();

            // Hiển thị thông báo lưu thành công
            ShowMessage("Vui lòng khởi động lại ứng dụng để cập nhật thông tin.");
        }

        /// <summary>
        /// Hiển thị thông báo lưu thành công
        /// </summary>
        /// <param name="message"></param>
        private async void ShowMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Thông tin đã được lưu",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        /// <summary>
        /// Hiển thị thông báo lỗi
        /// </summary>
        /// <param name="message"></param>
        private async void ShowError(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Lỗi",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

    }
}
