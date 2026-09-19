using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    /// <summary>
    /// Read-only data access for the Dashboard. Every method is a parameterized SELECT.
    /// </summary>
    public static class DashboardDAL
    {
        public static DashboardSummary GetSummary()
        {
            const string sql = @"
                SELECT
                    (SELECT COUNT(*) FROM Products) AS TotalProducts,
                    (SELECT COUNT(*) FROM Categories) AS TotalCategories,
                    (SELECT ISNULL(SUM(CAST(QtyInStock AS BIGINT)), 0) FROM Products) AS TotalUnits,
                    (SELECT COUNT(*) FROM Products WHERE QtyInStock <= ReorderLevel) AS LowStockCount,
                    (SELECT ISNULL(SUM(CAST(Quantity AS BIGINT)), 0) FROM StockIn) AS StockInQty,
                    (SELECT ISNULL(SUM(CAST(Quantity AS BIGINT)), 0) FROM StockOut) AS StockOutQty,
                    (SELECT COUNT(*) FROM Orders WHERE Status = @Confirmed) AS ConfirmedOrders,
                    (SELECT COUNT(*) FROM Orders WHERE Status = @Pending) AS PendingOrders;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Confirmed", SqlDbType.NVarChar, 20).Value = "Confirmed";
                command.Parameters.Add("@Pending", SqlDbType.NVarChar, 20).Value = "Pending";
                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    reader.Read();
                    return new DashboardSummary
                    {
                        TotalProducts = reader.GetInt32(0),
                        TotalCategories = reader.GetInt32(1),
                        TotalUnitsInStock = reader.GetInt64(2),
                        LowStockCount = reader.GetInt32(3),
                        StockInQuantity = reader.GetInt64(4),
                        StockOutQuantity = reader.GetInt64(5),
                        ConfirmedOrders = reader.GetInt32(6),
                        PendingOrders = reader.GetInt32(7)
                    };
                }
            }
        }

        /// <summary>Products at or below their reorder level, lowest stock first (at most <paramref name="limit"/>).</summary>
        public static List<LowStockItem> GetLowStock(int limit)
        {
            const string sql = @"
                SELECT TOP (@Limit) p.ProductName, c.CategoryName, p.QtyInStock, p.ReorderLevel
                FROM Products p
                INNER JOIN Categories c ON p.CategoryID = c.CategoryID
                WHERE p.QtyInStock <= p.ReorderLevel
                ORDER BY p.QtyInStock, p.ProductName;";

            var results = new List<LowStockItem>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Limit", SqlDbType.Int).Value = limit;
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new LowStockItem
                        {
                            ProductName = reader.GetString(0),
                            CategoryName = reader.GetString(1),
                            QtyInStock = reader.GetInt32(2),
                            ReorderLevel = reader.GetInt32(3)
                        });
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Latest StockIn, StockOut and Orders rows merged newest-first. Each source is limited with
        /// TOP before merging so the full history is never scanned into memory.
        /// </summary>
        public static List<RecentActivityRow> GetRecentActivity(int limit)
        {
            const string sql = @"
                SELECT TOP (@Limit) ActivityType, RefID, Subject, Party, Status, ActivityDate, Quantity, Amount
                FROM (
                    SELECT * FROM (
                        SELECT TOP (@Limit) N'Stock In' AS ActivityType, si.StockInID AS RefID, p.ProductName AS Subject,
                               s.SupplierName AS Party, CAST(NULL AS NVARCHAR(20)) AS Status, si.DateIn AS ActivityDate,
                               CAST(si.Quantity AS INT) AS Quantity, si.TotalCost AS Amount
                        FROM StockIn si
                        INNER JOIN Products p ON si.ProductID = p.ProductID
                        LEFT JOIN Suppliers s ON si.SupplierID = s.SupplierID
                        ORDER BY si.DateIn DESC, si.StockInID DESC) a
                    UNION ALL
                    SELECT * FROM (
                        SELECT TOP (@Limit) N'Stock Out' AS ActivityType, so.StockOutID AS RefID, p.ProductName AS Subject,
                               c.CustomerName AS Party, CAST(NULL AS NVARCHAR(20)) AS Status, so.DateOut AS ActivityDate,
                               CAST(so.Quantity AS INT) AS Quantity, so.TotalPrice AS Amount
                        FROM StockOut so
                        INNER JOIN Products p ON so.ProductID = p.ProductID
                        LEFT JOIN Customers c ON so.CustomerID = c.CustomerID
                        ORDER BY so.DateOut DESC, so.StockOutID DESC) b
                    UNION ALL
                    SELECT * FROM (
                        SELECT TOP (@Limit) N'Order' AS ActivityType, o.OrderID AS RefID, c.CustomerName AS Subject,
                               e.EmployeeName AS Party, o.Status AS Status, o.OrderDate AS ActivityDate,
                               CAST(NULL AS INT) AS Quantity, o.TotalAmount AS Amount
                        FROM Orders o
                        LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                        LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                        ORDER BY o.OrderDate DESC, o.OrderID DESC) d
                ) x
                ORDER BY ActivityDate DESC, RefID DESC;";

            var results = new List<RecentActivityRow>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Limit", SqlDbType.Int).Value = limit;
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new RecentActivityRow
                        {
                            ActivityType = reader.GetString(0),
                            ReferenceID = reader.GetInt32(1),
                            Subject = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Party = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Status = reader.IsDBNull(4) ? null : reader.GetString(4),
                            ActivityDate = reader.GetDateTime(5),
                            Quantity = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                            Amount = reader.GetDecimal(7)
                        });
                    }
                }
            }

            return results;
        }
    }
}
