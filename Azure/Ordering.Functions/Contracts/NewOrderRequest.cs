using FluentValidation;

namespace Ordering.Functions.Contracts;

public record NewOrderRequest(IList<OrderLineDto> OrderLines, AddressDto ShippingAddress, string CustomerId);

public class NewOrderRequestValidator : AbstractValidator<NewOrderRequest>
{
    public NewOrderRequestValidator(AddressDtoValidator addressDtoValidator, OrderLineDtoValidator orderLineDtoValidator)
    {
        RuleForEach(x => x.OrderLines).SetValidator(orderLineDtoValidator);
        RuleFor(x => x.ShippingAddress).NotNull().SetValidator(addressDtoValidator);
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}