-- ============================================================================================
-- DemoSeed.sql  -  clean, realistic agricultural demo dataset for InventoryManagementDB
-- ============================================================================================
-- DATA ONLY. No schema change, no application change. Currency is USD; money is stored as plain
-- DECIMAL values (no "$" in the database). All prices/costs below are demonstration values, NOT
-- real market prices. All people, phones, e-mails and addresses are fictional.
--
-- WHAT IT DOES
--   1. Removes the old disposable test/demo rows (the "ABC" product, "Test Customer", "Test
--      Supplier", "Test Employee", old Stock In test rows, ...) in foreign-key dependency order:
--          OrderDetails -> Orders -> StockOut -> StockIn -> Products -> Suppliers -> Customers
--          -> Employees -> Categories
--      The Users table (login accounts, e.g. devadmin) is NEVER touched.
--   2. Inserts 2 categories, 8 products, 3 suppliers, 4 customers, 3 employees, 12 Stock In,
--      7 Stock Out and 6 Orders (with 12 order lines) with natural sequential IDs (1, 2, 3 ...).
--   3. Sets Products.QtyInStock from the history, using the application's own rules:
--          QtyInStock = initial stock (0 - products start empty)
--                       + SUM(StockIn)  - SUM(StockOut)  - SUM(order lines of CONFIRMED orders)
--      Pending and Cancelled orders do NOT deduct stock. Orders never create StockOut rows.
--   4. Verifies foreign keys, calculations, statuses, stock and that stock never went negative
--      over time. Any failed check THROWs -> the whole transaction is rolled back.
--   5. Only after a successful COMMIT are the identity counters reseeded so that the next record
--      created in the application continues naturally (Products -> 9, StockIn -> 13, ...).
--
-- SAFE TO RE-RUN: running it again simply resets the demo data to exactly this dataset.
-- WARNING: it deletes ALL rows of the business tables (not Users). Use only on the demo database.
-- Run with:  sqlcmd -S localhost -E -I -b -i DemoSeed.sql      (-I = QUOTED_IDENTIFIER ON)
-- ============================================================================================
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
GO
USE InventoryManagementDB;
GO

SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    -- ----------------------------------------------------------------------------------
    -- 1. Remove old disposable test data (dependency order; Users is preserved)
    -- ----------------------------------------------------------------------------------
    DELETE FROM OrderDetails;
    DELETE FROM Orders;
    DELETE FROM StockOut;
    DELETE FROM StockIn;
    DELETE FROM Products;
    DELETE FROM Suppliers;
    DELETE FROM Customers;
    DELETE FROM Employees;
    DELETE FROM Categories;

    -- ----------------------------------------------------------------------------------
    -- 2. Categories (exactly two)
    -- ----------------------------------------------------------------------------------
    SET IDENTITY_INSERT Categories ON;
    INSERT Categories (CategoryID, CategoryName, Description) VALUES
        (1, N'Chemical Fertilizer', N'Chemical fertilizers for crop and rice production.'),
        (2, N'Natural Fertilizer',  N'Natural and organic fertilizers for agricultural use.');
    SET IDENTITY_INSERT Categories OFF;

    -- ----------------------------------------------------------------------------------
    -- 3. Suppliers, Customers, Employees (fictional demo contacts)
    -- ----------------------------------------------------------------------------------
    SET IDENTITY_INSERT Suppliers ON;
    INSERT Suppliers (SupplierID, SupplierName, ContactPerson, Phone, Email, Address) VALUES
        (1, N'Green Agro Supply',          N'Vannak Chea',   N'012 300 111', N'sales@greenagro.example.com',      N'No. 21, National Road 4, Phnom Penh'),
        (2, N'Mekong Agricultural Co.',    N'Sreyneang Kim', N'015 400 222', N'orders@mekongagri.example.com',    N'Street 271, Kampong Cham'),
        (3, N'Cambodia Fertilizer Supply', N'Rithy Ly',      N'097 500 333', N'info@cambodiafert.example.com',    N'National Road 5, Battambang');
    SET IDENTITY_INSERT Suppliers OFF;

    SET IDENTITY_INSERT Customers ON;
    INSERT Customers (CustomerID, CustomerName, Phone, Email, Address) VALUES
        (1, N'Sokha Farm',           N'012 111 001', N'sokhafarm@example.com',   N'Prey Veng Province'),
        (2, N'Dara Agriculture',     N'016 222 002', N'daraagri@example.com',    N'Kampong Thom Province'),
        (3, N'Chamroeun Rice Farm',  N'098 333 003', N'chamroeun@example.com',   N'Battambang Province'),
        (4, N'Happy Farmer',         N'077 444 004', N'happyfarmer@example.com', N'Takeo Province');
    SET IDENTITY_INSERT Customers OFF;

    SET IDENTITY_INSERT Employees ON;
    INSERT Employees (EmployeeID, EmployeeName, Gender, Phone, Email, Address) VALUES
        (1, N'Development Admin', N'Male',   N'012 700 001', N'admin@example.com', N'Phnom Penh'),
        (2, N'Sokha',             N'Male',   N'012 700 002', N'sokha@example.com', N'Phnom Penh'),
        (3, N'Dara',              N'Female', N'012 700 003', N'dara@example.com',  N'Phnom Penh');
    SET IDENTITY_INSERT Employees OFF;

    -- ----------------------------------------------------------------------------------
    -- 4. Products (start with QtyInStock = 0; stock is filled from the history in step 8)
    --    Barcodes are unique. Prices are USD demonstration values.
    --    Final stock vs reorder level (see verification): 2 clearly/near low, 1 close to reorder.
    -- ----------------------------------------------------------------------------------
    SET IDENTITY_INSERT Products ON;
    INSERT Products (ProductID, ProductName, CategoryID, UnitPrice, QtyInStock, Description, Barcode, ReorderLevel, CreatedAt, UpdatedAt) VALUES
        (1, N'Urea 46-0-0',         1, 22.00, 0, N'High-nitrogen chemical fertilizer, 50 kg bag.',            N'8851000000011', 20, '2025-01-05T08:00:00', '2025-01-05T08:00:00'),
        (2, N'DAP 18-46-0',         1, 30.00, 0, N'Diammonium phosphate for early crop growth, 50 kg bag.',   N'8851000000028', 15, '2025-01-05T08:00:00', '2025-01-05T08:00:00'),
        (3, N'NPK 15-15-15',        1, 25.00, 0, N'Balanced compound fertilizer, 50 kg bag.',                 N'8851000000035', 15, '2025-01-05T08:00:00', '2025-01-05T08:00:00'),
        (4, N'NPK 20-20-15',        1, 28.00, 0, N'High-phosphate compound fertilizer for rice, 50 kg bag.',  N'8851000000042', 10, '2025-01-05T08:00:00', '2025-01-05T08:00:00'),
        (5, N'Potassium 0-0-60',    1, 30.00, 0, N'Muriate of potash for fruiting and grain filling, 50 kg.', N'8851000000059', 12, '2025-01-05T08:00:00', '2025-01-05T08:00:00'),
        (6, N'Organic Fertilizer',  2, 16.00, 0, N'Organic fertilizer for vegetables and rice, 25 kg bag.',   N'8851000000066', 15, '2025-01-05T08:00:00', '2025-01-05T08:00:00'),
        (7, N'Compost Fertilizer',  2, 12.00, 0, N'Matured compost soil conditioner, 25 kg bag.',             N'8851000000073', 10, '2025-01-05T08:00:00', '2025-01-05T08:00:00'),
        (8, N'Natural Humic Fertilizer', 2, 20.00, 0, N'Humic-acid natural fertilizer to improve soil, 20 kg.', N'8851000000080', 8,  '2025-01-05T08:00:00', '2025-01-05T08:00:00');
    SET IDENTITY_INSERT Products OFF;

    -- ----------------------------------------------------------------------------------
    -- 5. Stock In history (12 rows, 2025 - 2026, chronological). TotalCost = Quantity x UnitCost.
    -- ----------------------------------------------------------------------------------
    SET IDENTITY_INSERT StockIn ON;
    INSERT StockIn (StockInID, ProductID, SupplierID, Quantity, UnitCost, TotalCost, DateIn, Notes) VALUES
        ( 1, 1, 1, 60, 17.50, 1050.00, '2025-01-10T09:15:00', N'Opening purchase'),
        ( 2, 3, 1, 35, 19.50,  682.50, '2025-01-10T09:40:00', NULL),
        ( 3, 2, 2, 50, 24.00, 1200.00, '2025-01-22T10:00:00', NULL),
        ( 4, 6, 3, 30, 12.00,  360.00, '2025-02-18T14:20:00', N'Organic batch A'),
        ( 5, 5, 2, 40, 24.00,  960.00, '2025-03-05T11:00:00', NULL),
        ( 6, 7, 3, 30,  9.00,  270.00, '2025-04-12T09:30:00', NULL),
        ( 7, 4, 1, 50, 22.50, 1125.00, '2025-06-03T15:10:00', N'Rice season stock'),
        ( 8, 8, 3, 25, 16.00,  400.00, '2025-09-15T10:45:00', NULL),
        ( 9, 1, 2, 40, 18.00,  720.00, '2025-11-20T13:30:00', N'Restock'),
        (10, 2, 1, 30, 25.00,  750.00, '2026-01-14T09:05:00', N'Restock'),
        (11, 3, 2, 25, 20.00,  500.00, '2026-03-09T10:20:00', NULL),
        (12, 6, 3, 20, 12.50,  250.00, '2026-08-25T08:50:00', N'Organic batch B');
    SET IDENTITY_INSERT StockIn OFF;

    -- ----------------------------------------------------------------------------------
    -- 6. Stock Out history (7 rows, chronological). TotalPrice = Quantity x UnitPrice.
    --    Direct stock issues; NOT related to Orders (Orders and Stock Out are separate concepts).
    -- ----------------------------------------------------------------------------------
    SET IDENTITY_INSERT StockOut ON;
    INSERT StockOut (StockOutID, ProductID, CustomerID, Quantity, UnitPrice, TotalPrice, DateOut, Notes) VALUES
        (1, 1, 1, 15, 22.00, 330.00, '2025-02-05T10:30:00', NULL),
        (2, 3, 2, 10, 25.00, 250.00, '2025-05-20T14:00:00', NULL),
        (3, 5, 3, 18, 30.00, 540.00, '2025-08-08T09:45:00', N'Delivered to farm'),
        (4, 1, 4, 10, 22.00, 220.00, '2025-12-02T11:15:00', NULL),
        (5, 7, 1, 14, 12.00, 168.00, '2026-02-17T13:20:00', NULL),
        (6, 6, 2, 20, 16.00, 320.00, '2026-09-02T10:10:00', NULL),
        (7, 2, 3, 20, 30.00, 600.00, '2026-09-10T15:40:00', N'Bulk sale');
    SET IDENTITY_INSERT StockOut OFF;

    -- ----------------------------------------------------------------------------------
    -- 7. Orders + order lines (6 orders: 4 Confirmed, 1 Pending, 1 Cancelled).
    --    Line Total = Quantity x UnitPrice; Order TotalAmount = SUM(line totals).
    --    Only CONFIRMED orders deduct stock (step 8); Pending/Cancelled do not.
    -- ----------------------------------------------------------------------------------
    SET IDENTITY_INSERT Orders ON;
    INSERT Orders (OrderID, CustomerID, EmployeeID, OrderDate, TotalAmount, Status) VALUES
        (1, 1, 2, '2025-04-14T10:00:00', 630.00, N'Confirmed'),
        (2, 3, 3, '2025-10-06T11:30:00', 860.00, N'Confirmed'),
        (3, 2, 2, '2026-04-22T09:50:00', 648.00, N'Confirmed'),
        (4, 3, 2, '2026-06-03T14:25:00', 140.00, N'Cancelled'),
        (5, 4, 1, '2026-07-15T16:05:00', 230.00, N'Confirmed'),
        (6, 1, 3, '2026-09-19T10:30:00', 460.00, N'Pending');
    SET IDENTITY_INSERT Orders OFF;

    SET IDENTITY_INSERT OrderDetails ON;
    INSERT OrderDetails (OrderDetailID, OrderID, ProductID, Quantity, UnitPrice, Total) VALUES
        ( 1, 1, 1, 15, 22.00, 330.00),   -- order 1: Urea
        ( 2, 1, 3, 12, 25.00, 300.00),   --          NPK 15-15-15
        ( 3, 2, 2, 10, 30.00, 300.00),   -- order 2: DAP
        ( 4, 2, 4, 20, 28.00, 560.00),   --          NPK 20-20-15
        ( 5, 3, 5,  8, 30.00, 240.00),   -- order 3: Potassium
        ( 6, 3, 6, 18, 16.00, 288.00),   --          Organic
        ( 7, 3, 8,  6, 20.00, 120.00),   --          Natural Humic
        ( 8, 4, 4,  5, 28.00, 140.00),   -- order 4 (Cancelled): NPK 20-20-15
        ( 9, 5, 7, 10, 12.00, 120.00),   -- order 5: Compost
        (10, 5, 1,  5, 22.00, 110.00),   --          Urea
        (11, 6, 2,  8, 30.00, 240.00),   -- order 6 (Pending): DAP
        (12, 6, 1, 10, 22.00, 220.00);   --          Urea
    SET IDENTITY_INSERT OrderDetails OFF;

    -- ----------------------------------------------------------------------------------
    -- 8. Product stock = history (initial 0 + Stock In - Stock Out - Confirmed order lines)
    -- ----------------------------------------------------------------------------------
    UPDATE p
    SET QtyInStock = ISNULL((SELECT SUM(Quantity) FROM StockIn  WHERE ProductID = p.ProductID), 0)
                   - ISNULL((SELECT SUM(Quantity) FROM StockOut WHERE ProductID = p.ProductID), 0)
                   - ISNULL((SELECT SUM(od.Quantity) FROM OrderDetails od JOIN Orders o ON o.OrderID = od.OrderID
                             WHERE od.ProductID = p.ProductID AND o.Status = N'Confirmed'), 0),
        UpdatedAt  = (SELECT MAX(d) FROM (SELECT DateIn AS d FROM StockIn WHERE ProductID = p.ProductID
                                          UNION ALL SELECT DateOut FROM StockOut WHERE ProductID = p.ProductID
                                          UNION ALL SELECT o.OrderDate FROM OrderDetails od JOIN Orders o ON o.OrderID = od.OrderID
                                                    WHERE od.ProductID = p.ProductID AND o.Status = N'Confirmed') x)
    FROM Products p;

    -- ----------------------------------------------------------------------------------
    -- 9. Verification - every failed check aborts (ROLLBACK) via THROW
    -- ----------------------------------------------------------------------------------
    DECLARE @n INT;

    -- 9a. row counts
    IF (SELECT COUNT(*) FROM Categories)   <> 2  THROW 51001, 'Expected 2 categories', 1;
    IF (SELECT COUNT(*) FROM Products)     <> 8  THROW 51002, 'Expected 8 products', 1;
    IF (SELECT COUNT(*) FROM Suppliers)    <> 3  THROW 51003, 'Expected 3 suppliers', 1;
    IF (SELECT COUNT(*) FROM Customers)    <> 4  THROW 51004, 'Expected 4 customers', 1;
    IF (SELECT COUNT(*) FROM Employees)    <> 3  THROW 51005, 'Expected 3 employees', 1;
    IF (SELECT COUNT(*) FROM StockIn)      <> 12 THROW 51006, 'Expected 12 stock-in rows', 1;
    IF (SELECT COUNT(*) FROM StockOut)     <> 7  THROW 51007, 'Expected 7 stock-out rows', 1;
    IF (SELECT COUNT(*) FROM Orders)       <> 6  THROW 51008, 'Expected 6 orders', 1;
    IF (SELECT COUNT(*) FROM OrderDetails) <> 12 THROW 51009, 'Expected 12 order lines', 1;
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = N'devadmin') THROW 51010, 'Login account devadmin missing', 1;

    -- 9b. relationships (FK constraints already enforce these; checked independently as well)
    SELECT @n = COUNT(*) FROM Products p WHERE NOT EXISTS (SELECT 1 FROM Categories c WHERE c.CategoryID = p.CategoryID);
    IF @n > 0 THROW 51011, 'Product with missing category', 1;
    SELECT @n = COUNT(*) FROM StockIn s WHERE NOT EXISTS (SELECT 1 FROM Products p WHERE p.ProductID = s.ProductID)
        OR (s.SupplierID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Suppliers x WHERE x.SupplierID = s.SupplierID));
    IF @n > 0 THROW 51012, 'StockIn with missing product/supplier', 1;
    SELECT @n = COUNT(*) FROM StockOut s WHERE NOT EXISTS (SELECT 1 FROM Products p WHERE p.ProductID = s.ProductID)
        OR (s.CustomerID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Customers x WHERE x.CustomerID = s.CustomerID));
    IF @n > 0 THROW 51013, 'StockOut with missing product/customer', 1;
    SELECT @n = COUNT(*) FROM Orders o WHERE (o.CustomerID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Customers x WHERE x.CustomerID = o.CustomerID))
        OR (o.EmployeeID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Employees e WHERE e.EmployeeID = o.EmployeeID));
    IF @n > 0 THROW 51014, 'Order with missing customer/employee', 1;
    SELECT @n = COUNT(*) FROM OrderDetails d WHERE NOT EXISTS (SELECT 1 FROM Orders o WHERE o.OrderID = d.OrderID)
        OR NOT EXISTS (SELECT 1 FROM Products p WHERE p.ProductID = d.ProductID);
    IF @n > 0 THROW 51015, 'OrderDetail with missing order/product', 1;

    -- 9c. calculations
    IF EXISTS (SELECT 1 FROM StockIn      WHERE Quantity * UnitCost  <> TotalCost) THROW 51021, 'StockIn total mismatch', 1;
    IF EXISTS (SELECT 1 FROM StockOut     WHERE Quantity * UnitPrice <> TotalPrice) THROW 51022, 'StockOut total mismatch', 1;
    IF EXISTS (SELECT 1 FROM OrderDetails WHERE Quantity * UnitPrice <> Total) THROW 51023, 'OrderDetail total mismatch', 1;
    IF EXISTS (SELECT 1 FROM Orders o WHERE o.TotalAmount <> (SELECT SUM(Total) FROM OrderDetails d WHERE d.OrderID = o.OrderID))
        THROW 51024, 'Order TotalAmount <> sum of its lines', 1;

    -- 9d. order statuses
    IF (SELECT COUNT(*) FROM Orders WHERE Status = N'Confirmed') < 2 OR (SELECT COUNT(*) FROM Orders WHERE Status = N'Pending') < 1
       OR (SELECT COUNT(*) FROM Orders WHERE Status = N'Cancelled') < 1 THROW 51031, 'Order status mix incorrect', 1;

    -- 9e. dates: nothing in the future (demo "today" = 2026-09-19)
    IF EXISTS (SELECT 1 FROM StockIn WHERE DateIn >= '2026-09-20') OR EXISTS (SELECT 1 FROM StockOut WHERE DateOut >= '2026-09-20')
       OR EXISTS (SELECT 1 FROM Orders WHERE OrderDate >= '2026-09-20') THROW 51032, 'Future-dated record', 1;

    -- 9f. stock consistency: QtyInStock equals the independent expected value per product
    --     (In - Out - Confirmed orders), hard-coded here so the check is independent of step 8
    DECLARE @expected TABLE (ProductID INT PRIMARY KEY, Expected INT);
    INSERT @expected VALUES (1, 55), (2, 50), (3, 38), (4, 30), (5, 14), (6, 12), (7, 6), (8, 19);
    IF EXISTS (SELECT 1 FROM Products p JOIN @expected e ON e.ProductID = p.ProductID WHERE p.QtyInStock <> e.Expected)
        THROW 51041, 'Product stock differs from expected final stock', 1;
    IF EXISTS (SELECT 1 FROM Products WHERE QtyInStock < 0) THROW 51042, 'Negative stock', 1;

    -- 9g. stock never went negative at any point in time (running balance over all movements)
    ;WITH mv AS (
        SELECT ProductID, DateIn AS d, Quantity AS q, 1 AS ord FROM StockIn
        UNION ALL SELECT ProductID, DateOut, -Quantity, 2 FROM StockOut
        UNION ALL SELECT od.ProductID, o.OrderDate, -od.Quantity, 2
                  FROM OrderDetails od JOIN Orders o ON o.OrderID = od.OrderID WHERE o.Status = N'Confirmed'),
    run AS (SELECT SUM(q) OVER (PARTITION BY ProductID ORDER BY d, ord ROWS UNBOUNDED PRECEDING) AS bal FROM mv)
    SELECT @n = COUNT(*) FROM run WHERE bal < 0;
    IF @n > 0 THROW 51043, 'Stock went negative at some point in the history', 1;

    -- 9h. dashboard demo: at least one clearly low, one close to reorder, one healthy product
    IF NOT EXISTS (SELECT 1 FROM Products WHERE QtyInStock <  ReorderLevel)                                THROW 51051, 'No low-stock product', 1;
    IF NOT EXISTS (SELECT 1 FROM Products WHERE QtyInStock >  ReorderLevel AND QtyInStock <= ReorderLevel + 3) THROW 51052, 'No product close to reorder level', 1;
    IF NOT EXISTS (SELECT 1 FROM Products WHERE QtyInStock >= ReorderLevel * 2)                            THROW 51053, 'No healthy-stock product', 1;

    COMMIT TRANSACTION;

    -- ----------------------------------------------------------------------------------
    -- 10. Only after a successful commit: continue identity numbering naturally
    -- ----------------------------------------------------------------------------------
    DBCC CHECKIDENT ('Categories',   RESEED, 2)  WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('Products',     RESEED, 8)  WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('Suppliers',    RESEED, 3)  WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('Customers',    RESEED, 4)  WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('Employees',    RESEED, 3)  WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('StockIn',      RESEED, 12) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('StockOut',     RESEED, 7)  WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('Orders',       RESEED, 6)  WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('OrderDetails', RESEED, 12) WITH NO_INFOMSGS;

    PRINT 'DemoSeed: COMMITTED - all verification checks passed.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'DemoSeed: ROLLED BACK - ' + ERROR_MESSAGE();
    THROW;
END CATCH;
GO
