using System;
using Npgsql;
using System.Collections.Generic;
using POS.Interfaces;
using POS.Models;
using POS.Helpers;
using System.Diagnostics;

namespace POS.Services.DAO
{
    /// <summary>
    /// DAO cho các thao tác liên quan đến Warehouse
    /// </summary>
    public class PostgresWarehouseDao : IWarehouseDao
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PostgresWarehouseDao() { }

        /// <summary>
        /// Lấy tất cả các kho hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public Tuple<int, List<Warehouse>> GetAllWarehouses(int page, int rowsPerPage, string searchKeyword, string sortColumn = null, string sortDirection = null)
        {
            var warehouses = new List<Warehouse>();
            int totalRecords = 0;

            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Đếm tổng số bản ghi
                var countSql = "SELECT COUNT(*) FROM khohang WHERE tennguyenlieu ILIKE @SearchKeyword";
                var countCommand = new NpgsqlCommand(countSql, connection);
                countCommand.Parameters.AddWithValue("@SearchKeyword", $"%{searchKeyword}%");
                totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());

                // Xây dựng câu SQL chính

                var sql = @"
                    SELECT khoid, tennguyenlieu, soluongton, donvitinh, ngaynhapkho, ngayhethan
                    FROM khohang
                    WHERE tennguyenlieu ILIKE @SearchKeyword
                ";

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortDirection))
                {
                    if (sortColumn == "EntryDate") sortColumn = "ngaynhapkho";
                    else if (sortColumn == "ExpirationDate") sortColumn = "ngayhethan";

                    sql += $" ORDER BY {sortColumn} {sortDirection}";
                }

                sql += @" LIMIT @ItemsPerPage OFFSET @Offset";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchKeyword", $"%{searchKeyword}%");
                command.Parameters.AddWithValue("@ItemsPerPage", rowsPerPage);
                command.Parameters.AddWithValue("@Offset", (page - 1) * rowsPerPage);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var warehouse = new Warehouse
                        {
                            WarehouseID = reader.GetInt32(reader.GetOrdinal("khoid")),
                            IngredientName = reader.GetString(reader.GetOrdinal("tennguyenlieu")),
                            StockQuantity = reader.GetInt32(reader.GetOrdinal("soluongton")),
                            Unit = reader.GetString(reader.GetOrdinal("donvitinh")),
                            EntryDate = reader.GetDateTime(reader.GetOrdinal("ngaynhapkho")),
                            ExpirationDate = reader.GetDateTime(reader.GetOrdinal("ngayhethan"))
                        };
                        warehouses.Add(warehouse);
                    }
                }
            }

            return new Tuple<int, List<Warehouse>>(totalRecords, warehouses);
        }

        /// <summary>
        /// Thêm một kho hàng mới
        /// </summary>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        public int InsertWarehouse(Warehouse warehouse)
        {
            int newId;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                    INSERT INTO khohang (tennguyenlieu, soluongton, donvitinh, ngaynhapkho, ngayhethan)
                    VALUES (@IngredientName, @StockQuantity, @Unit, @EntryDate, @ExpirationDate)
                    RETURNING khoid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IngredientName", warehouse.IngredientName);
                command.Parameters.AddWithValue("@StockQuantity", warehouse.StockQuantity);
                command.Parameters.AddWithValue("@Unit", warehouse.Unit);
                command.Parameters.AddWithValue("@EntryDate", warehouse.EntryDate);
                command.Parameters.AddWithValue("@ExpirationDate", warehouse.ExpirationDate);

                newId = Convert.ToInt32(command.ExecuteScalar());
            }

            return newId;
        }

        /// <summary>
        /// Lấy thông tin kho hàng theo ID
        /// </summary>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        public bool UpdateWarehouse(Warehouse warehouse)
        {
            int rowsAffected;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the warehouse exists
                var checkSql = "SELECT COUNT(*) FROM khohang WHERE khoid = @WarehouseID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@WarehouseID", warehouse.WarehouseID);
                var warehouseExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!warehouseExists)
                {
                    return false; // Warehouse does not exist, return false
                }

                var sql = @"
                    UPDATE khohang
                    SET tennguyenlieu = @IngredientName, soluongton = @StockQuantity, donvitinh = @Unit, ngaynhapkho = @EntryDate, ngayhethan = @ExpirationDate
                    WHERE khoid = @WarehouseID";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IngredientName", warehouse.IngredientName);
                command.Parameters.AddWithValue("@StockQuantity", warehouse.StockQuantity);
                command.Parameters.AddWithValue("@Unit", warehouse.Unit);
                command.Parameters.AddWithValue("@EntryDate", warehouse.EntryDate);
                command.Parameters.AddWithValue("@ExpirationDate", warehouse.ExpirationDate);
                command.Parameters.AddWithValue("@WarehouseID", warehouse.WarehouseID);

                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa một kho hàng theo ID
        /// </summary>
        /// <param name="warehouseId"></param>
        public void RemoveWarehouseById(int warehouseId)
        {
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the warehouse exists
                var checkSql = "SELECT COUNT(*) FROM khohang WHERE khoid = @WarehouseID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@WarehouseID", warehouseId);
                var warehouseExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!warehouseExists)
                {
                    return;
                }

                var sql = "DELETE FROM khohang WHERE khoid = @WarehouseID";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@WarehouseID", warehouseId);

                command.ExecuteNonQuery();
            }
        }
    }
}