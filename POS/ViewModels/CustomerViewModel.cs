using System.Collections.ObjectModel;
using System.ComponentModel;
using POS.Models;
using POS.Services.DAO;
using POS.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;


namespace POS.ViewModels
{
    /// <summary>
    /// ViewModel quản lý logic và dữ liệu của giao diện quản lý khách hàng.
    /// </summary>
    public sealed partial class CustomerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// DAO để thao tác với bảng khách hàng trong database.
        /// </summary>
        private ICustomerDao _customerDao = new PostgresCustomerDao();

        /// <summary>
        /// Danh sách các khách hàng hiện tại.
        /// </summary>
        public ObservableCollection<Customer> Customers { get; private set; }

        /// <summary>
        /// Từ khóa tìm kiếm được nhập từ giao diện.
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// Trang hiện tại dùng để phân trang.
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Sô hàng hiển thị trên mỗi trang.
        /// </summary>
        public int RowsPerPage { get; set; } = 10;

        /// <summary>
        /// Tổng số trang dùng để phân trang.
        /// </summary>
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// Tổng số khách hàng.
        /// </summary>
        public int TotalItems { get; set; } = 0;

        /// <summary>
        /// Cột hiện tại được sắp xếp.
        /// </summary>
        private string _currentSortColumn = null;
        /// <summary>
        /// Hướng sắp xếp hiện tại.
        /// </summary>
        private string _currentSortDirection = null;

        /// <summary>
        /// Khởi tạo ViewModel với việc lấy dữ liệu khách hàng từ database.
        /// </summary>
        public CustomerViewModel()
        {
            GetAllCustomers();
        }

        /// <summary>
        /// Lấy tất cả khách hàng từ database.
        /// </summary>
        public void GetAllCustomers()
        {
            var (totalItems, customers) = _customerDao.GetAllCustomers(CurrentPage, RowsPerPage, Keyword, _currentSortColumn, _currentSortDirection);
            Customers = new FullObservableCollection<Customer>(customers);

            TotalItems = totalItems;
            TotalPages = (TotalItems / RowsPerPage)
                + ((TotalItems % RowsPerPage == 0)
                        ? 0 : 1);
        }

        /// <summary>
        /// Lấy dữ liệu khách hàng theo trang.
        /// </summary>
        /// <param name="page"></param>
        public void LoadCustomers(int page)
        {
            CurrentPage = page;
            GetAllCustomers();
        }

        /// <summary>
        /// Thêm một khách hàng mới.
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns> Biến bool cho biết thêm thành công hay chưa</returns>
        public bool AddCustomer(Customer customer)
        {
            try
            {
                // Save to database
                int newId = _customerDao.InsertCustomer(customer);
                customer.CustomerID = newId;

                // Recall from database to get the full object
                var (totalItems, customers) = _customerDao.GetAllCustomers(CurrentPage, RowsPerPage, Keyword, _currentSortColumn, _currentSortDirection);
                Customers = new FullObservableCollection<Customer>(customers);

                // Notify the change to UI
                OnPropertyChanged(nameof(Customers));

                return true;
            }
            catch (Exception ex)
            {
                // Log err if failed
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Kho hàng được chọn từ giao diện.
        /// </summary>
        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (_selectedCustomer != value)
                {
                    if (_selectedCustomer != null)
                    {
                        _selectedCustomer.PropertyChanged -= OnCustomerPropertyChanged;
                    }

                    _selectedCustomer = value;

                    if (_selectedCustomer != null)
                    {
                        _selectedCustomer.PropertyChanged += OnCustomerPropertyChanged;
                    }

                    OnPropertyChanged(nameof(SelectedCustomer));
                }
            }
        }

        /// <summary>
        /// Xóa một khách hàng.
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns></returns>
        public bool DeleteCustomer(Customer customer)
        {
            try
            {
                // Delete from database
                _customerDao.RemoveCustomerById(customer.CustomerID);

                // Delete from the displayed list
                Customers.Remove(customer);
                OnPropertyChanged(nameof(Customers));
                // Recall the data
                LoadCustomers(CurrentPage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when delete: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật một khách hàng.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCustomerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var customer = sender as Customer;
            if (customer != null)
            {
                // Update the database with the changed property
                UpdateCustomerInDatabase(customer);
            }
        }

        /// <summary>
        /// Cập nhật khách hàng trong database.
        /// </summary>
        /// <param name="customer"></param>
        private void UpdateCustomerInDatabase(Customer customer)
        {
            try
            {
                _customerDao.UpdateCustomer(customer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when updating customer: {ex.Message}");
            }
        }

        /// <summary>
        /// Sắp xếp khách hàng theo cột.
        /// </summary>
        /// <param name="columnName"></param>
        public void SortCustomers(string columnName)
        {
            if (columnName == _currentSortColumn)
            {
                // Change sort direction
                _currentSortDirection = _currentSortDirection switch
                {
                    null => "ASC",   
                    "ASC" => "DESC", 
                    "DESC" => null, 
                    _ => null
                };
            }
            else
            {
                // Change sort column
                _currentSortColumn = columnName;
                _currentSortDirection = "ASC";
            }

            // Recall the data
            LoadCustomers(CurrentPage);
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