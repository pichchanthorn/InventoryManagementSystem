using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>
    /// Regression runner.
    ///   (no args)         Isolated run: creates a disposable ZZREG_InventoryManagementDB from Database\InventoryManagementDB.sql,
    ///                     runs every group against it, then drops it. The development database is only READ (snapshot before/after).
    ///   --dev-readonly    Read-only smoke run against the real development database (no writes at all).
    ///   --keep-db         Isolated run, but leave the test database in place afterwards.
    /// Exit code 0 = all tests passed.
    /// </summary>
    internal static class Program
    {
        internal static string RepoRoot;
        internal static string DevConn;
        internal static string DevDbName;

        [STAThread]
        private static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool devReadOnly = Array.IndexOf(args, "--dev-readonly") >= 0;
            bool keepDb = Array.IndexOf(args, "--keep-db") >= 0;

            RepoRoot = FindRepoRoot();
            DevConn = ReadMainProjectConnectionString();
            DevDbName = new SqlConnectionStringBuilder(DevConn).InitialCatalog;

            Console.WriteLine("InventoryManagementSystem regression suite");
            Console.WriteLine("Mode: " + (devReadOnly ? "DEV READ-ONLY (" + DevDbName + ")" : "ISOLATED (disposable " + T.TestDbPrefix + " database)"));

            int exit;
            if (devReadOnly) exit = RunDevReadOnly();
            else exit = RunIsolated(keepDb);

            PrintSummary();
            return exit != 0 || T.TotalFail > 0 ? 1 : 0;
        }

        // ---------------------------------------------------------------------------------------
        private static int RunIsolated(bool keepDb)
        {
            string devBefore = null;
            try { devBefore = T.Snapshot(DevConn); }
            catch (Exception ex) { Console.WriteLine("NOTE: development database not readable (" + ex.Message + "); dev-untouched check skipped."); }

            string testDb = T.TestDbPrefix + DevDbName;
            var master = new SqlConnectionStringBuilder(DevConn) { InitialCatalog = "master" }.ConnectionString;
            string testConn = new SqlConnectionStringBuilder(DevConn) { InitialCatalog = testDb }.ConnectionString;

            try
            {
                DropDatabase(master, testDb);
                CreateDatabase(master, testDb);
                Console.WriteLine("Created test database " + testDb);

                Override(testConn);
                T.ConnStr = testConn;
                T.DbName = testDb;

                T.Run("Group A", () => Tests_Foundation.Run(true));
                T.Run("Empty states", Tests_ReportsDashboard.RunEmptyStates);
                T.Run("Group B", Tests_Masters.RunCategory);
                T.Run("Group C", Tests_Masters.RunProduct);
                T.Run("Group D", Tests_Masters.RunCustomerSupplierEmployee);
                T.Run("Group E", Tests_Stock.RunStockIn);
                T.Run("Group F", Tests_Stock.RunStockOut);
                T.Run("Group G", Tests_Orders.Run);
                T.Run("Group H", Tests_ReportsDashboard.RunReports);
                T.Run("Group I", Tests_ReportsDashboard.RunDashboard);
                T.Run("Group J", Tests_LoginCross.RunLoginAndNavigation);
                T.Run("Group K", Tests_LoginCross.RunCrossModule);
                T.Run("Group M", Tests_Shell.Run);
                T.Run("Group N", Tests_ReportPreview.Run);
                T.Run("Group O", Tests_UxCleanup.Run);

                T.Group("L Data integrity");
                T.Wipe();
                string emptyBaseline = T.Snapshot();
                // pristine = every table empty and every identity counter unused (NULL) or reset (0) so the next id is 1
                bool pristine = emptyBaseline.Split(new[] { " | " }, StringSplitOptions.None)
                    .All(part => System.Text.RegularExpressions.Regex.IsMatch(part, @"^\w+=0/0/(-|0)$"));
                T.Check("L01 wiped test DB is pristine (every table empty, every identity counter unused/reset)", pristine, emptyBaseline);
                T.Check("L02 test DB is not the development database", T.DbName != DevDbName && T.DbName.StartsWith(T.TestDbPrefix));
            }
            finally
            {
                T.CloseAllFormsSafe();
                if (!keepDb)
                {
                    try { DropDatabase(master, testDb); Console.WriteLine("Dropped test database " + testDb); }
                    catch (Exception ex) { Console.WriteLine("WARNING: could not drop " + testDb + ": " + ex.Message); }
                }
            }

            if (devBefore != null)
            {
                T.Group("L Data integrity");
                string devAfter = T.Snapshot(DevConn);
                T.Check("L03 development database untouched by the whole isolated run (counts, checksums, identities)",
                    devBefore == devAfter, "before: " + devBefore + "  after: " + devAfter);
                bool leftover = Convert.ToInt32(T.RawOn(master, "SELECT COUNT(*) FROM sys.databases WHERE name = @p0", testDb)) != 0;
                T.Check("L04 disposable test database " + (keepDb ? "kept (--keep-db)" : "dropped"), keepDb || !leftover);
            }
            return 0;
        }

        private static int RunDevReadOnly()
        {
            Override(DevConn);
            T.ConnStr = DevConn;
            T.DbName = DevDbName;

            string before = T.Snapshot();
            T.Run("Group A", () => Tests_Foundation.Run(false));
            T.Run("Dev smoke", Tests_ReportsDashboard.RunDevReadOnlySmoke);
            T.CloseAllFormsSafe();
            T.Group("L Data integrity");
            string after = T.Snapshot();
            T.Check("L03 development database unchanged by read-only run", before == after, "before: " + before + "  after: " + after);
            return 0;
        }

        // ---------------------------------------------------------------------------------------
        private static void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine("================ SUMMARY ================");
            foreach (string g in T.GroupOrder)
            {
                int[] s = T.Stats[g];
                Console.WriteLine(string.Format("{0,-40} pass {1,4}   fail {2,3}", g, s[0], s[1]));
            }
            Console.WriteLine(string.Format("{0,-40} pass {1,4}   fail {2,3}", "TOTAL", T.TotalPass, T.TotalFail));
            if (T.Failures.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("FAILURES:");
                foreach (string f in T.Failures) Console.WriteLine("  " + f);
            }
        }

        // ---------------------------------------------------------------------------------------
        private static string FindRepoRoot()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir, "InventoryManagementSystem", "Database", "InventoryManagementDB.sql")))
                    return dir;
                dir = Path.GetDirectoryName(dir.TrimEnd(Path.DirectorySeparatorChar));
            }
            throw new InvalidOperationException("Cannot locate the repository root (InventoryManagementSystem\\Database\\InventoryManagementDB.sql).");
        }

        internal static string MainProjectConfigPath { get { return Path.Combine(RepoRoot, "InventoryManagementSystem", "App.config"); } }

        private static string ReadMainProjectConnectionString()
        {
            XDocument doc = XDocument.Load(MainProjectConfigPath);
            foreach (XElement add in doc.Descendants("add"))
                if ((string)add.Attribute("name") == "InventoryDb")
                    return (string)add.Attribute("connectionString");
            throw new InvalidOperationException("InventoryDb connection string not found in the main project's App.config.");
        }

        /// <summary>Re-points the application's own DbConnection at another database. Must run before the first DAL call.</summary>
        private static void Override(string connectionString)
        {
            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings["InventoryDb"];
            typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(s, false);
            s.ConnectionString = connectionString;
        }

        private static void CreateDatabase(string master, string dbName)
        {
            string script = File.ReadAllText(Path.Combine(RepoRoot, "InventoryManagementSystem", "Database", "InventoryManagementDB.sql"));
            script = script.Replace("InventoryManagementDB", dbName);
            using (var c = new SqlConnection(master))
            {
                c.Open();
                foreach (string batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(batch)) continue;
                    using (var cmd = new SqlCommand(batch, c)) cmd.ExecuteNonQuery();
                }
            }
        }

        private static void DropDatabase(string master, string dbName)
        {
            if (!dbName.StartsWith(T.TestDbPrefix, StringComparison.Ordinal))
                throw new InvalidOperationException("Refusing to drop a non-" + T.TestDbPrefix + " database.");

            SqlConnection.ClearAllPools();
            using (var c = new SqlConnection(master))
            {
                c.Open();
                string sql = "IF DB_ID(@n) IS NOT NULL BEGIN ALTER DATABASE [" + dbName + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + dbName + "]; END";
                using (var cmd = new SqlCommand(sql, c)) { cmd.Parameters.AddWithValue("@n", dbName); cmd.ExecuteNonQuery(); }
            }
        }
    }
}
