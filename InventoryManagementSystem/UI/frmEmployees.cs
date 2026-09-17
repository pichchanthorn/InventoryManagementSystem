using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class frmEmployees : Form
    {
        private readonly EmployeeBLL _employeeBLL = new EmployeeBLL();
        private int _selectedEmployeeId;
        private bool _suppressSelectionChanged;

        public frmEmployees()
        {
            InitializeComponent();
        }

        private void frmEmployees_Load(object sender, EventArgs e)
        {
            LoadEmployees();
            ClearForm();
        }

        private void LoadEmployees()
        {
            try
            {
                List<EmployeeEntity> employees = _employeeBLL.GetAll();
                dgvEmployees.DataSource = null;
                dgvEmployees.DataSource = employees;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Employees",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChanged)
                return;

            if (dgvEmployees.CurrentRow == null || dgvEmployees.CurrentRow.DataBoundItem == null)
                return;

            var employee = (EmployeeEntity)dgvEmployees.CurrentRow.DataBoundItem;

            _selectedEmployeeId = employee.EmployeeID;
            txtEmployeeName.Text = employee.EmployeeName;
            txtGender.Text = employee.Gender;
            txtPhone.Text = employee.Phone;
            txtEmail.Text = employee.Email;
            txtAddress.Text = employee.Address;

            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EmployeeEntity employee = BuildEmployeeFromForm();
            EmployeeResult result = _employeeBLL.Add(employee);
            HandleResult(result, "Unable to Add Employee");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedEmployeeId <= 0)
            {
                MessageBox.Show(this, "Please select an employee to update.", "No Employee Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            EmployeeEntity employee = BuildEmployeeFromForm();
            employee.EmployeeID = _selectedEmployeeId;

            EmployeeResult result = _employeeBLL.Update(employee);
            HandleResult(result, "Unable to Update Employee");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedEmployeeId <= 0)
            {
                MessageBox.Show(this, "Please select an employee to delete.", "No Employee Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(this,
                "Are you sure you want to delete the selected employee?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            EmployeeResult result = _employeeBLL.Delete(_selectedEmployeeId);
            HandleResult(result, "Unable to Delete Employee");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private EmployeeEntity BuildEmployeeFromForm()
        {
            return new EmployeeEntity
            {
                EmployeeID = _selectedEmployeeId,
                EmployeeName = txtEmployeeName.Text,
                Gender = txtGender.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text,
                Address = txtAddress.Text
            };
        }

        private void HandleResult(EmployeeResult result, string failureTitle)
        {
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEmployees();
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
                _selectedEmployeeId = 0;
                txtEmployeeName.Clear();
                txtGender.Clear();
                txtPhone.Clear();
                txtEmail.Clear();
                txtAddress.Clear();
                dgvEmployees.ClearSelection();
                dgvEmployees.CurrentCell = null;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
            finally
            {
                _suppressSelectionChanged = false;
            }

            txtEmployeeName.Focus();
        }
    }
}
