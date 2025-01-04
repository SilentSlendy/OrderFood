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
    /// ViewModel quản lý logic và dữ liệu của giao diện quản lý nhân viên.

    /// </summary>
    public sealed partial class EmployeeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// DAO để thao tác với bảng nhân viên trong database.
        /// </summary>
        private IEmployeeDao _employeeDao = new PostgresEmployeeDao();

        /// <summary>
        /// Danh sách các nhân viên hiện tại.
        /// </summary>
        public ObservableCollection<Employee> Employees { get; private set; }

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
        /// Tổng số nhân viên.
        /// </summary>
        public int TotalItems { get; set; } = 0;

        /// <summary>
        /// Cột hiện tại được sắp xếp.
        /// </summary>
        private string _currentSortColumn = null;
        /// <summary>
        /// Hướng sắp xếp hiện tại.
        /// </summary>
        private string _currentSortDirection;

        /// <summary>
        /// Khởi tạo ViewModel với việc lấy dữ liệu nhân viên từ database.
        /// </summary>
        public EmployeeViewModel()
        {
            GetAllEmployees();
        }

        /// <summary>
        /// Lấy tất cả nhân viên từ database.
        /// </summary>
        public void GetAllEmployees()
        {
            var (totalItems, employees) = _employeeDao.GetAllEmployees(CurrentPage, RowsPerPage, Keyword, _currentSortColumn, _currentSortDirection);
            Employees = new FullObservableCollection<Employee>(employees);

            TotalItems = totalItems;
            TotalPages = (TotalItems / RowsPerPage)
                + ((TotalItems % RowsPerPage == 0)
                        ? 0 : 1);
        }

        /// <summary>
        /// Lấy dữ liệu nhân viên theo trang.
        /// </summary>
        /// <param name="page"></param>
        public void LoadEmployees(int page)
        {
            CurrentPage = page;
            GetAllEmployees();
        }

        /// <summary>
        /// Thêm một nhân viên mới.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns> Biến bool cho biết thêm thành công hay chưa</returns>
        public bool AddEmployee(Employee employee)
        {
            try
            {
                // Save to database
                int newId = _employeeDao.InsertEmployee(employee);
                employee.EmployeeID = newId;

                // Recall from database to get the full object
                var (totalItems, employees) = _employeeDao.GetAllEmployees(CurrentPage, RowsPerPage, Keyword, _currentSortColumn, _currentSortDirection);
                Employees = new FullObservableCollection<Employee>(employees);

                // Notify the change to UI
                OnPropertyChanged(nameof(Employees));

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
        /// Nhân viên được chọn từ giao diện.
        /// </summary>
        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (_selectedEmployee != value)
                {
                    if (_selectedEmployee != null)
                    {
                        _selectedEmployee.PropertyChanged -= OnEmployeePropertyChanged;
                    }

                    _selectedEmployee = value;

                    if (_selectedEmployee != null)
                    {
                        _selectedEmployee.PropertyChanged += OnEmployeePropertyChanged;
                    }

                    OnPropertyChanged(nameof(SelectedEmployee));
                }
            }
        }

        /// <summary>
        /// Xóa một nhân viên.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool DeleteEmployee(Employee employee)
        {
            try
            {
                // Delete from database
                _employeeDao.RemoveEmployeeById(employee.EmployeeID);

                // Delete from the displayed list
                Employees.Remove(employee);
                OnPropertyChanged(nameof(Employees));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when delete: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật một nhân viên.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEmployeePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var employee = sender as Employee;
            if (employee != null)
            {
                // Update the database with the changed property
                UpdateEmployeeInDatabase(employee);
            }
        }

        /// <summary>
        /// Cập nhật nhân viên trong database.
        /// </summary>
        /// <param name="employee"></param>
        private void UpdateEmployeeInDatabase(Employee employee)
        {
            try
            {
                _employeeDao.UpdateEmployee(employee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when updating employee: {ex.Message}");
            }
        }

        /// <summary>
        /// Sắp xếp nhân viên theo cột.
        /// </summary>
        /// <param name="columnName"></param>
        public void SortEmployees(string columnName)
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
            LoadEmployees(CurrentPage);
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