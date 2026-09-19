using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class ucDashboard : UserControl
    {
        private readonly DashboardBLL _dashboardBLL = new DashboardBLL();
        private readonly Dictionary<string, Label> _cardValues = new Dictionary<string, Label>();

        public ucDashboard()
        {
            Font = Theme.BaseFont;
            InitializeComponent();
            Theme.Apply(this);
            Padding = new Padding(24, 16, 24, 12);
            lblTitle.Font = Theme.PageTitleFont;
            lblLowStockEmpty.TextAlign = ContentAlignment.TopCenter;
            lblLowStockEmpty.Padding = new Padding(0, 40, 0, 0);
            lblRecentEmpty.TextAlign = ContentAlignment.TopCenter;
            lblRecentEmpty.Padding = new Padding(0, 40, 0, 0);
            pnlTop.Resize += (s, e) => ArrangeTopRow();
            ArrangeTopRow();
            BuildCards();
            BuildLowStockColumns();
            BuildRecentColumns();
        }

        /// <summary>Re-queries the database and repaints everything.</summary>
        public void LoadData()
        {
            DashboardResult result;
            try
            {
                result = _dashboardBLL.Load();
            }
            catch (ApplicationException ex)
            {
                lblLastRefreshed.Text = ex.Message;
                lblLastRefreshed.ForeColor = Color.Firebrick;
                return;
            }

            DashboardSummary s = result.Summary;
            SetCard("Products", s.TotalProducts.ToString("N0"));
            SetCard("Categories", s.TotalCategories.ToString("N0"));
            SetCard("Units", s.TotalUnitsInStock.ToString("N0"));
            SetCard("LowStock", s.LowStockCount.ToString("N0"));
            SetCard("StockIn", s.StockInQuantity.ToString("N0"));
            SetCard("StockOut", s.StockOutQuantity.ToString("N0"));
            SetCard("Confirmed", s.ConfirmedOrders.ToString("N0"));
            SetCard("Pending", s.PendingOrders.ToString("N0"));
            _cardValues["LowStock"].ForeColor = s.LowStockCount > 0 ? Color.Firebrick : Color.FromArgb(0, 120, 215);

            dgvLowStock.DataSource = null;
            dgvLowStock.DataSource = result.LowStock;
            bool noLow = result.LowStock.Count == 0;
            dgvLowStock.Visible = !noLow;
            lblLowStockEmpty.Visible = noLow;
            lblLowStockHeader.Text = s.LowStockCount > result.LowStock.Count
                ? "Low Stock Products (showing " + result.LowStock.Count + " of " + s.LowStockCount + ")"
                : "Low Stock Products";

            dgvRecent.DataSource = null;
            dgvRecent.DataSource = result.RecentActivity;
            bool noRecent = result.RecentActivity.Count == 0;
            dgvRecent.Visible = !noRecent;
            lblRecentEmpty.Visible = noRecent;

            lblLastRefreshed.ForeColor = Color.DimGray;
            lblLastRefreshed.Text = "Last refreshed: " + result.RefreshedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // Refresh button pinned to the right edge; the "last refreshed" text sits immediately left of it.
        private void ArrangeTopRow()
        {
            btnRefresh.Height = 32;
            btnRefresh.Top = Math.Max(0, (pnlTop.ClientSize.Height - btnRefresh.Height) / 2);
            btnRefresh.Left = pnlTop.ClientSize.Width - btnRefresh.Width;
            lblLastRefreshed.Height = btnRefresh.Height;
            lblLastRefreshed.Top = btnRefresh.Top;
            lblLastRefreshed.Width = Math.Max(120, btnRefresh.Left - lblTitle.Right - 24);
            lblLastRefreshed.Left = btnRefresh.Left - 14 - lblLastRefreshed.Width;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void SetCard(string key, string value)
        {
            _cardValues[key].Text = value;
        }

        // ---- Layout helpers -------------------------------------------------------------------

        private void BuildCards()
        {
            AddCard("Products", "Total Products", 0, 0);
            AddCard("Categories", "Total Categories", 1, 0);
            AddCard("Units", "Total Units In Stock", 2, 0);
            AddCard("LowStock", "Low Stock Products", 3, 0);
            AddCard("StockIn", "Stock In Quantity", 0, 1);
            AddCard("StockOut", "Stock Out Quantity", 1, 1);
            AddCard("Confirmed", "Confirmed Orders", 2, 1);
            AddCard("Pending", "Pending Orders", 3, 1);
        }

        private void AddCard(string key, string caption, int column, int row)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 8),
                BackColor = Color.FromArgb(240, 244, 250),
                Padding = new Padding(10, 6, 6, 4)
            };

            var value = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                Text = "-",
                TextAlign = ContentAlignment.MiddleLeft
            };
            var title = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                ForeColor = Color.DimGray,
                Text = caption,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(value);
            card.Controls.Add(title);
            tlpCards.Controls.Add(card, column, row);
            _cardValues[key] = value;
        }

        private void BuildLowStockColumns()
        {
            AddColumn(dgvLowStock, "ProductName", "Product", 0);
            AddColumn(dgvLowStock, "CategoryName", "Category", 90);
            AddColumn(dgvLowStock, "QtyInStock", "Stock", 50, true);
            AddColumn(dgvLowStock, "ReorderLevel", "Reorder", 60, true);
            AddColumn(dgvLowStock, "Status", "Status", 85);
        }

        private void BuildRecentColumns()
        {
            AddColumn(dgvRecent, "ActivityType", "Type", 65);
            AddColumn(dgvRecent, "ReferenceID", "ID", 45, true);
            AddColumn(dgvRecent, "Description", "Description", 0);
            AddColumn(dgvRecent, "ActivityDate", "Date", 110, false, "yyyy-MM-dd HH:mm");
            AddColumn(dgvRecent, "QtyOrAmount", "Qty / Amount", 90, true);

            // Order rows carry a money amount (USD); show it with a $ sign. Display only - the value is unchanged.
            dgvRecent.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvRecent.Columns[e.ColumnIndex].DataPropertyName != "QtyOrAmount")
                    return;
                string text = e.Value as string;
                var activity = dgvRecent.Rows[e.RowIndex].Cells[0].Value as string;
                if (activity == "Order" && !string.IsNullOrEmpty(text) && !text.StartsWith("$"))
                {
                    e.Value = "$" + text;
                    e.FormattingApplied = true;
                }
            };
        }

        // width 0 => fill remaining space
        private static void AddColumn(DataGridView grid, string property, string header, int width,
            bool rightAlign = false, string format = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = "col" + property,
                DataPropertyName = property,
                HeaderText = header,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
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

            grid.Columns.Add(column);
        }
    }
}
