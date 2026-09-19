using System;
using System.Data.SqlClient;
using System.Linq;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>Groups B (Category), C (Product) and D (Customer / Supplier / Employee).</summary>
    internal static class Tests_Masters
    {
        private static string Z(int totalLength) { return T.TestDbPrefix + new string('x', totalLength - T.TestDbPrefix.Length); }

        // =====================================================================================
        public static void RunCategory()
        {
            T.Group("B Category");
            T.Wipe();
            var bll = new CategoryBLL();

            T.Check("B01 blank name rejected", !bll.Add(new CategoryEntity { CategoryName = "   " }).Success);
            T.Check("B01 null name rejected", !bll.Add(new CategoryEntity { CategoryName = null }).Success);
            T.Check("B01 null entity rejected", !bll.Add(null).Success);
            T.Eq("B01 nothing was inserted by rejected adds", 0L, T.Count("Categories"));

            CategoryResult r = bll.Add(new CategoryEntity { CategoryName = "  ZZREG_Trim  ", Description = "  a description  " });
            T.Check("B02 valid insert succeeds", r.Success, r.Message);
            T.Eq("B02 name is trimmed in the database", "ZZREG_Trim", T.Str("SELECT CategoryName FROM Categories"));
            T.Eq("B02 description is trimmed in the database", "a description", T.Str("SELECT Description FROM Categories"));

            T.Check("B03 100-char name accepted", bll.Add(new CategoryEntity { CategoryName = Z(100) }).Success);
            T.Check("B03 101-char name rejected", !bll.Add(new CategoryEntity { CategoryName = Z(101) }).Success);
            T.Check("B03 101-char name that is 100 after trimming is accepted (trim before length check)",
                bll.Add(new CategoryEntity { CategoryName = " " + Z(100).Replace("x", "y") }).Success);

            CategoryResult dup = bll.Add(new CategoryEntity { CategoryName = "ZZREG_Trim" });
            T.Check("B05 duplicate name rejected", !dup.Success && dup.Message.Contains("already exists"), dup.Message);
            T.Check("B05 duplicate ignoring case rejected", !bll.Add(new CategoryEntity { CategoryName = "zzreg_TRIM" }).Success);
            T.Check("B05 duplicate ignoring surrounding spaces rejected", !bll.Add(new CategoryEntity { CategoryName = "   ZZREG_Trim " }).Success);

            T.Check("B10 blank description stored as NULL", bll.Add(new CategoryEntity { CategoryName = "ZZREG_NoDesc", Description = "   " }).Success
                && T.IsNull("SELECT Description FROM Categories WHERE CategoryName='ZZREG_NoDesc'"));
            T.Check("B10 255-char description accepted", bll.Add(new CategoryEntity { CategoryName = "ZZREG_Desc255", Description = new string('d', 255) }).Success);
            T.Check("B10 256-char description rejected", !bll.Add(new CategoryEntity { CategoryName = "ZZREG_Desc256", Description = new string('d', 256) }).Success);

            // update
            int id = T.Id("SELECT CategoryID FROM Categories WHERE CategoryName='ZZREG_Trim'");
            T.Check("B06 valid update succeeds", bll.Update(new CategoryEntity { CategoryID = id, CategoryName = " ZZREG_Renamed ", Description = null }).Success);
            T.Eq("B06 update persisted (trimmed)", "ZZREG_Renamed", T.Str("SELECT CategoryName FROM Categories WHERE CategoryID=@p0", id));
            T.Check("B06 update cleared description to NULL", T.IsNull("SELECT Description FROM Categories WHERE CategoryID=@p0", id));
            T.Check("B06 update to its own current name is allowed", bll.Update(new CategoryEntity { CategoryID = id, CategoryName = "ZZREG_Renamed" }).Success);
            T.Check("B06 update to another category's name rejected", !bll.Update(new CategoryEntity { CategoryID = id, CategoryName = "ZZREG_NoDesc" }).Success);
            T.Check("B06 update with blank name rejected", !bll.Update(new CategoryEntity { CategoryID = id, CategoryName = " " }).Success);
            T.Eq("B06 rejected updates left the name unchanged", "ZZREG_Renamed", T.Str("SELECT CategoryName FROM Categories WHERE CategoryID=@p0", id));
            T.Check("B07 update with ID 0 rejected", !bll.Update(new CategoryEntity { CategoryID = 0, CategoryName = "ZZREG_X" }).Success);
            T.Check("B07 update with negative ID rejected", !bll.Update(new CategoryEntity { CategoryID = -5, CategoryName = "ZZREG_X" }).Success);
            T.Check("B07 update with null entity rejected", !bll.Update(null).Success);
            string snap = T.Snapshot();
            bll.Update(new CategoryEntity { CategoryID = 999999, CategoryName = "ZZREG_Ghost" });
            T.Eq("B07 update of a non-existent ID changes no data", snap, T.Snapshot());

            // delete
            T.Check("B07 delete with ID 0 rejected", !bll.Delete(0).Success);
            T.Check("B07 delete with negative ID rejected", !bll.Delete(-1).Success);

            int usedCat = T.NewCategory("ZZREG_Used");
            T.NewProduct("ZZREG_Prod", usedCat, 1);
            CategoryResult del = bll.Delete(usedCat);
            T.Check("B09 delete with dependent product rejected safely", !del.Success && del.Message.Contains("used by"), del.Message);
            T.Eq("B09 category still exists after rejected delete", 1L, T.Scalar("SELECT COUNT(*) FROM Categories WHERE CategoryID=@p0", usedCat));
            T.Eq("B09 product still exists after rejected delete", 1L, T.Scalar("SELECT COUNT(*) FROM Products WHERE CategoryID=@p0", usedCat));

            int freeCat = T.NewCategory("ZZREG_Free");
            T.Check("B08 delete of an unused category succeeds", bll.Delete(freeCat).Success);
            T.Eq("B08 category is gone", 0L, T.Scalar("SELECT COUNT(*) FROM Categories WHERE CategoryID=@p0", freeCat));

            // SQL-level failure handling
            long before = T.Count("Categories");
            bool threw = false;
            try { CategoryDAL.Insert(new CategoryEntity { CategoryName = "ZZREG_Renamed" }); }
            catch (SqlException ex) { threw = ex.Number == 2627 || ex.Number == 2601; }
            T.Check("B11 DAL surfaces a unique-key violation as SqlException (which the BLL maps to a friendly message)", threw);
            T.Eq("B11 state not corrupted by the failed insert", before, T.Count("Categories"));
            T.Check("B11 BLL keeps working after a SQL error", bll.Add(new CategoryEntity { CategoryName = "ZZREG_AfterError" }).Success);
            T.Check("B11 GetAll returns the rows", bll.GetAll().Any(c => c.CategoryName == "ZZREG_AfterError"));
        }

        // =====================================================================================
        private static ProductEntity P(int cat, string name = "ZZREG_P", decimal price = 10m, int stock = 5, int reorder = 2, string barcode = null, string desc = null)
        {
            return new ProductEntity { ProductName = name, CategoryID = cat, UnitPrice = price, QtyInStock = stock, ReorderLevel = reorder, Barcode = barcode, Description = desc };
        }

        public static void RunProduct()
        {
            T.Group("C Product");
            T.Wipe();
            var bll = new ProductBLL();
            int cat = T.NewCategory("ZZREG_Cat");

            T.Check("C01 blank name rejected", !bll.Add(P(cat, "  ")).Success);
            T.Check("C01 null entity rejected", !bll.Add(null).Success);
            T.Check("C03 category 0 rejected", !bll.Add(P(0)).Success);
            T.Check("C03 negative category rejected", !bll.Add(P(-1)).Success);
            T.Check("C04 non-existent category rejected", !bll.Add(P(999999)).Success);
            T.Check("C05 negative unit price rejected", !bll.Add(P(cat, price: -0.01m)).Success);
            T.Check("C05 zero unit price accepted", bll.Add(P(cat, "ZZREG_Zero", price: 0m)).Success);
            T.Check("C06 negative initial stock rejected", !bll.Add(P(cat, stock: -1)).Success);
            T.Check("C07 negative reorder level rejected", !bll.Add(P(cat, reorder: -1)).Success);
            T.Eq("C01-07 rejected adds inserted nothing (only the zero-price product exists)", 1L, T.Count("Products"));

            ProductResult r = bll.Add(P(cat, "  ZZREG_Widget  ", 12.5m, 7, 3, "  ZZREG-BC-1  ", "  desc  "));
            T.Check("C11 valid insert succeeds", r.Success, r.Message);
            ProductEntity w = bll.GetAll().Single(p => p.ProductName == "ZZREG_Widget");
            T.Eq("C02 name trimmed", "ZZREG_Widget", w.ProductName);
            T.Eq("C08 barcode trimmed", "ZZREG-BC-1", w.Barcode);
            T.Eq("C16 description trimmed", "desc", w.Description);
            T.Eq("C11 initial stock stored", 7, w.QtyInStock);
            T.Eq("C11 price stored", 12.5m, w.UnitPrice);
            T.Eq("C11 reorder stored", 3, w.ReorderLevel);

            T.Check("C08 barcode is optional", bll.Add(P(cat, "ZZREG_NoBarcode")).Success && T.IsNull("SELECT Barcode FROM Products WHERE ProductName='ZZREG_NoBarcode'"));
            T.Check("C08 blank barcode stored as NULL (so several products may have no barcode)",
                bll.Add(P(cat, "ZZREG_BlankBarcode", barcode: "   ")).Success && T.IsNull("SELECT Barcode FROM Products WHERE ProductName='ZZREG_BlankBarcode'"));
            T.Check("C16 description optional -> NULL", T.IsNull("SELECT Description FROM Products WHERE ProductName='ZZREG_NoBarcode'"));

            ProductResult dup = bll.Add(P(cat, "ZZREG_Dup", barcode: "ZZREG-BC-1"));
            T.Check("C09 duplicate barcode rejected on insert", !dup.Success && dup.Message.Contains("barcode"), dup.Message);

            T.Check("C10 150-char name accepted", bll.Add(P(cat, Z(150))).Success);
            T.Check("C10 151-char name rejected", !bll.Add(P(cat, Z(151))).Success);
            T.Check("C10 100-char barcode accepted", bll.Add(P(cat, "ZZREG_BC100", barcode: new string('b', 100))).Success);
            T.Check("C10 101-char barcode rejected", !bll.Add(P(cat, "ZZREG_BC101", barcode: new string('b', 101))).Success);
            T.Check("C10 255-char description accepted", bll.Add(P(cat, "ZZREG_D255", desc: new string('d', 255))).Success);
            T.Check("C10 256-char description rejected", !bll.Add(P(cat, "ZZREG_D256", desc: new string('d', 256))).Success);

            // update
            var upd = P(cat, " ZZREG_Widget2 ", 99m, 12345, 9, "ZZREG-BC-2", null);
            upd.ProductID = w.ProductID;
            T.Check("C12 valid update succeeds", bll.Update(upd).Success);
            ProductEntity w2 = bll.GetAll().Single(p => p.ProductID == w.ProductID);
            T.Eq("C12 name/price/reorder/barcode updated", "ZZREG_Widget2|99|9|ZZREG-BC-2", w2.ProductName + "|" + (int)w2.UnitPrice + "|" + w2.ReorderLevel + "|" + w2.Barcode);
            T.Eq("C13 update did NOT change QtyInStock (entity carried 12345)", 7, w2.QtyInStock);
            T.Check("C12 update keeping its own barcode is allowed", bll.Update(upd).Success);

            var clash = P(cat, "ZZREG_Widget2", barcode: "ZZREG-BC-1"); // BC-1 was freed by the rename above
            clash.ProductID = w.ProductID;
            int other = bll.GetAll().Single(p => p.ProductName == "ZZREG_NoBarcode").ProductID;
            T.Check("C09 the old barcode was released by the update", bll.Update(new ProductEntity { ProductID = other, ProductName = "ZZREG_NoBarcode", CategoryID = cat, UnitPrice = 1, Barcode = "ZZREG-BC-1" }).Success);
            T.Check("C09 duplicate barcode rejected on update", !bll.Update(clash).Success);
            T.Check("C14 update with ID 0 rejected", !bll.Update(P(cat)).Success);
            T.Check("C14 update with negative ID rejected", !bll.Update(new ProductEntity { ProductID = -1, ProductName = "ZZREG_X", CategoryID = cat }).Success);
            var badCat = P(cat); badCat.ProductID = w.ProductID; badCat.CategoryID = 999999;
            T.Check("C04 update to a non-existent category rejected", !bll.Update(badCat).Success);
            var neg = P(cat, price: -1m); neg.ProductID = w.ProductID;
            T.Check("C05 update with negative price rejected", !bll.Update(neg).Success);

            // delete
            T.Check("C14 delete with ID 0 rejected", !bll.Delete(0).Success);
            int hist = T.NewProduct("ZZREG_History", cat, 5);
            T.Exec("INSERT StockIn(ProductID,Quantity,UnitCost,TotalCost,DateIn) VALUES (@p0,1,1,1,SYSDATETIME())", hist);
            ProductResult del = bll.Delete(hist);
            T.Check("C15 delete of a product with StockIn history rejected safely", !del.Success && del.Message.Contains("referenced"), del.Message);
            T.Eq("C15 product still there", 1L, T.Scalar("SELECT COUNT(*) FROM Products WHERE ProductID=@p0", hist));

            int histOut = T.NewProduct("ZZREG_HistoryOut", cat, 5);
            T.Exec("INSERT StockOut(ProductID,Quantity,UnitPrice,TotalPrice,DateOut) VALUES (@p0,1,1,1,SYSDATETIME())", histOut);
            T.Check("C15 delete with StockOut history rejected", !bll.Delete(histOut).Success);

            int histOrd = T.NewProduct("ZZREG_HistoryOrd", cat, 5);
            int oid = T.Id("INSERT Orders(OrderDate,TotalAmount,Status) OUTPUT INSERTED.OrderID VALUES (SYSDATETIME(),1,'Pending')");
            T.Exec("INSERT OrderDetails(OrderID,ProductID,Quantity,UnitPrice,Total) VALUES (@p0,@p1,1,1,1)", oid, histOrd);
            T.Check("C15 delete with OrderDetails history rejected", !bll.Delete(histOrd).Success);

            int clean = T.NewProduct("ZZREG_Clean", cat, 0);
            T.Check("C15 delete of a product with no history succeeds", bll.Delete(clean).Success);
            T.Eq("C15 product removed", 0L, T.Scalar("SELECT COUNT(*) FROM Products WHERE ProductID=@p0", clean));
        }

        // =====================================================================================
        // Customer / Supplier / Employee share one contract; an adapter lets one body test all three.
        private sealed class Master
        {
            public string Name, Table, IdCol, NameCol, ExtraCol; // ExtraCol: ContactPerson / Gender / null
            public int MaxName, MaxPhone = 30, MaxEmail = 100, MaxAddress = 255, MaxExtra;
            public Func<string, string, string, string, string, Tuple<bool, string>> Add;            // name, extra, phone, email, address
            public Func<int, string, string, string, string, string, Tuple<bool, string>> Update;   // id, name, extra, phone, email, address
            public Func<int, Tuple<bool, string>> Delete;
            public Func<int> CountViaBll;
            public Func<string, int> Referenced; // creates a referencing transactional row, returns master id
        }

        private static Tuple<bool, string> Rt(bool ok, string msg) { return Tuple.Create(ok, msg); }

        private static Master[] Masters()
        {
            var cb = new CustomerBLL(); var sb = new SupplierBLL(); var eb = new EmployeeBLL();
            return new[]
            {
                new Master { Name = "Customer", Table = "Customers", IdCol = "CustomerID", NameCol = "CustomerName", MaxName = 150,
                    Add = (n, x, p, e, a) => { var r = cb.Add(new CustomerEntity { CustomerName = n, Phone = p, Email = e, Address = a }); return Rt(r.Success, r.Message); },
                    Update = (id, n, x, p, e, a) => { var r = cb.Update(new CustomerEntity { CustomerID = id, CustomerName = n, Phone = p, Email = e, Address = a }); return Rt(r.Success, r.Message); },
                    Delete = id => { var r = cb.Delete(id); return Rt(r.Success, r.Message); },
                    CountViaBll = () => cb.GetAll().Count,
                    Referenced = n => { int c = T.NewCustomer(n); int cat = T.NewCategory("ZZREG_C" + n); int pr = T.NewProduct("ZZREG_P" + n, cat, 1);
                        T.Exec("INSERT StockOut(ProductID,CustomerID,Quantity,UnitPrice,TotalPrice,DateOut) VALUES (@p0,@p1,1,1,1,SYSDATETIME())", pr, c); return c; } },
                new Master { Name = "Supplier", Table = "Suppliers", IdCol = "SupplierID", NameCol = "SupplierName", ExtraCol = "ContactPerson", MaxName = 150, MaxExtra = 150,
                    Add = (n, x, p, e, a) => { var r = sb.Add(new SupplierEntity { SupplierName = n, ContactPerson = x, Phone = p, Email = e, Address = a }); return Rt(r.Success, r.Message); },
                    Update = (id, n, x, p, e, a) => { var r = sb.Update(new SupplierEntity { SupplierID = id, SupplierName = n, ContactPerson = x, Phone = p, Email = e, Address = a }); return Rt(r.Success, r.Message); },
                    Delete = id => { var r = sb.Delete(id); return Rt(r.Success, r.Message); },
                    CountViaBll = () => sb.GetAll().Count,
                    Referenced = n => { int s = T.NewSupplier(n); int cat = T.NewCategory("ZZREG_C" + n); int pr = T.NewProduct("ZZREG_P" + n, cat, 1);
                        T.Exec("INSERT StockIn(ProductID,SupplierID,Quantity,UnitCost,TotalCost,DateIn) VALUES (@p0,@p1,1,1,1,SYSDATETIME())", pr, s); return s; } },
                new Master { Name = "Employee", Table = "Employees", IdCol = "EmployeeID", NameCol = "EmployeeName", ExtraCol = "Gender", MaxName = 150, MaxExtra = 20,
                    Add = (n, x, p, e, a) => { var r = eb.Add(new EmployeeEntity { EmployeeName = n, Gender = x, Phone = p, Email = e, Address = a }); return Rt(r.Success, r.Message); },
                    Update = (id, n, x, p, e, a) => { var r = eb.Update(new EmployeeEntity { EmployeeID = id, EmployeeName = n, Gender = x, Phone = p, Email = e, Address = a }); return Rt(r.Success, r.Message); },
                    Delete = id => { var r = eb.Delete(id); return Rt(r.Success, r.Message); },
                    CountViaBll = () => eb.GetAll().Count,
                    Referenced = n => { int e = T.NewEmployee(n); T.Exec("INSERT Orders(EmployeeID,OrderDate,TotalAmount,Status) VALUES (@p0,SYSDATETIME(),0,'Pending')", e); return e; } }
            };
        }

        public static void RunCustomerSupplierEmployee()
        {
            T.Group("D Customer / Supplier / Employee");
            foreach (Master m in Masters())
            {
                T.Wipe();
                string n = m.Name + ": ";
                string col(string c, string name) { return T.Str("SELECT " + c + " FROM " + m.Table + " WHERE " + m.NameCol + "=@p0", name); }
                bool isNull(string c, string name) { return T.IsNull("SELECT " + c + " FROM " + m.Table + " WHERE " + m.NameCol + "=@p0", name); }

                T.Check(n + "D01 blank name rejected", !m.Add("   ", null, null, null, null).Item1);
                T.Check(n + "D01 null name rejected", !m.Add(null, null, null, null, null).Item1);
                T.Eq(n + "D01 rejected adds inserted nothing", 0L, T.Count(m.Table));

                Tuple<bool, string> ok = m.Add("  ZZREG_" + m.Name + "  ", "  x ", "  012  ", "  a@b.c ", "  addr  ");
                T.Check(n + "D06 valid insert succeeds", ok.Item1, ok.Item2);
                string nm = "ZZREG_" + m.Name;
                T.Eq(n + "D02 name trimmed", 1L, T.Scalar("SELECT COUNT(*) FROM " + m.Table + " WHERE " + m.NameCol + "=@p0", nm));
                T.Eq(n + "D02 phone trimmed", "012", col("Phone", nm));
                T.Eq(n + "D02 email trimmed", "a@b.c", col("Email", nm));
                T.Eq(n + "D02 address trimmed", "addr", col("Address", nm));
                if (m.ExtraCol != null) T.Eq(n + "D02 " + m.ExtraCol + " trimmed", "x", col(m.ExtraCol, nm));

                // optional fields -> NULL
                m.Add("ZZREG_Optional", "  ", "  ", "", "   ");
                foreach (string c in new[] { "Phone", "Email", "Address" })
                    T.Check(n + "D04/D05 blank " + c + " stored as NULL", isNull(c, "ZZREG_Optional"));
                if (m.ExtraCol != null) T.Check(n + "D04/D05 blank " + m.ExtraCol + " stored as NULL", isNull(m.ExtraCol, "ZZREG_Optional"));
                T.Check(n + "D04 only the name is required", m.Add("ZZREG_OnlyName", null, null, null, null).Item1);

                // max lengths
                T.Check(n + "D03 max-length name accepted", m.Add(Z(m.MaxName), null, null, null, null).Item1);
                T.Check(n + "D03 name +1 char rejected", !m.Add(Z(m.MaxName + 1), null, null, null, null).Item1);
                T.Check(n + "D03 phone at limit accepted", m.Add("ZZREG_Ph1", null, new string('1', m.MaxPhone), null, null).Item1);
                T.Check(n + "D03 phone +1 rejected", !m.Add("ZZREG_Ph2", null, new string('1', m.MaxPhone + 1), null, null).Item1);
                T.Check(n + "D03 email at limit accepted", m.Add("ZZREG_Em1", null, null, new string('e', m.MaxEmail), null).Item1);
                T.Check(n + "D03 email +1 rejected", !m.Add("ZZREG_Em2", null, null, new string('e', m.MaxEmail + 1), null).Item1);
                T.Check(n + "D03 address at limit accepted", m.Add("ZZREG_Ad1", null, null, null, new string('a', m.MaxAddress)).Item1);
                T.Check(n + "D03 address +1 rejected", !m.Add("ZZREG_Ad2", null, null, null, new string('a', m.MaxAddress + 1)).Item1);
                if (m.ExtraCol != null)
                {
                    T.Check(n + "D03 " + m.ExtraCol + " at limit accepted", m.Add("ZZREG_Ex1", new string('g', m.MaxExtra), null, null, null).Item1);
                    T.Check(n + "D03 " + m.ExtraCol + " +1 rejected", !m.Add("ZZREG_Ex2", new string('g', m.MaxExtra + 1), null, null, null).Item1);
                }

                // D10: no invented uniqueness rule - duplicate names and phones are still accepted
                T.Check(n + "D10 duplicate name accepted (no uniqueness rule exists)", m.Add(nm, null, null, null, null).Item1 && T.Scalar("SELECT COUNT(*) FROM " + m.Table + " WHERE " + m.NameCol + "=@p0", nm) == 2);
                T.Check(n + "D10 duplicate phone accepted", m.Add("ZZREG_PhoneA", null, "555", null, null).Item1 && m.Add("ZZREG_PhoneB", null, "555", null, null).Item1);

                // update
                int id = T.Id("SELECT TOP 1 " + m.IdCol + " FROM " + m.Table + " WHERE " + m.NameCol + "=@p0 ORDER BY 1", nm);
                Tuple<bool, string> up = m.Update(id, "  ZZREG_Renamed ", "y", " 999 ", null, "  ");
                T.Check(n + "D07 valid update succeeds", up.Item1, up.Item2);
                T.Eq(n + "D07 update persisted + trimmed", "999", col("Phone", "ZZREG_Renamed"));
                T.Check(n + "D07 update nulled blank optional fields", isNull("Email", "ZZREG_Renamed") && isNull("Address", "ZZREG_Renamed"));
                T.Check(n + "D07 update with blank name rejected", !m.Update(id, " ", null, null, null, null).Item1);
                T.Check(n + "D07 update with over-long name rejected", !m.Update(id, Z(m.MaxName + 1), null, null, null, null).Item1);
                T.Eq(n + "D07 rejected updates did not alter the row", 1L, T.Scalar("SELECT COUNT(*) FROM " + m.Table + " WHERE " + m.NameCol + "='ZZREG_Renamed'"));
                T.Check(n + "D08 update with ID 0 rejected", !m.Update(0, "ZZREG_X", null, null, null, null).Item1);
                T.Check(n + "D08 update with negative ID rejected", !m.Update(-3, "ZZREG_X", null, null, null, null).Item1);
                T.Check(n + "D08 delete with ID 0 rejected", !m.Delete(0).Item1);
                T.Check(n + "D08 delete with negative ID rejected", !m.Delete(-1).Item1);

                // delete
                int freeId = T.Id("SELECT " + m.IdCol + " FROM " + m.Table + " WHERE " + m.NameCol + "='ZZREG_OnlyName'");
                T.Check(n + "D09 delete of an unreferenced record succeeds", m.Delete(freeId).Item1);
                T.Eq(n + "D09 record removed", 0L, T.Scalar("SELECT COUNT(*) FROM " + m.Table + " WHERE " + m.IdCol + "=@p0", freeId));

                int usedId = m.Referenced("ZZREG_Used" + m.Name);
                Tuple<bool, string> del = m.Delete(usedId);
                T.Check(n + "D09 delete of a record referenced by history is rejected safely (friendly message)", !del.Item1 && !string.IsNullOrEmpty(del.Item2) && !del.Item2.Contains("SqlException"), del.Item2);
                T.Eq(n + "D09 referenced record still exists", 1L, T.Scalar("SELECT COUNT(*) FROM " + m.Table + " WHERE " + m.IdCol + "=@p0", usedId));
                T.Check(n + "GetAll works", m.CountViaBll() == T.Count(m.Table));
            }
        }
    }
}
