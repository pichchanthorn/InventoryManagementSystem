using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class ucCustomers : UserControl
    {
        private readonly CustomerBLL _customerBLL = new CustomerBLL();
        private int _selectedCustomerId;
        private bool _suppressSelectionChanged;

        public ucCustomers()
        {
            Font = Theme.BaseFont;
            InitializeComponent();
            Theme.Apply(this);
            Theme.AttachEmptyState(dgvCustomers, "No customers found.");
        }

        private void ucCustomers_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            ClearForm();

            // A freshly bound DataGridView re-selects its first row once the page is first laid out (after Load),
            // which would silently put the first record into edit mode. Reset once more after that has happened.
            BeginInvoke(new Action(ClearForm));
        }

        private void LoadCustomers()
        {
            try
            {
                List<CustomerEntity> customers = _customerBLL.GetAll();
                dgvCustomers.DataSource = null;
                dgvCustomers.DataSource = customers;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Customers",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChanged)
                return;

            if (dgvCustomers.CurrentRow == null || dgvCustomers.CurrentRow.DataBoundItem == null)
                return;

            var customer = (CustomerEntity)dgvCustomers.CurrentRow.DataBoundItem;

            _selectedCustomerId = customer.CustomerID;
            txtCustomerName.Text = customer.CustomerName;
            txtPhone.Text = customer.Phone;
            txtEmail.Text = customer.Email;
            txtAddress.Text = customer.Address;

            SetEditMode(true);
        }

        /// <summary>Add mode: Add enabled, Update/Delete disabled. Edit mode (a row is selected): the reverse.</summary>
        private void SetEditMode(bool editing)
        {
            btnAdd.Enabled = !editing;
            btnUpdate.Enabled = editing;
            btnDelete.Enabled = editing;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            CustomerEntity customer = BuildCustomerFromForm();
            CustomerResult result = _customerBLL.Add(customer);
            HandleResult(result, "Unable to Add Customer");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerId <= 0)
            {
                MessageBox.Show(this, "Please select a customer to update.", "No Customer Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CustomerEntity customer = BuildCustomerFromForm();
            customer.CustomerID = _selectedCustomerId;

            CustomerResult result = _customerBLL.Update(customer);
            HandleResult(result, "Unable to Update Customer");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerId <= 0)
            {
                MessageBox.Show(this, "Please select a customer to delete.", "No Customer Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(this,
                "Are you sure you want to delete the selected customer?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            CustomerResult result = _customerBLL.Delete(_selectedCustomerId);
            HandleResult(result, "Unable to Delete Customer");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private CustomerEntity BuildCustomerFromForm()
        {
            return new CustomerEntity
            {
                CustomerID = _selectedCustomerId,
                CustomerName = txtCustomerName.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text,
                Address = txtAddress.Text
            };
        }

        private void HandleResult(CustomerResult result, string failureTitle)
        {
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCustomers();
                ClearForm();
            }
            else
            {
                MessageBox.Show(this, result.Message, failureTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearForm()
        {
            _suppressSelectionChanged = true;
            try
            {
                _selectedCustomerId = 0;
                txtCustomerName.Clear();
                txtPhone.Clear();
                txtEmail.Clear();
                txtAddress.Clear();
                dgvCustomers.ClearSelection();
                dgvCustomers.CurrentCell = null;
                SetEditMode(false);
            }
            finally
            {
                _suppressSelectionChanged = false;
            }

            txtCustomerName.Focus();
        }
    }
}
