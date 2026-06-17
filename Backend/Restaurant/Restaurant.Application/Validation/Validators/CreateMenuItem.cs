using BuildingBlocks.Application.Validation.Extensions;
using FluentValidation;
using Restaurant.Application.DTOs;
using Restaurant.Application.Validation.Extensions;

namespace Restaurant.Application.Validation.Validators;

public sealed class CreateMenuItem : AbstractValidator<IMenuItemDto.CreateRequest>
{
    public CreateMenuItem()
    {
        RuleFor(x => x.MenuItemPrice)
            .Required()
            .MenuItemPrice();

        RuleFor(x => x.MenuItemName)
            .Required()
            .MinLength(3)
            .MaxLength(100);

        RuleFor(x => x.MenuItemDescription)
            .MinLength(20)
            .MaxLength(500)
            .When(x => !string.IsNullOrEmpty(x.MenuItemDescription));
    }
}