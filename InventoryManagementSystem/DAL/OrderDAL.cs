using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    public enum OrderConfirmOutcome
    {
        Success,
        OrderNotFound,
        NotPending,
        NoDetails,
        InsufficientStock
    }

    public static class OrderDAL
    {
        private const string SelectColumns = @"
            o.OrderID, o.CustomerID, c.CustomerName, o.EmployeeID, e.EmployeeName,
            o.OrderDate, o.TotalAmount, o.Status";

        private const string FromJoin = @"
            FROM Orders o
            LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
            LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID";

        public static List<OrderEntity> GetAll(string status, int? customerId, DateTime? dateFrom,
            DateTime? dateToExclusive, string searchText)
        {
            string sql = "SELECT " + SelectColumns + " " + FromJoin + @"
                WHERE (@Status IS NULL OR o.Status = @Status)
                  AND (@CustomerID IS NULL OR o.CustomerID = @CustomerID)
                  AND (@DateFrom IS NULL OR o.OrderDate >= @DateFrom)
                  AND (@DateTo IS NULL OR o.OrderDate < @DateTo)
                  AND (@SearchText IS NULL
                       OR c.CustomerName LIKE @SearchText
                       OR e.EmployeeName LIKE @SearchText
                       OR CAST(o.OrderID AS NVARCHAR(20)) LIKE @SearchText)
                ORDER BY o.OrderDate DESC;";

            var results = new List<OrderEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                SqlParameter statusParam = command.Parameters.Add("@Status", SqlDbType.NVarChar, 20);
                statusParam.Value = string.IsNullOrEmpty(status) ? (object)DBNull.Value : status;

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
                        results.Add(ReadOrder(reader));
                    }
                }
            }

            return results;
        }

        public static OrderEntity GetById(int orderId)
        {
            string sql = "SELECT " + SelectColumns + " " + FromJoin + " WHERE o.OrderID = @OrderID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderId;

                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                        return null;

                    return ReadOrder(reader);
                }
            }
        }

        public static List<OrderDetailEntity> GetDetails(int orderId)
        {
            const string sql = @"
                SELECT od.OrderDetailID, od.OrderID, od.ProductID, p.ProductName, od.Quantity, od.UnitPrice, od.Total
                FROM OrderDetails od
                INNER JOIN Products p ON od.ProductID = p.ProductID
                WHERE od.OrderID = @OrderID
                ORDER BY od.OrderDetailID ASC;";

            var results = new List<OrderDetailEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderId;

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new OrderDetailEntity
                        {
                            OrderDetailID = reader.GetInt32(reader.GetOrdinal("OrderDetailID")),
                            OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                            ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total"))
                        });
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Inserts the Order header (Status = Pending) and its OrderDetails in a single
        /// transaction, so the order and its lines are created atomically. Stock is never
        /// touched here - Pending orders do not affect QtyInStock.
        /// </summary>
        public static int CreatePendingOrder(OrderEntity order, List<OrderDetailEntity> details)
        {
            using (var connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int orderId;

                        const string insertOrderSql = @"
                            INSERT INTO Orders (CustomerID, EmployeeID, OrderDate, TotalAmount, Status)
                            OUTPUT INSERTED.OrderID
                            VALUES (@CustomerID, @EmployeeID, SYSDATETIME(), @TotalAmount, N'Pending');";

                        using (var command = new SqlCommand(insertOrderSql, connection, transaction))
                        {
                            AddNullableIntParameter(command, "@CustomerID", order.CustomerID);
                            AddNullableIntParameter(command, "@EmployeeID", order.EmployeeID);
                            AddMoneyParameter(command, "@TotalAmount", 14, order.TotalAmount);

                            orderId = (int)command.ExecuteScalar();
                        }

                        const string insertDetailSql = @"
                            INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice, Total)
                            VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, @Total);";

                        foreach (OrderDetailEntity detail in details)
                        {
                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderId;
                                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = detail.ProductID;
                                command.Parameters.Add("@Quantity", SqlDbType.Int).Value = detail.Quantity;
                                AddMoneyParameter(command, "@UnitPrice", 12, detail.UnitPrice);
                                AddMoneyParameter(command, "@Total", 14, detail.Total);

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return orderId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Confirms a Pending order atomically: every product's stock is deducted with a
        /// guarded UPDATE (QtyInStock >= Quantity, the same pattern StockOut uses) and the
        /// order is flipped to Confirmed, all inside one transaction. If any product has
        /// insufficient stock, or the order is not Pending, everything rolls back - no
        /// partial stock deduction and no status change. The final "Status = Pending" guard
        /// also makes repeated confirm attempts safe: only one can ever win.
        /// </summary>
        public static OrderConfirmOutcome ConfirmOrder(int orderId)
        {
            using (var connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // UPDLOCK/ROWLOCK serializes concurrent confirm attempts on the same
                        // order so a second attempt blocks here instead of racing the deduction.
                        const string lockOrderSql =
                            "SELECT Status FROM Orders WITH (UPDLOCK, ROWLOCK) WHERE OrderID = @OrderID;";

                        string status;
                        using (var command = new SqlCommand(lockOrderSql, connection, transaction))
                        {
                            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderId;
                            object result = command.ExecuteScalar();
                            if (result == null)
                            {
                                transaction.Rollback();
                                return OrderConfirmOutcome.OrderNotFound;
                            }
                            status = (string)result;
                        }

                        if (!string.Equals(status, "Pending", StringComparison.Ordinal))
                        {
                            transaction.Rollback();
                            return OrderConfirmOutcome.NotPending;
                        }

                        var details = new List<OrderDetailEntity>();
                        const string detailsSql =
                            "SELECT ProductID, Quantity FROM OrderDetails WHERE OrderID = @OrderID;";

                        using (var command = new SqlCommand(detailsSql, connection, transaction))
                        {
                            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderId;
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    details.Add(new OrderDetailEntity
                                    {
                                        ProductID = reader.GetInt32(0),
                                        Quantity = reader.GetInt32(1)
                                    });
                                }
                            }
                        }

                        if (details.Count == 0)
                        {
                            transaction.Rollback();
                            return OrderConfirmOutcome.NoDetails;
                        }

                        const string deductSql = @"
                            UPDATE Products
                            SET QtyInStock = QtyInStock - @Quantity,
                                UpdatedAt = SYSDATETIME()
                            WHERE ProductID = @ProductID
                              AND QtyInStock >= @Quantity;";

                        foreach (OrderDetailEntity detail in details)
                        {
                            using (var command = new SqlCommand(deductSql, connection, transaction))
                            {
                                command.Parameters.Add("@Quantity", SqlDbType.Int).Value = detail.Quantity;
                                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = detail.ProductID;

                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected != 1)
                                {
                                    transaction.Rollback();
                                    return OrderConfirmOutcome.InsufficientStock;
                                }
                            }
                        }

                        const string confirmSql = @"
                            UPDATE Orders
                            SET Status = N'Confirmed'
                            WHERE OrderID = @OrderID
                              AND Status = N'Pending';";

                        using (var command = new SqlCommand(confirmSql, connection, transaction))
                        {
                            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderId;

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected != 1)
                            {
                                transaction.Rollback();
                                return OrderConfirmOutcome.NotPending;
                            }
                        }

                        transaction.Commit();
                        return OrderConfirmOutcome.Success;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Cancels a Pending order via a guarded update. Never touches Products - cancelling
        /// a pending order must not change stock. Returns false if the order was not found
        /// or was not Pending (including already Cancelled or Confirmed).
        /// </summary>
        public static bool CancelPendingOrder(int orderId)
        {
            const string sql = @"
                UPDATE Orders
                SET Status = N'Cancelled'
                WHERE OrderID = @OrderID
                  AND Status = N'Pending';";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderId;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
        }

        private static OrderEntity ReadOrder(SqlDataReader reader)
        {
            int customerIdOrdinal = reader.GetOrdinal("CustomerID");
            int customerNameOrdinal = reader.GetOrdinal("CustomerName");
            int employeeIdOrdinal = reader.GetOrdinal("EmployeeID");
            int employeeNameOrdinal = reader.GetOrdinal("EmployeeName");

            return new OrderEntity
            {
                OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                CustomerID = reader.IsDBNull(customerIdOrdinal) ? (int?)null : reader.GetInt32(customerIdOrdinal),
                CustomerName = reader.IsDBNull(customerNameOrdinal) ? null : reader.GetString(customerNameOrdinal),
                EmployeeID = reader.IsDBNull(employeeIdOrdinal) ? (int?)null : reader.GetInt32(employeeIdOrdinal),
                EmployeeName = reader.IsDBNull(employeeNameOrdinal) ? null : reader.GetString(employeeNameOrdinal),
                OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                Status = reader.GetString(reader.GetOrdinal("Status"))
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
