namespace Rest.Products.Models.Category;

public abstract record BaseCategory (string Name, string Description);

public record Category(Guid Id, string Name, string Description)
    : BaseCategory(Name, Description);

public record CreateCategory(string Name, string Description)
    : BaseCategory(Name, Description);

public record UpdateCategory(string Name, string Description)
    : BaseCategory(Name, Description);
