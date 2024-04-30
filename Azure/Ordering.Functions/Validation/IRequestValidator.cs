using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ErrorOr;

namespace Ordering.Functions.Validation;

public interface IRequestValidator
{
    ErrorOr<Success> Validate<T>([NotNull] T model, [CallerArgumentExpression("model")] string? paramName = null);
}