using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    public static class CategoryDAL
    {
        public static List<CategoryEntity> GetAll()
        {
            const string sql = @"
                SELECT CategoryID, CategoryName, Description
                FROM Categories
                ORDER BY CategoryName ASC;";

            var categories = new List<CategoryEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    int descriptionOrdinal = reader.GetOrdinal("Description");

                    while (reader.Read())
                    {
                        categories.Add(new CategoryEntity
                        {
                            CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                            Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal)
                        });
                    }
                }
            }

            return categories;
        }

        public static void Insert(CategoryEntity category)
        {
            const string sql = @"
                INSERT INTO Categories (CategoryName, Description)
                VALUES (@CategoryName, @Description);";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.CategoryName;
                AddDescriptionParameter(command, category.Description);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Update(CategoryEntity category)
        {
            const string sql = @"
                UPDATE Categories
                SET CategoryName = @CategoryName,
                    Description = @Description
                WHERE CategoryID = @CategoryID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.CategoryName;
                AddDescriptionParameter(command, category.Description);
                command.Parameters.Add("@CategoryID", SqlDbType.Int).Value = category.CategoryID;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Delete(int categoryId)
        {
            const string sql = "DELETE FROM Categories WHERE CategoryID = @CategoryID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryId;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static bool ExistsByName(string categoryName, int? excludeCategoryId)
        {
            string sql = "SELECT COUNT(1) FROM Categories WHERE CategoryName = @CategoryName";
            if (excludeCategoryId.HasValue)
            {
                sql += " AND CategoryID <> @ExcludeCategoryID";
            }
            sql += ";";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = categoryName;
                if (excludeCategoryId.HasValue)
                {
                    command.Parameters.Add("@ExcludeCategoryID", SqlDbType.Int).Value = excludeCategoryId.Value;
                }

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private static void AddDescriptionParameter(SqlCommand command, string description)
        {
            SqlParameter parameter = command.Parameters.Add("@Description", SqlDbType.NVarChar, 255);
            parameter.Value = string.IsNullOrEmpty(description) ? (object)DBNull.Value : description;
        }
    }
}
