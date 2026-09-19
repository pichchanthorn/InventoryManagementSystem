using System;
using System.Collections.Generic;
using System.Linq;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>Group G: Orders. Approved semantics: confirmation deducts stock directly and never creates StockOut rows.</summary>
    internal static class Tests_Orders
    {
        private static OrderDetailEntity Line(int product, int qty, decimal price)
        { return new OrderDetailEntity { ProductID = product, Quantity = qty, UnitPrice = price }; }

        private static string Status(int orderId) { return T.Str("SELECT Status FROM Orders WHERE OrderID=@p0", orderId); }

        public static void Run()
        {
            T.Group("G Orders");
            T.Wipe();
            var bll = new OrderBLL();
            int cat = T.NewCategory("ZZREG_Cat");
            int a = T.NewProduct("ZZREG_A", cat, 10, 20m);
            int b = T.NewProduct("ZZREG_B", cat, 5, 7.5m);
            int cust = T.NewCustomer("ZZREG_Cust");
            int emp = T.NewEmployee("ZZREG_Emp");
            var header = new OrderEntity { CustomerID = cust, EmployeeID = emp };

            // ---- validation -------------------------------------------------------------------
            T.Check("G00 null order rejected", !bll.CreateOrder(null, new List<OrderDetailEntity> { Line(a, 1, 1) }).Success);
            T.Check("G00 order without items rejected", !bll.CreateOrder(header, new List<OrderDetailEntity>()).Success);
            T.Check("G00 null item list rejected", !bll.CreateOrder(header, null).Success);
            T.Check("G00 quantity 0 rejected", !bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, 0, 1) }).Success);
            T.Check("G00 negative price rejected", !bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, 1, -1) }).Success);
            T.Check("G00 product 0 rejected", !bll.CreateOrder(header, new List<OrderDetailEntity> { Line(0, 1, 1) }).Success);
            T.Check("G00 unknown product rejected", !bll.CreateOrder(header, new List<OrderDetailEntity> { Line(999999, 1, 1) }).Success);
            T.Check("G00 unknown customer rejected", !bll.CreateOrder(new OrderEntity { CustomerID = 999999 }, new List<OrderDetailEntity> { Line(a, 1, 1) }).Success);
            T.Check("G00 unknown employee rejected", !bll.CreateOrder(new OrderEntity { EmployeeID = 999999 }, new List<OrderDetailEntity> { Line(a, 1, 1) }).Success);
            T.Check("G00 one bad line rejects the whole order", !bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, 1, 1), Line(b, 0, 1) }).Success);
            T.Eq("G00 rejected orders created no Orders rows", 0L, T.Count("Orders"));
            T.Eq("G00 rejected orders created no OrderDetails rows", 0L, T.Count("OrderDetails"));

            // ---- creation ---------------------------------------------------------------------
            long stockOutBefore = T.Count("StockOut");
            OrderResult c1 = bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, 3, 19.99m), Line(b, 2, 7.5m) });
            T.Check("G01 order created", c1.Success && c1.OrderID > 0, c1.Message);
            T.Eq("G01 new order starts Pending", "Pending", Status(c1.OrderID));
            T.Check("G01 OrderDate is set", !T.IsNull("SELECT OrderDate FROM Orders WHERE OrderID=@p0", c1.OrderID));
            T.Eq("G02 Pending order did NOT deduct stock (A)", 10, T.Stock(a));
            T.Eq("G02 Pending order did NOT deduct stock (B)", 5, T.Stock(b));
            T.Eq("G03 two OrderDetails rows stored", 2L, T.Scalar("SELECT COUNT(*) FROM OrderDetails WHERE OrderID=@p0", c1.OrderID));
            T.Eq("G04 line total = qty x price (3 x 19.99)", 59.97m, T.Dec("SELECT Total FROM OrderDetails WHERE OrderID=@p0 AND ProductID=@p1", c1.OrderID, a));
            T.Eq("G04 line total (2 x 7.50)", 15.00m, T.Dec("SELECT Total FROM OrderDetails WHERE OrderID=@p0 AND ProductID=@p1", c1.OrderID, b));
            T.Eq("G05 order TotalAmount = sum of line totals", 74.97m, T.Dec("SELECT TotalAmount FROM Orders WHERE OrderID=@p0", c1.OrderID));
            T.Eq("G05 TotalAmount equals SUM(OrderDetails.Total)", T.Dec("SELECT TotalAmount FROM Orders WHERE OrderID=@p0", c1.OrderID),
                T.Dec("SELECT SUM(Total) FROM OrderDetails WHERE OrderID=@p0", c1.OrderID));
            T.Check("G01 customer and employee are optional", bll.CreateOrder(new OrderEntity(), new List<OrderDetailEntity> { Line(a, 1, 1) }).Success);
            int noPartyOrder = T.Id("SELECT MAX(OrderID) FROM Orders");
            T.Check("G01 optional parties stored as NULL", T.IsNull("SELECT CustomerID FROM Orders WHERE OrderID=@p0", noPartyOrder) && T.IsNull("SELECT EmployeeID FROM Orders WHERE OrderID=@p0", noPartyOrder));

            // ---- confirmation -----------------------------------------------------------------
            OrderResult cf = bll.ConfirmOrder(c1.OrderID);
            T.Check("G06 Pending order confirms", cf.Success, cf.Message);
            T.Eq("G07 status is Confirmed", "Confirmed", Status(c1.OrderID));
            T.Eq("G06 stock A deducted exactly once (10 - 3)", 7, T.Stock(a));
            T.Eq("G06 stock B deducted exactly once (5 - 2)", 3, T.Stock(b));
            T.Eq("G16 confirmation created NO StockOut row", stockOutBefore, T.Count("StockOut"));
            OrderResult again = bll.ConfirmOrder(c1.OrderID);
            T.Check("G08 Confirmed order cannot be confirmed again", !again.Success, again.Message);
            T.Eq("G08 no second deduction (A)", 7, T.Stock(a));
            T.Eq("G08 no second deduction (B)", 3, T.Stock(b));
            OrderResult cancelConfirmed = bll.CancelOrder(c1.OrderID);
            T.Check("G09 Confirmed order cannot be cancelled", !cancelConfirmed.Success, cancelConfirmed.Message);
            T.Eq("G09 order stays Confirmed", "Confirmed", Status(c1.OrderID));
            T.Eq("G09 stock unaffected by the rejected cancel", 7, T.Stock(a));

            // ---- cancellation -----------------------------------------------------------------
            OrderResult c2 = bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, 2, 20m) });
            OrderResult cx = bll.CancelOrder(c2.OrderID);
            T.Check("G10 Pending order cancels", cx.Success, cx.Message);
            T.Eq("G10 status is Cancelled", "Cancelled", Status(c2.OrderID));
            T.Eq("G11 cancelling deducted no stock", 7, T.Stock(a));
            T.Check("G12 Cancelled order cannot be confirmed", !bll.ConfirmOrder(c2.OrderID).Success);
            T.Eq("G12 ... status unchanged, stock unchanged", "Cancelled|7", Status(c2.OrderID) + "|" + T.Stock(a));
            T.Check("G13 Cancelled order cannot be cancelled again", !bll.CancelOrder(c2.OrderID).Success);
            T.Eq("G16 still no StockOut rows", stockOutBefore, T.Count("StockOut"));

            // ---- insufficient stock -> full rollback -------------------------------------------
            int sa = T.Stock(a), sb = T.Stock(b);
            OrderResult c3 = bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, 1, 20m), Line(b, sb + 50, 7.5m) });
            T.Check("G14 order with an unfulfillable line can still be created as Pending", c3.Success);
            OrderResult f3 = bll.ConfirmOrder(c3.OrderID);
            T.Check("G14 confirmation rejected for insufficient stock", !f3.Success && f3.Message.Contains("insufficient"), f3.Message);
            T.Eq("G14 rollback: product A (fulfillable line, processed first) NOT deducted", sa, T.Stock(a));
            T.Eq("G14 rollback: product B untouched", sb, T.Stock(b));
            T.Eq("G15 order was NOT partially confirmed - still Pending", "Pending", Status(c3.OrderID));

            OrderResult c4 = bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, sa, 20m), Line(a, 1, 20m) });
            T.Check("G14 same product twice: line 1 fits, line 1+2 do not -> rejected", !bll.ConfirmOrder(c4.OrderID).Success);
            T.Eq("G14 rollback undid line 1's deduction", sa, T.Stock(a));
            T.Eq("G15 order still Pending", "Pending", Status(c4.OrderID));
            T.Eq("G16 no StockOut rows from failed confirmations", stockOutBefore, T.Count("StockOut"));

            // ---- other confirmation edge cases -------------------------------------------------
            int emptyId = OrderDAL.CreatePendingOrder(new OrderEntity { TotalAmount = 0 }, new List<OrderDetailEntity>());
            OrderResult empty = bll.ConfirmOrder(emptyId);
            T.Check("G06 an order with no items cannot be confirmed", !empty.Success && Status(emptyId) == "Pending", empty.Message);
            T.Check("G06 confirming a non-existent order fails", !bll.ConfirmOrder(999999).Success);
            T.Check("G06 confirming ID 0 fails", !bll.ConfirmOrder(0).Success);
            T.Check("G10 cancelling a non-existent order fails", !bll.CancelOrder(999999).Success);
            T.Check("G10 cancelling ID 0 fails", !bll.CancelOrder(0).Success);

            // ---- concurrency: two confirmations of one order deduct only once -------------------
            T.Exec("UPDATE Products SET QtyInStock=10 WHERE ProductID=@p0", a);
            OrderResult race = bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, 6, 20m) });
            var wins = Tests_Stock.Race(() => bll.ConfirmOrder(race.OrderID).Success, () => bll.ConfirmOrder(race.OrderID).Success);
            T.Eq("G06 exactly one of two racing confirmations wins", 1, wins.Count(x => x));
            T.Eq("G06 stock deducted once (10 - 6)", 4, T.Stock(a));

            // ---- historical price -------------------------------------------------------------
            T.Exec("UPDATE Products SET QtyInStock=10, UnitPrice=20 WHERE ProductID=@p0", a);
            OrderResult h = bll.CreateOrder(header, new List<OrderDetailEntity> { Line(a, 2, 20m) });
            bll.ConfirmOrder(h.OrderID);
            var prod = new ProductBLL().GetAll().Single(p => p.ProductID == a);
            prod.UnitPrice = 99m;
            T.Check("G17 product price changed to 99", new ProductBLL().Update(prod).Success && T.Dec("SELECT UnitPrice FROM Products WHERE ProductID=@p0", a) == 99m);
            OrderDetailEntity hd = bll.GetDetails(h.OrderID).Single();
            T.Eq("G17 historical OrderDetail UnitPrice unchanged (20.00)", 20m, hd.UnitPrice);
            T.Eq("G17 historical line Total unchanged (40.00)", 40m, hd.Total);
            T.Eq("G17 historical order TotalAmount unchanged", 40m, T.Dec("SELECT TotalAmount FROM Orders WHERE OrderID=@p0", h.OrderID));

            // ---- history + details are read-only ------------------------------------------------
            string snap = T.Snapshot();
            List<OrderEntity> all = bll.GetAll();
            T.Check("G18 history lists every order", all.Count == T.Count("Orders"));
            T.Eq("G18 status filter Confirmed", (int)T.Scalar("SELECT COUNT(*) FROM Orders WHERE Status='Confirmed'"), bll.GetAll("Confirmed").Count);
            T.Eq("G18 status filter Cancelled", 1, bll.GetAll("Cancelled").Count);
            T.Eq("G18 customer filter", (int)T.Scalar("SELECT COUNT(*) FROM Orders WHERE CustomerID=@p0", cust), bll.GetAll(null, cust).Count);
            T.Check("G18 search by customer name", bll.GetAll(null, null, null, null, "ZZREG_Cust").Count > 0);
            T.Check("G18 search by order id", bll.GetAll(null, null, null, null, c1.OrderID.ToString()).Any(o => o.OrderID == c1.OrderID));
            T.Check("G18 orders without customer/employee load with null names (no crash)", all.Any(o => o.CustomerName == null && o.EmployeeName == null));
            foreach (OrderEntity o in all) bll.GetDetails(o.OrderID);
            T.Eq("G19 viewing history and every order's details modified nothing", snap, T.Snapshot());
        }
    }
}
