using Domain.Models.Requests.v1;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Domain.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
	public UpdateProductValidator()
	{
        RuleFor(x => x.Sku)
        .Must(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))
        .WithMessage("Product SKU cannot be null or whitespace.")
        .Must(x => x.Length == 8)
        .WithMessage("SKU must be 8 characters long.")
        .Must(x => Regex.IsMatch(x, @"^[A-Za-z]{3}\d{5}$"))
        .WithMessage("Invalid SKU, first 3 letters of SKU should be alphanumeric followed by 5 digits.");

        RuleFor(x => x.Name)
        .Must(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))
        .WithMessage("Product name cannot be null or whitespace.")
        .MinimumLength(2)
        .WithMessage("Product name must be at least 2 characters long.")
        .MaximumLength(200)
        .WithMessage("Product name must be at most 200 characters long.");

        RuleFor(x => x.ManufacturedDate)
        .Must(x => x.Year >= DateTime.UtcNow.Year)
        .WithMessage("Product cannot be manufactured less than current year, you have entered {propertyValue}.");

        RuleFor(x => x.ListPrice)
        .Must(x => x > 0)
        .WithMessage("List price cannot be negative.");
    }
}
