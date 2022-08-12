using FluentValidation;
using Rest.Products.Models.Product;

namespace Rest.Products.Validation;

public class BaseProductValidation : AbstractValidator<BaseProduct>
{
    public BaseProductValidation()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50)
            .MinimumLength(1);
    }
}

public class CreateProductValidator : AbstractValidator<CreateProduct>
{
    public CreateProductValidator()
    {
        Include(new BaseProductValidation());
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProduct>
{
    public UpdateProductValidator()
    {
        Include(new BaseProductValidation());
    }
}

