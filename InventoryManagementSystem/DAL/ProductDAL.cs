using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    public static class ProductDAL
    {
        private const string SelectColumns = @"
            p.ProductID, p.ProductName, p.CategoryID, p.UnitPrice, p.QtyInStock,
            p.Description, p.Barcode, p.ReorderLevel, p.CreatedAt, p.UpdatedAt,
            c.CategoryName";

        private const string FromJoin = @"
            FROM Products p
            INNER JOIN Categories c ON p.CategoryID = c.CategoryID";

        public static List<ProductEntity> GetAll()
        {
            string sql = "SELECT " + SelectColumns + " " + FromJoin + " ORDER BY p.ProductName ASC;";

            var products = new List<ProductEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(ReadProduct(reader));
                    }
                }
            }

            return products;
        }

        public static ProductEntity GetById(int productId)
        {
            string sql = "SELECT " + SelectColumns + " " + FromJoin + " WHERE p.ProductID = @ProductID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productId;

                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                        return null;

                    return ReadProduct(reader);
                }
            }
        }

        public static void Insert(ProductEntity product)
        {
            const string sql = @"
                INSERT INTO Products (ProductName, CategoryID, UnitPrice, QtyInStock, Description, Barcode, ReorderLevel, CreatedAt, UpdatedAt)
                VALUES (@ProductName, @CategoryID, @UnitPrice, @QtyInStock, @Description, @Barcode, @ReorderLevel, SYSDATETIME(), SYSDATETIME());";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = product.ProductName;
                command.Parameters.Add("@CategoryID", SqlDbType.Int).Value = product.CategoryID;
                AddUnitPriceParameter(command, product.UnitPrice);
                command.Parameters.Add("@QtyInStock", SqlDbType.Int).Value = product.QtyInStock;
                AddDescriptionParameter(command, product.Description);
                AddBarcodeParameter(command, product.Barcode);
                command.Parameters.Add("@ReorderLevel", SqlDbType.Int).Value = product.ReorderLevel;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Update(ProductEntity product)
        {
            // QtyInStock is intentionally NOT updated here. Stock quantity is owned by
            // Stock In / Stock Out / Order workflows, not by normal product editing.
            const string sql = @"
                UPDATE Products
                SET ProductName = @ProductName,
                    CategoryID = @CategoryID,
                    UnitPrice = @UnitPrice,
                    Description = @Description,
                    Barcode = @Barcode,
                    ReorderLevel = @ReorderLevel,
                    UpdatedAt = SYSDATETIME()
                WHERE ProductID = @ProductID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = product.ProductName;
                command.Parameters.Add("@CategoryID", SqlDbType.Int).Value = product.CategoryID;
                AddUnitPriceParameter(command, product.UnitPrice);
                AddDescriptionParameter(command, product.Description);
                AddBarcodeParameter(command, product.Barcode);
                command.Parameters.Add("@ReorderLevel", SqlDbType.Int).Value = product.ReorderLevel;
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = product.ProductID;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Delete(int productId)
        {
            const string sql = "DELETE FROM Products WHERE ProductID = @ProductID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productId;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static bool ExistsByBarcode(string barcode, int? excludeProductId)
        {
            string sql = "SELECT COUNT(1) FROM Products WHERE Barcode = @Barcode";
            if (excludeProductId.HasValue)
            {
                sql += " AND ProductID <> @ExcludeProductID";
            }
            sql += ";";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Barcode", SqlDbType.NVarChar, 100).Value = barcode;
                if (excludeProductId.HasValue)
                {
                    command.Parameters.Add("@ExcludeProductID", SqlDbType.Int).Value = excludeProductId.Value;
                }

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private static ProductEntity ReadProduct(SqlDataReader reader)
        {
            int descriptionOrdinal = reader.GetOrdinal("Description");
            int barcodeOrdinal = reader.GetOrdinal("Barcode");

            return new ProductEntity
            {
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                QtyInStock = reader.GetInt32(reader.GetOrdinal("QtyInStock")),
                Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                Barcode = reader.IsDBNull(barcodeOrdinal) ? null : reader.GetString(barcodeOrdinal),
                ReorderLevel = reader.GetInt32(reader.GetOrdinal("ReorderLevel")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
            };
        }

        private static void AddUnitPriceParameter(SqlCommand command, decimal unitPrice)
        {
            SqlParameter parameter = command.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
            parameter.Precision = 12;
            parameter.Scale = 2;
            parameter.Value = unitPrice;
        }

        private static void AddDescriptionParameter(SqlCommand command, string description)
        {
            SqlParameter parameter = command.Parameters.Add("@Description", SqlDbType.NVarChar, 255);
            parameter.Value = string.IsNullOrEmpty(description) ? (object)DBNull.Value : description;
        }

        private static void AddBarcodeParameter(SqlCommand command, string barcode)
        {
            SqlParameter parameter = command.Parameters.Add("@Barcode", SqlDbType.NVarChar, 100);
            parameter.Value = string.IsNullOrEmpty(barcode) ? (object)DBNull.Value : barcode;
        }
    }
}
