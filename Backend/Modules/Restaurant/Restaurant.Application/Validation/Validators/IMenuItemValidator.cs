using BuildingBlocks.Application.Extensions;
using FluentValidation;
using Restaurant.Application.Validation.Extensions;
using Restaurant.Domain.Contracts;
using Restaurant.Domain.Enumerations;
using Restaurant.Domain.Errors;

namespace Restaurant.Application.Validation.Validators;

public interface IMenuItemValidator
{
    public sealed class CreateCommand : AbstractValidator<IMenuItemCommand.Create>
    {
        public CreateCommand()
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
            
            RuleFor(x => x)
                .Must(x =>
                    x.Category switch
                    {
                        MenuCategory.Food => x.SubCategory is
                            MenuSubCategory.Breakfast or
                            MenuSubCategory.Lunch or
                            MenuSubCategory.Dinner,

                        MenuCategory.Drink => x.SubCategory is
                            MenuSubCategory.SoftDrink or
                            MenuSubCategory.Alcohol,

                        _ => false
                    })
                .WithMessage(MenuItemErrors.InvalidCategoryMapping.MessageKey);
        }
    }
}