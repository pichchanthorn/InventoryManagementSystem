using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class ProductBLL
    {
        private const int MaxNameLength = 150;
        private const int MaxBarcodeLength = 100;
        private const int MaxDescriptionLength = 255;
        private const int SqlForeignKeyViolationNumber = 547;

        public List<ProductEntity> GetAll()
        {
            try
            {
                return ProductDAL.GetAll();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load products. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load products. Please check your database connection and try again.", ex);
            }
        }

        public ProductResult Add(ProductEntity product)
        {
            ProductResult validation = Validate(product, isUpdate: false);
            if (!validation.Success)
                return validation;

            string name = product.ProductName.Trim();
            string barcode = NormalizeOptional(product.Barcode);
            string description = NormalizeOptional(product.Description);

            try
            {
                if (barcode != null && ProductDAL.ExistsByBarcode(barcode, null))
                    return ProductResult.Fail("A product with this barcode already exists.");

                ProductDAL.Insert(new ProductEntity
                {
                    ProductName = name,
                    CategoryID = product.CategoryID,
                    UnitPrice = product.UnitPrice,
                    QtyInStock = product.QtyInStock,
                    Description = description,
                    Barcode = barcode,
                    ReorderLevel = product.ReorderLevel
                });
                return ProductResult.Ok("Product added successfully.");
            }
            catch (SqlException)
            {
                return ProductResult.Fail("Unable to save the product due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return ProductResult.Fail("Unable to save the product due to a database error. Please try again later.");
            }
        }

        public ProductResult Update(ProductEntity product)
        {
            if (product == null || product.ProductID <= 0)
                return ProductResult.Fail("Please select a product to update.");

            ProductResult validation = Validate(product, isUpdate: true);
            if (!validation.Success)
                return validation;

            string name = product.ProductName.Trim();
            string barcode = NormalizeOptional(product.Barcode);
            string description = NormalizeOptional(product.Description);

            try
            {
                if (barcode != null && ProductDAL.ExistsByBarcode(barcode, product.ProductID))
                    return ProductResult.Fail("A product with this barcode already exists.");

                ProductDAL.Update(new ProductEntity
                {
                    ProductID = product.ProductID,
                    ProductName = name,
                    CategoryID = product.CategoryID,
                    UnitPrice = product.UnitPrice,
                    Description = description,
                    Barcode = barcode,
                    ReorderLevel = product.ReorderLevel
                });
                return ProductResult.Ok("Product updated successfully.");
            }
            catch (SqlException)
            {
                return ProductResult.Fail("Unable to update the product due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return ProductResult.Fail("Unable to update the product due to a database error. Please try again later.");
            }
        }

        public ProductResult Delete(int productId)
        {
            if (productId <= 0)
                return ProductResult.Fail("Please select a product to delete.");

            try
            {
                ProductDAL.Delete(productId);
                return ProductResult.Ok("Product deleted successfully.");
            }
            catch (SqlException ex) when (ex.Number == SqlForeignKeyViolationNumber)
            {
                return ProductResult.Fail(
                    "This product cannot be deleted because it is referenced by inventory or sales history (stock in/out or orders).");
            }
            catch (SqlException)
            {
                return ProductResult.Fail("Unable to delete the product due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return ProductResult.Fail("Unable to delete the product due to a database error. Please try again later.");
            }
        }

        private static ProductResult Validate(ProductEntity product, bool isUpdate)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.ProductName))
                return ProductResult.Fail("Product name is required.");

            if (product.ProductName.Trim().Length > MaxNameLength)
                return ProductResult.Fail("Product name cannot exceed " + MaxNameLength + " characters.");

            if (product.CategoryID <= 0)
                return ProductResult.Fail("Please select a valid category.");

            try
            {
                bool categoryExists = CategoryDAL.GetAll().Any(c => c.CategoryID == product.CategoryID);
                if (!categoryExists)
                    return ProductResult.Fail("Please select a valid category.");
            }
            catch (SqlException)
            {
                return ProductResult.Fail("Unable to validate the category due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return ProductResult.Fail("Unable to validate the category due to a database error. Please try again later.");
            }

            if (product.UnitPrice < 0)
                return ProductResult.Fail("Unit price cannot be negative.");

            if (!isUpdate && product.QtyInStock < 0)
                return ProductResult.Fail("Initial stock cannot be negative.");

            if (product.ReorderLevel < 0)
                return ProductResult.Fail("Reorder level cannot be negative.");

            if (!string.IsNullOrWhiteSpace(product.Barcode) &&
                product.Barcode.Trim().Length > MaxBarcodeLength)
                return ProductResult.Fail("Barcode cannot exceed " + MaxBarcodeLength + " characters.");

            if (!string.IsNullOrWhiteSpace(product.Description) &&
                product.Description.Trim().Length > MaxDescriptionLength)
                return ProductResult.Fail("Description cannot exceed " + MaxDescriptionLength + " characters.");

            return ProductResult.Ok();
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
