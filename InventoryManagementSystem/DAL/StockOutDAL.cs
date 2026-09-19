using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    public static class StockOutDAL
    {
        private const string SelectColumns = @"
            so.StockOutID, so.ProductID, p.ProductName, so.CustomerID, c.CustomerName,
            so.Quantity, so.UnitPrice, so.TotalPrice, so.DateOut, so.Notes";

        private const string FromJoin = @"
            FROM StockOut so
            INNER JOIN Products p ON so.ProductID = p.ProductID
            LEFT JOIN Customers c ON so.CustomerID = c.CustomerID";

        public static List<StockOutEntity> GetAll(int? productId, int? customerId, DateTime? dateFrom,
            DateTime? dateToExclusive, string searchText)
        {
            string sql = "SELECT " + SelectColumns + " " + FromJoin + @"
                WHERE (@ProductID IS NULL OR so.ProductID = @ProductID)
                  AND (@CustomerID IS NULL OR so.CustomerID = @CustomerID)
                  AND (@DateFrom IS NULL OR so.DateOut >= @DateFrom)
                  AND (@DateTo IS NULL OR so.DateOut < @DateTo)
                  AND (@SearchText IS NULL OR p.ProductName LIKE @SearchText OR so.Notes LIKE @SearchText)
                ORDER BY so.DateOut DESC;";

            var results = new List<StockOutEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddNullableIntParameter(command, "@ProductID", productId);
                AddNullableIntParameter(command, "@CustomerID", customerId);

                SqlParameter dateFromParam = command.Parameters.Add("@DateFrom", SqlDbType.DateTime2);
                dateFromParam.Value = dateFrom.HasValue ? (object)dateFrom.Value : DBNull.Value;

                SqlParameter dateToParam = command.Parameters.Add("@DateTo", SqlDbType.DateTime2);
                dateToParam.Value = dateToExclusive.HasValue ? (object)dateToExclusive.Value : DBNull.Value;

                SqlParameter searchParam = command.Parameters.Add("@SearchText", SqlDbType.NVarChar, 255 + 2);
                searchParam.Value = string.IsNullOrEmpty(searchText) ? (object)DBNull.Value : "%" + searchText + "%";

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(ReadStockOut(reader));
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Atomically deducts stock (guarded by QtyInStock >= @Quantity so concurrent Stock Out
        /// operations cannot oversell) and inserts the StockOut row in a single transaction.
        /// Returns false (with no changes committed) if there was insufficient stock or the
        /// product no longer exists. Either both changes are committed or neither is.
        /// </summary>
        public static bool DeductStockAndInsert(StockOutEntity stockOut)
        {
            using (var connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        const string updateSql = @"
                            UPDATE Products
                            SET QtyInStock = QtyInStock - @Quantity,
                                UpdatedAt = SYSDATETIME()
                            WHERE ProductID = @ProductID
                              AND QtyInStock >= @Quantity;";

                        using (var updateCommand = new SqlCommand(updateSql, connection, transaction))
                        {
                            updateCommand.Parameters.Add("@Quantity", SqlDbType.Int).Value = stockOut.Quantity;
                            updateCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = stockOut.ProductID;

                            int rowsAffected = updateCommand.ExecuteNonQuery();
                            if (rowsAffected != 1)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        const string insertSql = @"
                            INSERT INTO StockOut (ProductID, CustomerID, Quantity, UnitPrice, TotalPrice, DateOut, Notes)
                            VALUES (@ProductID, @CustomerID, @Quantity, @UnitPrice, @TotalPrice, SYSDATETIME(), @Notes);";

                        using (var insertCommand = new SqlCommand(insertSql, connection, transaction))
                        {
                            insertCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = stockOut.ProductID;

                            SqlParameter customerParam = insertCommand.Parameters.Add("@CustomerID", SqlDbType.Int);
                            customerParam.Value = stockOut.CustomerID.HasValue ? (object)stockOut.CustomerID.Value : DBNull.Value;

                            insertCommand.Parameters.Add("@Quantity", SqlDbType.Int).Value = stockOut.Quantity;
                            AddMoneyParameter(insertCommand, "@UnitPrice", 12, stockOut.UnitPrice);
                            AddMoneyParameter(insertCommand, "@TotalPrice", 14, stockOut.TotalPrice);

                            SqlParameter notesParam = insertCommand.Parameters.Add("@Notes", SqlDbType.NVarChar, 255);
                            notesParam.Value = string.IsNullOrEmpty(stockOut.Notes) ? (object)DBNull.Value : stockOut.Notes;

                            insertCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static StockOutEntity ReadStockOut(SqlDataReader reader)
        {
            int customerIdOrdinal = reader.GetOrdinal("CustomerID");
            int customerNameOrdinal = reader.GetOrdinal("CustomerName");
            int notesOrdinal = reader.GetOrdinal("Notes");

            return new StockOutEntity
            {
                StockOutID = reader.GetInt32(reader.GetOrdinal("StockOutID")),
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                CustomerID = reader.IsDBNull(customerIdOrdinal) ? (int?)null : reader.GetInt32(customerIdOrdinal),
                CustomerName = reader.IsDBNull(customerNameOrdinal) ? null : reader.GetString(customerNameOrdinal),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                DateOut = reader.GetDateTime(reader.GetOrdinal("DateOut")),
                Notes = reader.IsDBNull(notesOrdinal) ? null : reader.GetString(notesOrdinal)
            };
        }

        private static void AddNullableIntParameter(SqlCommand command, string name, int? value)
        {
            SqlParameter parameter = command.Parameters.Add(name, SqlDbType.Int);
            parameter.Value = value.HasValue ? (object)value.Value : DBNull.Value;
        }

        private static void AddMoneyParameter(SqlCommand command, string name, byte precision, decimal value)
        {
            SqlParameter parameter = command.Parameters.Add(name, SqlDbType.Decimal);
            parameter.Precision = precision;
            parameter.Scale = 2;
            parameter.Value = value;
        }
    }
}
