namespace Insurance.Application.Common.Models;

/// <summary>
/// Non-generic result wrapper for operations that don't return data.
/// Used for operations like Delete that only need success/failure status.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Error message if the operation failed. Null if successful.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    private Result(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="errorMessage">Optional success message</param>
    /// <returns>Successful Result instance</returns>
    public static Result SuccessResult(string? errorMessage = null)
        => new(true, errorMessage);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">Description of what went wrong</param>
    /// <returns>Failed Result instance</returns>
    public static Result FailureResult(string errorMessage)
        => new(false, errorMessage);
}

/// <summary>
/// Generic result wrapper for command/query responses using the Result pattern.
/// Replaces throwing exceptions with explicit success/failure states for cleaner error handling.
/// </summary>
/// <typeparam name="T">The type of data returned on success</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// The data returned on successful completion. Null if operation failed.
    /// </summary>
    public T? Data { get; private set; }

    /// <summary>
    /// Error message if the operation failed. Null if successful.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Collection of validation errors when Result is created from FluentValidation failures.
    /// </summary>
    public List<ValidationError> ValidationErrors { get; private set; } = new();

    private Result(bool isSuccess, T? data = default, string? errorMessage = null, List<ValidationError>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors ?? new();
    }

    /// <summary>
    /// Creates a successful result with the specified data.
    /// </summary>
    /// <param name="data">The data to return</param>
    /// <param name="message">Optional success message</param>
    /// <returns>Successful Result instance</returns>
    public static Result<T> Success(T data, string? message = null)
        => new(true, data, message);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">Description of what went wrong</param>
    /// <returns>Failed Result instance</returns>
    public static Result<T> Failure(string errorMessage)
        => new(false, default, errorMessage);

    /// <summary>
    /// Creates a failed result with validation errors.
    /// </summary>
    /// <param name="validationErrors">Collection of validation errors</param>
    /// <returns>Failed Result instance with validation errors</returns>
    public static Result<T> ValidationFailure(List<ValidationError> validationErrors)
        => new(false, default, "Validation failed", validationErrors);
}

/// <summary>
/// Represents a single validation error for a property.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Name of the property that failed validation.
    /// </summary>
    public required string PropertyName { get; set; }

    /// <summary>
    /// The error message for this validation failure.
    /// </summary>
    public required string ErrorMessage { get; set; }

    public ValidationError() { }

    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}
