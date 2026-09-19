using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class ucStockIn : UserControl
    {
        private readonly StockInBLL _stockInBLL = new StockInBLL();
        private readonly ProductBLL _productBLL = new ProductBLL();
        private readonly SupplierBLL _supplierBLL = new SupplierBLL();

        public ucStockIn()
        {
            Font = Theme.BaseFont;
            InitializeComponent();
            Theme.Apply(this);
            Theme.AttachEmptyState(dgvStockIn, "No stock-in records found.");
            Theme.InsertSectionBand(this, pnlFilter, "Stock In History");
            cmbProduct.SelectedIndexChanged += (s, e) => UpdateRecordButtonState();
        }

        private void ucStockIn_Load(object sender, EventArgs e)
        {
            LoadProductOptions();
            LoadSupplierOptions();
            LoadFilterProductOptions();
            LoadFilterSupplierOptions();
            ClearForm();
            LoadHistory(null, null, null, null, null);
        }

        private void LoadProductOptions()
        {
            try
            {
                cmbProduct.DataSource = _productBLL.GetAll();
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductID";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSupplierOptions()
        {
            try
            {
                var suppliers = new List<SupplierEntity> { new SupplierEntity { SupplierID = 0, SupplierName = "(No Supplier)" } };
                suppliers.AddRange(_supplierBLL.GetAll());
                cmbSupplier.DataSource = suppliers;
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Suppliers", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFilterProductOptions()
        {
            try
            {
                var products = new List<ProductEntity> { new ProductEntity { ProductID = 0, ProductName = "(All Products)" } };
                products.AddRange(_productBLL.GetAll());
                cmbFilterProduct.DataSource = products;
                cmbFilterProduct.DisplayMember = "ProductName";
                cmbFilterProduct.ValueMember = "ProductID";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFilterSupplierOptions()
        {
            try
            {
                var suppliers = new List<SupplierEntity> { new SupplierEntity { SupplierID = 0, SupplierName = "(All Suppliers)" } };
                suppliers.AddRange(_supplierBLL.GetAll());
                cmbFilterSupplier.DataSource = suppliers;
                cmbFilterSupplier.DisplayMember = "SupplierName";
                cmbFilterSupplier.ValueMember = "SupplierID";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Suppliers", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHistory(int? productId, int? supplierId, DateTime? dateFrom, DateTime? dateTo, string searchText)
        {
            try
            {
                List<StockInEntity> history = _stockInBLL.GetAll(productId, supplierId, dateFrom, dateTo, searchText);
                dgvStockIn.DataSource = null;
                dgvStockIn.DataSource = history;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Stock In History", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int? productId = cmbFilterProduct.SelectedValue is int && (int)cmbFilterProduct.SelectedValue > 0
                ? (int?)(int)cmbFilterProduct.SelectedValue
                : null;
            int? supplierId = cmbFilterSupplier.SelectedValue is int && (int)cmbFilterSupplier.SelectedValue > 0
                ? (int?)(int)cmbFilterSupplier.SelectedValue
                : null;

            DateTime? dateFrom = chkDateFilter.Checked ? dtpFrom.Value.Date : (DateTime?)null;
            DateTime? dateTo = chkDateFilter.Checked ? dtpTo.Value.Date : (DateTime?)null;

            if (chkDateFilter.Checked && dateFrom.Value > dateTo.Value)
            {
                MessageBox.Show(this, "The 'From' date cannot be later than the 'To' date.", "Invalid Date Range",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadHistory(productId, supplierId, dateFrom, dateTo, txtSearch.Text);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (cmbFilterProduct.Items.Count > 0)
                cmbFilterProduct.SelectedIndex = 0;
            if (cmbFilterSupplier.Items.Count > 0)
                cmbFilterSupplier.SelectedIndex = 0;
            chkDateFilter.Checked = false;
            txtSearch.Clear();

            LoadHistory(null, null, null, null, null);
        }

        private void chkDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            dtpFrom.Enabled = chkDateFilter.Checked;
            dtpTo.Enabled = chkDateFilter.Checked;
        }

        private void QuantityOrUnitCost_TextChanged(object sender, EventArgs e)
        {
            UpdateRecordButtonState();

            decimal quantity;
            decimal unitCost;
            if (decimal.TryParse(txtQuantity.Text.Trim(), out quantity) &&
                decimal.TryParse(txtUnitCost.Text.Trim(), out unitCost))
            {
                txtTotalCost.Text = (quantity * unitCost).ToString("0.00");
            }
            else
            {
                txtTotalCost.Text = "0.00";
            }
        }

        /// <summary>
        /// UX only: "Record Stock In" looks ready once a product, a whole-number quantity above zero and a unit
        /// cost are entered. StockInBLL still performs every real validation.
        /// </summary>
        private void UpdateRecordButtonState()
        {
            int quantity;
            decimal unitCost;
            btnAdd.Enabled = cmbProduct.SelectedIndex >= 0 &&
                             int.TryParse(txtQuantity.Text.Trim(), out quantity) && quantity > 0 &&
                             decimal.TryParse(txtUnitCost.Text.Trim(), out unitCost) && unitCost >= 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            StockInEntity stockIn;
            string parseError;
            if (!TryBuildStockInFromForm(out stockIn, out parseError))
            {
                MessageBox.Show(this, parseError, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StockInResult result = _stockInBLL.Add(stockIn);
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadProductOptions();
                LoadFilterProductOptions();
                ClearForm();
                LoadHistory(null, null, null, null, null);
            }
            else
            {
                MessageBox.Show(this, result.Message, "Unable to Record Stock In", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private bool TryBuildStockInFromForm(out StockInEntity stockIn, out string error)
        {
            stockIn = null;
            error = null;

            int productId = cmbProduct.SelectedValue is int ? (int)cmbProduct.SelectedValue : 0;
            int supplierId = cmbSupplier.SelectedValue is int ? (int)cmbSupplier.SelectedValue : 0;

            int quantity;
            if (!int.TryParse(txtQuantity.Text.Trim(), out quantity))
            {
                error = "Quantity must be a whole number.";
                return false;
            }

            decimal unitCost;
            if (!decimal.TryParse(txtUnitCost.Text.Trim(), out unitCost))
            {
                error = "Unit cost must be a valid number.";
                return false;
            }

            stockIn = new StockInEntity
            {
                ProductID = productId,
                SupplierID = supplierId > 0 ? supplierId : (int?)null,
                Quantity = quantity,
                UnitCost = unitCost,
                Notes = txtNotes.Text
            };
            return true;
        }

        private void ClearForm()
        {
            cmbProduct.SelectedIndex = -1;
            if (cmbSupplier.Items.Count > 0)
                cmbSupplier.SelectedIndex = 0;
            txtQuantity.Clear();
            txtUnitCost.Clear();
            txtTotalCost.Text = "0.00";
            txtNotes.Clear();
            UpdateRecordButtonState();
            cmbProduct.Focus();
        }
    }
}
