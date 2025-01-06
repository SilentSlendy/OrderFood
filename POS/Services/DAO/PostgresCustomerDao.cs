using System;
using Npgsql;
using System.Collections.Generic;
using POS.Interfaces;
using POS.Models;
using POS.Helpers;

namespace POS.Services.DAO
{
    /// <summary>
    /// DAO cho các thao tác liên quan đến khách hàng
    /// </summary>
    public class PostgresCustomerDao : ICustomerDao
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PostgresCustomerDao() { }

        /// <summary>
        /// Lấy tất cả khách hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="customerType"></param>
        /// <returns></returns>
        public Tuple<int, List<Customer>> GetAllCustomers(int page, int rowsPerPage, string searchKeyword, string customerType)
        {
            var customers = new List<Customer>();
            int totalItems = 0;

            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                SELECT COUNT(*) OVER() AS TotalItems, khachhangid, tenkhachhang, sodienthoai, email, diachi, loaikhachhang
                FROM khachhang
                WHERE tenkhachhang ILIKE @SearchKeyword and loaikhachhang ILIKE @CustomerType
                OFFSET @Skip LIMIT @Take";

                var skip = (page - 1) * rowsPerPage;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", rowsPerPage);
                command.Parameters.AddWithValue("@SearchKeyword", $"%{searchKeyword}%");
                command.Parameters.AddWithValue("@CustomerType", $"%{customerType}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (totalItems == 0)
                        {
                            totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                        }

                        var customer = new Customer
                        {
                            CustomerID = reader.GetInt32(reader.GetOrdinal("khachhangid")),
                            Name = reader.GetString(reader.GetOrdinal("tenkhachhang")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("sodienthoai")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Address = reader.GetString(reader.GetOrdinal("diachi")),
                            CustomerType = reader.GetString(reader.GetOrdinal("loaikhachhang"))
                        };
                        customers.Add(customer);
                    }
                }
            }

            return new Tuple<int, List<Customer>>(totalItems, customers);
        }

        //=======================================================================================================
        //Get all Customers
        /// <summary>
        /// Lấy tất cả khách hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="position"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public Tuple<int, List<Customer>> GetAllCustomers(
            int page = 1,
            int rowsPerPage = 10,
            string searchKeyword = "",
            string position = "",
            string sortDirection = null
        )
        {
            var customers = new List<Customer>();
            int totalItems = 0;

            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                SELECT COUNT(*) OVER() AS TotalItems, khachhangid, tenkhachhang, sodienthoai, email, diachi, loaikhachhang
                FROM khachhang
                WHERE tenkhachhang ILIKE @SearchKeyword
                OFFSET @Skip LIMIT @Take";

                var skip = (page - 1) * rowsPerPage;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", rowsPerPage);
                command.Parameters.AddWithValue("@SearchKeyword", $"%{searchKeyword}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (totalItems == 0)
                        {
                            totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                        }

                        var customer = new Customer
                        {
                            CustomerID = reader.GetInt32(reader.GetOrdinal("khachhangid")),
                            Name = reader.GetString(reader.GetOrdinal("tenkhachhang")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("sodienthoai")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Address = reader.GetString(reader.GetOrdinal("diachi")),
                            CustomerType = reader.GetString(reader.GetOrdinal("loaikhachhang"))
                        };
                        customers.Add(customer);
                    }
                }
            }

            return new Tuple<int, List<Customer>>(totalItems, customers);
        }

        public List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();
                var sql = @"
                SELECT khachhangid, tenkhachhang, sodienthoai, email, diachi, loaikhachhang
                FROM khachhang";
                var command = new NpgsqlCommand(sql, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var customer = new Customer
                        {
                            CustomerID = reader.GetInt32(reader.GetOrdinal("khachhangid")),
                            Name = reader.GetString(reader.GetOrdinal("tenkhachhang")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("sodienthoai")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Address = reader.GetString(reader.GetOrdinal("diachi")),
                            CustomerType = reader.GetString(reader.GetOrdinal("loaikhachhang"))
                        };
                        customers.Add(customer);
                    }
                }
            }
            return customers;
        }
        //=======================================================================================================
        // Thêm sản phẩm mới


        /// <summary>
        /// Thêm khách hàng mới
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>

        public int InsertCustomer(Customer customer)
        {
            int newId;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                INSERT INTO khachhang (tenkhachhang, sodienthoai, email, diachi, loaikhachhang)
                VALUES (@Name, @PhoneNumber, @Email, @Address, @CustomerType)
                RETURNING khachhangid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", customer.Name);
                command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@Address", customer.Address);
                command.Parameters.AddWithValue("@CustomerType", customer.CustomerType);

                newId = Convert.ToInt32(command.ExecuteScalar());
            }

            return newId;
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdateCustomer(Customer customer)
        {
            int rowsAffected;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the customer exists
                var checkSql = "SELECT COUNT(*) FROM khachhang WHERE khachhangid = @CustomerID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                var customerExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!customerExists)
                {
                    return false; // Customer does not exist, return false
                }

                var sql = @"
                UPDATE khachhang
                SET tenkhachhang = @Name, sodienthoai = @PhoneNumber, email = @Email, diachi = @Address, loaikhachhang = @CustomerType
                WHERE khachhangid = @CustomerID";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", customer.Name);
                command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@Address", customer.Address);
                command.Parameters.AddWithValue("@CustomerType", customer.CustomerType);
                command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);

                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa khách hàng theo ID
        /// </summary>
        /// <param name="customerId"></param>
        public void RemoveCustomerById(int customerId)
        {
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the customer exists
                var checkSql = "SELECT COUNT(*) FROM khachhang WHERE khachhangid = @CustomerID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@CustomerID", customerId);
                var customerExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!customerExists)
                {
                    return;
                }

                var sql = "DELETE FROM khachhang WHERE khachhangid = @CustomerID";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CustomerID", customerId);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Lấy tên khách hàng theo ID
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public string GetCustomerNameById(int customerId)
        {
            string customerName = "";
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();
                var sql = @"
                SELECT tenkhachhang
                FROM khachhang
                WHERE khachhangid = @CustomerID";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CustomerID", customerId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customerName = reader.GetString(reader.GetOrdinal("tenkhachhang"));
                    }
                }
            }
            return customerName;
        }
    }
}