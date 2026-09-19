# InventoryManagementSystem.RegressionTests

Automated regression suite (BLL / DAL / database behaviour plus UI smoke checks) for the completed phases 1-7B.
It is a plain console project (no test framework dependency) and is **not** part of `InventoryManagementSystem.slnx`,
so the application build is unaffected.

## Build
```
MSBuild InventoryManagementSystem.RegressionTests\InventoryManagementSystem.RegressionTests.csproj -p:Configuration=Debug
```
(builds the application project first, via a project reference)

## Run
```
InventoryManagementSystem.RegressionTests\bin\Debug\InventoryManagementSystem.RegressionTests.exe
```
| Mode | What it does |
|------|--------------|
| *(no args)* | **Isolated run.** Creates a disposable database `ZZREG_InventoryManagementDB` from `InventoryManagementSystem\Database\InventoryManagementDB.sql`, runs every test group against it, then drops it. The development database is only *read* (row counts + checksums before/after, to prove it was not touched). |
| `--keep-db` | Isolated run, but leaves the test database in place for inspection. |
| `--dev-readonly` | Read-only smoke run against the real development database (schema checks, dashboard/report numbers vs independent SQL). Performs no writes and proves it with before/after checksums. |

Exit code is `0` only if every check passed.

## Safety rules built in
* The suite re-points the application's `DbConnection` at the test database before any data access.
* Destructive helpers (`Wipe`, database drop) refuse to run unless the database name starts with `ZZREG_`.
* Test accounts use a random throw-away password generated at run time; no real credential is used.
