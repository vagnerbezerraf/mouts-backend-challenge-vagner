using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number cannot be empty.");

        RuleFor(sale => sale.CustomerId)
            .NotEmpty()
            .WithMessage("Customer must be specified.");

        RuleFor(sale => sale.BranchId)
            .NotEmpty()
            .WithMessage("Branch must be specified.");

        RuleFor(sale => sale.Items)
            .NotEmpty()
            .WithMessage("A sale must contain at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new SaleItemValidator());
    }
}
