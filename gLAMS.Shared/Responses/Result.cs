

namespace gLAMS.Shared.Responses
{
    /// <summary>
    /// Represents the outcome of an operation, encapsulating both the success state and any potential error messages.
    /// </summary>
    /// <typeparam name="T">The type of the data being returned upon success.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the error message if the operation failed. Will be null if successful.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Gets the requested data. Will be default if the operation failed.
        /// </summary>
        public T? Data { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// Private constructor to force the use of static factory methods.
        /// </summary>
        private Result(bool isSuccess, string? errorMessage, T? data)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Data = data;
        }

        /// <summary>
        /// Creates a successful result containing the provided data.
        /// </summary>
        /// <param name="data">The data to return.</param>
        /// <returns>A successful <see cref="Result{T}"/>.</returns>
        public static Result<T> Success(T data)
        {
            return new Result<T>(true, null, data);
        }

        /// <summary>
        /// Creates a failed result containing an error message.
        /// </summary>
        /// <param name="errorMessage">The reason for the failure.</param>
        /// <returns>A failed <see cref="Result{T}"/>.</returns>
        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T>(false, errorMessage, default);
        }
    }
}