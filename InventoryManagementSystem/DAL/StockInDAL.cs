using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    public static class StockInDAL
    {
        private const string SelectColumns = @"
            si.StockInID, si.ProductID, p.ProductName, si.SupplierID, s.SupplierName,
            si.Quantity, si.UnitCost, si.TotalCost, si.DateIn, si.Notes";

        private const string FromJoin = @"
            FROM StockIn si
            INNER JOIN Products p ON si.ProductID = p.ProductID
            LEFT JOIN Suppliers s ON si.SupplierID = s.SupplierID";

        public static List<StockInEntity> GetAll(int? productId, int? supplierId, DateTime? dateFrom,
            DateTime? dateToExclusive, string searchText)
        {
            string sql = "SELECT " + SelectColumns + " " + FromJoin + @"
                WHERE (@ProductID IS NULL OR si.ProductID = @ProductID)
                  AND (@SupplierID IS NULL OR si.SupplierID = @SupplierID)
                  AND (@DateFrom IS NULL OR si.DateIn >= @DateFrom)
                  AND (@DateTo IS NULL OR si.DateIn < @DateTo)
                  AND (@SearchText IS NULL OR p.ProductName LIKE @SearchText OR si.Notes LIKE @SearchText)
                ORDER BY si.DateIn DESC;";

            var results = new List<StockInEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddNullableIntParameter(command, "@ProductID", productId);
                AddNullableIntParameter(command, "@SupplierID", supplierId);

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
                        results.Add(ReadStockIn(reader));
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Inserts the StockIn row and increases Products.QtyInStock in a single transaction.
        /// Either both changes are committed or neither is.
        /// </summary>
        public static void InsertWithStockUpdate(StockInEntity stockIn)
        {
            using (var connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        const string insertSql = @"
                            INSERT INTO StockIn (ProductID, SupplierID, Quantity, UnitCost, TotalCost, DateIn, Notes)
                            VALUES (@ProductID, @SupplierID, @Quantity, @UnitCost, @TotalCost, SYSDATETIME(), @Notes);";

                        using (var insertCommand = new SqlCommand(insertSql, connection, transaction))
                        {
                            insertCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = stockIn.ProductID;

                            SqlParameter supplierParam = insertCommand.Parameters.Add("@SupplierID", SqlDbType.Int);
                            supplierParam.Value = stockIn.SupplierID.HasValue ? (object)stockIn.SupplierID.Value : DBNull.Value;

                            insertCommand.Parameters.Add("@Quantity", SqlDbType.Int).Value = stockIn.Quantity;
                            AddMoneyParameter(insertCommand, "@UnitCost", 12, stockIn.UnitCost);
                            AddMoneyParameter(insertCommand, "@TotalCost", 14, stockIn.TotalCost);

                            SqlParameter notesParam = insertCommand.Parameters.Add("@Notes", SqlDbType.NVarChar, 255);
                            notesParam.Value = string.IsNullOrEmpty(stockIn.Notes) ? (object)DBNull.Value : stockIn.Notes;

                            insertCommand.ExecuteNonQuery();
                        }

                        const string updateSql = @"
                            UPDATE Products
                            SET QtyInStock = QtyInStock + @Quantity,
                                UpdatedAt = SYSDATETIME()
                            WHERE ProductID = @ProductID;";

                        using (var updateCommand = new SqlCommand(updateSql, connection, transaction))
                        {
                            updateCommand.Parameters.Add("@Quantity", SqlDbType.Int).Value = stockIn.Quantity;
                            updateCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = stockIn.ProductID;

                            int rowsAffected = updateCommand.ExecuteNonQuery();
                            if (rowsAffected != 1)
                                throw new InvalidOperationException("The product no longer exists; stock quantity was not updated.");
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static StockInEntity ReadStockIn(SqlDataReader reader)
        {
            int supplierIdOrdinal = reader.GetOrdinal("SupplierID");
            int supplierNameOrdinal = reader.GetOrdinal("SupplierName");
            int notesOrdinal = reader.GetOrdinal("Notes");

            return new StockInEntity
            {
                StockInID = reader.GetInt32(reader.GetOrdinal("StockInID")),
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                SupplierID = reader.IsDBNull(supplierIdOrdinal) ? (int?)null : reader.GetInt32(supplierIdOrdinal),
                SupplierName = reader.IsDBNull(supplierNameOrdinal) ? null : reader.GetString(supplierNameOrdinal),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitCost = reader.GetDecimal(reader.GetOrdinal("UnitCost")),
                TotalCost = reader.GetDecimal(reader.GetOrdinal("TotalCost")),
                DateIn = reader.GetDateTime(reader.GetOrdinal("DateIn")),
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
