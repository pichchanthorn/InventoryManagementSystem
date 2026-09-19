# InventoryManagementSystem — Development Notes

## Project Purpose

A new Inventory Management System (products, categories, customers, suppliers,
employees, stock movements, and orders) built as a university Client-Server
Application Development project. This is an independent, ground-up project —
it does not share code, database, or architecture with any prior project.

## Technology Stack

- C# (.NET Framework 4.8)
- Windows Forms (WinForms)
- SQL Server (Windows Authentication)
- `System.Data.SqlClient` for data access
- PBKDF2 (`System.Security.Cryptography.Rfc2898DeriveBytes`, HMAC-SHA256) for
  password hashing — no external packages

## Architecture

```
UI  →  BLL  →  DAL  →  SQL Server
         ↑       ↑
       Entity (shared model layer)
```

- **UI** — Windows Forms. Contains no SQL and does not access the database
  directly. `frmMain`'s navigation shell is presentation-only. `frmCategories`
  talks only to `CategoryBLL`; `frmProducts` talks only to `ProductBLL` (and
  `CategoryBLL`, to populate the Category dropdown); `frmCustomers` talks only
  to `CustomerBLL`; `frmSuppliers` talks only to `SupplierBLL`; `frmEmployees`
  talks only to `EmployeeBLL`; `frmStockIn` talks only to `StockInBLL` (and
  `ProductBLL`/`SupplierBLL`, to populate the Product/Supplier dropdowns and
  filters).
- **BLL** — Business logic layer. Owns business rules, validation, and
  authentication (`UserBLL`, `PasswordHasher`, `LoginResult`, `CategoryBLL`,
  `CategoryResult`, `ProductBLL`, `ProductResult`, `CustomerBLL`,
  `CustomerResult`, `SupplierBLL`, `SupplierResult`, `EmployeeBLL`,
  `EmployeeResult`, `StockInBLL`, `StockInResult`).
- **DAL** — Data access layer. Owns all database access (`DbConnection`,
  `UserDAL`, `CategoryDAL`, `ProductDAL`, `CustomerDAL`, `SupplierDAL`,
  `EmployeeDAL`, `StockInDAL`). All SQL is parameterized.
- **Entity** — Plain model classes shared across layers. No business or database
  logic. `ProductEntity` carries one display-only, non-persisted property
  (`CategoryName`) populated by `ProductDAL`'s join — it is not a database
  column and is never written by Insert/Update. `StockInEntity` similarly
  carries two display-only, non-persisted properties (`ProductName`,
  `SupplierName`) populated by `StockInDAL`'s join.

## Folder Structure

```
InventoryManagementSystem/
  Entity/            10 plain model classes
  DAL/
    DbConnection.cs   Connection factory reading from App.config
    UserDAL.cs        Users table access (GetByUsername) — parameterized SQL
    CategoryDAL.cs    Categories table access (CRUD + ExistsByName) — parameterized SQL
    ProductDAL.cs     Products table access (CRUD + ExistsByBarcode), joined
                       with Categories for display — parameterized SQL
    CustomerDAL.cs    Customers table access (CRUD) — parameterized SQL
    SupplierDAL.cs    Suppliers table access (CRUD) — parameterized SQL
    EmployeeDAL.cs    Employees table access (CRUD) — parameterized SQL
  BLL/
    PasswordHasher.cs Password hashing/verification (PBKDF2-HMACSHA256)
    LoginResult.cs    Login outcome type (status/message/user)
    UserBLL.cs        Login validation + authentication rules
    CategoryResult.cs Category operation outcome type (success/message)
    CategoryBLL.cs    Category validation, duplicate checks, FK-safe delete handling
    ProductResult.cs  Product operation outcome type (success/message)
    ProductBLL.cs     Product validation, barcode/category checks, stock-quantity
                       protection, FK-safe delete handling
    CustomerResult.cs Customer operation outcome type (success/message)
    CustomerBLL.cs    Customer validation (required name, optional
                       phone/email/address with length limits and NULL
                       normalization)
    SupplierResult.cs Supplier operation outcome type (success/message)
    SupplierBLL.cs    Supplier validation (required name, optional contact
                       person/phone/email/address with length limits and
                       NULL normalization)
    EmployeeResult.cs Employee operation outcome type (success/message)
    EmployeeBLL.cs    Employee validation (required name, optional
                       gender/phone/email/address with length limits and
                       NULL normalization)
    StockInResult.cs  Stock In operation outcome type (success/message)
    StockInBLL.cs     Stock In validation (required product, optional
                       supplier, quantity > 0, unit cost >= 0, notes length),
                       existence checks for product/supplier, Total Cost
                       calculation (Quantity × Unit Cost, rounded to 2
                       decimals)
  UI/
    frmLogin.cs / .Designer.cs      Login form (startup form)
    frmMain.cs  / .Designer.cs      Post-login shell: header, left navigation
                                     (Categories, Products, Customers,
                                     Suppliers, Employees, and Stock In
                                     functional; 4 other items still
                                     placeholders), content area, logout
    frmCategories.cs / .Designer.cs Category list/add/edit/delete window
    frmProducts.cs   / .Designer.cs Product list/add/edit/delete window
    frmCustomers.cs  / .Designer.cs Customer list/add/edit/delete window
    frmSuppliers.cs  / .Designer.cs Supplier list/add/edit/delete window
    frmEmployees.cs  / .Designer.cs Employee list/add/edit/delete window
    frmStockIn.cs    / .Designer.cs Stock In entry form (Product required,
                                     Supplier optional, Quantity, Unit Cost,
                                     read-only Total Cost preview, Notes) plus
                                     a Stock In history grid with search,
                                     refresh, and Product/Supplier/date-range
                                     filters
  Database/
    InventoryManagementDB.sql   Full schema script (idempotent)
  Program.cs          Application entry point (frmLogin → frmMain flow)
  App.config          Connection string configuration
```

## Database

- **Name:** `InventoryManagementDB`
- **Auth:** Windows Authentication (`Trusted_Connection=True`)
- **Connection string name:** `InventoryDb` (read via `ConfigurationManager.ConnectionStrings`)
- Schema script: `Database/InventoryManagementDB.sql` — creates the database
  and all 10 tables only if they do not already exist (safe to re-run).
- No schema changes have been made since Phase 1C. Phase 4C made no schema
  changes — it uses the existing `Employees` table (`EmployeeID`,
  `EmployeeName`, `Gender`, `Phone`, `Email`, `Address`) exactly as-is, and
  does not touch any other table. `Gender` is treated as plain optional text
  within the existing `NVARCHAR(20)` column — no fixed enum/lookup was
  introduced, since none exists in the current schema or application.
  Phase 5A also made no schema changes — the existing `StockIn` table
  (`StockInID`, `ProductID`, `SupplierID`, `Quantity`, `UnitCost`,
  `TotalCost`, `DateIn`, `Notes`) and `Products.QtyInStock` were already
  sufficient; both were used exactly as defined in `Database/InventoryManagementDB.sql`.
- The pre-existing `InventoryAppDB` database was not touched, modified, or
  written to (confirmed unchanged `create_date` before/after every phase).

### Tables

Users, Categories, Products, Customers, Suppliers, Employees, StockIn,
StockOut, Orders, OrderDetails — with primary keys, foreign keys (all
`ON DELETE NO ACTION`), check constraints, a unique filtered index on
`Products.Barcode`, and indexes on FK/date columns (unchanged since Phase 1C).

### Development/Test Accounts (Users table)

Two development-only accounts exist for verifying the login flow:

| Username      | Role  | IsActive | Purpose                          |
|---------------|-------|----------|-----------------------------------|
| `devadmin`    | Admin | 1        | Valid-login / success-path test  |
| `devinactive` | Staff | 0        | Inactive-account rejection test  |

Only PBKDF2 password hashes are stored in `Users.PasswordHash` — no plaintext
password exists in the database, in source code, or in any file in this
repository. These accounts are for local development verification only and
should be removed or rotated before any non-development use. Phase 4C made no
changes to these records. No Employee test/seed data was left in the database
after Phase 4C verification — all test employees created during testing were
deleted as part of the same test runs.

## Current Phase

**Phase 5A — Stock In — COMPLETE**

Prior completed phases:
- **Phase 4C — Employee CRUD — COMPLETE**
- **Phase 4B — Supplier CRUD — COMPLETE**
- **Phase 4A — Customer CRUD — COMPLETE**
- **Phase 3B — Product CRUD — COMPLETE**
- **Phase 3A — Category CRUD — COMPLETE**
- **Phase 2B — Main Form & Navigation Foundation — COMPLETE**
- **Phase 2A — Login & Authentication Foundation — COMPLETE**

## What Has Been Implemented

Phase 2A (Login & Authentication):

- `DAL/UserDAL.cs` — `GetByUsername(string)`, fully parameterized.
- `BLL/PasswordHasher.cs` — PBKDF2-HMACSHA256, random 16-byte salt per
  password, 100,000 iterations.
- `BLL/UserBLL.cs` — `Login(username, password)` validates input, verifies
  the password hash, checks `IsActive`, and returns a `LoginResult` without
  revealing which field was wrong on failure.
- `UI/frmLogin` — Username/Password (masked) fields, Login/Clear/Exit
  buttons, startup form, no SQL/DB access in the form itself.

Phase 2B (Main Form & Navigation Foundation):

- `UI/frmMain` shell: header (title, `FullName`, `Role`, Logout), left
  navigation with 10 items, Dashboard static welcome panel, placeholder
  content for unimplemented modules, Logout → back to `frmLogin`.
- `Program.cs` flow: `frmLogin` (`ShowDialog`) → on success, `frmMain(UserEntity)`
  via `Application.Run`; on Logout, loops back to `frmLogin`; closing/cancelling
  Login exits the application cleanly.

Phase 3A (Category CRUD):

- `DAL/CategoryDAL.cs` / `BLL/CategoryBLL.cs` — full CRUD with required/unique
  name (max 100 chars), optional description (max 255 chars, empty → `NULL`),
  duplicate check excluding the row being edited on Update, FK-safe delete
  (`error 547` → friendly message).
- `UI/frmCategories` — list/add/edit/delete window; `frmMain`'s "Categories"
  nav item opens/reuses it.

Phase 3B (Product CRUD):

- `DAL/ProductDAL.cs` / `BLL/ProductBLL.cs` — full CRUD joined with
  `Categories` for display, required name (max 150 chars), category
  existence check, non-negative price/initial-stock/reorder level, optional
  unique barcode (max 100 chars) and description (max 255 chars),
  **`QtyInStock` intentionally excluded from the `Update` SQL so normal
  editing can never change stock**, FK-safe delete (`error 547` → friendly
  message referencing inventory/sales history).
- `UI/frmProducts` — list/add/edit/delete window with a Category dropdown;
  Initial Stock field becomes read-only when editing an existing product;
  `frmMain`'s "Products" nav item opens/reuses it.

Phase 4A (Customer CRUD):

- `DAL/CustomerDAL.cs` / `BLL/CustomerBLL.cs` — full CRUD; required name
  (max 150 chars); optional phone (max 30), email (max 100), address
  (max 255), all normalized to `NULL` when blank; no invented uniqueness
  rules on phone/email.
- `UI/frmCustomers` — list/add/edit/delete window; `frmMain`'s "Customers"
  nav item opens/reuses it. Introduced the `_suppressSelectionChanged` guard
  flag pattern (originally added reactively in Phase 3B after a real bug was
  found there) from the start.

Phase 4B (Supplier CRUD):

- `DAL/SupplierDAL.cs` / `BLL/SupplierBLL.cs` — full CRUD; required name
  (max 150 chars); optional contact person (max 150), phone (max 30), email
  (max 100), address (max 255), all normalized to `NULL` when blank; no
  invented uniqueness rules.
- `UI/frmSuppliers` — list/add/edit/delete window; `frmMain`'s "Suppliers"
  nav item opens/reuses it.

Phase 4C (Employee CRUD):

- `DAL/EmployeeDAL.cs`:
  - `GetAll()` — ordered by `EmployeeName` ascending.
  - `GetById(int)`, `Insert`, `Update`, `Delete(int)`.
  - All methods use parameterized `SqlCommand`/`SqlParameter` inside `using`
    blocks; no string concatenation of user input into SQL.
- `BLL/EmployeeBLL.cs` — business rules:
  - `EmployeeName` required, trimmed, capped at 150 characters
    (`NVARCHAR(150)`).
  - `Gender` optional, trimmed, capped at 20 characters (`NVARCHAR(20)`) —
    treated as free text, not a fixed enum (none exists in the schema or
    elsewhere in the application).
  - `Phone` optional, trimmed, capped at 30 characters (`NVARCHAR(30)`).
  - `Email` optional, trimmed, capped at 100 characters (`NVARCHAR(100)`).
  - `Address` optional, trimmed, capped at 255 characters (`NVARCHAR(255)`).
  - Empty/whitespace-only optional fields are normalized to `NULL` before
    being persisted.
  - No uniqueness rule is enforced on `Phone`/`Email` — the existing database
    schema does not require it, and none was invented.
  - `EmployeeID` must be a positive, selected value before Update/Delete.
  - Database failures are caught and translated into a generic friendly
    message — no raw SQL exception text or connection string is ever shown
    to the user, and no catch block is empty.
- `UI/frmEmployees` — Employee Name, Gender, Phone, Email, Address fields,
  plus Add/Update/Delete/Clear and a `DataGridView` list (columns: ID,
  Employee Name, Gender, Phone, Email, Address). Follows the same visual
  conventions as `frmCustomers`/`frmSuppliers`.
  - On load, all employees are fetched into the grid.
  - Selecting a grid row populates every field and enables Update/Delete.
  - Add/Update/Delete call `EmployeeBLL` and show the resulting
    success/failure message; Delete asks for Yes/No confirmation first.
    Clear resets every field, the internal selected ID, the grid selection,
    and button state, and does not touch the database.
  - `frmEmployees.cs` contains no `SqlConnection`, `SqlCommand`, or SQL of
    any kind.
  - Uses the established `_suppressSelectionChanged` guard flag around
    `ClearForm`'s body, so a stray `SelectionChanged` re-fire when the grid
    regains focus cannot undo `ClearForm`. No new defect was found or needed
    fixing in this phase.
- `UI/frmMain` — the "Employees" navigation item now opens `frmEmployees`
  (reusing the existing instance and bringing it to front on repeated clicks,
  identical pattern to Categories/Products/Customers/Suppliers). The other 5
  navigation items (Stock In, Stock Out, Orders, Reports, plus Dashboard) are
  unchanged Phase 2B placeholders/welcome panel. Categories', Products',
  Customers', and Suppliers' navigation behavior was not modified.

Phase 5A (Stock In):

- Pre-existing build-breaking defect fixed first (required before any build
  or test could run, unrelated to the Stock In feature itself): `frmLogin.cs`
  and `frmMain.cs` each contained a stray, incomplete duplicate
  `InitializeComponent()` method alongside the real one already generated in
  their `.Designer.cs` partial file, causing `CS0111` and blocking
  compilation entirely. The stray duplicate stub was deleted from both
  `frmLogin.cs` and `frmMain.cs`; the real, complete `InitializeComponent()`
  in each `.Designer.cs` file was not touched. This is a pure duplicate-code
  deletion with no behavior change — confirmed by full regression testing
  below.
- `DAL/StockInDAL.cs`:
  - `GetAll(productId, supplierId, dateFrom, dateToExclusive, searchText)` —
    joined with `Products` (inner) and `Suppliers` (left, since Supplier is
    optional) for display; all filters are optional and parameterized;
    ordered by `DateIn` descending.
  - `InsertWithStockUpdate(StockInEntity)` — inserts the `StockIn` row and
    increases `Products.QtyInStock` by the same quantity inside a single
    `SqlTransaction`; either both changes commit or neither does (any
    exception triggers `transaction.Rollback()` before rethrowing).
    `DateIn` is set by `SYSDATETIME()` in the SQL itself (database/server
    controlled, never supplied by the caller). All parameters are
    strongly-typed `SqlParameter`s; no string concatenation of user input
    into SQL.
- `BLL/StockInBLL.cs`:
  - `GetAll(...)` — thin pass-through to `StockInDAL.GetAll`, converting an
    inclusive `dateTo` into the exclusive upper bound the DAL expects, and
    wrapping database failures in a friendly `ApplicationException` (same
    pattern as `ProductBLL.GetAll`).
  - `Add(StockInEntity)` — business rules: Product selection required
    (`ProductID > 0`); Supplier optional (`SupplierID <= 0` or `null` is
    normalized to `NULL`); Quantity required and must be `> 0`; Unit Cost
    must be `>= 0`; Notes optional, trimmed, capped at 255 characters
    (`NVARCHAR(255)`); Product existence is verified via `ProductDAL.GetById`
    before insert; Supplier existence (if selected) is verified via
    `SupplierDAL.GetById` before insert; **Total Cost is calculated here**
    as `Math.Round(Quantity * UnitCost, 2, MidpointRounding.AwayFromZero)` —
    never accepted from the caller/UI. A `547` (foreign-key violation) from
    the database is translated to a friendly message for the rare race where
    the product/supplier is deleted between validation and insert; all other
    database failures are caught and translated into a generic friendly
    message — no raw SQL exception text or connection string is ever shown
    to the user.
  - Since the schema's `CK_Products_QtyInStock` check constraint already
    guarantees `QtyInStock >= 0`, and Stock In only ever adds a positive
    quantity, no additional application-level negative-stock guard was
    needed — increasing a non-negative value by a positive one cannot go
    negative.
- `UI/frmStockIn.cs` / `.Designer.cs`:
  - Entry section: Product dropdown (required, populated from
    `ProductBLL.GetAll()`), Supplier dropdown (optional, with a
    `"(No Supplier)"` sentinel item populated from `SupplierBLL.GetAll()`),
    Quantity, Unit Cost, a read-only Total Cost **preview** field (recomputed
    client-side on every keystroke purely for user feedback — the value
    actually persisted is always recalculated authoritatively by
    `StockInBLL.Add`, so a stale/mismatched preview can never reach the
    database), Notes (optional, multiline, `MaxLength = 255`), "Record Stock
    In" and "Clear" buttons. There is no Update/Delete for Stock In records —
    Stock In is treated as an append-only movement ledger, consistent with
    the schema (no schema change) and with the fact that this phase's
    requirements only asked for recording stock in, not editing/reversing it.
  - History section: a `DataGridView` (`StockInID`, Product, Supplier,
    Quantity, Unit Cost, Total Cost, Date In, Notes) with a filter bar above
    it — Product filter dropdown (`"(All Products)"` default), Supplier
    filter dropdown (`"(All Suppliers)"` default), a "Filter by date"
    checkbox that enables/disables a From/To `DateTimePicker` range, a free-text
    Search box (matches Product Name or Notes via a parameterized `LIKE`),
    a **Search** button (applies the current filter/search state), and a
    **Refresh** button (clears all filters/search and reloads the full
    unfiltered history).
  - `frmMain` — the "Stock In" navigation item now opens `frmStockIn`
    (reusing the existing instance and bringing it to front on repeated
    clicks, identical pattern to the other modules). No other navigation
    item's behavior was changed. The "Stock Out", "Orders", and "Reports"
    navigation items are unchanged Phase 2B placeholders.
  - `frmStockIn.cs` contains no `SqlConnection`, `SqlCommand`, or SQL of any
    kind.
  - Product Management (`frmProducts`) was not modified in this phase;
    `ProductDAL.Update`'s exclusion of `QtyInStock` (established in Phase
    3B) already prevents manual editing of stock quantity from that form, so
    no additional change was needed there to satisfy "do not allow manual
    `QtyInStock` edits from Product Management."

## What Has NOT Been Implemented

- Stock Out workflow
- Orders / order details workflow
- Reports
- A real Dashboard (statistics, charts, computed business data)
- Per-module authorization / permission management
- Editing or deleting a recorded Stock In transaction (by design — see
  Phase 5A notes above)

## Verification Performed

### Phase 2A / 2B / 3A / 3B / 4A / 4B (previously verified, unaffected by Phase 4C)

See prior verification: direct `UserBLL.Login`/`CategoryBLL`/`ProductBLL`/
`CustomerBLL`/`SupplierBLL` branch testing, parameterized SQL in
`UserDAL`/`CategoryDAL`/`ProductDAL`/`CustomerDAL`/`SupplierDAL`, no plaintext
password storage, full login → main → logout → re-login UI flow,
Category/Product CRUD with duplicate and FK-safe-delete handling,
Customer/Supplier CRUD with NULL-normalization — all previously confirmed and
unchanged by this phase (re-confirmed live: Categories, Products, Customers,
and Suppliers modules all still open correctly as part of Phase 4C's own test
run).

### Phase 4C

1. `MSBuild /t:Rebuild` — **Build succeeded, 0 warnings, 0 errors.**
2. UI Automation end-to-end run against the real built `.exe` (logged in as
   `devadmin`) confirmed, in order:
   - Clicking the Employees navigation item opens `frmEmployees` with the
     (empty) employee list loaded.
   - Empty Employee Name → "Employee name is required."
   - Adding a valid employee ("ZZTestEmployee4C", gender "Female", phone
     `555-1234`, email `employeeA@example.com`) shows "Employee added
     successfully."
   - Selecting the new row populates Employee Name, Gender, Phone, and
     Email correctly, and enables Update/Delete.
   - Editing the name/gender/phone/email and clicking Update shows
     "Employee updated successfully."
   - Re-selecting the row confirms all four edited values are correct.
   - Clear empties the fields and disables Update/Delete.
   - Re-selecting the edited row and deleting it asks for Yes/No
     confirmation and, on confirming, shows "Employee deleted
     successfully."
   - Adding a second employee ("ZZTestEmployeeNulls4C") with Gender/Phone/
     Email/Address left blank succeeds ("Employee added successfully.") and
     the row is selectable.
   - The Categories, Products, Customers, and Suppliers modules all still
     open correctly afterward.
   - The Stock In and Reports navigation items still show the unchanged
     Phase 2B placeholder text, and the Dashboard welcome panel still works
     unchanged, confirming other modules were not affected.
   - Logout returns to `frmLogin`, and logging in again succeeds.
   - All 28 automated checks passed on the first run (no regressions found;
     the `_suppressSelectionChanged` pattern continued to work correctly).
3. Direct database query confirmed the blank-optional-fields employee
   ("ZZTestEmployeeNulls4C") was stored with `Gender`, `Phone`, `Email`, and
   `Address` all `NULL` — checked before that test employee was cleaned up.
4. Direct `EmployeeBLL` test (standalone console harness referencing the
   built assembly, run once against the live database, then deleted — not
   part of the shipped project) confirmed, including cases the UI could not
   reliably drive (exact length boundaries — the UI's `MaxLength` on each
   textbox already prevents typing past the limit, so over-limit input
   isn't reachable through the real UI at all, matching the pattern used in
   earlier phases):
   - Name/gender/phone/email/address with leading/trailing whitespace are
     trimmed before being persisted (verified by reading back via
     `GetAll`).
   - Whitespace-only Gender/Phone/Email/Address are stored as `NULL`.
   - Exact maximum lengths (name 150, gender 20, phone 30, email 100,
     address 255) are all accepted.
   - One character over each maximum (151/21/31/101/256) is rejected with
     the corresponding "cannot exceed N characters" message.
   - `Update`/`Delete` with `EmployeeID <= 0` are rejected with "Please
     select an employee" messages.
   - All harness-created test employees were deleted at the end of the run;
     `Employees` was confirmed empty afterward.
5. Verified by search: no `SqlConnection`, `SqlCommand`, or SQL keywords
   exist anywhere in `frmEmployees.cs` or `frmMain.cs`.
6. Verified by code inspection: every `EmployeeDAL` method uses
   parameterized `SqlCommand`/`SqlParameter`; no user input is concatenated
   into SQL text.
7. Verified by direct query: after all testing, `Employees` has 0 rows,
   `Categories`/`Products`/`Customers`/`Suppliers` are unchanged (all 0
   rows, as before this phase), and `Users` still has exactly the two
   development accounts, unchanged.
8. Verified `InventoryAppDB`'s `create_date` is unchanged — it was not
   written to during this phase.
9. Confirmed no schema changes were made (no new tables/columns/indexes).

### Phase 5A

1. Fixed a pre-existing build-breaking defect found during initial
   inspection (unrelated to Stock In): before any Phase 5A code was written,
   `MSBuild /t:Rebuild` failed with `CS0111` ("already defines a member
   called 'InitializeComponent'") on both `frmLogin` and `frmMain`, because
   each had a stray duplicate `InitializeComponent()` stub in its `.cs` file
   in addition to the real one in its `.Designer.cs` file. The project had
   never successfully compiled with `MSBuild` in this state. The duplicate
   stubs were deleted (no other change to either file); `MSBuild /t:Rebuild`
   then succeeded with 0 errors, 0 warnings, confirming this was a pure
   duplicate-declaration issue with no behavioral effect.
2. `MSBuild /t:Rebuild` after implementing Phase 5A — **Build succeeded, 0
   warnings, 0 errors.**
3. Direct `StockInBLL`/`StockInDAL`/`ProductDAL` test (standalone console
   harness referencing the built assembly, compiled with `csc.exe`, run once
   against the live `InventoryManagementDB`, then deleted — not part of the
   shipped project) using the pre-existing test fixtures already in the
   database (`ProductID=1` "NPK 15-15-15", baseline `QtyInStock=100`;
   `SupplierID=1` "Test Supplier"). All 23 checks passed on the first run:
   - Valid Stock In with a Supplier selected succeeds, persists a `StockIn`
     row with the correct `SupplierID`, and increases `Products.QtyInStock`
     by exactly the quantity recorded.
   - `Quantity = 0` is rejected ("Quantity must be greater than zero.").
   - Negative `Quantity` (`-5`) is rejected with the same message.
   - Negative `UnitCost` (`-1`) is rejected ("Unit cost cannot be
     negative.").
   - No product selected (`ProductID = 0`) is rejected ("Please select a
     product.").
   - A non-existent `ProductID` is rejected ("Selected product does not
     exist.").
   - A non-existent `SupplierID` is rejected ("Selected supplier does not
     exist.").
   - Stock In with `SupplierID = NULL` (Supplier omitted) succeeds and
     persists with `SupplierID` correctly `NULL`, confirming Supplier is
     genuinely optional.
   - Total Cost is calculated correctly, including a rounding case
     (`Quantity=7, UnitCost=3.335` → `TotalCost=23.35`, i.e.
     `Math.Round(23.345, 2, AwayFromZero)`), confirming the calculation is
     owned by the BLL and not just `Quantity * UnitCost` done elsewhere.
   - Notes over 255 characters is rejected ("Notes cannot exceed 255
     characters.").
   - `Products.QtyInStock` was independently re-read before/after each
     successful Stock In and increased by exactly the recorded quantity
     each time (10, then 7, then 7 — cumulative 100 → 124 before cleanup).
   - Transaction rollback (verified where practical, calling
     `StockInDAL.InsertWithStockUpdate` directly with a non-existent
     `ProductID` to force a foreign-key violation on the `INSERT INTO
     StockIn` statement): the call throws, no `StockIn` row was persisted,
     and `Products.QtyInStock` was confirmed unchanged afterward — the
     failed first statement did not leave a partial update. (The reverse
     ordering — insert succeeds but the subsequent `UPDATE Products` fails —
     was not independently reproduced; see Known Testing Limitations.)
   - Regression: `CategoryBLL`, `ProductBLL`, `CustomerBLL`, `SupplierBLL`,
     and `EmployeeBLL` `GetAll()` all still return data with no exceptions;
     `UserBLL.Login` is still callable and still rejects invalid credentials.
   - All harness-created `StockIn` rows (tagged `ZZ-HARNESS-*` in `Notes`)
     were deleted at the end of the run and `Products.QtyInStock` for
     `ProductID=1` was decremented back by the same total (24), restoring
     the exact pre-test baseline (`QtyInStock=100`, 0 `StockIn` rows) —
     confirmed by direct query.
4. Verified by search: no `SqlConnection`, `SqlCommand`, or SQL keywords
   exist anywhere in `frmStockIn.cs` or the modified parts of `frmMain.cs`.
5. Verified by code inspection: `StockInDAL.GetAll` and
   `StockInDAL.InsertWithStockUpdate` use only parameterized
   `SqlCommand`/`SqlParameter`; no user input (including the search text) is
   concatenated into SQL text — the `LIKE` wildcard is applied to the
   parameter's *value*, not the SQL string.
6. Verified by code inspection: `InsertWithStockUpdate` performs the
   `StockIn` insert and the `Products.QtyInStock` update on the same
   `SqlConnection`/`SqlTransaction`, with `transaction.Commit()` only after
   both `ExecuteNonQuery()` calls succeed and an explicit `transaction.Rollback()`
   in a `catch` block that rethrows — satisfying "one transaction, no partial
   updates."
7. Verified by direct query: after all testing and cleanup, `StockIn` has 0
   rows, `Categories`/`Products`/`Customers`/`Suppliers`/`Employees` each
   still have exactly 1 row (the pre-existing test fixtures, unchanged), and
   `Users` still has exactly the two development accounts, unchanged.
8. Verified `InventoryAppDB`'s `create_date` is unchanged — it was not
   written to during this phase.
9. Confirmed no schema changes were made (no new tables/columns/indexes);
   `Database/InventoryManagementDB.sql` was not modified.
10. Confirmed via `git status` that no commit, stage, or other Git history
    operation was performed — all changes remain unstaged/untracked on the
    pre-existing `feature/phase-5a-stock-in` branch, as instructed.

## Phase 7B.1 — Report Preview & Printing

The Reports module (`ucReports`) keeps its four reports (Inventory / Stock, Stock In, Stock Out, Orders), filters and
grid. A **Preview / Print** button (enabled once a report has loaded) opens a Report Viewer-style preview.

- **Technology:** native WinForms only (`PrintDocument`, `PrintPreviewControl`, `PrintDialog`). No ReportViewer, no RDLC,
  no NuGet packages, no database or BLL/DAL changes.
- **`UI/Reporting/ReportDocument.cs`** - presentation model (title, generated time, filter text, columns, formatted rows,
  summary lines, portrait/landscape). Separate from database entities.
- **`UI/Reporting/ReportDocumentBuilder.cs`** - maps the existing `ReportBLL` results to a `ReportDocument`; totals and
  counts come from the BLL result, so the printout always matches the grid. The on-screen summary bar uses the same lines.
- **`UI/Reporting/ReportPrinter.cs`** - `PrintDocument` that draws header, repeated table header, rows, summary (last page)
  and footer "Page X of N". Pagination is deterministic (fixed row height; depends only on page size and row count);
  long text is clipped with an ellipsis. Inventory prints portrait; Stock In / Stock Out / Orders print landscape.
- **`UI/frmReportPreview.cs`** - toolbar (Print, Previous, "Page X of N", Next, Zoom, Close) over a `PrintPreviewControl`.
  Preview and printing use the same `ReportPrinter` instance. Printing goes through the normal Windows print dialog;
  nothing is sent to a printer without the user confirming it there.
- **Not implemented (deferred):** PDF / Excel / CSV export.
- **Tests:** regression group N (`Tests.ReportPreview.cs`) - pagination rules, all four reports with 0 / 1 / many rows and
  filters, PrintPage generation via `PreviewPrintController`, preview form navigation and zoom, `ucReports` wiring.

## UX Cleanup Pass (after Phase 7B.1)

Usability/consistency pass over the existing ten modules. **No new module, table, feature or schema change; no
BLL/DAL/business-rule change.** UI code only (plus one data-only cleanup, below).

- **Sidebar** is grouped under headings: OVERVIEW (Dashboard) / MASTER DATA (Categories, Products, Customers,
  Suppliers, Employees) / INVENTORY (Stock In, Stock Out) / OPERATIONS (Orders) / REPORTING (Reports). Same theme; the
  headings are plain labels in `frmMain`.
- **Products:** opens in a clean Add state (no row selected, no category chosen, empty name/price/barcode/description).
  Selecting a row switches to Edit mode: Add is disabled, Update/Delete enabled, and "Initial Stock" becomes a read-only
  "Current Stock" (Product update still never writes `QtyInStock`). Clear returns to the Add state. A one-shot reset
  after first layout stops the freshly bound grid from silently selecting the first product.
- **Stock In / Stock Out / Orders:** open with no product selected and empty quantity / unit cost / unit price
  (totals preview 0.00; Available Stock shows a dash until a product is picked). Selecting a product still fills
  Available Stock and Unit Price where it did before. "Record Stock In", "Record Stock Out" and "Add Item" stay disabled
  until a product, a whole-number quantity > 0 and a valid price/cost are entered; "Save Pending Order" and
  "Remove Item" need at least one item. These are display states only - the BLL still performs every validation.
  After a successful save the form reloads its product list *then* clears (previously the reload could re-select the
  first product). Confirming an order no longer changes the product picked in the New Order tab.
- **Demo-data / ID cleanup (`InventoryManagementDB` only):** the IDs 1001/1002/1003 were **not** application logic or a
  broken IDENTITY (all seeds are `IDENTITY(1,1)`); the counters had jumped to 1000/1001, which is SQL Server's
  identity-cache jump after an unclean service restart. After a full backup, one transaction re-inserted the same
  rows with sequential IDs: Product 1001 -> 2, Supplier 1001 -> 2, StockIn 5/6/1002/1003 -> 1-4 (all names, quantities,
  costs, dates and stock levels unchanged; StockIn references remapped), then identity counters were reseeded to the
  current max. Verified: no FK violations/orphans, total stock unchanged. `InventoryAppDB` was not touched.
- **Tests:** regression group O (`Tests.UxCleanup.cs`) covers the clean states, Add/Edit mode and button states; groups
  M (sidebar order, disabled primary buttons) updated to match. Result: 962 pass / 0 fail (isolated run); dev-readonly
  run 58 pass / 0 fail. Build: 0 errors, 0 warnings.

## System Currency

**USD ($)** - the system uses one currency only. All monetary values are stored as numeric DECIMAL values (no "$" in
the database) and are shown as USD in the UI/report presentation layer: field labels and grid headers read
"Unit Price (USD)", "Unit Cost (USD)", "Total Cost (USD)", "Total Price (USD)", "Line Total (USD)" and
"Total Amount (USD)"; Report Preview / Print and the report summary lines format money as `$1,250.00`; the Dashboard's
recent-activity order amounts show a `$`. Display only - no schema, BLL, DAL or calculation change, and no
multi-currency / exchange-rate support.

## Demo Dataset (`Database/DemoSeed.sql`)

Data-only script that resets `InventoryManagementDB` to a clean agricultural demo (USD): 2 categories (Chemical /
Natural Fertilizer), 8 products, 3 suppliers, 4 customers, 3 employees, 12 Stock In (2025-2026), 7 Stock Out and
6 Orders (4 Confirmed, 1 Pending, 1 Cancelled) with natural IDs (1, 2, 3 ...). It deletes the old disposable test rows
in FK order but never touches `Users`. Stock is derived with the app's own rules (initial 0 + Stock In - Stock Out -
Confirmed order lines; Pending/Cancelled do not deduct; Orders never create Stock Out rows). It runs in one
transaction, verifies FKs, calculations, statuses, final stock and that stock never went negative over time, and
only reseeds identity counters after a successful COMMIT. Re-runnable. Prices are demonstration values, not market
prices. Run: `sqlcmd -S localhost -E -I -b -i Database\DemoSeed.sql`.

## Known Testing Limitations

- As in earlier CRUD phases, exact over-limit-length rejections cannot be
  driven through the real UI because each textbox's `MaxLength` already
  blocks typing past the limit — verified instead via the `EmployeeBLL`
  harness. This is a UI input safeguard working as intended, not a gap in
  coverage.
- `DataGridView` row selection was automated via real mouse clicks at
  computed screen coordinates rather than UI Automation's `GridPattern`,
  consistent with the approach used in earlier phases.
- Phase 5A testing exercised `frmStockIn` only through the same
  direct-BLL/DAL console harness approach used for `EmployeeBLL` in Phase
  4C, not through UI Automation driving the real running window (no UI
  Automation tooling was available in this environment for this phase). The
  business logic, validation, transaction, and persistence behavior behind
  every UI action were verified directly and are exactly what the UI calls,
  but the UI event wiring itself (button clicks, combo box population,
  filter controls, grid refresh) was verified by code inspection rather than
  by driving the compiled `.exe`'s window. See "Manual QA steps" in the
  Phase 5A report for the walkthrough a human tester should run to close
  this gap.
- The transaction-rollback test above only exercises the case where the
  first statement (`INSERT INTO StockIn`) fails. The case where the insert
  succeeds but the second statement (`UPDATE Products`) fails independently
  could not be reproduced practically with the current schema (the same
  `ProductID` that lets the insert succeed will also satisfy the update's
  `WHERE` clause), so that specific ordering of failure remains unverified
  by an executed test, though the code path (`catch` → `Rollback()` →
  rethrow) is identical for both statements.

## Stop Condition

Phase 5A is complete and verified. Do not proceed to Stock Out, Orders,
Reports, real Dashboard, or authorization work without explicit direction.
