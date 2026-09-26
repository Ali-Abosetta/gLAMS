using System;
using System.Threading.Tasks;
using gLAMS.Application.Interfaces.Factories;
using gLAMS.Application.Interfaces.Logging;
using gLAMS.Shared.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;

namespace gLAMS.Infrastructure.Repositories.Base
{
    /// <summary>
    /// Serves as the abstract foundation for all data access repositories.
    /// Centralizes connection creation, ADO.NET boilerplate, and safe execution wrappers with integrated logging.
    /// </summary>
    public abstract class BaseRepository
    {
        protected readonly ISqlConnectionFactory _connectionFactory;
        protected readonly IAppLogger _logger;

        protected BaseRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        /// <summary>
        /// Safely executes a database operation, catching and logging any exceptions, 
        /// and returning a unified Result object. Prevents application crashes from SQL errors.
        /// </summary>
        /// <typeparam name="T">The expected return type of the data.</typeparam>
        /// <param name="operation">The asynchronous database operation to run.</param>
        /// <returns>A Result containing either the data or an error message.</returns>
        protected async Task<Result<T>> ExecuteSafeAsync<T>(Func<Task<Result<T>>> operation)
        {
            try
            {
                return await operation();
            }
            catch (SqlException ex)
            {
                _logger.LogError("A SQL database error occurred during execution.", ex);
                return Result<T>.Failure($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred during database execution.", ex);
                return Result<T>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// A reusable ADO.NET helper that executes a SELECT command, manages the SQL Connection, 
        /// and maps the resulting data rows into a List of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to map the data to.</typeparam>
        /// <param name="command">The SqlCommand containing the Stored Procedure and Parameters.</param>
        /// <param name="mapReader">A function delegate that defines how to map a SqlDataReader row to the entity.</param>
        /// <returns>A populated list of entities.</returns>
        protected async Task<List<TEntity>> FetchListAsync<TEntity>(SqlCommand command, Func<SqlDataReader, TEntity> mapReader)
        {
            List<TEntity> list = new List<TEntity>();

            using (command.Connection = (SqlConnection)await _connectionFactory.CreateConnectionAsync())
            {
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(mapReader(reader));
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// A reusable ADO.NET helper that executes an INSERT, UPDATE, or DELETE command 
        /// and manages the SQL Connection lifecycle.
        /// </summary>
        /// <param name="command">The SqlCommand containing the Stored Procedure and Parameters.</param>
        protected async Task<int> ExecuteCommandAsync(SqlCommand command)
        {
            using (command.Connection = (SqlConnection)await _connectionFactory.CreateConnectionAsync())
            {
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
