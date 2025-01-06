using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using POS.DTOs;
using POS.Models;
using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace POS.Views
{
    /// <summary>
    /// Menu
    /// 
    /// </summary>
    public sealed partial class Menu : Page
    {
        /// <summary>
        /// View model cho Product
        /// </summary>
        public ProductViewModel ViewModel { get; set; }
        /// <summary>
        /// Sản phẩm được chọn
        /// </summary>
        public Product SelectedProduct { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public Menu()
        {
            this.InitializeComponent();
            this.ViewModel = new ProductViewModel();
            //this.DataContext = ViewModel;
            this.SelectedProduct = new Product();
            //this.DataContext = ViewModel;
            UpdatePagingInfo_bootstrap();
        }
        //==========================================================
        /// <summary>
        /// Sự kiện khi click vào một danh mục
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            if (menuItem != null && menuItem.Tag != null)
            {
                ViewModel.selectedCategory = menuItem.Tag.ToString();
                
            }
            else
            {
                ViewModel.selectedCategory = "";
            }
            ViewModel.LoadProducts(1);
            UpdatePagingInfo_bootstrap();
        }

        /// <summary>
        /// Sự kiện khi click vào một thứ tự sắp xếp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sort_Item_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            ViewModel.selectedSortOrder = int.Parse(menuItem.Tag.ToString());
            ViewModel.LoadProducts(1);
            UpdatePagingInfo_bootstrap();
        }

        /// <summary>
        /// Sự kiện khi nhập từ khóa tìm kiếm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            
                ViewModel.Keyword = SearchBox.Text;
                ViewModel.LoadProducts(1);
                UpdatePagingInfo_bootstrap();
        }

        /// <summary>
        /// Sự kiện khi click vào một sản phẩm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
           
            var menuItem = sender as NavigationViewItem;


            if (menuItem != null)
            {
                var accountWindow = new ShellWindow();
                accountWindow.Activate();
            }
        }
        //================================================================
        /// <summary>
        /// Sự kiện khi click vào nút next để chuyển trang tiếp theo
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
        /// Sự kiện khi click vào nút previous để chuyển trang trước
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
        /// Cập nhật thông tin phân trang
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
        /// Sự kiện khi chọn một trang
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic item = pagesComboBox.SelectedItem;
            if (item != null)
            {
                ViewModel.LoadProducts(item.Page);
            }
        }
        //================================================================================================
        /// <summary>
        /// Sự kiện khi chọn một sản phẩm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemListBox_selectionChanged(object sender, TappedRoutedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                if (listBox.SelectedItem != null)
                {
                    var product = listBox.SelectedItem as Product;
                    if (product != null)
                    {
                        //var dialog = new ContentDialog();
                        //dialog.XamlRoot = this.XamlRoot;
                        //await dialog.ShowAsync();
                        //SelectedProduct = product;
                        SelectedProduct.AssignFrom(product);
                        // Hiển thị hình ảnh tương ứng
                        var bitmap = new BitmapImage(new Uri(product.ImagePath, UriKind.RelativeOrAbsolute));
                        SelectedProductImage.Source = bitmap;
                        FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender); 
                    }
                }
            }
        }

        /// <summary>
        /// Sự kiện khi click vào nút thêm vào hóa đơn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AddToBillClick(object sender, RoutedEventArgs args)
        {
            OrdersUserControl.AddToOrder(SelectedProduct,((int)QuanlityBox.Value), NoteTextBox.Text);
            CenteredFlyout.Hide();
            QuanlityBox.Value = 1;
        }


        /// <summary>
        /// Xử lý sự kiện khi người dùng chọn nút thêm món ăn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowAddMenuDialog();
            UploadImageButton_Click(sender, e);
        }

        /// <summary>
        /// Hiển thị dialog thêm món ăn.
        /// </summary>
        private void ShowAddMenuDialog()
        {
            // Reset dialog fields
            ProductIDTextBox.Text = "";
            NameTextBox.Text = "";
            CategoryTextBox.Text = "";
            PriceTextBox.Text = "";
            DescriptionTextBox.Text = "";
            ImagePathTextBox.Text = "";
            StatusCheckBox.IsChecked = true;

            _ = AddMenuDialog.ShowAsync();
        }

        /// <summary>
        /// Hiển thị thông báo thêm món ăn thành công.
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
        /// Xử lý sự kiện khi người dùng chọn nút lưu món ăn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnSaveMenu(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            int productID = int.Parse(ProductIDTextBox.Text);
            string name = NameTextBox.Text;
            string category = CategoryTextBox.Text;
            int price = int.Parse(PriceTextBox.Text);
            string description = DescriptionTextBox.Text;
            string imagePath = ImagePathTextBox.Text;
            bool status = StatusCheckBox.IsChecked ?? false;

            var newMenu = new Menu
            {
                ProductID = productID,
                Name = name,
                Category = category,
                Price = price,
                Description = description,
                ImagePath = imagePath,
                Status = status
            };

            bool result = ViewModel.AddMenu(newMenu);
            if (result)
            {
                ShowAddSuccessTeachingTip();
                AddMenuDialog.Hide();
            }
            else
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Không thể lưu món ăn. Vui lòng thử lại.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };

                await errorDialog.ShowAsync();
                args.Cancel = true;
            }
        }
        //Handle image choose to use
        private async void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                //Product.ImagePath = file.Path;
            }
        }
        //Load data from invoice to order more dishes

        /// <summary>
        /// Sự kiện khi chuyển trang để load dữ liệu từ hóa đơn đê thêm vào order
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is InvoiceToOrderObject)
            {
                var cart = e.Parameter as InvoiceToOrderObject;
                OrdersUserControl.ViewModel.InvoiceID = cart.InvoiceId;
                OrdersUserControl.ViewModel.InvoiceDate = DateTime.Now;
                foreach (var item in cart.InvoiceDetailToCartItemObjects)
                {
                    OrdersUserControl.AddToOrder(item.Product, item.Quantity, item.Note);
                }
            }
        }
        /// <summary>
        /// Xử lý sự kiện khi người dùng chọn nút thêm món ăn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditMenu(Menu menu)
        {
            ProductIDTextBox.Text = menu.ProductID.ToString();
            NameTextBox.Text = menu.Name;
            CategoryTextBox.Text = menu.Category;
            PriceTextBox.Text = menu.Price.ToString();
            DescriptionTextBox.Text = menu.Description;
            ImagePathTextBox.Text = menu.ImagePath;
            StatusCheckBox.IsChecked = menu.Status;

            _ = EditMenuDialog.ShowAsync();
            UploadImageButton_Click(null, null);
        }

        private async void OnSaveMenu(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            int productID = int.Parse(ProductIDTextBox.Text);
            string name = NameTextBox.Text;
            string category = CategoryTextBox.Text;
            int price = int.Parse(PriceTextBox.Text);
            string description = DescriptionTextBox.Text;
            string imagePath = ImagePathTextBox.Text;
            bool status = StatusCheckBox.IsChecked ?? false;

            var editedMenu = new Menu
            {
                ProductID = productID,
                Name = name,
                Category = category,
                Price = price,
                Description = description,
                ImagePath = imagePath,
                Status = status
            };

            bool result = ViewModel.EditMenu(editedMenu);
            if (result)
            {
                ShowEditSuccessTeachingTip();
                EditMenuDialog.Hide();
            }
            else
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Không thể lưu chỉnh sửa. Vui lòng thử lại.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };

                await errorDialog.ShowAsync();
                args.Cancel = true;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi người dùng chọn ảnh
        /// </summary>
        private async void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);

                    // Convert the byte array to a base64-encoded string
                    var imagePath = Convert.ToBase64String(bytes);

                    // Update the ImagePath property of the Menu object
                    ImagePathTextBox.Text = imagePath;
                }
            }
        }
    }
}
