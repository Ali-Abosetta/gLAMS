using System;
using System.Diagnostics;
using gLAMS.Application.Interfaces.Logging;

namespace gLAMS.Infrastructure.Logging
{
    /// <summary>
    /// Provides an implementation of <see cref="IAppLogger"/> that writes diagnostic events to the Windows Event Log.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public class WindowsEventLogger : IAppLogger
    {
        private readonly string source = "gLAMS";

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsEventLogger"/> class.
        /// Ensures the application event source is registered prior to initialization.
        /// </summary>
        /// <remarks>
        /// Requires administrative privileges during the initial execution to successfully register the event source.
        /// </remarks>
        public WindowsEventLogger()
        {
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, "Application");
            }
        }

        /// <inheritdoc />
        public void LogInformation(string message)
        {
            EventLog.WriteEntry(source, message, EventLogEntryType.Information);
        }

        /// <inheritdoc />
        public void LogWarning(string message)
        {
            EventLog.WriteEntry(source, message, EventLogEntryType.Warning);
        }

        /// <inheritdoc />
        public void LogError(string message, Exception exception)
        {
            string errorMessage = $"{message}\nException: {exception.Message}\nStack Trace: {exception.StackTrace}";
            EventLog.WriteEntry(source, errorMessage, EventLogEntryType.Error);
        }
    }
}
