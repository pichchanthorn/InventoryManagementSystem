using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class ucSuppliers : UserControl
    {
        private readonly SupplierBLL _supplierBLL = new SupplierBLL();
        private int _selectedSupplierId;
        private bool _suppressSelectionChanged;

        public ucSuppliers()
        {
            Font = Theme.BaseFont;
            InitializeComponent();
            Theme.Apply(this);
            Theme.AttachEmptyState(dgvSuppliers, "No suppliers found.");
        }

        private void ucSuppliers_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            ClearForm();

            // A freshly bound DataGridView re-selects its first row once the page is first laid out (after Load),
            // which would silently put the first record into edit mode. Reset once more after that has happened.
            BeginInvoke(new Action(ClearForm));
        }

        private void LoadSuppliers()
        {
            try
            {
                List<SupplierEntity> suppliers = _supplierBLL.GetAll();
                dgvSuppliers.DataSource = null;
                dgvSuppliers.DataSource = suppliers;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Suppliers",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChanged)
                return;

            if (dgvSuppliers.CurrentRow == null || dgvSuppliers.CurrentRow.DataBoundItem == null)
                return;

            var supplier = (SupplierEntity)dgvSuppliers.CurrentRow.DataBoundItem;

            _selectedSupplierId = supplier.SupplierID;
            txtSupplierName.Text = supplier.SupplierName;
            txtContactPerson.Text = supplier.ContactPerson;
            txtPhone.Text = supplier.Phone;
            txtEmail.Text = supplier.Email;
            txtAddress.Text = supplier.Address;

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
            SupplierEntity supplier = BuildSupplierFromForm();
            SupplierResult result = _supplierBLL.Add(supplier);
            HandleResult(result, "Unable to Add Supplier");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedSupplierId <= 0)
            {
                MessageBox.Show(this, "Please select a supplier to update.", "No Supplier Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SupplierEntity supplier = BuildSupplierFromForm();
            supplier.SupplierID = _selectedSupplierId;

            SupplierResult result = _supplierBLL.Update(supplier);
            HandleResult(result, "Unable to Update Supplier");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedSupplierId <= 0)
            {
                MessageBox.Show(this, "Please select a supplier to delete.", "No Supplier Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(this,
                "Are you sure you want to delete the selected supplier?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            SupplierResult result = _supplierBLL.Delete(_selectedSupplierId);
            HandleResult(result, "Unable to Delete Supplier");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private SupplierEntity BuildSupplierFromForm()
        {
            return new SupplierEntity
            {
                SupplierID = _selectedSupplierId,
                SupplierName = txtSupplierName.Text,
                ContactPerson = txtContactPerson.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text,
                Address = txtAddress.Text
            };
        }

        private void HandleResult(SupplierResult result, string failureTitle)
        {
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSuppliers();
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
                _selectedSupplierId = 0;
                txtSupplierName.Clear();
                txtContactPerson.Clear();
                txtPhone.Clear();
                txtEmail.Clear();
                txtAddress.Clear();
                dgvSuppliers.ClearSelection();
                dgvSuppliers.CurrentCell = null;
                SetEditMode(false);
            }
            finally
            {
                _suppressSelectionChanged = false;
            }

            txtSupplierName.Focus();
        }
    }
}
