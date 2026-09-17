using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.DAL
{
    public static class SupplierDAL
    {
        public static List<SupplierEntity> GetAll()
        {
            const string sql = @"
                SELECT SupplierID, SupplierName, ContactPerson, Phone, Email, Address
                FROM Suppliers
                ORDER BY SupplierName ASC;";

            var suppliers = new List<SupplierEntity>();

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        suppliers.Add(ReadSupplier(reader));
                    }
                }
            }

            return suppliers;
        }

        public static SupplierEntity GetById(int supplierId)
        {
            const string sql = @"
                SELECT SupplierID, SupplierName, ContactPerson, Phone, Email, Address
                FROM Suppliers
                WHERE SupplierID = @SupplierID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = supplierId;

                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                        return null;

                    return ReadSupplier(reader);
                }
            }
        }

        public static void Insert(SupplierEntity supplier)
        {
            const string sql = @"
                INSERT INTO Suppliers (SupplierName, ContactPerson, Phone, Email, Address)
                VALUES (@SupplierName, @ContactPerson, @Phone, @Email, @Address);";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@SupplierName", SqlDbType.NVarChar, 150).Value = supplier.SupplierName;
                AddOptionalParameter(command, "@ContactPerson", SqlDbType.NVarChar, 150, supplier.ContactPerson);
                AddOptionalParameter(command, "@Phone", SqlDbType.NVarChar, 30, supplier.Phone);
                AddOptionalParameter(command, "@Email", SqlDbType.NVarChar, 100, supplier.Email);
                AddOptionalParameter(command, "@Address", SqlDbType.NVarChar, 255, supplier.Address);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Update(SupplierEntity supplier)
        {
            const string sql = @"
                UPDATE Suppliers
                SET SupplierName = @SupplierName,
                    ContactPerson = @ContactPerson,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address
                WHERE SupplierID = @SupplierID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@SupplierName", SqlDbType.NVarChar, 150).Value = supplier.SupplierName;
                AddOptionalParameter(command, "@ContactPerson", SqlDbType.NVarChar, 150, supplier.ContactPerson);
                AddOptionalParameter(command, "@Phone", SqlDbType.NVarChar, 30, supplier.Phone);
                AddOptionalParameter(command, "@Email", SqlDbType.NVarChar, 100, supplier.Email);
                AddOptionalParameter(command, "@Address", SqlDbType.NVarChar, 255, supplier.Address);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = supplier.SupplierID;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void Delete(int supplierId)
        {
            const string sql = "DELETE FROM Suppliers WHERE SupplierID = @SupplierID;";

            using (var connection = DbConnection.GetConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = supplierId;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static SupplierEntity ReadSupplier(SqlDataReader reader)
        {
            int contactPersonOrdinal = reader.GetOrdinal("ContactPerson");
            int phoneOrdinal = reader.GetOrdinal("Phone");
            int emailOrdinal = reader.GetOrdinal("Email");
            int addressOrdinal = reader.GetOrdinal("Address");

            return new SupplierEntity
            {
                SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
                SupplierName = reader.GetString(reader.GetOrdinal("SupplierName")),
                ContactPerson = reader.IsDBNull(contactPersonOrdinal) ? null : reader.GetString(contactPersonOrdinal),
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
