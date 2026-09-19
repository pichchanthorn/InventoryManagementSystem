using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class StockInBLL
    {
        private const int MaxNotesLength = 255;
        private const int SqlForeignKeyViolationNumber = 547;

        public List<StockInEntity> GetAll(int? productId = null, int? supplierId = null,
            DateTime? dateFrom = null, DateTime? dateTo = null, string searchText = null)
        {
            DateTime? dateToExclusive = dateTo.HasValue ? dateTo.Value.Date.AddDays(1) : (DateTime?)null;
            string normalizedSearch = NormalizeOptional(searchText);

            try
            {
                return StockInDAL.GetAll(productId, supplierId, dateFrom, dateToExclusive, normalizedSearch);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load stock in history. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load stock in history. Please check your database connection and try again.", ex);
            }
        }

        public StockInResult Add(StockInEntity stockIn)
        {
            StockInResult validation = Validate(stockIn);
            if (!validation.Success)
                return validation;

            int? supplierId = stockIn.SupplierID.HasValue && stockIn.SupplierID.Value > 0
                ? stockIn.SupplierID
                : null;
            string notes = NormalizeOptional(stockIn.Notes);

            try
            {
                ProductEntity product = ProductDAL.GetById(stockIn.ProductID);
                if (product == null)
                    return StockInResult.Fail("Selected product does not exist.");

                if (supplierId.HasValue && SupplierDAL.GetById(supplierId.Value) == null)
                    return StockInResult.Fail("Selected supplier does not exist.");
            }
            catch (SqlException)
            {
                return StockInResult.Fail("Unable to validate the product/supplier due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return StockInResult.Fail("Unable to validate the product/supplier due to a database error. Please try again later.");
            }

            decimal totalCost = Math.Round(stockIn.Quantity * stockIn.UnitCost, 2, MidpointRounding.AwayFromZero);

            try
            {
                StockInDAL.InsertWithStockUpdate(new StockInEntity
                {
                    ProductID = stockIn.ProductID,
                    SupplierID = supplierId,
                    Quantity = stockIn.Quantity,
                    UnitCost = stockIn.UnitCost,
                    TotalCost = totalCost,
                    Notes = notes
                });
                return StockInResult.Ok("Stock in recorded successfully.");
            }
            catch (SqlException ex) when (ex.Number == SqlForeignKeyViolationNumber)
            {
                return StockInResult.Fail("The selected product or supplier no longer exists.");
            }
            catch (SqlException)
            {
                return StockInResult.Fail("Unable to save the stock in record due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return StockInResult.Fail("Unable to save the stock in record due to a database error. Please try again later.");
            }
        }

        private static StockInResult Validate(StockInEntity stockIn)
        {
            if (stockIn == null || stockIn.ProductID <= 0)
                return StockInResult.Fail("Please select a product.");

            if (stockIn.Quantity <= 0)
                return StockInResult.Fail("Quantity must be greater than zero.");

            if (stockIn.UnitCost < 0)
                return StockInResult.Fail("Unit cost cannot be negative.");

            if (!string.IsNullOrWhiteSpace(stockIn.Notes) && stockIn.Notes.Trim().Length > MaxNotesLength)
                return StockInResult.Fail("Notes cannot exceed " + MaxNotesLength + " characters.");

            return StockInResult.Ok();
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
