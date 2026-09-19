using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    /// <summary>
    /// Read-only review dialog for a single order. Header values come from the OrderEntity
    /// already loaded into the Order History grid (itself read from SQL Server); line items
    /// are always re-read from OrderDetails via OrderBLL.GetDetails, never recomputed from the
    /// current Product.UnitPrice, so historical orders stay accurate even if prices change
    /// later. Opening this dialog never writes to the database.
    /// </summary>
    public partial class frmOrderDetails : Form
    {
        private readonly OrderBLL _orderBLL = new OrderBLL();
        private readonly OrderEntity _order;

        public frmOrderDetails(OrderEntity order)
        {
            InitializeComponent();
            _order = order;
        }

        private void frmOrderDetails_Load(object sender, EventArgs e)
        {
            if (_order == null)
            {
                MessageBox.Show(this, "No order was selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            txtOrderId.Text = _order.OrderID.ToString();
            txtStatus.Text = _order.Status;
            txtOrderDate.Text = _order.OrderDate.ToString("yyyy-MM-dd HH:mm");
            txtCustomerName.Text = string.IsNullOrEmpty(_order.CustomerName) ? "(No Customer)" : _order.CustomerName;
            txtEmployeeName.Text = string.IsNullOrEmpty(_order.EmployeeName) ? "(No Employee)" : _order.EmployeeName;
            txtTotalAmount.Text = _order.TotalAmount.ToString("0.00");

            LoadItems();
        }

        private void LoadItems()
        {
            try
            {
                List<OrderDetailEntity> details = _orderBLL.GetDetails(_order.OrderID);
                dgvItems.DataSource = null;
                dgvItems.DataSource = details;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Loading Order Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
