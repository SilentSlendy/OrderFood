using System;
using Npgsql;
using System.Collections.Generic;
using POS.Interfaces;
using POS.Models;
using POS.Helpers;

namespace POS.Services.DAO
{
    /// <summary>
    /// DAO cho các thao tác liên quan đến WorkShift
    /// </summary>
    public class PostgresWorkShiftDao : IWorkShiftDao
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PostgresWorkShiftDao() { }

        /// <summary>
        /// Lấy tất cả các ca làm việc
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rowsPerPage"></param>
        /// <returns></returns>
        public Tuple<int, List<WorkShift>> GetAllWorkShifts(int page, int rowsPerPage)
        {
            var workShifts = new List<WorkShift>();
            int totalItems = 0;

            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                    SELECT COUNT(*) OVER() AS TotalItems, calamviecid, nhanvienid, giobatdau, gioketthuc, ngaylamviec
                    FROM calamviec
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

                        var workShift = new WorkShift
                        {
                            ShiftID = reader.GetInt32(reader.GetOrdinal("calamviecid")),
                            EmployeeID = reader.GetInt32(reader.GetOrdinal("nhanvienid")),
                            StartTime = reader.GetTimeSpan(reader.GetOrdinal("giobatdau")),
                            EndTime = reader.GetTimeSpan(reader.GetOrdinal("gioketthuc")),
                            WorkDate = reader.GetDateTime(reader.GetOrdinal("ngaylamviec"))
                        };
                        workShifts.Add(workShift);
                    }
                }
            }

            return new Tuple<int, List<WorkShift>>(totalItems, workShifts);
        }

        /// <summary>
        /// Thêm một ca làm việc mới
        /// </summary>
        /// <param name="workShift"></param>
        /// <returns></returns>
        public int InsertWorkShift(WorkShift workShift)
        {
            int newId;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                var sql = @"
                    INSERT INTO calamviec (nhanvienid, giobatdau, gioketthuc, ngaylamviec)
                    VALUES (@EmployeeID, @StartTime, @EndTime, @WorkDate)
                    RETURNING calamviecid";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EmployeeID", workShift.EmployeeID);
                command.Parameters.AddWithValue("@StartTime", workShift.StartTime);
                command.Parameters.AddWithValue("@EndTime", workShift.EndTime);
                command.Parameters.AddWithValue("@WorkDate", workShift.WorkDate);

                newId = Convert.ToInt32(command.ExecuteScalar());
            }

            return newId;
        }

        /// <summary>
        /// Cập nhật một ca làm việc
        /// </summary>
        /// <param name="workShift"></param>
        /// <returns></returns>
        public bool UpdateWorkShift(WorkShift workShift)
        {
            int rowsAffected;
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the work shift exists
                var checkSql = "SELECT COUNT(*) FROM calamviec WHERE calamviecid = @ShiftID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@ShiftID", workShift.ShiftID);
                var workShiftExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!workShiftExists)
                {
                    return false; // Work shift does not exist, return false
                }

                var sql = @"
                    UPDATE calamviec
                    SET nhanvienid = @EmployeeID, giobatdau = @StartTime, gioketthuc = @EndTime, ngaylamviec = @WorkDate
                    WHERE calamviecid = @ShiftID";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EmployeeID", workShift.EmployeeID);
                command.Parameters.AddWithValue("@StartTime", workShift.StartTime);
                command.Parameters.AddWithValue("@EndTime", workShift.EndTime);
                command.Parameters.AddWithValue("@WorkDate", workShift.WorkDate);
                command.Parameters.AddWithValue("@ShiftID", workShift.ShiftID);

                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Xóa một ca làm việc theo ID
        /// </summary>
        /// <param name="workShiftId"></param>
        public void RemoveWorkShiftById(int workShiftId)
        {
            using (var connection = new NpgsqlConnection(ConnectionHelper.BuildConnectionString()))
            {
                connection.Open();

                // Check if the work shift exists
                var checkSql = "SELECT COUNT(*) FROM calamviec WHERE calamviecid = @ShiftID";
                var checkCommand = new NpgsqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@ShiftID", workShiftId);
                var workShiftExists = (long)checkCommand.ExecuteScalar() > 0;

                if (!workShiftExists)
                {
                    return;
                }

                var sql = "DELETE FROM calamviec WHERE calamviecid = @ShiftID";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ShiftID", workShiftId);

                command.ExecuteNonQuery();
            }
        }
    }
}