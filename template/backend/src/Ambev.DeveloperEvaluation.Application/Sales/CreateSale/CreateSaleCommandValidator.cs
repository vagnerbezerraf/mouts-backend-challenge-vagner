using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand that defines validation rules for sale creation command.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleCommandValidator with defined validation rules.
    /// </summary>
    public CreateSaleCommandValidator()
    {
        RuleFor(sale => sale.CustomerId).NotEmpty();
        RuleFor(sale => sale.BranchId).NotEmpty();
        RuleFor(sale => sale.Items).NotEmpty();
        RuleForEach(sale => sale.Items).SetValidator(new CreateSaleItemCommandValidator());
    }
}

public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemCommandValidator()
    {
        RuleFor(item => item.ProductId).NotEmpty();
        RuleFor(item => item.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
        RuleFor(item => item.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
