using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.DAL;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>Group A: database / foundation (read-only against whichever DB the application is pointed at).</summary>
    internal static class Tests_Foundation
    {
        private static readonly Dictionary<string, string[]> RequiredColumns = new Dictionary<string, string[]>
        {
            { "Users", new[] { "UserID", "Username", "PasswordHash", "FullName", "Role", "IsActive", "CreatedAt" } },
            { "Categories", new[] { "CategoryID", "CategoryName", "Description" } },
            { "Customers", new[] { "CustomerID", "CustomerName", "Phone", "Email", "Address" } },
            { "Suppliers", new[] { "SupplierID", "SupplierName", "ContactPerson", "Phone", "Email", "Address" } },
            { "Employees", new[] { "EmployeeID", "EmployeeName", "Gender", "Phone", "Email", "Address" } },
            { "Products", new[] { "ProductID", "ProductName", "CategoryID", "UnitPrice", "QtyInStock", "Description", "Barcode", "ReorderLevel", "CreatedAt", "UpdatedAt" } },
            { "StockIn", new[] { "StockInID", "ProductID", "SupplierID", "Quantity", "UnitCost", "TotalCost", "DateIn", "Notes" } },
            { "StockOut", new[] { "StockOutID", "ProductID", "CustomerID", "Quantity", "UnitPrice", "TotalPrice", "DateOut", "Notes" } },
            { "Orders", new[] { "OrderID", "CustomerID", "EmployeeID", "OrderDate", "TotalAmount", "Status" } },
            { "OrderDetails", new[] { "OrderDetailID", "OrderID", "ProductID", "Quantity", "UnitPrice", "Total" } }
        };

        private static readonly string[] ExpectedForeignKeys =
        {
            "FK_Products_Categories", "FK_StockIn_Products", "FK_StockIn_Suppliers", "FK_StockOut_Products", "FK_StockOut_Customers",
            "FK_Orders_Customers", "FK_Orders_Employees", "FK_OrderDetails_Orders", "FK_OrderDetails_Products"
        };

        public static void Run(bool isolated)
        {
            T.Group("A Database / foundation");

            // A01 the shipped application config targets the real dev database
            T.Eq("A01 main project App.config targets InventoryManagementDB", "InventoryManagementDB", Program.DevDbName);

            // A02 DbConnection is usable and points where we think it does
            T.Run("A02", () =>
            {
                using (SqlConnection c = DbConnection.GetConnection())
                {
                    T.Eq("A02a DbConnection returns a SqlConnection targeting the expected DB", T.DbName, c.Database);
                    c.Open();
                    using (var cmd = new SqlCommand("SELECT DB_NAME()", c))
                        T.Eq("A02b opened connection is really on the expected DB", T.DbName, (string)cmd.ExecuteScalar());
                }
            });

            if (isolated)
            {
                T.Check("A03 isolated run is NOT pointed at the development database",
                    T.DbName != Program.DevDbName && T.DbName.StartsWith(T.TestDbPrefix, StringComparison.Ordinal));
            }
            else
            {
                T.Eq("A03 dev run is pointed at the development database", Program.DevDbName, T.DbName);
            }

            // A04 tables + columns (schema compatibility with what the application reads/writes)
            foreach (var kv in RequiredColumns)
            {
                bool exists = T.Scalar("SELECT COUNT(*) FROM sys.tables WHERE name=@p0", kv.Key) == 1;
                T.Check("A04 table " + kv.Key + " exists", exists);
                if (!exists) continue;

                string[] have = ColumnsOf(kv.Key);
                string[] missing = kv.Value.Where(c => !have.Contains(c)).ToArray();
                T.Check("A05 " + kv.Key + " has every column the application uses", missing.Length == 0, "missing: " + string.Join(",", missing));
            }

            // A06 foreign keys
            foreach (string fk in ExpectedForeignKeys)
                T.Check("A06 foreign key " + fk + " exists", T.Scalar("SELECT COUNT(*) FROM sys.foreign_keys WHERE name=@p0", fk) == 1);

            // A07 integrity rules the application relies on
            foreach (string ck in new[] { "CK_Orders_Status", "CK_Products_QtyInStock", "CK_StockOut_Quantity", "CK_StockIn_Quantity", "CK_OrderDetails_Quantity" })
                T.Check("A07 check constraint " + ck + " exists", T.Scalar("SELECT COUNT(*) FROM sys.check_constraints WHERE name=@p0", ck) == 1);
            T.Check("A07 unique category name", T.Scalar("SELECT COUNT(*) FROM sys.key_constraints WHERE name='UQ_Categories_CategoryName'") == 1);
            T.Check("A07 unique username", T.Scalar("SELECT COUNT(*) FROM sys.key_constraints WHERE name='UQ_Users_Username'") == 1);
            T.Check("A07 filtered unique barcode index", T.Scalar("SELECT COUNT(*) FROM sys.indexes WHERE name='UX_Products_Barcode' AND is_unique=1 AND has_filter=1") == 1);

            // A08 the application's own read paths work against the schema
            T.Run("A08", () =>
            {
                new CategoryBLL().GetAll(); new ProductBLL().GetAll(); new CustomerBLL().GetAll();
                new SupplierBLL().GetAll(); new EmployeeBLL().GetAll(); new StockInBLL().GetAll();
                new StockOutBLL().GetAll(); new OrderBLL().GetAll(); new ReportBLL().GetInventoryReport();
                new DashboardBLL().Load();
                T.Check("A08 all BLL read paths execute without error", true);
            });
        }

        private static string[] ColumnsOf(string table)
        {
            var cols = new List<string>();
            using (var c = new SqlConnection(T.ConnStr))
            using (var cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@t", c))
            {
                cmd.Parameters.AddWithValue("@t", table);
                c.Open();
                using (var r = cmd.ExecuteReader()) while (r.Read()) cols.Add(r.GetString(0));
            }
            return cols.ToArray();
        }
    }
}
