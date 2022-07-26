using FluentValidation;
using Rest.Products.Models.Filter;

namespace Rest.Products.Validation;

public class FilterValidation : AbstractValidator<PageFilter>
{
    public FilterValidation()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.Size)
            .GreaterThan(0);
    }
}

public class ProductFilterValidation : AbstractValidator<ProductFilter>
{
    public ProductFilterValidation()
    {
        Include(new FilterValidation());
    }
}
