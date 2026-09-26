using System.Data;
using System.Threading.Tasks;

namespace gLAMS.Application.Interfaces.Factories

{
    /// <summary>
    /// Defines a factory for creating and opening database connections.
    /// Used by the Dependency Injection container to manage connection lifetimes.
    /// </summary>
    public interface ISqlConnectionFactory
    {
        /// <summary>
        /// Creates and opens a new database connection asynchronously.
        /// </summary>
        /// <returns>An open IDbConnection.</returns>
        Task<IDbConnection> CreateConnectionAsync();
    }
}
