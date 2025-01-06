using System;
using Npgsql;
using System.Collections.Generic;
using POS.Interfaces;
using POS.Models;
using POS.Helpers;

namespace POS.Services.DAO
{
    /// <summary>
    /// DAO cho các thao tác liên quan đến RevenueReport
    /// </summary>
    public class PostgresRevenueReportDao : IRevenueReportDao
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PostgresRevenueReportDao() { }

        /// <summary>
        /// Lấy tất cả các báo cáo doanh thu
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rowsPerPage"></param>
        /// <returns></returns>
        public Tuple<int, List<RevenueReport>> GetAllRevenueReports(int page, int rowsPerPage)
        {
            var reports = new List<RevenueReport>();
            int totalItems = 0;

            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                    SELECT COUNT(*) OVER() AS TotalItems, baocaoid, ngaybaocao, doanhthu, hoadonid
                    FROM baocaodoanhthu
                    OFFSET @Skip LIMIT @Take";

                var skip = (page - 1) * rowsPerPage;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", rowsPerPage);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (totalItems == 0)
                        {
                            totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                        }

                        var report = new RevenueReport
                        {
                            ReportID = reader.GetInt32(reader.GetOrdinal("baocaoid")),
                            ReportDate = reader.GetDateTime(reader.GetOrdinal("ngaybaocao")),
                            Revenue = reader.GetInt32(reader.GetOrdinal("doanhthu")),
                            InvoiceID = reader.GetInt32(reader.GetOrdinal("hoadonid"))
                        };
                        reports.Add(report);
                    }
                }
            }

            return new Tuple<int, List<RevenueReport>>(totalItems, reports);
        }

        /// <summary>
        /// Thêm báo cáo doanh thu mới
        /// </summary>
        /// <param name="revenueReport"></param>
        /// <returns></returns>
        public int InsertRevenueReport(RevenueReport revenueReport)
        {
            int newId;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                    INSERT INTO baocaodoanhthu (ngaybaocao, doanhthu, hoadonid)
                    VALUES (@ngaybaocao, @doanhthu, @hoadonid)
                    RETURNING baocaoid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ngaybaocao", revenueReport.ReportDate);
                command.Parameters.AddWithValue("@doanhthu", revenueReport.Revenue);
                command.Parameters.AddWithValue("@hoadonid", revenueReport.InvoiceID);

                newId = Convert.ToInt32(command.ExecuteScalar());
            }

            return newId;
        }

        /// <summary>
        /// Cập nhật báo cáo doanh thu
        /// </summary>
        /// <param name="revenueReport"></param>
        /// <returns></returns>
        public bool UpdateRevenueReport(RevenueReport revenueReport)
        {
            int rowsAffected;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the report exists
                var checkSql = "SELECT COUNT(*) FROM baocaodoanhthu WHERE baocaoid = @baocaoid";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@baocaoid", revenueReport.ReportID);
                var reportExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!reportExists)
                {
                    return false; // Report does not exist, return false
                }

                var sql = @"
                    UPDATE baocaodoanhthu
                    SET ngaybaocao = @ngaybaocao, doanhthu = @doanhthu, hoadonid = @hoadonid
                    WHERE baocaoid = @baocaoid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ngaybaocao", revenueReport.ReportDate);
                command.Parameters.AddWithValue("@doanhthu", revenueReport.Revenue);
                command.Parameters.AddWithValue("@hoadonid", revenueReport.InvoiceID);
                command.Parameters.AddWithValue("@baocaoid", revenueReport.ReportID);

                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa báo cáo doanh thu theo ID
        /// </summary>
        /// <param name="revenueReportId"></param>
        public void RemoveRevenueReportById(int revenueReportId)
        {
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the report exists
                var checkSql = "SELECT COUNT(*) FROM baocaodoanhthu WHERE baocaoid = @baocaoid";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@baocaoid", revenueReportId);
                var reportExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!reportExists)
                {
                    return;
                }

                var sql = "DELETE FROM baocaodoanhthu WHERE baocaoid = @baocaoid";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@baocaoid", revenueReportId);

                command.ExecuteNonQuery();
            }
        }
    }
}