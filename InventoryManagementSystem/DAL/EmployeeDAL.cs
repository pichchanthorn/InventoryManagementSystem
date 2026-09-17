using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    public static class EmployeeDAL
    {
        public static List<EmployeeEntity> GetAll()
        {
            const string sql = @"
                SELECT EmployeeID, EmployeeName, Gender, Phone, Email, Address
                FROM Employees
                ORDER BY EmployeeName ASC;";

            var employees = new List<EmployeeEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(ReadEmployee(reader));
                    }
                }
            }

            return employees;
        }

        public static EmployeeEntity GetById(int employeeId)
        {
            const string sql = @"
                SELECT EmployeeID, EmployeeName, Gender, Phone, Email, Address
                FROM Employees
                WHERE EmployeeID = @EmployeeID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = employeeId;

                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                        return null;

                    return ReadEmployee(reader);
                }
            }
        }

        public static void Insert(EmployeeEntity employee)
        {
            const string sql = @"
                INSERT INTO Employees (EmployeeName, Gender, Phone, Email, Address)
                VALUES (@EmployeeName, @Gender, @Phone, @Email, @Address);";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeName", SqlDbType.NVarChar, 150).Value = employee.EmployeeName;
                AddOptionalParameter(command, "@Gender", SqlDbType.NVarChar, 20, employee.Gender);
                AddOptionalParameter(command, "@Phone", SqlDbType.NVarChar, 30, employee.Phone);
                AddOptionalParameter(command, "@Email", SqlDbType.NVarChar, 100, employee.Email);
                AddOptionalParameter(command, "@Address", SqlDbType.NVarChar, 255, employee.Address);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Update(EmployeeEntity employee)
        {
            const string sql = @"
                UPDATE Employees
                SET EmployeeName = @EmployeeName,
                    Gender = @Gender,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address
                WHERE EmployeeID = @EmployeeID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeName", SqlDbType.NVarChar, 150).Value = employee.EmployeeName;
                AddOptionalParameter(command, "@Gender", SqlDbType.NVarChar, 20, employee.Gender);
                AddOptionalParameter(command, "@Phone", SqlDbType.NVarChar, 30, employee.Phone);
                AddOptionalParameter(command, "@Email", SqlDbType.NVarChar, 100, employee.Email);
                AddOptionalParameter(command, "@Address", SqlDbType.NVarChar, 255, employee.Address);
                command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = employee.EmployeeID;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Delete(int employeeId)
        {
            const string sql = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = employeeId;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static EmployeeEntity ReadEmployee(SqlDataReader reader)
        {
            int genderOrdinal = reader.GetOrdinal("Gender");
            int phoneOrdinal = reader.GetOrdinal("Phone");
            int emailOrdinal = reader.GetOrdinal("Email");
            int addressOrdinal = reader.GetOrdinal("Address");

            return new EmployeeEntity
            {
                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                Gender = reader.IsDBNull(genderOrdinal) ? null : reader.GetString(genderOrdinal),
                Phone = reader.IsDBNull(phoneOrdinal) ? null : reader.GetString(phoneOrdinal),
                Email = reader.IsDBNull(emailOrdinal) ? null : reader.GetString(emailOrdinal),
                Address = reader.IsDBNull(addressOrdinal) ? null : reader.GetString(addressOrdinal)
            };
        }

        private static void AddOptionalParameter(SqlCommand command, string name, SqlDbType type, int size, string value)
        {
            SqlParameter parameter = command.Parameters.Add(name, type, size);
            parameter.Value = string.IsNullOrEmpty(value) ? (object)DBNull.Value : value;
        }
    }
}
