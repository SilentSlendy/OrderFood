using System;
using Npgsql;
using System.Collections.Generic;
using POS.Interfaces;
using POS.Models;
using POS.Helpers;

namespace POS.Services.DAO
{
    /// <summary>
    /// DAO cho các thao tác liên quan đến InvoiceDetail
    /// </summary>
    public class PostgresInvoiceDetailDao: IInvoiceDetailDao
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PostgresInvoiceDetailDao() { }

        /// <summary>
        /// Lấy tất cả các chi tiết hóa đơn
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rowsPerPage"></param>
        /// <returns></returns>
        public Tuple<int, List<InvoiceDetail>> GetAllInvoiceDetails(int page, int rowsPerPage)
        {
            var invoiceDetails = new List<InvoiceDetail>();
            int totalRecords = 0;

            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var countSql = "SELECT COUNT(*) FROM chitiethoadon";
                var countCommand = new NpgsqlCommand(countSql, connection);
                totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());

                var sql = @"
                    SELECT chitietid, hoadonid, monanid, soluong, giaban, thanhtien, ghichu
                    FROM chitiethoadon
                    WHERE hoadonid = @InvoiceID
                    ORDER BY chitietid
                    LIMIT @ItemsPerPage OFFSET @Offset";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ItemsPerPage", rowsPerPage);
                command.Parameters.AddWithValue("@Offset", (page - 1) * rowsPerPage);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var invoiceDetail = new InvoiceDetail
                        {
                            DetailID = reader.GetInt32(reader.GetOrdinal("chitietid")),
                            InvoiceID = reader.GetInt32(reader.GetOrdinal("hoadonid")),
                            ProductID = reader.GetInt32(reader.GetOrdinal("monanid")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("soluong")),
                            Price = reader.GetInt32(reader.GetOrdinal("giaban")),
                            TotalAmount = reader.GetInt32(reader.GetOrdinal("thanhtien")),
                            Note = reader.IsDBNull(reader.GetOrdinal("ghichu")) ? null : reader.GetString(reader.GetOrdinal("ghichu"))
                        };
                        invoiceDetails.Add(invoiceDetail);
                    }
                }
            }
            return new Tuple<int, List<InvoiceDetail>>(totalRecords, invoiceDetails);
        }

        /// <summary>
        /// Lấy tất cả các chi tiết hóa đơn kèm thông tin sản phẩm
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public Tuple<int, List<InvoiceDetailWithProductInfo>> GetAllInvoiceDetailsWithProductInformation(int invoiceId)
            { 

            var invoiceDetailsWithProductInformation = new List<InvoiceDetailWithProductInfo>();
            int totalRecords = 0;
            var productDao = new PostgresProductDao();
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var countSql = "SELECT COUNT(*) FROM chitiethoadon";
                var countCommand = new NpgsqlCommand(countSql, connection);
                totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());

                var sql = @"
                    SELECT chitietid, hoadonid, chitiethoadon.monanid, soluong, giaban, thanhtien, ghichu, tenmonan, loaimonan, mota
                    FROM 
                    chitiethoadon
                    INNER JOIN 
                    menu 
                    ON 
                    chitiethoadon.monanid = menu.monanid
                    WHERE hoadonid = @InvoiceID AND chitiethoadon.monanid = menu.monanid
                    ORDER BY chitietid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@InvoiceID", invoiceId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var invoiceDetailWithProductInformation = new InvoiceDetailWithProductInfo
                        {
                            InvoiceDetailProperty = new InvoiceDetail
                            {
                                DetailID = reader.GetInt32(reader.GetOrdinal("chitietid")),
                                InvoiceID = reader.GetInt32(reader.GetOrdinal("hoadonid")),
                                ProductID = reader.GetInt32(reader.GetOrdinal("monanid")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("soluong")),
                                Price = reader.GetInt32(reader.GetOrdinal("giaban")),
                                TotalAmount = reader.GetInt32(reader.GetOrdinal("thanhtien")),
                                Note = reader.IsDBNull(reader.GetOrdinal("ghichu")) ? null : reader.GetString(reader.GetOrdinal("ghichu"))
                            }
                            ,
                            ProductName = reader.GetString(reader.GetOrdinal("tenmonan")),
                            CategoryName = reader.GetString(reader.GetOrdinal("loaimonan")),
                            Description = reader.IsDBNull(reader.GetOrdinal("mota")) ? null : reader.GetString(reader.GetOrdinal("mota"))
                            ,
                            ProductInfo = productDao.FindProductByID(reader.GetInt32(reader.GetOrdinal("monanid")))
                        };
                        invoiceDetailsWithProductInformation.Add(invoiceDetailWithProductInformation);
                    }
                }
            }

            return new Tuple<int, List<InvoiceDetailWithProductInfo>>(totalRecords, invoiceDetailsWithProductInformation);
        }

        /// <summary>
        /// Lấy chi tiết hóa đơn theo ID
        /// </summary>
        /// <param name="invoiceDetail"></param>
        /// <returns></returns>
        public int InsertInvoiceDetail(InvoiceDetail invoiceDetail)
        {
            int newId;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                    INSERT INTO chitiethoadon (hoadonid, monanid, soluong, giaban, ghichu)
                    VALUES (@InvoiceID, @ProductID, @Quantity, @Price, @Note)
                    RETURNING chitietid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@InvoiceID", invoiceDetail.InvoiceID);
                command.Parameters.AddWithValue("@ProductID", invoiceDetail.ProductID);
                command.Parameters.AddWithValue("@Quantity", invoiceDetail.Quantity);
                command.Parameters.AddWithValue("@Price", invoiceDetail.Price);
                command.Parameters.AddWithValue("@Note", invoiceDetail.Note ?? (object)DBNull.Value);

                newId = Convert.ToInt32(command.ExecuteScalar());
            }

            return newId;
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết hóa đơn
        /// </summary>
        /// <param name="invoiceDetail"></param>
        /// <returns></returns>
        public bool UpdateInvoiceDetail(InvoiceDetail invoiceDetail)
        {
            int rowsAffected;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the invoice detail exists
                var checkSql = "SELECT COUNT(*) FROM chitiethoadon WHERE chitietid = @DetailID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@DetailID", invoiceDetail.DetailID);
                var detailExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!detailExists)
                {
                    return false; // Invoice detail does not exist, return false
                }

                var sql = @"
                    UPDATE chitiethoadon
                    SET hoadonid = @InvoiceID, monanid = @ProductID, soluong = @Quantity, giaban = @Price, ghichu = @Note
                    WHERE chitietid = @DetailID";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@InvoiceID", invoiceDetail.InvoiceID);
                command.Parameters.AddWithValue("@ProductID", invoiceDetail.ProductID);
                command.Parameters.AddWithValue("@Quantity", invoiceDetail.Quantity);
                command.Parameters.AddWithValue("@Price", invoiceDetail.Price);
                command.Parameters.AddWithValue("@Note", invoiceDetail.Note);
                command.Parameters.AddWithValue("@DetailID", invoiceDetail.DetailID);

                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa một chi tiết hóa đơn theo ID
        /// </summary>
        /// <param name="detailId"></param>
        public void RemoveInvoiceDetailById(int detailId)
        {
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the invoice detail exists
                var checkSql = "SELECT COUNT(*) FROM chitiethoadon WHERE chitietid = @DetailID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@DetailID", detailId);
                var detailExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!detailExists)
                {
                    return;
                }

                var sql = "DELETE FROM chitiethoadon WHERE chitietid = @DetailID";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DetailID", detailId);

                command.ExecuteNonQuery();
            }
        }
    }
}