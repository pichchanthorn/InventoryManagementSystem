using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class ucStockOut : UserControl
    {
        private readonly StockOutBLL _stockOutBLL = new StockOutBLL();
        private readonly ProductBLL _productBLL = new ProductBLL();
        private readonly CustomerBLL _customerBLL = new CustomerBLL();

        public ucStockOut()
        {
            Font = Theme.BaseFont;
            InitializeComponent();
            Theme.Apply(this);
            Theme.AttachEmptyState(dgvStockOut, "No stock-out records found.");
            Theme.InsertSectionBand(this, pnlFilter, "Stock Out History");
        }

        private void ucStockOut_Load(object sender, EventArgs e)
        {
            LoadProductOptions();
            LoadCustomerOptions();
            LoadFilterProductOptions();
            LoadFilterCustomerOptions();
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

        private void LoadCustomerOptions()
        {
            try
            {
                var customers = new List<CustomerEntity> { new CustomerEntity { CustomerID = 0, CustomerName = "(No Customer)" } };
                customers.AddRange(_customerBLL.GetAll());
                cmbCustomer.DataSource = customers;
                cmbCustomer.DisplayMember = "CustomerName";
                cmbCustomer.ValueMember = "CustomerID";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Customers", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void LoadFilterCustomerOptions()
        {
            try
            {
                var customers = new List<CustomerEntity> { new CustomerEntity { CustomerID = 0, CustomerName = "(All Customers)" } };
                customers.AddRange(_customerBLL.GetAll());
                cmbFilterCustomer.DataSource = customers;
                cmbFilterCustomer.DisplayMember = "CustomerName";
                cmbFilterCustomer.ValueMember = "CustomerID";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Customers", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHistory(int? productId, int? customerId, DateTime? dateFrom, DateTime? dateTo, string searchText)
        {
            try
            {
                List<StockOutEntity> history = _stockOutBLL.GetAll(productId, customerId, dateFrom, dateTo, searchText);
                dgvStockOut.DataSource = null;
                dgvStockOut.DataSource = history;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Stock Out History", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            var product = cmbProduct.SelectedItem as ProductEntity;
            if (product != null)
            {
                txtAvailableStock.Text = product.QtyInStock.ToString();
                txtUnitPrice.Text = product.UnitPrice.ToString("0.00");
            }
            else
            {
                txtAvailableStock.Text = "\u2014";
            }

            UpdateRecordButtonState();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int? productId = cmbFilterProduct.SelectedValue is int && (int)cmbFilterProduct.SelectedValue > 0
                ? (int?)(int)cmbFilterProduct.SelectedValue
                : null;
            int? customerId = cmbFilterCustomer.SelectedValue is int && (int)cmbFilterCustomer.SelectedValue > 0
                ? (int?)(int)cmbFilterCustomer.SelectedValue
                : null;

            DateTime? dateFrom = chkDateFilter.Checked ? dtpFrom.Value.Date : (DateTime?)null;
            DateTime? dateTo = chkDateFilter.Checked ? dtpTo.Value.Date : (DateTime?)null;

            if (chkDateFilter.Checked && dateFrom.Value > dateTo.Value)
            {
                MessageBox.Show(this, "The 'From' date cannot be later than the 'To' date.", "Invalid Date Range",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadHistory(productId, customerId, dateFrom, dateTo, txtSearch.Text);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (cmbFilterProduct.Items.Count > 0)
                cmbFilterProduct.SelectedIndex = 0;
            if (cmbFilterCustomer.Items.Count > 0)
                cmbFilterCustomer.SelectedIndex = 0;
            chkDateFilter.Checked = false;
            txtSearch.Clear();

            LoadHistory(null, null, null, null, null);
        }

        private void chkDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            dtpFrom.Enabled = chkDateFilter.Checked;
            dtpTo.Enabled = chkDateFilter.Checked;
        }

        private void QuantityOrUnitPrice_TextChanged(object sender, EventArgs e)
        {
            UpdateRecordButtonState();

            decimal quantity;
            decimal unitPrice;
            if (decimal.TryParse(txtQuantity.Text.Trim(), out quantity) &&
                decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice))
            {
                txtTotalPrice.Text = (quantity * unitPrice).ToString("0.00");
            }
            else
            {
                txtTotalPrice.Text = "0.00";
            }
        }

        /// <summary>
        /// UX only: "Record Stock Out" looks ready once a product, a whole-number quantity above zero and a unit
        /// price are entered. StockOutBLL / StockOutDAL still enforce stock availability atomically.
        /// </summary>
        private void UpdateRecordButtonState()
        {
            int quantity;
            decimal unitPrice;
            btnAdd.Enabled = cmbProduct.SelectedIndex >= 0 &&
                             int.TryParse(txtQuantity.Text.Trim(), out quantity) && quantity > 0 &&
                             decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice) && unitPrice >= 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            StockOutEntity stockOut;
            string parseError;
            if (!TryBuildStockOutFromForm(out stockOut, out parseError))
            {
                MessageBox.Show(this, parseError, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StockOutResult result = _stockOutBLL.Add(stockOut);
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
                MessageBox.Show(this, result.Message, "Unable to Record Stock Out", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private bool TryBuildStockOutFromForm(out StockOutEntity stockOut, out string error)
        {
            stockOut = null;
            error = null;

            int productId = cmbProduct.SelectedValue is int ? (int)cmbProduct.SelectedValue : 0;
            int customerId = cmbCustomer.SelectedValue is int ? (int)cmbCustomer.SelectedValue : 0;

            int quantity;
            if (!int.TryParse(txtQuantity.Text.Trim(), out quantity))
            {
                error = "Quantity must be a whole number.";
                return false;
            }

            decimal unitPrice;
            if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice))
            {
                error = "Unit value must be a valid number.";
                return false;
            }

            stockOut = new StockOutEntity
            {
                ProductID = productId,
                CustomerID = customerId > 0 ? customerId : (int?)null,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Notes = txtNotes.Text
            };
            return true;
        }

        private void ClearForm()
        {
            cmbProduct.SelectedIndex = -1;
            txtAvailableStock.Text = "\u2014";
            if (cmbCustomer.Items.Count > 0)
                cmbCustomer.SelectedIndex = 0;
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            txtTotalPrice.Text = "0.00";
            txtNotes.Clear();
            UpdateRecordButtonState();
            cmbProduct.Focus();
        }
    }
}
