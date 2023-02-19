using Domain.Models.Requests.v1;
using FluentValidation;
using System.Globalization;

namespace Domain.Validators;

public class GetProductsPaginationQueryValidator : AbstractValidator<GetProductsPaginationQuery>
{
	public GetProductsPaginationQueryValidator()
	{
		RuleFor(x => x.SearchAfter)
		.Must(x => DateTime.TryParseExact(x, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
		.When(x => !string.IsNullOrEmpty(x.SearchAfter) && !string.IsNullOrWhiteSpace(x.SearchAfter))
		.WithMessage(x => "Search after should be formatted in ISO-8601 format.");

		RuleFor(x => x.Limit)
		.Must(x => x > 0)
		.WithMessage("Limit cannot be a negative number.");
	}
}
