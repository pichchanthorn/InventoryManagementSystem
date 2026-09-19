using System;
using System.Windows.Forms;
using InventoryManagementSystem.UI;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>
    /// Group O (UX cleanup pass): pages open in a clean state, Add/Edit modes on Products, transaction buttons
    /// only look ready when the form is valid, and product selection fills Available Stock / Unit Price.
    /// UI-state only - no business data is written by these checks (the seed rows are created directly by SQL).
    /// </summary>
    internal static class Tests_UxCleanup
    {
        private const string Dash = "—";

        public static void Run()
        {
            T.Group("O UX cleanup (clean initial states)");
            T.Wipe();

            int cat = T.NewCategory("UX Category");
            int p1 = T.NewProduct("UX Product A", cat, 25, 12.5m);
            T.NewProduct("UX Product B", cat, 0, 3m);
            T.NewCustomer("UX Customer");
            T.NewSupplier("UX Supplier");
            T.NewEmployee("UX Employee");
            string snapshot = T.Snapshot();

            frmMain main = Tests_ReportsDashboard.OpenMain("UX Tester", "Admin");
            Action<string> go = key => { T.F<Button>(main, "btnNav" + key).PerformClick(); T.Pump(); T.Pump(); };

            // ---- Products -------------------------------------------------------------------------
            go("Products");
            UserControl pr = main.CurrentPage;
            var dgv = T.F<DataGridView>(pr, "dgvProducts");
            var name = T.F<TextBox>(pr, "txtProductName");
            var category = T.F<ComboBox>(pr, "cmbCategory");
            var price = T.F<TextBox>(pr, "txtUnitPrice");
            var stock = T.F<TextBox>(pr, "txtInitialStock");
            var stockLabel = T.F<Label>(pr, "lblInitialStock");
            var add = T.F<Button>(pr, "btnAdd"); var upd = T.F<Button>(pr, "btnUpdate");
            var del = T.F<Button>(pr, "btnDelete"); var clear = T.F<Button>(pr, "btnClear");

            Action checkProductsClean = () =>
            {
                T.Check("O01 Products: name, price empty; no category chosen", name.Text == "" && price.Text == "" && category.SelectedIndex == -1);
                T.Check("O01 Products: no grid row selected", dgv.SelectedRows.Count == 0);
                T.Check("O01 Products: Add mode - Add enabled, Update/Delete disabled", add.Enabled && !upd.Enabled && !del.Enabled);
                T.Check("O01 Products: stock box is 'Initial Stock' and editable", stockLabel.Text == "Initial Stock:" && !stock.ReadOnly);
            };
            checkProductsClean();

            dgv.CurrentCell = dgv.Rows[0].Cells[1]; dgv.ClearSelection(); dgv.Rows[0].Selected = true; T.Pump();   // what a click on the row does
            T.Check("O02 Products: selecting a row fills the form", name.Text.Length > 0 && category.SelectedIndex >= 0 && price.Text.Length > 0, "rows=" + dgv.Rows.Count + " cur=" + (dgv.CurrentCell == null ? "null" : dgv.CurrentCell.RowIndex + "/" + dgv.CurrentCell.ColumnIndex) + " sel=" + dgv.SelectedRows.Count + " name=[" + name.Text + "]");
            T.Check("O02 Products: Edit mode - Update/Delete enabled, Add disabled", !add.Enabled && upd.Enabled && del.Enabled);
            T.Check("O02 Products: stock box becomes read-only 'Current Stock'", stockLabel.Text == "Current Stock:" && stock.ReadOnly);

            clear.PerformClick(); T.Pump();
            checkProductsClean();

            // ---- Stock In -------------------------------------------------------------------------
            go("StockIn");
            UserControl si = main.CurrentPage;
            var siProduct = T.F<ComboBox>(si, "cmbProduct"); var siSupplier = T.F<ComboBox>(si, "cmbSupplier");
            var siQty = T.F<TextBox>(si, "txtQuantity"); var siCost = T.F<TextBox>(si, "txtUnitCost");
            var siTotal = T.F<TextBox>(si, "txtTotalCost"); var siNotes = T.F<TextBox>(si, "txtNotes");
            var siAdd = T.F<Button>(si, "btnAdd"); var siClear = T.F<Button>(si, "btnClear");

            Action checkStockInClean = () =>
            {
                T.Check("O03 Stock In: no product chosen, supplier '(No Supplier)'", siProduct.SelectedIndex == -1 && siSupplier.SelectedIndex == 0);
                T.Check("O03 Stock In: quantity, unit cost, notes empty; total 0.00", siQty.Text == "" && siCost.Text == "" && siNotes.Text == "" && siTotal.Text == "0.00");
                T.Check("O03 Stock In: Record button disabled until the form is valid", !siAdd.Enabled);
            };
            checkStockInClean();

            siProduct.SelectedIndex = 0; T.Pump();
            T.Check("O04 Stock In: product alone is not enough", !siAdd.Enabled);
            siQty.Text = "4"; siCost.Text = "2.5"; T.Pump();
            T.Check("O04 Stock In: product + quantity + unit cost enables Record; total previews 10.00", siAdd.Enabled && siTotal.Text == "10.00", siTotal.Text);
            siQty.Text = "0"; T.Pump();
            T.Check("O04 Stock In: quantity 0 disables Record again", !siAdd.Enabled);
            siClear.PerformClick(); T.Pump();
            checkStockInClean();

            // ---- Stock Out ------------------------------------------------------------------------
            go("StockOut");
            UserControl so = main.CurrentPage;
            var soProduct = T.F<ComboBox>(so, "cmbProduct"); var soAvail = T.F<TextBox>(so, "txtAvailableStock");
            var soCustomer = T.F<ComboBox>(so, "cmbCustomer"); var soQty = T.F<TextBox>(so, "txtQuantity");
            var soPrice = T.F<TextBox>(so, "txtUnitPrice"); var soTotal = T.F<TextBox>(so, "txtTotalPrice");
            var soAdd = T.F<Button>(so, "btnAdd"); var soClear = T.F<Button>(so, "btnClear");

            Action checkStockOutClean = () =>
            {
                T.Check("O05 Stock Out: no product, Available Stock blank ('" + Dash + "'), customer '(No Customer)'",
                    soProduct.SelectedIndex == -1 && soAvail.Text == Dash && soCustomer.SelectedIndex == 0);
                T.Check("O05 Stock Out: quantity, unit price empty; total 0.00", soQty.Text == "" && soPrice.Text == "" && soTotal.Text == "0.00");
                T.Check("O05 Stock Out: Record button disabled", !soAdd.Enabled);
            };
            checkStockOutClean();

            soProduct.SelectedIndex = 0; T.Pump();     // "UX Product A" (alphabetical / first row)
            T.Check("O06 Stock Out: selecting a product shows its Available Stock and Unit Price",
                soAvail.Text == T.Stock(p1).ToString() && soPrice.Text == "12.50", soAvail.Text + " / " + soPrice.Text);
            T.Check("O06 Stock Out: Record still disabled without a quantity", !soAdd.Enabled);
            soQty.Text = "2"; T.Pump();
            T.Check("O06 Stock Out: quantity enables Record; total previews 25.00", soAdd.Enabled && soTotal.Text == "25.00", soTotal.Text);
            soClear.PerformClick(); T.Pump();
            checkStockOutClean();

            // ---- Orders ---------------------------------------------------------------------------
            go("Orders");
            UserControl od = main.CurrentPage;
            var oProduct = T.F<ComboBox>(od, "cmbProduct"); var oAvail = T.F<TextBox>(od, "txtAvailableStock");
            var oQty = T.F<TextBox>(od, "txtQuantity"); var oPrice = T.F<TextBox>(od, "txtUnitPrice");
            var oAddItem = T.F<Button>(od, "btnAddItem"); var oRemove = T.F<Button>(od, "btnRemoveItem");
            var oSave = T.F<Button>(od, "btnSaveOrder"); var oClear = T.F<Button>(od, "btnClear");
            var oTotal = T.F<TextBox>(od, "txtTotalAmount");

            T.Check("O07 Orders: no product chosen, Available Stock blank, quantity/price empty",
                oProduct.SelectedIndex == -1 && oAvail.Text == Dash && oQty.Text == "" && oPrice.Text == "");
            T.Check("O07 Orders: Add Item, Remove Item and Save are disabled on a new order", !oAddItem.Enabled && !oRemove.Enabled && !oSave.Enabled);

            oProduct.SelectedIndex = 0; T.Pump();
            T.Check("O08 Orders: product shows Available Stock and Unit Price; Add Item still needs a quantity",
                oAvail.Text == T.Stock(p1).ToString() && oPrice.Text == "12.50" && !oAddItem.Enabled);
            oQty.Text = "3"; T.Pump();
            T.Check("O08 Orders: valid product + quantity enables Add Item", oAddItem.Enabled);
            oAddItem.PerformClick(); T.Pump();
            T.Check("O08 Orders: after adding an item the line is in the order, total 37.50, quantity box cleared",
                oTotal.Text == "37.50" && oQty.Text == "" && !oAddItem.Enabled, oTotal.Text);
            T.Check("O08 Orders: Save and Remove Item are enabled once the order has an item", oSave.Enabled && oRemove.Enabled);
            oClear.PerformClick(); T.Pump();
            T.Check("O09 Orders: Clear returns to the clean state", oProduct.SelectedIndex == -1 && oTotal.Text == "0.00" && !oSave.Enabled && !oAddItem.Enabled);

            // ---- Categories / Customers / Suppliers / Employees: same Add/Edit behaviour as Products ----
            foreach (string key in new[] { "Categories", "Customers", "Suppliers", "Employees" })
            {
                go(key);
                UserControl page = main.CurrentPage;
                var grid = T.F<DataGridView>(page, "dgv" + key);
                var pAdd = T.F<Button>(page, "btnAdd"); var pUpd = T.F<Button>(page, "btnUpdate");
                var pDel = T.F<Button>(page, "btnDelete"); var pClear = T.F<Button>(page, "btnClear");
                var pName = T.F<TextBox>(page, "txt" + (key == "Categories" ? "Category" : key.Substring(0, key.Length - 1)) + "Name");

                Action clean = () =>
                {
                    T.Check("O11 " + key + ": no row selected and name empty", grid.SelectedRows.Count == 0 && pName.Text == "");
                    T.Check("O11 " + key + ": Add mode - Add enabled, Update/Delete disabled", pAdd.Enabled && !pUpd.Enabled && !pDel.Enabled);
                };
                clean();

                grid.CurrentCell = grid.Rows[0].Cells[1]; grid.ClearSelection(); grid.Rows[0].Selected = true; T.Pump();
                T.Check("O12 " + key + ": selecting a row fills the form", pName.Text.Length > 0, "name=[" + pName.Text + "]");
                T.Check("O12 " + key + ": Edit mode - Add disabled, Update/Delete enabled", !pAdd.Enabled && pUpd.Enabled && pDel.Enabled);

                pClear.PerformClick(); T.Pump();
                clean();
            }
            // ---- USD currency labels (display only) -----------------------------------------------
            go("Products");
            T.Eq("O20 Products: price label", "Unit Price (USD):", T.F<Label>(main.CurrentPage, "lblUnitPrice").Text);
            T.Eq("O20 Products: grid header", "Unit Price (USD)", T.F<DataGridViewColumn>(main.CurrentPage, "colUnitPrice").HeaderText);
            go("StockIn");
            T.Eq("O21 Stock In: labels", "Unit Cost (USD):|Total Cost (USD):", T.F<Label>(main.CurrentPage, "lblUnitCost").Text + "|" + T.F<Label>(main.CurrentPage, "lblTotalCost").Text);
            T.F<ComboBox>(main.CurrentPage, "cmbProduct").SelectedIndex = 0;
            T.F<TextBox>(main.CurrentPage, "txtQuantity").Text = "10"; T.F<TextBox>(main.CurrentPage, "txtUnitCost").Text = "25.00"; T.Pump();
            T.Eq("O21 Stock In: 10 x 25.00 = 250.00 (plain number, no $ in the field)", "250.00", T.F<TextBox>(main.CurrentPage, "txtTotalCost").Text);
            go("StockOut");
            T.Eq("O22 Stock Out: labels", "Unit Price (USD):|Total Price (USD):", T.F<Label>(main.CurrentPage, "lblUnitPrice").Text + "|" + T.F<Label>(main.CurrentPage, "lblTotalPrice").Text);
            go("Orders");
            T.Eq("O23 Orders: labels", "Unit Price (USD):|Line Total (USD):|Total Amount (USD):",
                T.F<Label>(main.CurrentPage, "lblUnitPrice").Text + "|" + T.F<Label>(main.CurrentPage, "lblLineTotal").Text + "|" + T.F<Label>(main.CurrentPage, "lblTotalAmountCaption").Text);
            T.Eq("O23 Orders: grid headers", "Unit Price (USD)|Line Total (USD)|Total Amount (USD)",
                T.F<DataGridViewColumn>(main.CurrentPage, "colDetailUnitPrice").HeaderText + "|" + T.F<DataGridViewColumn>(main.CurrentPage, "colDetailTotal").HeaderText + "|" + T.F<DataGridViewColumn>(main.CurrentPage, "colHistTotalAmount").HeaderText);
            // ---- nothing was written by any of the above ------------------------------------------
            T.Eq("O10 UX checks wrote no business data", snapshot, T.Snapshot());

            main.Close(); T.CloseAllForms();
        }
    }
}
