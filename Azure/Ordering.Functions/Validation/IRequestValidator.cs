using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ErrorOr;

namespace Ordering.Functions.Validation;

/// <summary>
/// Interface for request validation operations.
/// </summary>
public interface IRequestValidator
{
    /// <summary>
    /// Validates the given model of type T and returns a list of errors if validation fails.
    /// </summary>
    /// <typeparam name="T">The type of the model to be validated.</typeparam>
    /// <param name="model">The model instance to validate. This parameter is marked as NotNull, indicating it cannot be null.</param>
    /// <param name="paramName">Optional. The name of the parameter that is being validated, automatically provided by the CallerArgumentExpression attribute. This helps in identifying which parameter failed validation in the calling method.</param>
    /// <returns>A list of Error objects. If the list is empty, it indicates that no validation errors were found.</returns>
    List<Error> Validate<T>([NotNull] T model, [CallerArgumentExpression("model")] string? paramName = null);
}