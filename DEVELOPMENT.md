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
  talks only to `EmployeeBLL`.
- **BLL** — Business logic layer. Owns business rules, validation, and
  authentication (`UserBLL`, `PasswordHasher`, `LoginResult`, `CategoryBLL`,
  `CategoryResult`, `ProductBLL`, `ProductResult`, `CustomerBLL`,
  `CustomerResult`, `SupplierBLL`, `SupplierResult`, `EmployeeBLL`,
  `EmployeeResult`).
- **DAL** — Data access layer. Owns all database access (`DbConnection`,
  `UserDAL`, `CategoryDAL`, `ProductDAL`, `CustomerDAL`, `SupplierDAL`,
  `EmployeeDAL`). All SQL is parameterized.
- **Entity** — Plain model classes shared across layers. No business or database
  logic. `ProductEntity` carries one display-only, non-persisted property
  (`CategoryName`) populated by `ProductDAL`'s join — it is not a database
  column and is never written by Insert/Update.

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
  UI/
    frmLogin.cs / .Designer.cs      Login form (startup form)
    frmMain.cs  / .Designer.cs      Post-login shell: header, left navigation
                                     (Categories, Products, Customers,
                                     Suppliers, and Employees functional; 5
                                     other items still placeholders), content
                                     area, logout
    frmCategories.cs / .Designer.cs Category list/add/edit/delete window
    frmProducts.cs   / .Designer.cs Product list/add/edit/delete window
    frmCustomers.cs  / .Designer.cs Customer list/add/edit/delete window
    frmSuppliers.cs  / .Designer.cs Supplier list/add/edit/delete window
    frmEmployees.cs  / .Designer.cs Employee list/add/edit/delete window
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

**Phase 4C — Employee CRUD — COMPLETE**

Prior completed phases:
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

## What Has NOT Been Implemented

- Stock In / Stock Out workflows
- Orders / order details workflow
- Reports
- A real Dashboard (statistics, charts, computed business data)
- Per-module authorization / permission management

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

## Known Testing Limitations

- As in earlier CRUD phases, exact over-limit-length rejections cannot be
  driven through the real UI because each textbox's `MaxLength` already
  blocks typing past the limit — verified instead via the `EmployeeBLL`
  harness. This is a UI input safeguard working as intended, not a gap in
  coverage.
- `DataGridView` row selection was automated via real mouse clicks at
  computed screen coordinates rather than UI Automation's `GridPattern`,
  consistent with the approach used in earlier phases.

## Stop Condition

Phase 4C is complete and verified. Do not proceed to Stock In, Stock Out,
Orders, Reports, real Dashboard, or authorization work without explicit
direction.
