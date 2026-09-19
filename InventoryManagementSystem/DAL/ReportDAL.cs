using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    /// <summary>
    /// Read-only data access for the Reports module. Every method is a parameterized SELECT.
    /// </summary>
    public static class ReportDAL
    {
        public const string StatusInStock = "In Stock";
        public const string StatusLowStock = "Reorder / Low Stock";

        private const string StockStatusCase =
            "CASE WHEN p.QtyInStock <= p.ReorderLevel THEN N'Reorder / Low Stock' ELSE N'In Stock' END";

        public static List<InventoryReportRow> GetInventory(int? categoryId, string stockStatus, string searchText)
        {
            string sql = @"
                SELECT p.ProductID, p.ProductName, c.CategoryName, p.UnitPrice, p.QtyInStock, p.ReorderLevel,
                       " + StockStatusCase + @" AS StockStatus
                FROM Products p
                INNER JOIN Categories c ON p.CategoryID = c.CategoryID
                WHERE (@CategoryID IS NULL OR p.CategoryID = @CategoryID)
                  AND (@StockStatus IS NULL OR " + StockStatusCase + @" = @StockStatus)
                  AND (@SearchText IS NULL OR p.ProductName LIKE @SearchText ESCAPE '\'
                       OR c.CategoryName LIKE @SearchText ESCAPE '\')
                ORDER BY p.ProductName;";

            var results = new List<InventoryReportRow>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddNullableInt(command, "@CategoryID", categoryId);

                SqlParameter statusParam = command.Parameters.Add("@StockStatus", SqlDbType.NVarChar, 30);
                statusParam.Value = string.IsNullOrEmpty(stockStatus) ? (object)DBNull.Value : stockStatus;

                AddSearch(command, searchText);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new InventoryReportRow
                        {
                            ProductID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            CategoryName = reader.GetString(2),
                            UnitPrice = reader.GetDecimal(3),
                            QtyInStock = reader.GetInt32(4),
                            ReorderLevel = reader.GetInt32(5),
                            StockStatus = reader.GetString(6)
                        });
                    }
                }
            }

            return results;
        }

        public static List<StockInReportRow> GetStockIn(int? productId, int? supplierId, DateTime? dateFrom,
            DateTime? dateToExclusive, string searchText)
        {
            const string sql = @"
                SELECT si.StockInID, p.ProductName, s.SupplierName, si.Quantity, si.UnitCost, si.TotalCost,
                       si.DateIn, si.Notes
                FROM StockIn si
                INNER JOIN Products p ON si.ProductID = p.ProductID
                LEFT JOIN Suppliers s ON si.SupplierID = s.SupplierID
                WHERE (@ProductID IS NULL OR si.ProductID = @ProductID)
                  AND (@SupplierID IS NULL OR si.SupplierID = @SupplierID)
                  AND (@DateFrom IS NULL OR si.DateIn >= @DateFrom)
                  AND (@DateTo IS NULL OR si.DateIn < @DateTo)
                  AND (@SearchText IS NULL OR p.ProductName LIKE @SearchText ESCAPE '\'
                       OR s.SupplierName LIKE @SearchText ESCAPE '\'
                       OR si.Notes LIKE @SearchText ESCAPE '\')
                ORDER BY si.DateIn DESC, si.StockInID DESC;";

            var results = new List<StockInReportRow>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddNullableInt(command, "@ProductID", productId);
                AddNullableInt(command, "@SupplierID", supplierId);
                AddDateRange(command, dateFrom, dateToExclusive);
                AddSearch(command, searchText);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new StockInReportRow
                        {
                            StockInID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            SupplierName = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Quantity = reader.GetInt32(3),
                            UnitCost = reader.GetDecimal(4),
                            TotalCost = reader.GetDecimal(5),
                            DateIn = reader.GetDateTime(6),
                            Notes = reader.IsDBNull(7) ? null : reader.GetString(7)
                        });
                    }
                }
            }

            return results;
        }

        public static List<StockOutReportRow> GetStockOut(int? productId, int? customerId, DateTime? dateFrom,
            DateTime? dateToExclusive, string searchText)
        {
            const string sql = @"
                SELECT so.StockOutID, p.ProductName, c.CustomerName, so.Quantity, so.UnitPrice, so.TotalPrice,
                       so.DateOut, so.Notes
                FROM StockOut so
                INNER JOIN Products p ON so.ProductID = p.ProductID
                LEFT JOIN Customers c ON so.CustomerID = c.CustomerID
                WHERE (@ProductID IS NULL OR so.ProductID = @ProductID)
                  AND (@CustomerID IS NULL OR so.CustomerID = @CustomerID)
                  AND (@DateFrom IS NULL OR so.DateOut >= @DateFrom)
                  AND (@DateTo IS NULL OR so.DateOut < @DateTo)
                  AND (@SearchText IS NULL OR p.ProductName LIKE @SearchText ESCAPE '\'
                       OR c.CustomerName LIKE @SearchText ESCAPE '\'
                       OR so.Notes LIKE @SearchText ESCAPE '\')
                ORDER BY so.DateOut DESC, so.StockOutID DESC;";

            var results = new List<StockOutReportRow>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                AddNullableInt(command, "@ProductID", productId);
                AddNullableInt(command, "@CustomerID", customerId);
                AddDateRange(command, dateFrom, dateToExclusive);
                AddSearch(command, searchText);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new StockOutReportRow
                        {
                            StockOutID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            CustomerName = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Quantity = reader.GetInt32(3),
                            UnitPrice = reader.GetDecimal(4),
                            TotalPrice = reader.GetDecimal(5),
                            DateOut = reader.GetDateTime(6),
                            Notes = reader.IsDBNull(7) ? null : reader.GetString(7)
                        });
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Reads the Orders table only (order-level TotalAmount); OrderDetails is deliberately not
        /// joined so order totals can never be double-counted.
        /// </summary>
        public static List<OrderReportRow> GetOrders(string status, int? customerId, DateTime? dateFrom,
            DateTime? dateToExclusive, string searchText)
        {
            const string sql = @"
                SELECT o.OrderID, c.CustomerName, e.EmployeeName, o.OrderDate, o.TotalAmount, o.Status
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                WHERE (@Status IS NULL OR o.Status = @Status)
                  AND (@CustomerID IS NULL OR o.CustomerID = @CustomerID)
                  AND (@DateFrom IS NULL OR o.OrderDate >= @DateFrom)
                  AND (@DateTo IS NULL OR o.OrderDate < @DateTo)
                  AND (@SearchText IS NULL OR c.CustomerName LIKE @SearchText ESCAPE '\'
                       OR e.EmployeeName LIKE @SearchText ESCAPE '\'
                       OR CAST(o.OrderID AS NVARCHAR(20)) LIKE @SearchText ESCAPE '\')
                ORDER BY o.OrderDate DESC, o.OrderID DESC;";

            var results = new List<OrderReportRow>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                SqlParameter statusParam = command.Parameters.Add("@Status", SqlDbType.NVarChar, 20);
                statusParam.Value = string.IsNullOrEmpty(status) ? (object)DBNull.Value : status;

                AddNullableInt(command, "@CustomerID", customerId);
                AddDateRange(command, dateFrom, dateToExclusive);
                AddSearch(command, searchText);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new OrderReportRow
                        {
                            OrderID = reader.GetInt32(0),
                            CustomerName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            EmployeeName = reader.IsDBNull(2) ? null : reader.GetString(2),
                            OrderDate = reader.GetDateTime(3),
                            TotalAmount = reader.GetDecimal(4),
                            Status = reader.GetString(5)
                        });
                    }
                }
            }

            return results;
        }

        private static void AddNullableInt(SqlCommand command, string name, int? value)
        {
            command.Parameters.Add(name, SqlDbType.Int).Value = value.HasValue ? (object)value.Value : DBNull.Value;
        }

        private static void AddDateRange(SqlCommand command, DateTime? dateFrom, DateTime? dateToExclusive)
        {
            command.Parameters.Add("@DateFrom", SqlDbType.DateTime2).Value =
                dateFrom.HasValue ? (object)dateFrom.Value : DBNull.Value;
            command.Parameters.Add("@DateTo", SqlDbType.DateTime2).Value =
                dateToExclusive.HasValue ? (object)dateToExclusive.Value : DBNull.Value;
        }

        /// <summary>
        /// Adds @SearchText as a contains-pattern. LIKE wildcards typed by the user are escaped
        /// (backslash escape character) so they match literally.
        /// </summary>
        private static void AddSearch(SqlCommand command, string searchText)
        {
            SqlParameter parameter = command.Parameters.Add("@SearchText", SqlDbType.NVarChar, 600);

            if (string.IsNullOrEmpty(searchText))
            {
                parameter.Value = DBNull.Value;
                return;
            }

            string escaped = searchText.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[");
            parameter.Value = "%" + escaped + "%";
        }
    }
}
