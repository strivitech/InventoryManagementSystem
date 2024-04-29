using FluentValidation;

namespace Ordering.Functions.Contracts;

public record AddressDto(string Street, string City, string State, string ZipCode);

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
        RuleFor(x => x.ZipCode).NotEmpty();
    }
}