using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class ucOrders : UserControl
    {
        private readonly OrderBLL _orderBLL = new OrderBLL();
        private readonly ProductBLL _productBLL = new ProductBLL();
        private readonly CustomerBLL _customerBLL = new CustomerBLL();
        private readonly EmployeeBLL _employeeBLL = new EmployeeBLL();

        private readonly BindingList<OrderDetailEntity> _currentDetails = new BindingList<OrderDetailEntity>();

        private bool _suppressSelectionChanged;

        public ucOrders()
        {
            Font = Theme.BaseFont;
            InitializeComponent();
            Theme.Apply(this);
            Theme.AttachEmptyState(dgvHistory, "No orders found.");
            Theme.AttachEmptyState(dgvOrderDetails, "No items added to this order yet.");
            tabOrders.Font = Theme.SemiboldFont;
        }

        private void ucOrders_Load(object sender, EventArgs e)
        {
            LoadCustomerOptions();
            LoadEmployeeOptions();
            LoadProductOptions();
            LoadFilterCustomerOptions();
            LoadFilterStatusOptions();

            dgvOrderDetails.AutoGenerateColumns = false;
            dgvOrderDetails.DataSource = _currentDetails;

            ClearForm();
            LoadHistory(null, null, null, null, null);
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

        private void LoadEmployeeOptions()
        {
            try
            {
                var employees = new List<EmployeeEntity> { new EmployeeEntity { EmployeeID = 0, EmployeeName = "(No Employee)" } };
                employees.AddRange(_employeeBLL.GetAll());
                cmbEmployee.DataSource = employees;
                cmbEmployee.DisplayMember = "EmployeeName";
                cmbEmployee.ValueMember = "EmployeeID";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Employees", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void LoadFilterStatusOptions()
        {
            cmbFilterStatus.Items.Clear();
            cmbFilterStatus.Items.Add("(All Statuses)");
            cmbFilterStatus.Items.Add("Pending");
            cmbFilterStatus.Items.Add("Confirmed");
            cmbFilterStatus.Items.Add("Cancelled");
            cmbFilterStatus.SelectedIndex = 0;
        }

        private void LoadHistory(string status, int? customerId, DateTime? dateFrom, DateTime? dateTo, string searchText)
        {
            try
            {
                List<OrderEntity> history = _orderBLL.GetAll(status, customerId, dateFrom, dateTo, searchText);

                // Rebinding the grid changes its selection (and can transiently clear
                // CurrentRow). Suppress SelectionChanged while that happens so the refresh
                // itself never triggers an action - only an explicit user click/double-click
                // ever opens a dialog or mutates state.
                _suppressSelectionChanged = true;
                try
                {
                    dgvHistory.DataSource = null;
                    dgvHistory.DataSource = history;
                }
                finally
                {
                    _suppressSelectionChanged = false;
                }

                UpdateHistoryActionButtons();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Order History", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHistory_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChanged)
                return;

            UpdateHistoryActionButtons();
        }

        /// <summary>
        /// Enables/disables Confirm, Cancel and View Details based on the selected order's
        /// status. This is a UX convenience only - OrderBLL/OrderDAL still enforce the
        /// Pending-only guard on Confirm/Cancel regardless of button state.
        /// </summary>
        private void UpdateHistoryActionButtons()
        {
            OrderEntity selected = GetSelectedHistoryOrder();
            bool isPending = selected != null && selected.Status == "Pending";

            btnConfirmOrder.Enabled = isPending;
            btnCancelOrder.Enabled = isPending;
            btnViewDetails.Enabled = selected != null;
        }

        private void dgvHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            OpenSelectedOrderDetails();
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            OpenSelectedOrderDetails();
        }

        private void OpenSelectedOrderDetails()
        {
            OrderEntity selected = GetSelectedHistoryOrder();
            if (selected == null)
            {
                MessageBox.Show(this, "Please select an order from the history grid.", "No Order Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var detailsForm = new frmOrderDetails(selected))
            {
                detailsForm.ShowDialog(this);
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

            RecalculateLineTotalPreview();
        }

        private void QuantityOrUnitPrice_TextChanged(object sender, EventArgs e)
        {
            RecalculateLineTotalPreview();
        }

        private void RecalculateLineTotalPreview()
        {
            UpdateAddItemButtonState();

            decimal quantity;
            decimal unitPrice;
            if (decimal.TryParse(txtQuantity.Text.Trim(), out quantity) &&
                decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice))
            {
                txtLineTotal.Text = (quantity * unitPrice).ToString("0.00");
            }
            else
            {
                txtLineTotal.Text = "0.00";
            }
        }

        /// <summary>
        /// UX only: "Add Item" looks ready once a product, a whole-number quantity above zero and a non-negative
        /// unit price are entered. The click handler still re-validates everything.
        /// </summary>
        private void UpdateAddItemButtonState()
        {
            int quantity;
            decimal unitPrice;
            btnAddItem.Enabled = cmbProduct.SelectedIndex >= 0 &&
                                 int.TryParse(txtQuantity.Text.Trim(), out quantity) && quantity > 0 &&
                                 decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice) && unitPrice >= 0;
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            var product = cmbProduct.SelectedItem as ProductEntity;
            if (product == null)
            {
                MessageBox.Show(this, "Please select a product.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int quantity;
            if (!int.TryParse(txtQuantity.Text.Trim(), out quantity) || quantity <= 0)
            {
                MessageBox.Show(this, "Quantity must be a whole number greater than zero.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal unitPrice;
            if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice) || unitPrice < 0)
            {
                MessageBox.Show(this, "Unit price must be a valid, non-negative number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OrderDetailEntity existing = _currentDetails.FirstOrDefault(d => d.ProductID == product.ProductID);
            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.UnitPrice = unitPrice;
                existing.Total = Math.Round(existing.Quantity * existing.UnitPrice, 2, MidpointRounding.AwayFromZero);
                _currentDetails.ResetItem(_currentDetails.IndexOf(existing));
            }
            else
            {
                _currentDetails.Add(new OrderDetailEntity
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Total = Math.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero)
                });
            }

            RecalculateTotalAmount();
            txtQuantity.Clear();
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            var selected = dgvOrderDetails.CurrentRow != null
                ? dgvOrderDetails.CurrentRow.DataBoundItem as OrderDetailEntity
                : null;

            if (selected == null)
            {
                MessageBox.Show(this, "Please select an item to remove.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentDetails.Remove(selected);
            RecalculateTotalAmount();
        }

        private void RecalculateTotalAmount()
        {
            decimal total = _currentDetails.Sum(d => d.Total);
            txtTotalAmount.Text = total.ToString("0.00");

            // Save / Remove only make sense once the order has at least one item (OrderBLL still validates).
            btnSaveOrder.Enabled = _currentDetails.Count > 0;
            btnRemoveItem.Enabled = _currentDetails.Count > 0;
        }

        private void btnSaveOrder_Click(object sender, EventArgs e)
        {
            int customerId = cmbCustomer.SelectedValue is int ? (int)cmbCustomer.SelectedValue : 0;
            int employeeId = cmbEmployee.SelectedValue is int ? (int)cmbEmployee.SelectedValue : 0;

            var order = new OrderEntity
            {
                CustomerID = customerId > 0 ? customerId : (int?)null,
                EmployeeID = employeeId > 0 ? employeeId : (int?)null
            };

            OrderResult result = _orderBLL.CreateOrder(order, _currentDetails.ToList());
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadHistory(null, null, null, null, null);
            }
            else
            {
                MessageBox.Show(this, result.Message, "Unable to Save Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            OrderEntity selected = GetSelectedHistoryOrder();
            if (selected == null)
            {
                MessageBox.Show(this, "Please select an order from the history grid.", "No Order Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OrderResult result = _orderBLL.ConfirmOrder(selected.OrderID);
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadProductOptionsKeepingSelection();
                RefreshHistoryFromFilters();
            }
            else
            {
                MessageBox.Show(this, result.Message, "Unable to Confirm Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            OrderEntity selected = GetSelectedHistoryOrder();
            if (selected == null)
            {
                MessageBox.Show(this, "Please select an order from the history grid.", "No Order Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OrderResult result = _orderBLL.CancelOrder(selected.OrderID);
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshHistoryFromFilters();
            }
            else
            {
                MessageBox.Show(this, result.Message, "Unable to Cancel Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Refreshes the product list (stock changed) without silently selecting the first product in the
        /// New Order form - whatever the user had picked stays picked, and "nothing picked" stays that way.
        /// </summary>
        private void ReloadProductOptionsKeepingSelection()
        {
            int selectedId = cmbProduct.SelectedValue is int ? (int)cmbProduct.SelectedValue : 0;
            LoadProductOptions();
            if (selectedId > 0)
                cmbProduct.SelectedValue = selectedId;
            else
                cmbProduct.SelectedIndex = -1;
        }

        private OrderEntity GetSelectedHistoryOrder()
        {
            return dgvHistory.CurrentRow != null ? dgvHistory.CurrentRow.DataBoundItem as OrderEntity : null;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            if (cmbCustomer.Items.Count > 0)
                cmbCustomer.SelectedIndex = 0;
            if (cmbEmployee.Items.Count > 0)
                cmbEmployee.SelectedIndex = 0;
            cmbProduct.SelectedIndex = -1;
            txtAvailableStock.Text = "\u2014";

            txtOrderDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            lblStatusValue.Text = "Pending";

            txtQuantity.Clear();
            txtUnitPrice.Clear();
            txtLineTotal.Text = "0.00";

            _currentDetails.Clear();
            RecalculateTotalAmount();
            UpdateAddItemButtonState();
        }

        private void btnSearchOrders_Click(object sender, EventArgs e)
        {
            string status = GetSelectedFilterStatus();

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

            LoadHistory(status, customerId, dateFrom, dateTo, txtSearch.Text);
        }

        private void btnRefreshOrders_Click(object sender, EventArgs e)
        {
            if (cmbFilterStatus.Items.Count > 0)
                cmbFilterStatus.SelectedIndex = 0;
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

        private void RefreshHistoryFromFilters()
        {
            string status = GetSelectedFilterStatus();

            int? customerId = cmbFilterCustomer.SelectedValue is int && (int)cmbFilterCustomer.SelectedValue > 0
                ? (int?)(int)cmbFilterCustomer.SelectedValue
                : null;

            DateTime? dateFrom = chkDateFilter.Checked ? dtpFrom.Value.Date : (DateTime?)null;
            DateTime? dateTo = chkDateFilter.Checked ? dtpTo.Value.Date : (DateTime?)null;

            LoadHistory(status, customerId, dateFrom, dateTo, txtSearch.Text);
        }

        private string GetSelectedFilterStatus()
        {
            string selected = cmbFilterStatus.SelectedItem as string;
            return string.IsNullOrEmpty(selected) || selected == "(All Statuses)" ? null : selected;
        }
    }
}
