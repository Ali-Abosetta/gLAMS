using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using gLAMS.Application.Interfaces.Factories;

namespace gLAMS.Infrastructure.Factories
{
    /// <summary>
    /// Implements ISqlConnectionFactory for SQL Server using Microsoft.Data.SqlClient.
    /// Returns an open IDbConnection ready for querying.
    /// </summary>
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
