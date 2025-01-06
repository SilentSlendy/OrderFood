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
    /// ViewModel quản lý logic và dữ liệu của giao diện quản lý kho hàng.
    /// </summary>
    public sealed partial class WarehouseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// DAO để thao tác với bảng kho hàng trong database.
        /// </summary>
        private IWarehouseDao _warehouseDao = new PostgresWarehouseDao();

        /// <summary>
        /// Danh sách các kho hàng hiện tại.
        /// </summary>
        public ObservableCollection<Warehouse> Warehouses { get; private set; }

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
        /// Tổng số kho hàng.
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
        /// Khởi tạo ViewModel với việc lấy dữ liệu kho hàng từ database.
        /// </summary>
        public WarehouseViewModel()
        {
            GetAllWarehouses();
        }

        /// <summary>
        /// Lấy tất cả kho hàng từ database.
        /// </summary>
        public void GetAllWarehouses()
        {
            var (totalItems, warehouses) = _warehouseDao.GetAllWarehouses(CurrentPage, RowsPerPage, Keyword, _currentSortColumn, _currentSortDirection);
            Warehouses = new FullObservableCollection<Warehouse>(warehouses);

            TotalItems = totalItems;
            TotalPages = (TotalItems / RowsPerPage)
                + ((TotalItems % RowsPerPage == 0)
                        ? 0 : 1);
        }

        /// <summary>
        /// Lấy dữ liệu kho hàng theo trang.
        /// </summary>
        /// <param name="page"></param>
        public void LoadWarehouses(int page)
        {
            CurrentPage = page;
            GetAllWarehouses();
        }

        /// <summary>
        /// Thêm một kho hàng mới.
        /// </summary>
        /// <param name="warehouse"></param>
        /// <returns> Biến bool cho biết thêm thành công hay chưa</returns>
        public bool AddWarehouse(Warehouse warehouse)
        {
            try
            {
                // Save to database
                int newId = _warehouseDao.InsertWarehouse(warehouse);
                warehouse.WarehouseID = newId;

                // Recall from database to get the full object
                var (totalItems, warehouses) = _warehouseDao.GetAllWarehouses(CurrentPage, RowsPerPage, Keyword, _currentSortColumn, _currentSortDirection);
                Warehouses = new FullObservableCollection<Warehouse>(warehouses);

                // Notify the change to UI
                OnPropertyChanged(nameof(Warehouses));

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
        private Warehouse _selectedWarehouse;
        public Warehouse SelectedWarehouse
        {
            get => _selectedWarehouse;
            set
            {
                if (_selectedWarehouse != value)
                {
                    if (_selectedWarehouse != null)
                    {
                        _selectedWarehouse.PropertyChanged -= OnWarehousePropertyChanged;
                    }

                    _selectedWarehouse = value;

                    if (_selectedWarehouse != null)
                    {
                        _selectedWarehouse.PropertyChanged += OnWarehousePropertyChanged;
                    }

                    OnPropertyChanged(nameof(SelectedWarehouse));
                }
            }
        }

        /// <summary>
        /// Xóa một kho hàng.
        /// </summary>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        public bool DeleteWarehouse(Warehouse warehouse)
        {
            try
            {
                // Delete from database
                _warehouseDao.RemoveWarehouseById(warehouse.WarehouseID);

                // Delete from the displayed list
                Warehouses.Remove(warehouse);
                OnPropertyChanged(nameof(Warehouses));
                // Recall the data
                LoadWarehouses(CurrentPage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when delete: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật một kho hàng.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWarehousePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var warehouse = sender as Warehouse;
            if (warehouse != null)
            {
                // Update the database with the changed property
                UpdateWarehouseInDatabase(warehouse);
            }
        }

        /// <summary>
        /// Cập nhật kho hàng trong database.
        /// </summary>
        /// <param name="warehouse"></param>
        private void UpdateWarehouseInDatabase(Warehouse warehouse)
        {
            try
            {
                _warehouseDao.UpdateWarehouse(warehouse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when updating warehouse: {ex.Message}");
            }
        }

        /// <summary>
        /// Sắp xếp kho hàng theo cột.
        /// </summary>
        /// <param name="columnName"></param>
        public void SortWarehouses(string columnName)
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
            LoadWarehouses(CurrentPage);
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