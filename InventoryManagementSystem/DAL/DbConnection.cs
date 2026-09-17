using System.Configuration;
using System.Data.SqlClient;

namespace InventoryManagementSystem.DAL
{
    public static class DbConnection
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["InventoryDb"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
