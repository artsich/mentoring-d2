namespace Rest.Products.Models.Product;

public abstract record BaseProduct(string Name, Guid CategoryId);

public record Product(Guid Id, string Name, Guid CategoryId)
    : BaseProduct(Name, CategoryId);

public record CreateProduct(string Name, Guid CategoryId)
    : BaseProduct(Name, CategoryId);

public record UpdateProduct(string Name, Guid CategoryId)
    : BaseProduct(Name, CategoryId);
