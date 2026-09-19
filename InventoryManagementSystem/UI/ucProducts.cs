using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class ucProducts : UserControl
    {
        private readonly ProductBLL _productBLL = new ProductBLL();
        private readonly CategoryBLL _categoryBLL = new CategoryBLL();
        private int _selectedProductId;
        private bool _suppressSelectionChanged;

        public ucProducts()
        {
            Font = Theme.BaseFont;
            InitializeComponent();
            Theme.Apply(this);
            Theme.AttachEmptyState(dgvProducts, "No products found.");
        }

        private void ucProducts_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadProducts();
            ClearForm();

            // A freshly bound DataGridView re-selects its first row once the page is first laid out (after Load),
            // which would silently put the first product into edit mode. Reset once more after that has happened.
            BeginInvoke(new Action(ClearForm));
        }

        private void LoadCategories()
        {
            try
            {
                List<CategoryEntity> categories = _categoryBLL.GetAll();
                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Categories",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                List<ProductEntity> products = _productBLL.GetAll();
                dgvProducts.DataSource = null;
                dgvProducts.DataSource = products;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Products",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChanged)
                return;

            if (dgvProducts.CurrentRow == null || dgvProducts.CurrentRow.DataBoundItem == null)
                return;

            var product = (ProductEntity)dgvProducts.CurrentRow.DataBoundItem;

            _selectedProductId = product.ProductID;
            txtProductName.Text = product.ProductName;
            cmbCategory.SelectedValue = product.CategoryID;
            txtUnitPrice.Text = product.UnitPrice.ToString("0.##");
            txtInitialStock.Text = product.QtyInStock.ToString();
            SetEditMode(true);
            txtBarcode.Text = product.Barcode;
            txtReorderLevel.Text = product.ReorderLevel.ToString();
            txtDescription.Text = product.Description;
        }

        /// <summary>
        /// Add mode: "Initial Stock" is typed in and only used for the new row. Edit mode: the same box shows
        /// the product's "Current Stock" read-only (ProductDAL.Update never writes QtyInStock; stock only moves
        /// through Stock In / Stock Out / Orders).
        /// </summary>
        private void SetEditMode(bool editing)
        {
            lblInitialStock.Text = editing ? "Current Stock:" : "Initial Stock:";
            txtInitialStock.ReadOnly = editing;
            btnAdd.Enabled = !editing;
            btnUpdate.Enabled = editing;
            btnDelete.Enabled = editing;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ProductEntity product;
            string parseError;
            if (!TryBuildProductFromForm(out product, out parseError))
            {
                MessageBox.Show(this, parseError, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProductResult result = _productBLL.Add(product);
            HandleResult(result, "Unable to Add Product");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedProductId <= 0)
            {
                MessageBox.Show(this, "Please select a product to update.", "No Product Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProductEntity product;
            string parseError;
            if (!TryBuildProductFromForm(out product, out parseError))
            {
                MessageBox.Show(this, parseError, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProductResult result = _productBLL.Update(product);
            HandleResult(result, "Unable to Update Product");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedProductId <= 0)
            {
                MessageBox.Show(this, "Please select a product to delete.", "No Product Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(this,
                "Are you sure you want to delete the selected product?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            ProductResult result = _productBLL.Delete(_selectedProductId);
            HandleResult(result, "Unable to Delete Product");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private bool TryBuildProductFromForm(out ProductEntity product, out string error)
        {
            product = null;
            error = null;

            decimal unitPrice;
            if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice))
            {
                error = "Unit price must be a valid number.";
                return false;
            }

            int initialStock;
            if (!int.TryParse(txtInitialStock.Text.Trim(), out initialStock))
            {
                error = "Initial stock must be a whole number.";
                return false;
            }

            int reorderLevel;
            if (!int.TryParse(txtReorderLevel.Text.Trim(), out reorderLevel))
            {
                error = "Reorder level must be a whole number.";
                return false;
            }

            int categoryId = cmbCategory.SelectedValue is int ? (int)cmbCategory.SelectedValue : 0;

            product = new ProductEntity
            {
                ProductID = _selectedProductId,
                ProductName = txtProductName.Text,
                CategoryID = categoryId,
                UnitPrice = unitPrice,
                QtyInStock = initialStock,
                Barcode = txtBarcode.Text,
                ReorderLevel = reorderLevel,
                Description = txtDescription.Text
            };
            return true;
        }

        private void HandleResult(ProductResult result, string failureTitle)
        {
            if (result.Success)
            {
                MessageBox.Show(this, result.Message, "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadProducts();
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
                _selectedProductId = 0;
                txtProductName.Clear();
                cmbCategory.SelectedIndex = -1;
                txtUnitPrice.Clear();
                SetEditMode(false);
                txtInitialStock.Text = "0";
                txtBarcode.Clear();
                txtReorderLevel.Text = "0";
                txtDescription.Clear();
                dgvProducts.ClearSelection();
                dgvProducts.CurrentCell = null;
            }
            finally
            {
                _suppressSelectionChanged = false;
            }

            txtProductName.Focus();
        }
    }
}
