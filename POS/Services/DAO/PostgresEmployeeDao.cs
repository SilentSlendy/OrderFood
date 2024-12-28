﻿using System;
using Npgsql;
using System.Collections.Generic;
using POS.Interfaces;
using POS.Models;
using POS.Helpers;

namespace POS.Services.DAO
{
    /// <summary>
    /// DAO cho các thao tác liên quan đến Employee
    /// </summary>
    public class PostgresEmployeeDao : IEmployeeDao
    {
        /// <summary>
        ///     
        /// </summary>
        public PostgresEmployeeDao() { }

        /// <summary>
        /// Lấy tất cả các nhân viên
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="position"></param>
        /// <param name="isSalarySort"></param>
        /// <returns></returns>
        public Tuple<int, List<Employee>> GetAllEmployees(int page, int rowsPerPage, string searchKeyword, string position, string isSalarySort)
        {
            var employees = new List<Employee>();
            int totalItems = 0;

            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                SELECT COUNT(*) OVER() AS TotalItems, nhanvienid, tennhanvien, chucvu, luong, ngayvaolam, trangthai
                FROM nhanvien
                WHERE tennhanvien ILIKE @SearchKeyword AND chucvu ILIKE @Position
                OFFSET @Skip LIMIT @Take";

                var skip = (page - 1) * rowsPerPage;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", rowsPerPage);
                command.Parameters.AddWithValue("@SearchKeyword", $"%{searchKeyword}%");
                command.Parameters.AddWithValue("@Position", $"%{position}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (totalItems == 0)
                        {
                            totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                        }

                        var employee = new Employee
                        {
                            EmployeeID = reader.GetInt32(reader.GetOrdinal("nhanvienid")),
                            Name = reader.GetString(reader.GetOrdinal("tennhanvien")),
                            Position = reader.GetString(reader.GetOrdinal("chucvu")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("luong")),
                            HireDate = reader.GetDateTime(reader.GetOrdinal("ngayvaolam")),
                            Status = reader.GetBoolean(reader.GetOrdinal("trangthai"))
                        };
                        employees.Add(employee);
                    }
                }
            }

            return new Tuple<int, List<Employee>>(totalItems, employees);
        }

        /// <summary>
        /// Thêm một nhân viên mới
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public int InsertEmployee(Employee employee)
        {
            int newId;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                INSERT INTO nhanvien (tennhanvien, chucvu, luong, ngayvaolam, trangthai)
                VALUES (@Name, @Position, @Salary, @HireDate, @Status)
                RETURNING nhanvienid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Position", employee.Position);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                command.Parameters.AddWithValue("@Status", employee.Status);

                newId = Convert.ToInt32(command.ExecuteScalar());
            }

            return newId;
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool UpdateEmployee(Employee employee)
        {
            int rowsAffected;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the employee exists
                var checkSql = "SELECT COUNT(*) FROM nhanvien WHERE nhanvienid = @EmployeeID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                var employeeExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!employeeExists)
                {
                    return false; // Employee does not exist, return false
                }

                var sql = @"
                UPDATE nhanvien
                SET tennhanvien = @Name, chucvu = @Position, luong = @Salary, ngayvaolam = @HireDate, trangthai = @Status
                WHERE nhanvienid = @EmployeeID";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Position", employee.Position);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                command.Parameters.AddWithValue("@Status", employee.Status);
                command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);

                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa một nhân viên theo ID
        /// </summary>
        /// <param name="employeeId"></param>
        public void RemoveEmployeeById(int employeeId)
        {
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the employee exists
                var checkSql = "SELECT COUNT(*) FROM nhanvien WHERE nhanvienid = @EmployeeID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@EmployeeID", employeeId);
                var employeeExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!employeeExists)
                {
                    return;
                }

                var sql = "DELETE FROM nhanvien WHERE nhanvienid = @EmployeeID";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeId);

                command.ExecuteNonQuery();
            }
        }
    }
}