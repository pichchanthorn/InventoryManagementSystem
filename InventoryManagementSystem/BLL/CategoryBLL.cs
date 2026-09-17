using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class CategoryBLL
    {
        private const int MaxNameLength = 100;
        private const int MaxDescriptionLength = 255;
        private const int SqlForeignKeyViolationNumber = 547;

        public List<CategoryEntity> GetAll()
        {
            try
            {
                return CategoryDAL.GetAll();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load categories. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load categories. Please check your database connection and try again.", ex);
            }
        }

        public CategoryResult Add(CategoryEntity category)
        {
            CategoryResult validation = Validate(category);
            if (!validation.Success)
                return validation;

            string name = category.CategoryName.Trim();
            string description = NormalizeDescription(category.Description);

            try
            {
                if (CategoryDAL.ExistsByName(name, null))
                    return CategoryResult.Fail("A category with this name already exists.");

                CategoryDAL.Insert(new CategoryEntity { CategoryName = name, Description = description });
                return CategoryResult.Ok("Category added successfully.");
            }
            catch (SqlException)
            {
                return CategoryResult.Fail("Unable to save the category due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return CategoryResult.Fail("Unable to save the category due to a database error. Please try again later.");
            }
        }

        public CategoryResult Update(CategoryEntity category)
        {
            if (category == null || category.CategoryID <= 0)
                return CategoryResult.Fail("Please select a category to update.");

            CategoryResult validation = Validate(category);
            if (!validation.Success)
                return validation;

            string name = category.CategoryName.Trim();
            string description = NormalizeDescription(category.Description);

            try
            {
                if (CategoryDAL.ExistsByName(name, category.CategoryID))
                    return CategoryResult.Fail("A category with this name already exists.");

                CategoryDAL.Update(new CategoryEntity
                {
                    CategoryID = category.CategoryID,
                    CategoryName = name,
                    Description = description
                });
                return CategoryResult.Ok("Category updated successfully.");
            }
            catch (SqlException)
            {
                return CategoryResult.Fail("Unable to update the category due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return CategoryResult.Fail("Unable to update the category due to a database error. Please try again later.");
            }
        }

        public CategoryResult Delete(int categoryId)
        {
            if (categoryId <= 0)
                return CategoryResult.Fail("Please select a category to delete.");

            try
            {
                CategoryDAL.Delete(categoryId);
                return CategoryResult.Ok("Category deleted successfully.");
            }
            catch (SqlException ex) when (ex.Number == SqlForeignKeyViolationNumber)
            {
                return CategoryResult.Fail(
                    "This category cannot be deleted because it is being used by one or more products.");
            }
            catch (SqlException)
            {
                return CategoryResult.Fail("Unable to delete the category due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return CategoryResult.Fail("Unable to delete the category due to a database error. Please try again later.");
            }
        }

        private static CategoryResult Validate(CategoryEntity category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.CategoryName))
                return CategoryResult.Fail("Category name is required.");

            if (category.CategoryName.Trim().Length > MaxNameLength)
                return CategoryResult.Fail("Category name cannot exceed " + MaxNameLength + " characters.");

            if (!string.IsNullOrWhiteSpace(category.Description) &&
                category.Description.Trim().Length > MaxDescriptionLength)
                return CategoryResult.Fail("Description cannot exceed " + MaxDescriptionLength + " characters.");

            return CategoryResult.Ok();
        }

        private static string NormalizeDescription(string description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }
    }
}
