using FluentValidation;
using Rest.Products.Models.Category;

namespace Rest.Products.Validation;

public class BaseCategoryValidation : AbstractValidator<BaseCategory>
{
    public BaseCategoryValidation()
    {
        RuleFor(x => x.Name)
            .MinimumLength(5)
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MinimumLength(5)
            .MaximumLength(500);
    }
}

public class CreateCategoryValidation : AbstractValidator<CreateCategory>
{
    public CreateCategoryValidation()
    {
        Include(new BaseCategoryValidation());
    }
}

public class UpdateCategoryValidation : AbstractValidator<UpdateCategory>
{
    public UpdateCategoryValidation()
    {
        Include(new BaseCategoryValidation());
    }
}
 