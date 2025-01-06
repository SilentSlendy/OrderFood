using System;
using Npgsql;
using System.Collections.Generic;
using POS.Interfaces;
using POS.Models;
using POS.Helpers;
using static System.Net.Mime.MediaTypeNames;


namespace POS.Services.DAO
{
    /// <summary>
    /// DAO cho các thao tác liên quan đến Product
    /// </summary>
    public class PostgresProductDao : IProductDao
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PostgresProductDao() { }

        /// <summary>
        /// Lấy tất cả các sản phẩm
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="categoryType"></param>
        /// <param name="isPriceSort"></param>
        /// <returns></returns>
        public Tuple<int, List<Product>> GetAllProducts(int page, int rowsPerPage, string searchKeyword, string categoryType, int isPriceSort)
        {
            var products = new List<Product>();
            int totalItems = 0;

            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                SELECT COUNT(*) OVER() AS TotalItems, monanid, tenmonan, loaimonan, gia, mota, imagepath, trangthai 
                FROM menu
                WHERE tenmonan ILIKE @SearchKeyword
                ";

                if (categoryType == "")
                {
                    // do nothing
                }
                else if (categoryType == "Đồ uống")
                    sql += " AND loaimonan = 'Đồ uống'";
                else
                    sql += @" AND loaimonan = @CategoryType";

                if (isPriceSort == 1)
                    sql += " ORDER BY gia ASC ";
                else if (isPriceSort == 2)
                    sql += " ORDER BY gia DESC ";

                sql += " OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

                var skip = (page - 1) * rowsPerPage;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", rowsPerPage);
                command.Parameters.AddWithValue("@SearchKeyword", $"%{searchKeyword}%");
                command.Parameters.AddWithValue("@CategoryType", categoryType);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (totalItems == 0)
                        {
                            totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                        }

                        var product = new Product
                        {
                            ProductID = reader.GetInt32(reader.GetOrdinal("monanid")),
                            Name = reader.GetString(reader.GetOrdinal("tenmonan")),
                            Category = reader.GetString(reader.GetOrdinal("loaimonan")),
                            Price = reader.GetInt32(reader.GetOrdinal("gia")),
                            Description = reader.IsDBNull(reader.GetOrdinal("mota")) ? null : reader.GetString(reader.GetOrdinal("mota")),
                            ImagePath = reader.IsDBNull(reader.GetOrdinal("imagepath")) ? null : reader.GetString(reader.GetOrdinal("imagepath")),
                            Status = reader.GetBoolean(reader.GetOrdinal("trangthai"))
                        };
                        products.Add(product);
                    }
                }
            }

            return new Tuple<int, List<Product>>(totalItems, products);
        }

        /// <summary>
        /// Thêm sản phẩm mới
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public int InsertProduct(Product product)
        {
            int newId;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                INSERT INTO menu (tenmonan, loaimonan, gia, mota, imagepath, trangthai)
                VALUES (@Name, @Category, @Price, @Description, @ImagePath, @Status)
                RETURNING monanid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Category", product.Category);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImagePath", (object)product.ImagePath ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", product.Status);

                newId = Convert.ToInt32(command.ExecuteScalar());
            }

            return newId;
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool UpdateProduct(Product product)
        {
            int rowsAffected;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the product exists
                var checkSql = "SELECT COUNT(*) FROM menu WHERE monanid = @ProductID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@ProductID", product.ProductID);
                var productExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!productExists)
                {
                    return false; // Product does not exist, return false
                }

                var sql = @"
                UPDATE menu
                SET tenmonan = @Name, loaimonan = @Category, gia = @Price, mota = @Description, imagepath = @ImagePath, trangthai = @Status
                WHERE monanid = @ProductID";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Category", product.Category);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImagePath", (object)product.ImagePath ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", product.Status);
                command.Parameters.AddWithValue("@ProductID", product.ProductID);

                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa một sản phẩm theo ID
        /// </summary>
        /// <param name="productId"></param>
        public void RemoveProductById(int productId)
        {
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the product exists
                var checkSql = "SELECT COUNT(*) FROM menu WHERE monanid = @ProductID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@ProductID", productId);
                var productExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!productExists)
                {
                    return;
                }

                var sql = "DELETE FROM menu WHERE monanid = @ProductID";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ProductID", productId);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Tìm sản phẩm theo ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public Product FindProductByID(int productID)
        {
            Product product = new Product();
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();
                var sql = @"
                SELECT monanid, tenmonan, loaimonan, gia, mota, imagepath, trangthai
                FROM menu
                WHERE monanid = @ProductID";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ProductID", productID);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        product.ProductID = reader.GetInt32(reader.GetOrdinal("monanid"));
                        product.Name = reader.GetString(reader.GetOrdinal("tenmonan"));
                        product.Category = reader.GetString(reader.GetOrdinal("loaimonan"));
                        product.Price = reader.GetInt32(reader.GetOrdinal("gia"));
                        product.Description = reader.IsDBNull(reader.GetOrdinal("mota")) ? null : reader.GetString(reader.GetOrdinal("mota"));
                        product.ImagePath = reader.IsDBNull(reader.GetOrdinal("imagepath")) ? null : reader.GetString(reader.GetOrdinal("imagepath"));
                        product.Status = reader.GetBoolean(reader.GetOrdinal("trangthai"));
                    }
                }
            }
            return product;
        }
    }
}