using ECommerceAPI.Application.Dtos;
using FluentValidation;

namespace ECommerceAPI.Application.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required")
            .EmailAddress().WithMessage("Invalid email address format");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters");

        RuleFor(x => x.CustomerGSM)
            .NotEmpty().WithMessage("Customer GSM is required")
            .Matches(@"^[0-9]+$").WithMessage("GSM number should contain only digits")
            .Length(10).WithMessage("GSM number should be 10 digits");

        RuleFor(x => x.ProductDetails)
            .NotEmpty().WithMessage("Order must contain at least one product");

        RuleForEach(x => x.ProductDetails).ChildRules(details =>
        {
            details.RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Product ID is required");

            details.RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            details.RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than 0");
        });
    }
}
