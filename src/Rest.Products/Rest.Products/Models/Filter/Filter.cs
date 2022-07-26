namespace Rest.Products.Models.Filter;

public record PageFilter(int? Page, int? Size);

public record ProductFilter(int? Page, int? Size, Guid[]? CategoryIds = null)
 : PageFilter(Page, Size);