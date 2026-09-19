using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class DashboardResult
    {
        public DashboardSummary Summary { get; set; }
        public List<LowStockItem> LowStock { get; set; }
        public List<RecentActivityItem> RecentActivity { get; set; }
        public DateTime RefreshedAt { get; set; }
    }

    /// <summary>
    /// Read-only dashboard data. Always queries the database (no caching).
    /// </summary>
    public class DashboardBLL
    {
        public const int LowStockListLimit = 10;
        public const int RecentActivityLimit = 10;

        public const string StatusLowStock = "Low Stock";
        public const string StatusOutOfStock = "Out of Stock";

        public DashboardResult Load()
        {
            try
            {
                DashboardSummary summary = DashboardDAL.GetSummary();
                List<LowStockItem> lowStock = DashboardDAL.GetLowStock(LowStockListLimit);
                foreach (LowStockItem item in lowStock)
                    item.Status = item.QtyInStock <= 0 ? StatusOutOfStock : StatusLowStock;

                List<RecentActivityItem> recent = DashboardDAL.GetRecentActivity(RecentActivityLimit)
                    .Select(ToActivityItem)
                    .ToList();

                return new DashboardResult
                {
                    Summary = summary,
                    LowStock = lowStock,
                    RecentActivity = recent,
                    RefreshedAt = DateTime.Now
                };
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load the dashboard. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load the dashboard. Please check your database connection and try again.", ex);
            }
        }

        private static RecentActivityItem ToActivityItem(RecentActivityRow row)
        {
            var item = new RecentActivityItem
            {
                ActivityType = row.ActivityType,
                ReferenceID = row.ReferenceID,
                ActivityDate = row.ActivityDate
            };

            switch (row.ActivityType)
            {
                case "Stock In":
                    item.Description = row.Subject + (string.IsNullOrEmpty(row.Party) ? " (no supplier)" : " from " + row.Party);
                    item.QtyOrAmount = "Qty " + row.Quantity;
                    break;
                case "Stock Out":
                    item.Description = row.Subject + (string.IsNullOrEmpty(row.Party) ? " (no customer)" : " to " + row.Party);
                    item.QtyOrAmount = "Qty " + row.Quantity;
                    break;
                default:
                    item.Description = (string.IsNullOrEmpty(row.Subject) ? "(no customer)" : row.Subject) + " - " + row.Status;
                    item.QtyOrAmount = row.Amount.ToString("N2");
                    break;
            }

            return item;
        }
    }
}
