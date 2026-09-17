using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class frmCategories : Form
    {
        private readonly CategoryBLL _categoryBLL = new CategoryBLL();
        private int _selectedCategoryId;

        public frmCategories()
        {
            InitializeComponent();
        }

        private void frmCategories_Load(object sender, EventArgs e)
        {
            LoadCategories();
            ClearForm();
        }

        private void LoadCategories()
        {
            try
            {
                List<CategoryEntity> categories = _categoryBLL.GetAll();
                dgvCategories.DataSource = null;
                dgvCategories.DataSource = categories;
                ConfigureGridColumns();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Categories",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridColumns()
        {
            if (dgvCategories.Columns.Contains("CategoryID"))
            {
                dgvCategories.Columns["CategoryID"].HeaderText = "ID";
                dgvCategories.Columns["CategoryID"].Width = 60;
            }

            if (dgvCategories.Columns.Contains("CategoryName"))
            {
                dgvCategories.Columns["CategoryName"].HeaderText = "Category Name";
                dgvCategories.Columns["CategoryName"].Width = 220;
            }

            if (dgvCategories.Columns.Contains("Description"))
            {
                dgvCategories.Columns["Description"].HeaderText = "Description";
                dgvCategories.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void dgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null || dgvCategories.CurrentRow.DataBoundItem == null)
                return;

            var category = (CategoryEntity)dgvCategories.CurrentRow.DataBoundItem;

            _selectedCategoryId = category.CategoryID;
            txtCategoryName.Text = category.CategoryName;
            txtDescription.Text = category.Description;

            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var category = new CategoryEntity
            {
                CategoryName = txtCategoryName.Text,
                Description = txtDescription.Text
            };

            CategoryResult result = _categoryBLL.Add(category);
            HandleResult(result, "Unable to Add Category");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedCategoryId <= 0)
            {
                MessageBox.Show(this, "Please select a category to update.", "No Category Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var category = new CategoryEntity
            {
                CategoryID = _selectedCategoryId,
                CategoryName = txtCategoryName.Text,
                Description = txtDescription.Text
            };

            CategoryResult result = _categoryBLL.Update(category);
            HandleResult(result, "Unable to Update Category");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedCategoryId <= 0)
            {
                MessageBox.Show(this, "Please select a category to delete.", "No Category Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(this,
                "Are you sure you want to delete the selected category?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            CategoryResult result = _categoryBLL.Delete(_selectedCategoryId);
            HandleResult(result, "Unable to Delete Category");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void HandleResult(CategoryResult result, string failureTitle)
        {
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCategories();
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
            _selectedCategoryId = 0;
            txtCategoryName.Clear();
            txtDescription.Clear();
            dgvCategories.ClearSelection();
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            txtCategoryName.Focus();
        }
    }
}
