using Rest.Products.Models.Category;

namespace Rest.Products.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAll(int? page, int? size);
    
    Task<Category> Get(Guid id);
    
    Task<Category> Create(CreateCategory category);

    Task<Category> Update(Guid id, UpdateCategory category);

    Task Delete(Guid id);
}