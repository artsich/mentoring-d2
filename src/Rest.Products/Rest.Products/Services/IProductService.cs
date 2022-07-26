using Rest.Products.Models.Product;

namespace Rest.Products.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAll(int? page, int? size, Guid[]? categoryIds = null);
    
    Task<Product?> Get(Guid id);

    Task<Product> Create(CreateProduct product);

    Task<Product> Update(Guid id, UpdateProduct product);

    Task<bool> Delete(Guid id);
}