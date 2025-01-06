using System.Collections.ObjectModel;
using System.ComponentModel;
using POS.Models;
using POS.Services.DAO;
using POS.Interfaces;
using System;

namespace POS.ViewModels
{
    /// <summary>
    /// View model for Product
    /// </summary>
    public sealed partial class ProductViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Dao cho Product
        /// </summary>
        private IProductDao _productDao;
        /// <summary>
        /// Danh sách sản phẩm
        /// </summary>
        public ObservableCollection<Product> Products { get; private set; }
        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string searchText;
        /// <summary>
        /// Danh mục sản phẩm được chọn
        /// </summary>
        public string selectedCategory = "";
        /// <summary>
        /// Thứ tự sắp xếp được chọn
        /// </summary>
        public int selectedSortOrder = 0;

        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string Keyword { get; set; } = "";
        /// <summary>
        /// Sắp xếp theo tên tăng dần
        /// </summary>
        public bool NameAcending { get; set; } = false;
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; } = 1;
        /// <summary>
        /// Số hàng trên mỗi trang
        /// </summary>
        public int RowsPerPage { get; set; } = 4;
        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; } = 0;
        /// <summary>
        /// Tổng số sản phẩm
        /// </summary>
        public int TotalItems { get; set; } = 0;

        /// <summary>
        /// Khởi tạo một đối tượng ProductViewModel
        /// </summary>
        public ProductViewModel()
        {
            _productDao = new PostgresProductDao();
            GetAllProducts();
        }

        /// <summary>
        /// Lấy tất cả sản phẩm
        /// </summary>
        public void GetAllProducts()
        {
            var (totalItems, products) = _productDao.GetAllProducts(

                CurrentPage, RowsPerPage, Keyword,selectedCategory, selectedSortOrder);

            Products = new FullObservableCollection<Product>(
                products
            );

            TotalItems = totalItems;
            TotalPages = (TotalItems / RowsPerPage)
                + ((TotalItems % RowsPerPage == 0)
                        ? 0 : 1);
        }

        /// <summary>
        /// Lấy sản phẩm theo trang
        /// </summary>
        /// <param name="page"></param>
        public void LoadProducts(int page)
        {
            CurrentPage = page;
            GetAllProducts();
        }

        /// <summary>
        /// Sự kiện PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}