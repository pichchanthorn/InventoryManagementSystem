/*
    InventoryManagementDB
    Phase 1C - Foundation schema for the Inventory Management System.
    Safe to re-run: creates the database only if it does not already exist,
    and creates each table only if it does not already exist.
*/

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'InventoryManagementDB')
BEGIN
    CREATE DATABASE InventoryManagementDB;
END
GO

USE InventoryManagementDB;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- ============================================================
-- Users
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Users')
BEGIN
    CREATE TABLE Users
    (
        UserID        INT IDENTITY(1,1) PRIMARY KEY,
        Username      NVARCHAR(50)  NOT NULL,
        PasswordHash  NVARCHAR(255) NOT NULL,
        FullName      NVARCHAR(100) NOT NULL,
        Role          NVARCHAR(20)  NOT NULL,
        IsActive      BIT           NOT NULL,
        CreatedAt     DATETIME2     NOT NULL,
        CONSTRAINT UQ_Users_Username UNIQUE (Username)
    );
END
GO

-- ============================================================
-- Categories
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Categories')
BEGIN
    CREATE TABLE Categories
    (
        CategoryID    INT IDENTITY(1,1) PRIMARY KEY,
        CategoryName  NVARCHAR(100) NOT NULL,
        Description   NVARCHAR(255) NULL,
        CONSTRAINT UQ_Categories_CategoryName UNIQUE (CategoryName)
    );
END
GO

-- ============================================================
-- Customers
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Customers')
BEGIN
    CREATE TABLE Customers
    (
        CustomerID    INT IDENTITY(1,1) PRIMARY KEY,
        CustomerName  NVARCHAR(150) NOT NULL,
        Phone         NVARCHAR(30)  NULL,
        Email         NVARCHAR(100) NULL,
        Address       NVARCHAR(255) NULL
    );
END
GO

-- ============================================================
-- Suppliers
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Suppliers')
BEGIN
    CREATE TABLE Suppliers
    (
        SupplierID    INT IDENTITY(1,1) PRIMARY KEY,
        SupplierName  NVARCHAR(150) NOT NULL,
        ContactPerson NVARCHAR(150) NULL,
        Phone         NVARCHAR(30)  NULL,
        Email         NVARCHAR(100) NULL,
        Address       NVARCHAR(255) NULL
    );
END
GO

-- ============================================================
-- Employees
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Employees')
BEGIN
    CREATE TABLE Employees
    (
        EmployeeID    INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeName  NVARCHAR(150) NOT NULL,
        Gender        NVARCHAR(20)  NULL,
        Phone         NVARCHAR(30)  NULL,
        Email         NVARCHAR(100) NULL,
        Address       NVARCHAR(255) NULL
    );
END
GO

-- ============================================================
-- Products
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Products')
BEGIN
    CREATE TABLE Products
    (
        ProductID     INT IDENTITY(1,1) PRIMARY KEY,
        ProductName   NVARCHAR(150)  NOT NULL,
        CategoryID    INT            NOT NULL,
        UnitPrice     DECIMAL(12,2)  NOT NULL,
        QtyInStock    INT            NOT NULL,
        Description   NVARCHAR(255)  NULL,
        Barcode       NVARCHAR(100)  NULL,
        ReorderLevel  INT            NOT NULL,
        CreatedAt     DATETIME2      NOT NULL,
        UpdatedAt     DATETIME2      NOT NULL,
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID)
            REFERENCES Categories (CategoryID),
        CONSTRAINT CK_Products_UnitPrice CHECK (UnitPrice >= 0),
        CONSTRAINT CK_Products_QtyInStock CHECK (QtyInStock >= 0),
        CONSTRAINT CK_Products_ReorderLevel CHECK (ReorderLevel >= 0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Products_CategoryID' AND object_id = OBJECT_ID(N'Products'))
    CREATE INDEX IX_Products_CategoryID ON Products (CategoryID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Products_Barcode' AND object_id = OBJECT_ID(N'Products'))
    CREATE UNIQUE INDEX UX_Products_Barcode ON Products (Barcode) WHERE Barcode IS NOT NULL;
GO

-- ============================================================
-- StockIn
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'StockIn')
BEGIN
    CREATE TABLE StockIn
    (
        StockInID   INT IDENTITY(1,1) PRIMARY KEY,
        ProductID   INT           NOT NULL,
        SupplierID  INT           NULL,
        Quantity    INT           NOT NULL,
        UnitCost    DECIMAL(12,2) NOT NULL,
        TotalCost   DECIMAL(14,2) NOT NULL,
        DateIn      DATETIME2     NOT NULL,
        Notes       NVARCHAR(255) NULL,
        CONSTRAINT FK_StockIn_Products FOREIGN KEY (ProductID)
            REFERENCES Products (ProductID),
        CONSTRAINT FK_StockIn_Suppliers FOREIGN KEY (SupplierID)
            REFERENCES Suppliers (SupplierID),
        CONSTRAINT CK_StockIn_Quantity CHECK (Quantity > 0),
        CONSTRAINT CK_StockIn_UnitCost CHECK (UnitCost >= 0),
        CONSTRAINT CK_StockIn_TotalCost CHECK (TotalCost >= 0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockIn_ProductID' AND object_id = OBJECT_ID(N'StockIn'))
    CREATE INDEX IX_StockIn_ProductID ON StockIn (ProductID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockIn_SupplierID' AND object_id = OBJECT_ID(N'StockIn'))
    CREATE INDEX IX_StockIn_SupplierID ON StockIn (SupplierID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockIn_DateIn' AND object_id = OBJECT_ID(N'StockIn'))
    CREATE INDEX IX_StockIn_DateIn ON StockIn (DateIn);
GO

-- ============================================================
-- StockOut
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'StockOut')
BEGIN
    CREATE TABLE StockOut
    (
        StockOutID  INT IDENTITY(1,1) PRIMARY KEY,
        ProductID   INT           NOT NULL,
        CustomerID  INT           NULL,
        Quantity    INT           NOT NULL,
        UnitPrice   DECIMAL(12,2) NOT NULL,
        TotalPrice  DECIMAL(14,2) NOT NULL,
        DateOut     DATETIME2     NOT NULL,
        Notes       NVARCHAR(255) NULL,
        CONSTRAINT FK_StockOut_Products FOREIGN KEY (ProductID)
            REFERENCES Products (ProductID),
        CONSTRAINT FK_StockOut_Customers FOREIGN KEY (CustomerID)
            REFERENCES Customers (CustomerID),
        CONSTRAINT CK_StockOut_Quantity CHECK (Quantity > 0),
        CONSTRAINT CK_StockOut_UnitPrice CHECK (UnitPrice >= 0),
        CONSTRAINT CK_StockOut_TotalPrice CHECK (TotalPrice >= 0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockOut_ProductID' AND object_id = OBJECT_ID(N'StockOut'))
    CREATE INDEX IX_StockOut_ProductID ON StockOut (ProductID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockOut_CustomerID' AND object_id = OBJECT_ID(N'StockOut'))
    CREATE INDEX IX_StockOut_CustomerID ON StockOut (CustomerID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockOut_DateOut' AND object_id = OBJECT_ID(N'StockOut'))
    CREATE INDEX IX_StockOut_DateOut ON StockOut (DateOut);
GO

-- ============================================================
-- Orders
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Orders')
BEGIN
    CREATE TABLE Orders
    (
        OrderID      INT IDENTITY(1,1) PRIMARY KEY,
        CustomerID   INT           NULL,
        EmployeeID   INT           NULL,
        OrderDate    DATETIME2     NOT NULL,
        TotalAmount  DECIMAL(14,2) NOT NULL,
        Status       NVARCHAR(20)  NOT NULL,
        CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerID)
            REFERENCES Customers (CustomerID),
        CONSTRAINT FK_Orders_Employees FOREIGN KEY (EmployeeID)
            REFERENCES Employees (EmployeeID),
        CONSTRAINT CK_Orders_Status CHECK (Status IN (N'Pending', N'Confirmed', N'Cancelled'))
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Orders_CustomerID' AND object_id = OBJECT_ID(N'Orders'))
    CREATE INDEX IX_Orders_CustomerID ON Orders (CustomerID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Orders_EmployeeID' AND object_id = OBJECT_ID(N'Orders'))
    CREATE INDEX IX_Orders_EmployeeID ON Orders (EmployeeID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Orders_OrderDate' AND object_id = OBJECT_ID(N'Orders'))
    CREATE INDEX IX_Orders_OrderDate ON Orders (OrderDate);
GO

-- ============================================================
-- OrderDetails
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'OrderDetails')
BEGIN
    CREATE TABLE OrderDetails
    (
        OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
        OrderID       INT           NOT NULL,
        ProductID     INT           NOT NULL,
        Quantity      INT           NOT NULL,
        UnitPrice     DECIMAL(12,2) NOT NULL,
        Total         DECIMAL(14,2) NOT NULL,
        CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID)
            REFERENCES Orders (OrderID),
        CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID)
            REFERENCES Products (ProductID),
        CONSTRAINT CK_OrderDetails_Quantity CHECK (Quantity > 0),
        CONSTRAINT CK_OrderDetails_UnitPrice CHECK (UnitPrice >= 0),
        CONSTRAINT CK_OrderDetails_Total CHECK (Total >= 0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrderDetails_OrderID' AND object_id = OBJECT_ID(N'OrderDetails'))
    CREATE INDEX IX_OrderDetails_OrderID ON OrderDetails (OrderID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrderDetails_ProductID' AND object_id = OBJECT_ID(N'OrderDetails'))
    CREATE INDEX IX_OrderDetails_ProductID ON OrderDetails (ProductID);
GO
