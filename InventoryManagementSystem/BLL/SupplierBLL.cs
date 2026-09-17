using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class SupplierBLL
    {
        private const int MaxNameLength = 150;
        private const int MaxContactPersonLength = 150;
        private const int MaxPhoneLength = 30;
        private const int MaxEmailLength = 100;
        private const int MaxAddressLength = 255;

        public List<SupplierEntity> GetAll()
        {
            try
            {
                return SupplierDAL.GetAll();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load suppliers. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load suppliers. Please check your database connection and try again.", ex);
            }
        }

        public SupplierResult Add(SupplierEntity supplier)
        {
            SupplierResult validation = Validate(supplier);
            if (!validation.Success)
                return validation;

            var normalized = Normalize(supplier);

            try
            {
                SupplierDAL.Insert(normalized);
                return SupplierResult.Ok("Supplier added successfully.");
            }
            catch (SqlException)
            {
                return SupplierResult.Fail("Unable to save the supplier due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return SupplierResult.Fail("Unable to save the supplier due to a database error. Please try again later.");
            }
        }

        public SupplierResult Update(SupplierEntity supplier)
        {
            if (supplier == null || supplier.SupplierID <= 0)
                return SupplierResult.Fail("Please select a supplier to update.");

            SupplierResult validation = Validate(supplier);
            if (!validation.Success)
                return validation;

            var normalized = Normalize(supplier);
            normalized.SupplierID = supplier.SupplierID;

            try
            {
                SupplierDAL.Update(normalized);
                return SupplierResult.Ok("Supplier updated successfully.");
            }
            catch (SqlException)
            {
                return SupplierResult.Fail("Unable to update the supplier due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return SupplierResult.Fail("Unable to update the supplier due to a database error. Please try again later.");
            }
        }

        public SupplierResult Delete(int supplierId)
        {
            if (supplierId <= 0)
                return SupplierResult.Fail("Please select a supplier to delete.");

            try
            {
                SupplierDAL.Delete(supplierId);
                return SupplierResult.Ok("Supplier deleted successfully.");
            }
            catch (SqlException)
            {
                return SupplierResult.Fail("Unable to delete the supplier due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return SupplierResult.Fail("Unable to delete the supplier due to a database error. Please try again later.");
            }
        }

        private static SupplierResult Validate(SupplierEntity supplier)
        {
            if (supplier == null || string.IsNullOrWhiteSpace(supplier.SupplierName))
                return SupplierResult.Fail("Supplier name is required.");

            if (supplier.SupplierName.Trim().Length > MaxNameLength)
                return SupplierResult.Fail("Supplier name cannot exceed " + MaxNameLength + " characters.");

            if (!string.IsNullOrWhiteSpace(supplier.ContactPerson) &&
                supplier.ContactPerson.Trim().Length > MaxContactPersonLength)
                return SupplierResult.Fail("Contact person cannot exceed " + MaxContactPersonLength + " characters.");

            if (!string.IsNullOrWhiteSpace(supplier.Phone) &&
                supplier.Phone.Trim().Length > MaxPhoneLength)
                return SupplierResult.Fail("Phone cannot exceed " + MaxPhoneLength + " characters.");

            if (!string.IsNullOrWhiteSpace(supplier.Email) &&
                supplier.Email.Trim().Length > MaxEmailLength)
                return SupplierResult.Fail("Email cannot exceed " + MaxEmailLength + " characters.");

            if (!string.IsNullOrWhiteSpace(supplier.Address) &&
                supplier.Address.Trim().Length > MaxAddressLength)
                return SupplierResult.Fail("Address cannot exceed " + MaxAddressLength + " characters.");

            return SupplierResult.Ok();
        }

        private static SupplierEntity Normalize(SupplierEntity supplier)
        {
            return new SupplierEntity
            {
                SupplierName = supplier.SupplierName.Trim(),
                ContactPerson = NormalizeOptional(supplier.ContactPerson),
                Phone = NormalizeOptional(supplier.Phone),
                Email = NormalizeOptional(supplier.Email),
                Address = NormalizeOptional(supplier.Address)
            };
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
