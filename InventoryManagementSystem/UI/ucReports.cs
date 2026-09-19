using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;
using InventoryManagementSystem.UI.Reporting;

namespace InventoryManagementSystem.UI
{
    public partial class ucReports : UserControl
    {
        private enum ReportType
        {
            Inventory,
            StockIn,
            StockOut,
            Orders
        }

        private const string AllStatuses = "(All)";
        private const string NoRecordsMessage = "No records found for the selected filters.";

        private readonly ReportBLL _reportBLL = new ReportBLL();
        private readonly CategoryBLL _categoryBLL = new CategoryBLL();
        private readonly ProductBLL _productBLL = new ProductBLL();
        private readonly SupplierBLL _supplierBLL = new SupplierBLL();
        private readonly CustomerBLL _customerBLL = new CustomerBLL();

        private ReportType _currentReport = ReportType.Inventory;

        // Presentation model of the report currently shown in the grid (null until a report has loaded successfully).
        private ReportDocument _currentDocument;

        public ucReports()
        {
            Font = Theme.BaseFont;
            InitializeComponent();
            Theme.Apply(this);
            flpFilters.Resize += (s, e) => AdjustFilterHeight();
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnApply.PerformClick();
                }
            };
        }

        private void ucReports_Load(object sender, EventArgs e)
        {
            LoadFilterOptions();
            rdoInventory.Checked = true;
        }

        private void ReportType_CheckedChanged(object sender, EventArgs e)
        {
            var radio = (RadioButton)sender;
            if (!radio.Checked)
                return;

            if (radio == rdoInventory) SwitchReport(ReportType.Inventory);
            else if (radio == rdoStockIn) SwitchReport(ReportType.StockIn);
            else if (radio == rdoStockOut) SwitchReport(ReportType.StockOut);
            else SwitchReport(ReportType.Orders);
        }

        private void SwitchReport(ReportType report)
        {
            _currentReport = report;
            SetCurrentDocument(null);

            grpCategory.Visible = report == ReportType.Inventory;
            grpProduct.Visible = report == ReportType.StockIn || report == ReportType.StockOut;
            grpSupplier.Visible = report == ReportType.StockIn;
            grpCustomer.Visible = report == ReportType.StockOut || report == ReportType.Orders;
            grpStatus.Visible = report == ReportType.Inventory || report == ReportType.Orders;
            grpDate.Visible = report != ReportType.Inventory;

            var statuses = new List<string> { AllStatuses };
            if (report == ReportType.Inventory)
            {
                lblStatus.Text = "Stock status:";
                statuses.Add(ReportBLL.InventoryStatusInStock);
                statuses.Add(ReportBLL.InventoryStatusLowStock);
            }
            else
            {
                lblStatus.Text = "Status:";
                statuses.Add("Pending");
                statuses.Add("Confirmed");
                statuses.Add("Cancelled");
            }
            cmbStatus.DataSource = statuses;

            AdjustFilterHeight();
            ConfigureColumns(report);
            ResetFilters();
            RunReport();
        }

        // Filter bar is exactly as tall as its (wrapping) filter groups plus the button row - no dead space.
        private void AdjustFilterHeight()
        {
            int width = flpFilters.ClientSize.Width;
            if (width <= 0)
                return;

            Size preferred = flpFilters.GetPreferredSize(new Size(width, 0));
            int height = pnlButtons.Height + Math.Max(preferred.Height, 50) + 6;
            if (pnlFilter.Height != height)
                pnlFilter.Height = height;
        }

        // ---- Filter options -------------------------------------------------------------------

        private void LoadFilterOptions()
        {
            try
            {
                var categories = new List<CategoryEntity> { new CategoryEntity { CategoryID = 0, CategoryName = "(All Categories)" } };
                categories.AddRange(_categoryBLL.GetAll());
                Bind(cmbCategory, categories, "CategoryName", "CategoryID");

                var products = new List<ProductEntity> { new ProductEntity { ProductID = 0, ProductName = "(All Products)" } };
                products.AddRange(_productBLL.GetAll());
                Bind(cmbProduct, products, "ProductName", "ProductID");

                var suppliers = new List<SupplierEntity> { new SupplierEntity { SupplierID = 0, SupplierName = "(All Suppliers)" } };
                suppliers.AddRange(_supplierBLL.GetAll());
                Bind(cmbSupplier, suppliers, "SupplierName", "SupplierID");

                var customers = new List<CustomerEntity> { new CustomerEntity { CustomerID = 0, CustomerName = "(All Customers)" } };
                customers.AddRange(_customerBLL.GetAll());
                Bind(cmbCustomer, customers, "CustomerName", "CustomerID");
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Filter Options", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Bind(ComboBox combo, object dataSource, string displayMember, string valueMember)
        {
            combo.DataSource = dataSource;
            combo.DisplayMember = displayMember;
            combo.ValueMember = valueMember;
        }

        private static int? SelectedId(ComboBox combo)
        {
            return combo.SelectedValue is int && (int)combo.SelectedValue > 0 ? (int?)(int)combo.SelectedValue : null;
        }

        // ---- Buttons --------------------------------------------------------------------------

        private void btnApply_Click(object sender, EventArgs e)
        {
            RunReport();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Re-read the dropdown lists (new products/customers/etc.) but keep the user's current filters.
            int? category = SelectedId(cmbCategory);
            int? product = SelectedId(cmbProduct);
            int? supplier = SelectedId(cmbSupplier);
            int? customer = SelectedId(cmbCustomer);

            LoadFilterOptions();

            RestoreSelection(cmbCategory, category);
            RestoreSelection(cmbProduct, product);
            RestoreSelection(cmbSupplier, supplier);
            RestoreSelection(cmbCustomer, customer);

            RunReport();
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            ResetFilters();
            RunReport();
        }

        private static void RestoreSelection(ComboBox combo, int? id)
        {
            if (id.HasValue)
                combo.SelectedValue = id.Value;
            if (combo.SelectedIndex < 0 && combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }

        private void chkDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            dtpFrom.Enabled = chkDateFilter.Checked;
            dtpTo.Enabled = chkDateFilter.Checked;
        }

        private void ResetFilters()
        {
            foreach (ComboBox combo in new[] { cmbCategory, cmbProduct, cmbSupplier, cmbCustomer, cmbStatus })
            {
                if (combo.Items.Count > 0)
                    combo.SelectedIndex = 0;
            }

            chkDateFilter.Checked = false;
            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Today;
            txtSearch.Clear();
        }

        // ---- Running reports ------------------------------------------------------------------

        private void RunReport()
        {
            DateTime? dateFrom = chkDateFilter.Checked && grpDate.Visible ? dtpFrom.Value.Date : (DateTime?)null;
            DateTime? dateTo = chkDateFilter.Checked && grpDate.Visible ? dtpTo.Value.Date : (DateTime?)null;

            if (dateFrom.HasValue && dateFrom.Value > dateTo.Value)
            {
                MessageBox.Show(this, "The 'From' date cannot be later than the 'To' date.", "Invalid Date Range",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string search = txtSearch.Text;
            string status = cmbStatus.SelectedItem as string;
            if (status == AllStatuses)
                status = null;

            string filters = DescribeFilters(dateFrom, dateTo, status, search);

            try
            {
                switch (_currentReport)
                {
                    case ReportType.Inventory:
                        ShowInventory(_reportBLL.GetInventoryReport(SelectedId(cmbCategory), status, search), filters);
                        break;
                    case ReportType.StockIn:
                        ShowStockIn(_reportBLL.GetStockInReport(SelectedId(cmbProduct), SelectedId(cmbSupplier),
                            dateFrom, dateTo, search), filters);
                        break;
                    case ReportType.StockOut:
                        ShowStockOut(_reportBLL.GetStockOutReport(SelectedId(cmbProduct), SelectedId(cmbCustomer),
                            dateFrom, dateTo, search), filters);
                        break;
                    default:
                        ShowOrders(_reportBLL.GetOrderReport(status, SelectedId(cmbCustomer), dateFrom, dateTo, search), filters);
                        break;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowInventory(InventoryReportResult result, string filters)
        {
            Display(result.Rows, ReportDocumentBuilder.FromInventory(result, filters));
        }

        private void ShowStockIn(StockInReportResult result, string filters)
        {
            Display(result.Rows, ReportDocumentBuilder.FromStockIn(result, filters));
        }

        private void ShowStockOut(StockOutReportResult result, string filters)
        {
            Display(result.Rows, ReportDocumentBuilder.FromStockOut(result, filters));
        }

        private void ShowOrders(OrderReportResult result, string filters)
        {
            Display(result.Rows, ReportDocumentBuilder.FromOrders(result, filters));
        }

        private void Display<T>(List<T> rows, ReportDocument document)
        {
            dgvReport.DataSource = null;
            dgvReport.DataSource = rows;

            bool empty = rows.Count == 0;
            lblSummary.Text = empty ? string.Empty : string.Join("     ", document.SummaryLines);
            lblMessage.Text = empty ? NoRecordsMessage : string.Empty;

            SetCurrentDocument(document);
        }

        private void SetCurrentDocument(ReportDocument document)
        {
            _currentDocument = document;
            btnPreview.Enabled = document != null;
        }

        // Human-readable description of the filters that produced the displayed report (only non-default ones).
        private string DescribeFilters(DateTime? dateFrom, DateTime? dateTo, string status, string search)
        {
            var parts = new List<string>();

            if (grpCategory.Visible && SelectedId(cmbCategory).HasValue)
                parts.Add("Category: " + cmbCategory.Text);
            if (grpProduct.Visible && SelectedId(cmbProduct).HasValue)
                parts.Add("Product: " + cmbProduct.Text);
            if (grpSupplier.Visible && SelectedId(cmbSupplier).HasValue)
                parts.Add("Supplier: " + cmbSupplier.Text);
            if (grpCustomer.Visible && SelectedId(cmbCustomer).HasValue)
                parts.Add("Customer: " + cmbCustomer.Text);
            if (status != null)
                parts.Add(lblStatus.Text.TrimEnd(':') + ": " + status);
            if (dateFrom.HasValue)
                parts.Add("Date: " + dateFrom.Value.ToString("yyyy-MM-dd") + " to " + dateTo.Value.ToString("yyyy-MM-dd"));
            if (!string.IsNullOrWhiteSpace(search))
                parts.Add("Search: \"" + search.Trim() + "\"");

            return parts.Count == 0 ? ReportDocumentBuilder.NoFilters : string.Join("; ", parts);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            // Uses the report already loaded through the BLL; no new query is made here.
            if (_currentDocument == null)
                return;

            using (var preview = new frmReportPreview(_currentDocument))
                preview.ShowDialog(this);
        }

        // ---- Grid -----------------------------------------------------------------------------

        private void ConfigureColumns(ReportType report)
        {
            dgvReport.DataSource = null;
            dgvReport.Columns.Clear();

            const string money = "N2";
            const string dateTime = "yyyy-MM-dd HH:mm";

            switch (report)
            {
                case ReportType.Inventory:
                    AddColumn("ProductID", "Product ID", 80, null, true);
                    AddColumn("ProductName", "Product Name", 220);
                    AddColumn("CategoryName", "Category", 150);
                    AddColumn("UnitPrice", "Unit Price (USD)", 140, money, true);
                    AddColumn("QtyInStock", "Current Stock", 100, null, true);
                    AddColumn("ReorderLevel", "Reorder Level", 100, null, true);
                    AddColumn("StockStatus", "Stock Status", 0);
                    break;
                case ReportType.StockIn:
                    AddColumn("StockInID", "Stock In ID", 80, null, true);
                    AddColumn("ProductName", "Product", 180);
                    AddColumn("SupplierName", "Supplier", 150);
                    AddColumn("Quantity", "Quantity", 80, null, true);
                    AddColumn("UnitCost", "Unit Cost (USD)", 140, money, true);
                    AddColumn("TotalCost", "Total Cost (USD)", 145, money, true);
                    AddColumn("DateIn", "Date In", 130, dateTime);
                    AddColumn("Notes", "Notes", 0);
                    break;
                case ReportType.StockOut:
                    AddColumn("StockOutID", "Stock Out ID", 90, null, true);
                    AddColumn("ProductName", "Product", 180);
                    AddColumn("CustomerName", "Customer", 150);
                    AddColumn("Quantity", "Quantity", 80, null, true);
                    AddColumn("UnitPrice", "Unit Price (USD)", 140, money, true);
                    AddColumn("TotalPrice", "Total Price (USD)", 145, money, true);
                    AddColumn("DateOut", "Date Out", 130, dateTime);
                    AddColumn("Notes", "Notes", 0);
                    break;
                default:
                    AddColumn("OrderID", "Order ID", 80, null, true);
                    AddColumn("CustomerName", "Customer", 200);
                    AddColumn("EmployeeName", "Employee", 180);
                    AddColumn("OrderDate", "Order Date", 140, dateTime);
                    AddColumn("TotalAmount", "Total Amount (USD)", 165, money, true);
                    AddColumn("Status", "Status", 0);
                    break;
            }
        }

        // width 0 => fill the remaining space
        private void AddColumn(string property, string header, int width, string format = null, bool rightAlign = false)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = "col" + property,
                DataPropertyName = property,
                HeaderText = header,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            if (width > 0)
                column.Width = width;
            else
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (format != null)
                column.DefaultCellStyle.Format = format;
            if (rightAlign)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            dgvReport.Columns.Add(column);
        }

        private void dgvReport_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (_currentReport != ReportType.Inventory || !dgvReport.Columns.Contains("colStockStatus"))
                return;

            foreach (DataGridViewRow row in dgvReport.Rows)
            {
                var item = row.DataBoundItem as InventoryReportRow;
                if (item != null && item.StockStatus != ReportBLL.InventoryStatusInStock)
                    row.Cells["colStockStatus"].Style.ForeColor = Color.Firebrick;
            }
        }
    }
}
