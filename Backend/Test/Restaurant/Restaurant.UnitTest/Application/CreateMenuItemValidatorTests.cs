using FluentAssertions;
using Restaurant.Application.Validation.Validators;
using Restaurant.Domain.Contracts;

namespace Restaurant.UnitTest.Application;

public class CreateMenuItemValidatorTests
{
    private readonly CreateMenuItemCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Price_Is_Invalid()
    {
        var command = new CreateMenuItemCommand
        {
            MenuItemName = "Chicken",
            MenuItemPrice = -5
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(command.MenuItemPrice));
    }
}