using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    public static class CustomerDAL
    {
        public static List<CustomerEntity> GetAll()
        {
            const string sql = @"
                SELECT CustomerID, CustomerName, Phone, Email, Address
                FROM Customers
                ORDER BY CustomerName ASC;";

            var customers = new List<CustomerEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(ReadCustomer(reader));
                    }
                }
            }

            return customers;
        }

        public static CustomerEntity GetById(int customerId)
        {
            const string sql = @"
                SELECT CustomerID, CustomerName, Phone, Email, Address
                FROM Customers
                WHERE CustomerID = @CustomerID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;

                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                        return null;

                    return ReadCustomer(reader);
                }
            }
        }

        public static void Insert(CustomerEntity customer)
        {
            const string sql = @"
                INSERT INTO Customers (CustomerName, Phone, Email, Address)
                VALUES (@CustomerName, @Phone, @Email, @Address);";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 150).Value = customer.CustomerName;
                AddOptionalParameter(command, "@Phone", SqlDbType.NVarChar, 30, customer.Phone);
                AddOptionalParameter(command, "@Email", SqlDbType.NVarChar, 100, customer.Email);
                AddOptionalParameter(command, "@Address", SqlDbType.NVarChar, 255, customer.Address);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Update(CustomerEntity customer)
        {
            const string sql = @"
                UPDATE Customers
                SET CustomerName = @CustomerName,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address
                WHERE CustomerID = @CustomerID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 150).Value = customer.CustomerName;
                AddOptionalParameter(command, "@Phone", SqlDbType.NVarChar, 30, customer.Phone);
                AddOptionalParameter(command, "@Email", SqlDbType.NVarChar, 100, customer.Email);
                AddOptionalParameter(command, "@Address", SqlDbType.NVarChar, 255, customer.Address);
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customer.CustomerID;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Delete(int customerId)
        {
            const string sql = "DELETE FROM Customers WHERE CustomerID = @CustomerID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerId;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static CustomerEntity ReadCustomer(SqlDataReader reader)
        {
            int phoneOrdinal = reader.GetOrdinal("Phone");
            int emailOrdinal = reader.GetOrdinal("Email");
            int addressOrdinal = reader.GetOrdinal("Address");

            return new CustomerEntity
            {
                CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
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
