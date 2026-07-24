using BuildingBlocks.Application.Extensions;
using FluentValidation;
using Restaurant.Application.DTOs;
using Restaurant.Application.Validation.Extensions;
using Restaurant.Domain.Contracts;

namespace Restaurant.Application.Validation.Validators;

public interface IMenuItemValidator
{
    public sealed class CreateRequest : AbstractValidator<IMenuItemDto.CreateRequest>
    {
        public CreateRequest()
        {
            RuleFor(x => x.MenuItemPrice)
                .Required()
                .MenuItemPrice();

            RuleFor(x => x.MenuItemName!)
                .Required()
                .MinLength(3)
                .MaxLength(100);

            RuleFor(x => x.MenuItemDescription!)
                .MinLength(20)
                .MaxLength(500)
                .When(x => !string.IsNullOrEmpty(x.MenuItemDescription));
        }
    }

    public sealed class CreateCommand : AbstractValidator<IMenuItemCommand.Create>
    {
        public CreateCommand()
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
}