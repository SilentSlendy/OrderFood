using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using POS.ViewModels;
using System.Collections.Generic;
using CommunityToolkit.WinUI.UI.Controls;
using System;
using POS.Models;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using POS.Shells;
using Windows.ApplicationModel.VoiceCommands;


namespace POS.Views
{
    /// <summary>
    /// Giao diện quản lý nhân viên.
    /// </summary>
    public sealed partial class EmployeeView : Page
    {
        /// <summary>
        /// ViewModel quản lý logic và dữ liệu của giao diện quản lý nhân viên.
        /// </summary>
        public EmployeeViewModel ViewModel { get; set; }

        /// <summary>
        /// Khởi tạo giao diện quản lý nhân viên.
        /// </summary>
        public EmployeeView()
        {
            this.InitializeComponent();
            ViewModel = new EmployeeViewModel();
            this.DataContext = ViewModel;
            UpdatePagingInfo_bootstrap();
        }

        //==========================================================

        /// <summary>
        /// Xử lý sự kiện khi người dùng nhập từ khóa tìm kiếm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.Keyword = SearchBox.Text;
            ViewModel.LoadEmployees(1);
            UpdatePagingInfo_bootstrap();
        }

        //================================================================

        /// <summary>
        /// Xử lý sự kiện khi người dùng chuyển sang trang chức năng tài khoản.
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

        /// <summary>
        /// Xử lý sự kiện khi người dùng chuyển trang trong phân trang.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (pagesComboBox.SelectedIndex < ViewModel.TotalPages - 1)
            {
                pagesComboBox.SelectedIndex++;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi người dùng chuyển trang trong phân trang.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentPage > 1)
            {
                pagesComboBox.SelectedIndex--;
            }
        }

        /// <summary>
        /// Cập nhật thông tin phân trang.
        /// </summary>
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

        /// <summary>
        /// Xử lý sự kien khi người dùng chọn trang trong phân trang.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic item = pagesComboBox.SelectedItem;
            if (item != null)
            {
                ViewModel.LoadEmployees(item.Page);
            }
        }

        //================================================================

        /// <summary>
        /// Xử lý sự kiện khi người dùng chọn nút thêm nhân viên
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            ShowAddEmployeeDialog();
        }

        /// <summary>
        /// Hiển thị dialog thêm nhân viên.
        /// </summary>
        private void ShowAddEmployeeDialog()
        {
            // Reset dialog fields
            EmployeeNameTextBox.Text = "";
            PositionTextBox.Text = "";
            SalaryTextBox.Text = "";
            HireDateDatePicker.SelectedDate = null;

            _ = AddEmployeeDialog.ShowAsync(); 
        }

        /// <summary>
        /// Hiển thị thông báo thêm nhân viên thành công.
        /// </summary>
        private void ShowAddSuccessTeachingTip()
        {
            AddSuccessTeachingTip.IsOpen = true;

            // Auto close after 3s
            _ = Task.Delay(3000).ContinueWith(_ =>
            {
                DispatcherQueue.TryEnqueue(() => AddSuccessTeachingTip.IsOpen = false);
            });
        }

        /// <summary>
        /// Xử lý sự kien khi người dùng chọn nút lưu nhân viên.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnSaveEmployee(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Kiểm tra nếu người dùng chưa nhập tên, lương hoac ngày vào làm
            if (string.IsNullOrEmpty(EmployeeNameTextBox.Text) || string.IsNullOrEmpty(SalaryTextBox.Text) || HireDateDatePicker.SelectedDate == null)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Vui lòng nhập tên nhân viên, lương và ngày vào làm.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot
                };
                // Close CreateDiscountDialog and show error dialog
                AddEmployeeDialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            string employeeName = EmployeeNameTextBox.Text;
            string position = PositionTextBox.Text;
            decimal salary = decimal.Parse(SalaryTextBox.Text);
            DateTime hireDate = HireDateDatePicker.Date.DateTime;

            var newEmployee = new Employee
            {
                Name = employeeName,
                Position = position,
                Salary = salary,
                HireDate = hireDate
            };

            bool result = ViewModel.AddEmployee(newEmployee);
            if (result)
            {
                ShowAddSuccessTeachingTip();
                AddEmployeeDialog.Hide();
            }
            else
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Không thể lưu nhân viên. Vui lòng thử lại.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };

            await errorDialog.ShowAsync();
            args.Cancel = true;
            }
        }

        /// <summary>
        /// Xử lý sự kien khi người dùng chọn nút hủy thêm nhân viên.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCancelAddEmployee(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            AddEmployeeDialog.Hide();
        }

        //================================================================
        /// <summary>
        /// Xử lý sự kien khi người dùng chọn nút xóa nhân viên
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            await DeleteEmployeeAsync();
        }

        /// <summary>
        /// Xử lý sự kiện xóa nhân viên.
        /// </summary>
        /// <returns></returns>
        private async Task DeleteEmployeeAsync()
        {
            if (ViewModel.SelectedEmployee == null)
            {
                // Show an error dialog if no item is selected
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Vui lòng chọn nhân viên muốn xóa.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot // Bind to the current XamlRoot
                };

                await errorDialog.ShowAsync();
                return;
            }

            // Show confirmation dialog
            var confirmDialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = $"Bạn có chắc muốn xóa nhân viên '{ViewModel.SelectedEmployee.Name}'?",
                PrimaryButtonText = "Hủy",
                SecondaryButtonText = "Xóa",
                DefaultButton = ContentDialogButton.Secondary,
                XamlRoot = this.XamlRoot
            };

            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                // Remove the selected employee
                bool deleteResult = ViewModel.DeleteEmployee(ViewModel.SelectedEmployee);
                if (deleteResult)
                {
                    ShowDeleteSuccessTeachingTip();
                }
                else
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = "Không thể xóa nhân viên. Vui lòng thử lại.",
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = this.XamlRoot
                    };

                    await errorDialog.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Hiển thị thông báo xóa nhân viên thành công.
        /// </summary>
        private async void DeleteEmployee()
        {
            if (ViewModel.SelectedEmployee == null)
            {
                // Show an error dialog if no item is selected
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Vui lòng chọn một dòng trước khi xóa.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot // Bind to the current XamlRoot
                };

                await errorDialog.ShowAsync();
                return;
            }

            // Show confirmation dialog
            var confirmDialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = $"Bạn có chắc muốn xóa nhân viên '{ViewModel.SelectedEmployee.Name}'?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Remove the selected employee
                bool deleteResult = ViewModel.DeleteEmployee(ViewModel.SelectedEmployee);
                if (deleteResult)
                {
                    ShowDeleteSuccessTeachingTip();
                }
                else
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = "Không thể xóa nhân viên. Vui lòng thử lại.",
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = this.XamlRoot
                    };

                    await errorDialog.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Hiển thị thông báo xóa nhân viên thành công.
        /// </summary>
        private void ShowDeleteSuccessTeachingTip()
        {
            DeleteSuccessTeachingTip.IsOpen = true;

            // Auto close after 3s
            _ = Task.Delay(3000).ContinueWith(_ =>
            {
                DispatcherQueue.TryEnqueue(() => DeleteSuccessTeachingTip.IsOpen = false);
            });
        }

        /// <summary>
        /// Xử lý sự kiện khi người dùng chọn sắp xếp cột trong bảng nhân viên.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataGridSorting(object sender, DataGridColumnEventArgs e)
        {
            // Only apply for the columns that need to be sorted
            if (e.Column.Header?.ToString() == "Ngày nhập" || e.Column.Header?.ToString() == "Ngày hết hạn")
            {
                var column = e.Column as DataGridTextColumn;
                if (column != null)
                {
                    var binding = column.Binding as Binding;
                    if (binding != null)
                    {
                        var propertyName = binding.Path.Path; // Get the property name to sort
                        ViewModel.SortEmployees(propertyName);
                    }
                }
            }
            else
            {
                // Do nothing when click on other columns
            }
        }
        private async void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedEmployee == null)
            {
                // Show an error dialog if no item is selected
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Vui lòng chọn nhân viên.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot // Bind to the current XamlRoot
                };

                await errorDialog.ShowAsync();
                return;
            }
            Frame.Navigate(typeof(ChangeAccountForManagerPage),ViewModel.SelectedEmployee.EmployeeID);
        }
    }
}
