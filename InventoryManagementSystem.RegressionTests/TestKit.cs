using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>Minimal assertion + database helper kit (deliberately not a framework).</summary>
    internal static class T
    {
        public const string TestDbPrefix = "ZZREG_";
        public static readonly string[] Tables =
            { "Users", "Categories", "Customers", "Suppliers", "Employees", "Products", "StockIn", "StockOut", "Orders", "OrderDetails" };

        public static string ConnStr;   // connection string currently used by the application under test
        public static string DbName;

        private static string _group = "";
        public static readonly List<string> GroupOrder = new List<string>();
        public static readonly Dictionary<string, int[]> Stats = new Dictionary<string, int[]>(); // [pass, fail]
        public static readonly List<string> Failures = new List<string>();

        // ---- assertions ----------------------------------------------------------------------
        public static void Group(string name)
        {
            _group = name;
            if (!Stats.ContainsKey(name)) { Stats[name] = new[] { 0, 0 }; GroupOrder.Add(name); }
            Console.WriteLine();
            Console.WriteLine("== " + name + " ==");
        }

        public static void Check(string name, bool ok, string detail = null)
        {
            Stats[_group][ok ? 0 : 1]++;
            if (!ok) Failures.Add("[" + _group + "] " + name + (detail != null ? "  -> " + detail : ""));
            Console.WriteLine((ok ? "  PASS  " : "  FAIL  ") + name + (!ok && detail != null ? "  -> " + detail : ""));
        }

        public static void Eq<TV>(string name, TV expected, TV actual)
        {
            Check(name, Equals(expected, actual), "expected <" + expected + "> actual <" + actual + ">");
        }

        /// <summary>Runs one test body; an unexpected exception is recorded as a failure, never swallowed.</summary>
        public static void Run(string name, Action body)
        {
            try { body(); }
            catch (Exception ex) { Check(name + " (unexpected exception)", false, ex.GetType().Name + ": " + ex.Message); }
        }

        public static int TotalPass { get { return Stats.Values.Sum(v => v[0]); } }
        public static int TotalFail { get { return Stats.Values.Sum(v => v[1]); } }

        // ---- database helpers ----------------------------------------------------------------
        private static SqlCommand Cmd(SqlConnection c, string sql, object[] p)
        {
            var cmd = new SqlCommand(sql, c);
            for (int i = 0; i < p.Length; i++)
                cmd.Parameters.AddWithValue("@p" + i, p[i] ?? DBNull.Value);
            return cmd;
        }

        public static object Raw(string sql, params object[] p) { return RawOn(ConnStr, sql, p); }

        public static object RawOn(string connStr, string sql, params object[] p)
        {
            using (var c = new SqlConnection(connStr))
            using (var cmd = Cmd(c, sql, p)) { c.Open(); object o = cmd.ExecuteScalar(); return o == DBNull.Value ? null : o; }
        }

        public static long Scalar(string sql, params object[] p) { object o = Raw(sql, p); return o == null ? 0 : Convert.ToInt64(o); }
        public static decimal Dec(string sql, params object[] p) { object o = Raw(sql, p); return o == null ? 0m : Convert.ToDecimal(o); }
        public static string Str(string sql, params object[] p) { object o = Raw(sql, p); return o == null ? null : Convert.ToString(o); }
        public static bool IsNull(string sql, params object[] p) { return Raw(sql, p) == null; }

        public static void Exec(string sql, params object[] p)
        {
            using (var c = new SqlConnection(ConnStr))
            using (var cmd = Cmd(c, sql, p)) { c.Open(); cmd.ExecuteNonQuery(); }
        }

        public static int Id(string sql, params object[] p) { return (int)Scalar(sql, p); }

        /// <summary>Counts + checksums + identity high-water mark for every table.</summary>
        public static string Snapshot(string connStr)
        {
            var parts = new List<string>();
            foreach (string t in Tables)
            {
                string sql = "SELECT CAST(COUNT(*) AS NVARCHAR(20)) + '/' + CAST(ISNULL(CHECKSUM_AGG(BINARY_CHECKSUM(*)),0) AS NVARCHAR(20)) + '/' + " +
                             "ISNULL((SELECT CAST(last_value AS NVARCHAR(20)) FROM sys.identity_columns WHERE object_id = OBJECT_ID('" + t + "')),'-') FROM " + t;
                parts.Add(t + "=" + Convert.ToString(RawOn(connStr, sql)));
            }
            return string.Join(" | ", parts);
        }

        public static string Snapshot() { return Snapshot(ConnStr); }

        /// <summary>Empties the disposable test database and restores identity counters. Refuses to run on anything but a ZZREG_ database.</summary>
        public static void Wipe()
        {
            if (DbName == null || !DbName.StartsWith(TestDbPrefix, StringComparison.Ordinal))
                throw new InvalidOperationException("Refusing to wipe a database that is not a " + TestDbPrefix + " test database.");

            Exec(@"DELETE FROM OrderDetails; DELETE FROM Orders; DELETE FROM StockOut; DELETE FROM StockIn; DELETE FROM Products;
                   DELETE FROM Categories; DELETE FROM Customers; DELETE FROM Suppliers; DELETE FROM Employees; DELETE FROM Users;");
            // Only reseed counters that have been used. On a never-used identity column RESEED 0 would make the
            // NEXT id 0 (not 1), which the application (correctly) treats as "no selection".
            foreach (string t in Tables)
                if (IdentityLast(t).HasValue)
                    Exec("DBCC CHECKIDENT ('" + t + "', RESEED, 0) WITH NO_INFOMSGS");
        }

        /// <summary>
        /// Restores a table's identity counter to a previously captured last_value (NULL = never used).
        /// Used by selective cleanups so the counters return to their pre-test state.
        /// </summary>
        public static long? IdentityLast(string table)
        {
            object o = Raw("SELECT CAST(last_value AS BIGINT) FROM sys.identity_columns WHERE object_id = OBJECT_ID(@p0)", table);
            return o == null ? (long?)null : Convert.ToInt64(o);
        }

        public static void RestoreIdentity(string table, long? last)
        {
            Exec("DBCC CHECKIDENT ('" + table + "', RESEED, " + (last ?? 0) + ") WITH NO_INFOMSGS");
        }

        // ---- fixtures (direct SQL, independent of the BLL under test) -------------------------
        public static int NewCategory(string name)
        {
            return Id("INSERT Categories(CategoryName) OUTPUT INSERTED.CategoryID VALUES (@p0)", name);
        }

        public static int NewProduct(string name, int categoryId, int stock, decimal price = 10m, int reorder = 5)
        {
            return Id(@"INSERT Products(ProductName,CategoryID,UnitPrice,QtyInStock,ReorderLevel,CreatedAt,UpdatedAt)
                        OUTPUT INSERTED.ProductID VALUES (@p0,@p1,@p2,@p3,@p4,SYSDATETIME(),SYSDATETIME())", name, categoryId, price, stock, reorder);
        }

        public static int NewCustomer(string name) { return Id("INSERT Customers(CustomerName) OUTPUT INSERTED.CustomerID VALUES (@p0)", name); }
        public static int NewSupplier(string name) { return Id("INSERT Suppliers(SupplierName) OUTPUT INSERTED.SupplierID VALUES (@p0)", name); }
        public static int NewEmployee(string name) { return Id("INSERT Employees(EmployeeName) OUTPUT INSERTED.EmployeeID VALUES (@p0)", name); }
        public static int Stock(int productId) { return (int)Scalar("SELECT QtyInStock FROM Products WHERE ProductID=@p0", productId); }
        public static long Count(string table) { return Scalar("SELECT COUNT(*) FROM " + table); }

        // ---- UI helpers ----------------------------------------------------------------------
        public static TF F<TF>(object o, string field)
        {
            FieldInfo f = o.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            if (f == null) throw new MissingFieldException(o.GetType().Name, field);
            return (TF)f.GetValue(o);
        }

        public static void Pump() { Application.DoEvents(); }

        /// <summary>Module pages are UserControls; to test one on its own it is hosted in a plain, unattached Form.</summary>
        public static TUc HostPage<TUc>() where TUc : UserControl, new()
        {
            var host = new Form { ShowInTaskbar = false, ClientSize = new System.Drawing.Size(1000, 700), StartPosition = FormStartPosition.Manual };
            var page = new TUc { Dock = DockStyle.Fill };
            host.Controls.Add(page);
            host.Show();
            Pump();
            return page;
        }

        public static void ClosePage(UserControl page)
        {
            Form host = page.FindForm();
            if (host != null) host.Close();
            Pump();
        }

        public static void CloseAllFormsSafe()
        {
            try { CloseAllForms(); } catch (Exception) { /* best effort during teardown */ }
        }

        public static void CloseAllForms()
        {
            foreach (Form f in Application.OpenForms.Cast<Form>().ToList()) f.Close();
            Pump();
        }
    }
}
