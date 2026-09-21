using System;

namespace gLAMS.Application.Interfaces.Logging
{
    /// <summary>
    /// Represents a diagnostic logging service for the application.
    /// </summary>
    public interface IAppLogger
    {
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogInformation(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        void LogWarning(string message);

        /// <summary>
        /// Logs an error message alongside its associated exception.
        /// </summary>
        /// <param name="message">A description of the error context.</param>
        /// <param name="exception">The exception instance that triggered the error.</param>
        void LogError(string message, Exception exception);
    }
}
