using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>Groups E (Stock In) and F (Stock Out) - the transaction-critical modules.</summary>
    internal static class Tests_Stock
    {
        /// <summary>Runs two actions at (nearly) the same instant and returns their results.</summary>
        internal static TR[] Race<TR>(Func<TR> a, Func<TR> b)
        {
            var gate = new Barrier(2);
            Task<TR> t1 = Task.Run(() => { gate.SignalAndWait(); return a(); });
            Task<TR> t2 = Task.Run(() => { gate.SignalAndWait(); return b(); });
            Task.WaitAll(t1, t2);
            return new[] { t1.Result, t2.Result };
        }

        // =====================================================================================
        public static void RunStockIn()
        {
            T.Group("E Stock In");
            T.Wipe();
            var bll = new StockInBLL();
            int cat = T.NewCategory("ZZREG_Cat");
            int prod = T.NewProduct("ZZREG_P", cat, 10);
            int sup = T.NewSupplier("ZZREG_Sup");

            StockInEntity In(int p, int q, decimal cost, int? s = null, string notes = null)
            { return new StockInEntity { ProductID = p, SupplierID = s, Quantity = q, UnitCost = cost, Notes = notes }; }

            T.Check("E01 product 0 rejected", !bll.Add(In(0, 1, 1)).Success);
            T.Check("E01 negative product rejected", !bll.Add(In(-1, 1, 1)).Success);
            T.Check("E01 null entity rejected", !bll.Add(null).Success);
            T.Check("E01 non-existent product rejected", !bll.Add(In(999999, 1, 1)).Success);
            T.Check("E03 quantity 0 rejected", !bll.Add(In(prod, 0, 1)).Success);
            T.Check("E03 negative quantity rejected", !bll.Add(In(prod, -1, 1)).Success);
            T.Check("E04 negative unit cost rejected", !bll.Add(In(prod, 1, -0.01m)).Success);
            T.Check("E02 non-existent supplier rejected", !bll.Add(In(prod, 1, 1, 999999)).Success);
            T.Check("E10 notes over 255 chars rejected", !bll.Add(In(prod, 1, 1, null, new string('n', 256))).Success);
            T.Eq("E-all rejected requests created no StockIn row", 0L, T.Count("StockIn"));
            T.Eq("E-all rejected requests changed no stock", 10, T.Stock(prod));

            // valid, with supplier
            StockInResult r = bll.Add(In(prod, 7, 2.50m, sup, "  first  "));
            T.Check("E06 valid Stock In succeeds", r.Success, r.Message);
            T.Eq("E06 exactly one StockIn row created", 1L, T.Count("StockIn"));
            T.Eq("E07 stock increased by exactly the quantity (10 + 7)", 17, T.Stock(prod));
            T.Eq("E05 TotalCost = Quantity x UnitCost", 17.50m, T.Dec("SELECT TotalCost FROM StockIn"));
            T.Eq("E05 UnitCost persisted", 2.50m, T.Dec("SELECT UnitCost FROM StockIn"));
            T.Eq("E10 notes trimmed", "first", T.Str("SELECT Notes FROM StockIn"));
            T.Eq("E02 supplier persisted", (long)sup, T.Scalar("SELECT SupplierID FROM StockIn"));

            // supplier optional
            T.Check("E02 supplier may be null", bll.Add(In(prod, 1, 0m)).Success);
            T.Check("E04 zero unit cost accepted", T.Dec("SELECT TOP 1 TotalCost FROM StockIn ORDER BY StockInID DESC") == 0m);
            T.Check("E02 null supplier stored as NULL", T.IsNull("SELECT TOP 1 SupplierID FROM StockIn ORDER BY StockInID DESC"));
            T.Check("E02 supplier id 0 treated as no supplier (NULL)", bll.Add(In(prod, 1, 1m, 0)).Success && T.IsNull("SELECT TOP 1 SupplierID FROM StockIn ORDER BY StockInID DESC"));
            T.Check("E10 blank notes stored as NULL", T.IsNull("SELECT TOP 1 Notes FROM StockIn ORDER BY StockInID DESC"));
            T.Eq("E07 stock accumulates correctly (17 + 1 + 1)", 19, T.Stock(prod));

            T.Check("E05 TotalCost with fractions (3 x 0.33 = 0.99)", bll.Add(In(prod, 3, 0.33m)).Success && T.Dec("SELECT TOP 1 TotalCost FROM StockIn ORDER BY StockInID DESC") == 0.99m);

            // atomicity: DAL bypasses BLL validation so the database itself has to refuse
            long rows = T.Count("StockIn"); int stock = T.Stock(prod);
            bool threw = false;
            try { StockInDAL.InsertWithStockUpdate(new StockInEntity { ProductID = prod, Quantity = 5, UnitCost = 1, TotalCost = -1 }); }
            catch (SqlException) { threw = true; }
            T.Check("E08/E09 constraint violation inside the DAL transaction throws", threw);
            T.Eq("E09 failed Stock In left no StockIn row", rows, T.Count("StockIn"));
            T.Eq("E09 failed Stock In left stock untouched", stock, T.Stock(prod));

            threw = false;
            try { StockInDAL.InsertWithStockUpdate(new StockInEntity { ProductID = 999999, Quantity = 5, UnitCost = 1, TotalCost = 5 }); }
            catch (SqlException) { threw = true; }
            T.Check("E09 Stock In for a missing product fails at the database", threw);
            T.Eq("E09 ... and creates nothing", rows, T.Count("StockIn"));

            // concurrency: no lost update
            int s0 = T.Stock(prod);
            var res = Race(() => bll.Add(In(prod, 5, 1m)).Success, () => bll.Add(In(prod, 5, 1m)).Success);
            T.Check("E08 two simultaneous Stock Ins both succeed", res.All(x => x));
            T.Eq("E08 stock rose by exactly 10 (no lost update)", s0 + 10, T.Stock(prod));

            // history is read-only and filters correctly
            int prod2 = T.NewProduct("ZZREG_P2", cat, 0);
            bll.Add(In(prod2, 4, 1m, sup, "ZZREG history note"));
            string snap = T.Snapshot();
            var all = bll.GetAll();
            T.Check("E11 unfiltered history returns every row", all.Count == T.Count("StockIn"));
            T.Eq("E11 product filter", 1, bll.GetAll(prod2).Count);
            T.Eq("E11 supplier filter", (int)T.Scalar("SELECT COUNT(*) FROM StockIn WHERE SupplierID=@p0", sup), bll.GetAll(null, sup).Count);
            T.Eq("E11 search on notes", 1, bll.GetAll(null, null, null, null, "history note").Count);
            T.Eq("E11 today's date range (inclusive To)", all.Count, bll.GetAll(null, null, DateTime.Today, DateTime.Today).Count);
            T.Eq("E11 yesterday-only range is empty", 0, bll.GetAll(null, null, DateTime.Today.AddDays(-2), DateTime.Today.AddDays(-1)).Count);
            T.Eq("E11 history retrieval modified nothing", snap, T.Snapshot());
        }

        // =====================================================================================
        public static void RunStockOut()
        {
            T.Group("F Stock Out");
            T.Wipe();
            var bll = new StockOutBLL();
            int cat = T.NewCategory("ZZREG_Cat");
            int prod = T.NewProduct("ZZREG_P", cat, 10, 20m);
            int cust = T.NewCustomer("ZZREG_Cust");

            StockOutEntity Out(int p, int q, decimal price, int? c = null, string notes = null)
            { return new StockOutEntity { ProductID = p, CustomerID = c, Quantity = q, UnitPrice = price, Notes = notes }; }

            T.Check("F01 product 0 rejected", !bll.Add(Out(0, 1, 1)).Success);
            T.Check("F01 null entity rejected", !bll.Add(null).Success);
            T.Check("F01 non-existent product rejected", !bll.Add(Out(999999, 1, 1)).Success);
            T.Check("F03 quantity 0 rejected", !bll.Add(Out(prod, 0, 1)).Success);
            T.Check("F03 negative quantity rejected", !bll.Add(Out(prod, -1, 1)).Success);
            T.Check("F04 negative unit price rejected", !bll.Add(Out(prod, 1, -1m)).Success);
            T.Check("F02 non-existent customer rejected", !bll.Add(Out(prod, 1, 1, 999999)).Success);
            T.Check("F12 notes over 255 chars rejected", !bll.Add(Out(prod, 1, 1, null, new string('n', 256))).Success);
            T.Eq("F-all rejected requests created no StockOut row", 0L, T.Count("StockOut"));
            T.Eq("F-all rejected requests changed no stock", 10, T.Stock(prod));

            StockOutResult r = bll.Add(Out(prod, 3, 19.99m, cust, "  sold  "));
            T.Check("F06 valid Stock Out succeeds", r.Success, r.Message);
            T.Eq("F06 exactly one StockOut row", 1L, T.Count("StockOut"));
            T.Eq("F07 stock decreased by exactly the quantity (10 - 3)", 7, T.Stock(prod));
            T.Eq("F05 TotalPrice = Quantity x UnitPrice (3 x 19.99)", 59.97m, T.Dec("SELECT TotalPrice FROM StockOut"));
            T.Eq("F12 notes trimmed", "sold", T.Str("SELECT Notes FROM StockOut"));
            T.Eq("F02 customer persisted", (long)cust, T.Scalar("SELECT CustomerID FROM StockOut"));

            T.Check("F02 customer optional", bll.Add(Out(prod, 1, 0m)).Success);
            T.Check("F02 no customer stored as NULL", T.IsNull("SELECT TOP 1 CustomerID FROM StockOut ORDER BY StockOutID DESC"));
            T.Check("F12 blank notes stored as NULL", T.IsNull("SELECT TOP 1 Notes FROM StockOut ORDER BY StockOutID DESC"));
            T.Eq("F07 stock now 6", 6, T.Stock(prod));

            // overselling
            long rows = T.Count("StockOut"); int stock = T.Stock(prod);
            StockOutResult over = bll.Add(Out(prod, stock + 1, 1m, cust));
            T.Check("F08 overselling by one unit rejected", !over.Success && over.Message.Contains("Insufficient stock"), over.Message);
            T.Eq("F09 failed oversell created NO StockOut record", rows, T.Count("StockOut"));
            T.Eq("F10 failed oversell did NOT change stock", stock, T.Stock(prod));
            T.Check("F08 selling the exact remaining stock succeeds", bll.Add(Out(prod, stock, 1m, cust)).Success);
            T.Eq("F07 stock is exactly 0", 0, T.Stock(prod));
            T.Check("F08 selling from zero stock rejected", !bll.Add(Out(prod, 1, 1m)).Success);
            T.Eq("F10 stock never went negative", 0, T.Stock(prod));

            // rollback: deduction happens first inside the transaction, then the insert fails on a CHECK constraint
            T.Exec("UPDATE Products SET QtyInStock=10 WHERE ProductID=@p0", prod);
            rows = T.Count("StockOut");
            bool threw = false;
            try { StockOutDAL.DeductStockAndInsert(new StockOutEntity { ProductID = prod, Quantity = 4, UnitPrice = 1, TotalPrice = -1 }); }
            catch (SqlException) { threw = true; }
            T.Check("F11 failure after the stock deduction throws", threw);
            T.Eq("F11 rollback restored the deducted stock (still 10)", 10, T.Stock(prod));
            T.Eq("F11 rollback left no StockOut row", rows, T.Count("StockOut"));

            // concurrency: two buyers of 6 against 10 units - exactly one may win
            T.Exec("UPDATE Products SET QtyInStock=10 WHERE ProductID=@p0", prod);
            rows = T.Count("StockOut");
            var res = Race(() => bll.Add(Out(prod, 6, 1m, cust)).Success, () => bll.Add(Out(prod, 6, 1m, cust)).Success);
            T.Eq("F11 exactly one of two racing oversells succeeds", 1, res.Count(x => x));
            T.Eq("F11 stock is 4 (deducted once)", 4, T.Stock(prod));
            T.Eq("F11 exactly one new StockOut row", rows + 1, T.Count("StockOut"));

            // history read-only + filters
            string snap = T.Snapshot();
            var all = bll.GetAll();
            T.Check("F13 unfiltered history returns every row", all.Count == T.Count("StockOut"));
            T.Eq("F13 customer filter", (int)T.Scalar("SELECT COUNT(*) FROM StockOut WHERE CustomerID=@p0", cust), bll.GetAll(null, cust).Count);
            T.Eq("F13 product filter", all.Count, bll.GetAll(prod).Count);
            T.Eq("F13 search on notes", 1, bll.GetAll(null, null, null, null, "sold").Count);
            T.Eq("F13 today's date range (inclusive To)", all.Count, bll.GetAll(null, null, DateTime.Today, DateTime.Today).Count);
            T.Eq("F13 history retrieval modified nothing", snap, T.Snapshot());
        }
    }
}
