using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class StockOutBLL
    {
        private const int MaxNotesLength = 255;
        private const int SqlForeignKeyViolationNumber = 547;

        public List<StockOutEntity> GetAll(int? productId = null, int? customerId = null,
            DateTime? dateFrom = null, DateTime? dateTo = null, string searchText = null)
        {
            DateTime? dateToExclusive = dateTo.HasValue ? dateTo.Value.Date.AddDays(1) : (DateTime?)null;
            string normalizedSearch = NormalizeOptional(searchText);

            try
            {
                return StockOutDAL.GetAll(productId, customerId, dateFrom, dateToExclusive, normalizedSearch);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load stock out history. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load stock out history. Please check your database connection and try again.", ex);
            }
        }

        public StockOutResult Add(StockOutEntity stockOut)
        {
            StockOutResult validation = Validate(stockOut);
            if (!validation.Success)
                return validation;

            int? customerId = stockOut.CustomerID.HasValue && stockOut.CustomerID.Value > 0
                ? stockOut.CustomerID
                : null;
            string notes = NormalizeOptional(stockOut.Notes);

            ProductEntity product;
            try
            {
                product = ProductDAL.GetById(stockOut.ProductID);
                if (product == null)
                    return StockOutResult.Fail("Selected product does not exist.");

                if (customerId.HasValue && CustomerDAL.GetById(customerId.Value) == null)
                    return StockOutResult.Fail("Selected customer does not exist.");
            }
            catch (SqlException)
            {
                return StockOutResult.Fail("Unable to validate the product/customer due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return StockOutResult.Fail("Unable to validate the product/customer due to a database error. Please try again later.");
            }

            decimal totalPrice = Math.Round(stockOut.Quantity * stockOut.UnitPrice, 2, MidpointRounding.AwayFromZero);

            try
            {
                bool deducted = StockOutDAL.DeductStockAndInsert(new StockOutEntity
                {
                    ProductID = stockOut.ProductID,
                    CustomerID = customerId,
                    Quantity = stockOut.Quantity,
                    UnitPrice = stockOut.UnitPrice,
                    TotalPrice = totalPrice,
                    Notes = notes
                });

                if (!deducted)
                {
                    // Re-read for a friendlier message only; the atomic UPDATE's guarded
                    // WHERE clause (not this read) is what actually prevented overselling.
                    ProductEntity current = ProductDAL.GetById(stockOut.ProductID);
                    int available = current != null ? current.QtyInStock : 0;
                    return StockOutResult.Fail(
                        "Insufficient stock: only " + available + " unit(s) available for this product.");
                }

                return StockOutResult.Ok("Stock out recorded successfully.");
            }
            catch (SqlException ex) when (ex.Number == SqlForeignKeyViolationNumber)
            {
                return StockOutResult.Fail("The selected product or customer no longer exists.");
            }
            catch (SqlException)
            {
                return StockOutResult.Fail("Unable to save the stock out record due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return StockOutResult.Fail("Unable to save the stock out record due to a database error. Please try again later.");
            }
        }

        private static StockOutResult Validate(StockOutEntity stockOut)
        {
            if (stockOut == null || stockOut.ProductID <= 0)
                return StockOutResult.Fail("Please select a product.");

            if (stockOut.Quantity <= 0)
                return StockOutResult.Fail("Quantity must be greater than zero.");

            if (stockOut.UnitPrice < 0)
                return StockOutResult.Fail("Unit value cannot be negative.");

            if (!string.IsNullOrWhiteSpace(stockOut.Notes) && stockOut.Notes.Trim().Length > MaxNotesLength)
                return StockOutResult.Fail("Notes cannot exceed " + MaxNotesLength + " characters.");

            return StockOutResult.Ok();
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
