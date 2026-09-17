using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class CustomerBLL
    {
        private const int MaxNameLength = 150;
        private const int MaxPhoneLength = 30;
        private const int MaxEmailLength = 100;
        private const int MaxAddressLength = 255;

        public List<CustomerEntity> GetAll()
        {
            try
            {
                return CustomerDAL.GetAll();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load customers. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load customers. Please check your database connection and try again.", ex);
            }
        }

        public CustomerResult Add(CustomerEntity customer)
        {
            CustomerResult validation = Validate(customer);
            if (!validation.Success)
                return validation;

            var normalized = Normalize(customer);

            try
            {
                CustomerDAL.Insert(normalized);
                return CustomerResult.Ok("Customer added successfully.");
            }
            catch (SqlException)
            {
                return CustomerResult.Fail("Unable to save the customer due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return CustomerResult.Fail("Unable to save the customer due to a database error. Please try again later.");
            }
        }

        public CustomerResult Update(CustomerEntity customer)
        {
            if (customer == null || customer.CustomerID <= 0)
                return CustomerResult.Fail("Please select a customer to update.");

            CustomerResult validation = Validate(customer);
            if (!validation.Success)
                return validation;

            var normalized = Normalize(customer);
            normalized.CustomerID = customer.CustomerID;

            try
            {
                CustomerDAL.Update(normalized);
                return CustomerResult.Ok("Customer updated successfully.");
            }
            catch (SqlException)
            {
                return CustomerResult.Fail("Unable to update the customer due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return CustomerResult.Fail("Unable to update the customer due to a database error. Please try again later.");
            }
        }

        public CustomerResult Delete(int customerId)
        {
            if (customerId <= 0)
                return CustomerResult.Fail("Please select a customer to delete.");

            try
            {
                CustomerDAL.Delete(customerId);
                return CustomerResult.Ok("Customer deleted successfully.");
            }
            catch (SqlException)
            {
                return CustomerResult.Fail("Unable to delete the customer due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return CustomerResult.Fail("Unable to delete the customer due to a database error. Please try again later.");
            }
        }

        private static CustomerResult Validate(CustomerEntity customer)
        {
            if (customer == null || string.IsNullOrWhiteSpace(customer.CustomerName))
                return CustomerResult.Fail("Customer name is required.");

            if (customer.CustomerName.Trim().Length > MaxNameLength)
                return CustomerResult.Fail("Customer name cannot exceed " + MaxNameLength + " characters.");

            if (!string.IsNullOrWhiteSpace(customer.Phone) &&
                customer.Phone.Trim().Length > MaxPhoneLength)
                return CustomerResult.Fail("Phone cannot exceed " + MaxPhoneLength + " characters.");

            if (!string.IsNullOrWhiteSpace(customer.Email) &&
                customer.Email.Trim().Length > MaxEmailLength)
                return CustomerResult.Fail("Email cannot exceed " + MaxEmailLength + " characters.");

            if (!string.IsNullOrWhiteSpace(customer.Address) &&
                customer.Address.Trim().Length > MaxAddressLength)
                return CustomerResult.Fail("Address cannot exceed " + MaxAddressLength + " characters.");

            return CustomerResult.Ok();
        }

        private static CustomerEntity Normalize(CustomerEntity customer)
        {
            return new CustomerEntity
            {
                CustomerName = customer.CustomerName.Trim(),
                Phone = NormalizeOptional(customer.Phone),
                Email = NormalizeOptional(customer.Email),
                Address = NormalizeOptional(customer.Address)
            };
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
