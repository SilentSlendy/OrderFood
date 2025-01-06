using System.Collections.ObjectModel;
using System.ComponentModel;
using POS.Models;
using POS.Services.DAO;
using POS.Interfaces;
using System;
using POS.Helpers;


namespace POS.ViewModels
{
    /// <summary>
    /// ViewModel quản lý logic và dữ liệu của giao diện quản lý khuyến mãi.
    /// </summary>
    public sealed partial class DiscountViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// DAO để thao tác với bảng khuyến mãi trong database.
        /// </summary>
        private IDiscountDao _discountDao = new PostgresDiscountDao();
        /// <summary>
        /// Danh sách các khuyến mãi hiện tại.
        /// </summary>
        public ObservableCollection<Discount> Discounts { get; private set; }
        /// <summary>
        /// Mã hóa và giải mã chuỗi.
        /// </summary>
        private FeistelCipher _feistelCipher = new FeistelCipher(8, "winui3_discount_key");

        /// <summary>
        /// Trang hiện tại dùng để phân trang.
        /// </summary>
        public int CurrentPage { get; set; } = 1;
        /// <summary>
        /// Số hàng hiển thị trên mỗi trang.
        /// </summary>
        public int RowsPerPage { get; set; } = 10;
        /// <summary>
        /// Tổng số trang dùng để phân trang.
        /// </summary>
        public int TotalPages { get; set; } = 0;
        /// <summary>
        /// Tổng số khuyến mãi.
        /// </summary>
        public int TotalItems { get; set; } = 0;

        /// <summary>
        /// Giá trị của mã giảm giá.
        /// </summary>
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Số lượng mã giảm giá cần tạo.
        /// </summary>
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        /// <summary>
        /// Mã giảm giá được chọn.
        /// </summary>
        private Discount _selectedDiscount;
        public Discount SelectedDiscount
        {
            get { return _selectedDiscount; }
            set
            {
                if (_selectedDiscount != value)
                {
                    _selectedDiscount = value;
                    OnPropertyChanged(nameof(SelectedDiscount));
                }
            }
        }

        /// <summary>
        /// Khởi tạo ViewModel với việc lấy dữ liệu khuyến mãi từ database.
        /// </summary>
        public DiscountViewModel()
        {
            Discounts = new ObservableCollection<Discount>();
            GetAllDiscount();
        }

        /// <summary>
        /// Lấy tất cả khuyến mãi từ database.
        /// </summary>
        public void GetAllDiscount()
        {
            var (totalItems, discounts) = _discountDao.GetAllDiscount(CurrentPage, RowsPerPage);

            Discounts.Clear();
            foreach (var discount in discounts)
            {
                Discounts.Add(discount);
            }

            TotalItems = totalItems;
            TotalPages = (TotalItems / RowsPerPage) + ((TotalItems % RowsPerPage == 0) ? 0 : 1);

            OnPropertyChanged(nameof(Discounts));
            OnPropertyChanged(nameof(TotalPages));
        }

        /// <summary>
        /// Lấy dữ liệu khuyến mãi ở trang cụ thể.
        /// </summary>
        /// <param name="page"></param>
        public void LoadDiscounts(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                GetAllDiscount();
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        /// <summary>
        /// Tạo mã giảm giá.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool CreateDiscounts(int value, int quantity)
        {
            bool result = true;

            for (int i = 0; i < quantity; i++)
            {
                string discountCode = _feistelCipher.Encrypt(Value);
                if (_discountDao.InsertDiscount(discountCode, value) < 0)
                {
                    result = false;
                }
            }
            GetAllDiscount();
            OnPropertyChanged(nameof(Discounts));

            return result;
        }

        /// <summary>
        /// Xóa một mã giảm giá.
        /// </summary>
        /// <param name="discount"></param>
        /// <returns></returns>
        public bool DeleteDiscount(Discount discount)
        {
            try
            {
                _discountDao.RemoveDiscountByCode(discount.DiscountCode);
                Discounts.Remove(discount);
                OnPropertyChanged(nameof(Discounts));

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting discount: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sự kiện khi thuộc tính thay đổi.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Phát sự kiện khi thuộc tính thay đổi.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}