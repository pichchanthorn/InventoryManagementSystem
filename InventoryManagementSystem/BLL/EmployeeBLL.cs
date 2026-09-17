using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class EmployeeBLL
    {
        private const int MaxNameLength = 150;
        private const int MaxGenderLength = 20;
        private const int MaxPhoneLength = 30;
        private const int MaxEmailLength = 100;
        private const int MaxAddressLength = 255;

        public List<EmployeeEntity> GetAll()
        {
            try
            {
                return EmployeeDAL.GetAll();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load employees. Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load employees. Please check your database connection and try again.", ex);
            }
        }

        public EmployeeResult Add(EmployeeEntity employee)
        {
            EmployeeResult validation = Validate(employee);
            if (!validation.Success)
                return validation;

            var normalized = Normalize(employee);

            try
            {
                EmployeeDAL.Insert(normalized);
                return EmployeeResult.Ok("Employee added successfully.");
            }
            catch (SqlException)
            {
                return EmployeeResult.Fail("Unable to save the employee due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return EmployeeResult.Fail("Unable to save the employee due to a database error. Please try again later.");
            }
        }

        public EmployeeResult Update(EmployeeEntity employee)
        {
            if (employee == null || employee.EmployeeID <= 0)
                return EmployeeResult.Fail("Please select an employee to update.");

            EmployeeResult validation = Validate(employee);
            if (!validation.Success)
                return validation;

            var normalized = Normalize(employee);
            normalized.EmployeeID = employee.EmployeeID;

            try
            {
                EmployeeDAL.Update(normalized);
                return EmployeeResult.Ok("Employee updated successfully.");
            }
            catch (SqlException)
            {
                return EmployeeResult.Fail("Unable to update the employee due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return EmployeeResult.Fail("Unable to update the employee due to a database error. Please try again later.");
            }
        }

        public EmployeeResult Delete(int employeeId)
        {
            if (employeeId <= 0)
                return EmployeeResult.Fail("Please select an employee to delete.");

            try
            {
                EmployeeDAL.Delete(employeeId);
                return EmployeeResult.Ok("Employee deleted successfully.");
            }
            catch (SqlException)
            {
                return EmployeeResult.Fail("Unable to delete the employee due to a database error. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return EmployeeResult.Fail("Unable to delete the employee due to a database error. Please try again later.");
            }
        }

        private static EmployeeResult Validate(EmployeeEntity employee)
        {
            if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeName))
                return EmployeeResult.Fail("Employee name is required.");

            if (employee.EmployeeName.Trim().Length > MaxNameLength)
                return EmployeeResult.Fail("Employee name cannot exceed " + MaxNameLength + " characters.");

            if (!string.IsNullOrWhiteSpace(employee.Gender) &&
                employee.Gender.Trim().Length > MaxGenderLength)
                return EmployeeResult.Fail("Gender cannot exceed " + MaxGenderLength + " characters.");

            if (!string.IsNullOrWhiteSpace(employee.Phone) &&
                employee.Phone.Trim().Length > MaxPhoneLength)
                return EmployeeResult.Fail("Phone cannot exceed " + MaxPhoneLength + " characters.");

            if (!string.IsNullOrWhiteSpace(employee.Email) &&
                employee.Email.Trim().Length > MaxEmailLength)
                return EmployeeResult.Fail("Email cannot exceed " + MaxEmailLength + " characters.");

            if (!string.IsNullOrWhiteSpace(employee.Address) &&
                employee.Address.Trim().Length > MaxAddressLength)
                return EmployeeResult.Fail("Address cannot exceed " + MaxAddressLength + " characters.");

            return EmployeeResult.Ok();
        }

        private static EmployeeEntity Normalize(EmployeeEntity employee)
        {
            return new EmployeeEntity
            {
                EmployeeName = employee.EmployeeName.Trim(),
                Gender = NormalizeOptional(employee.Gender),
                Phone = NormalizeOptional(employee.Phone),
                Email = NormalizeOptional(employee.Email),
                Address = NormalizeOptional(employee.Address)
            };
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
