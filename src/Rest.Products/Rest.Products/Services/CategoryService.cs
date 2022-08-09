using MongoDB.Driver;
using Rest.Products.DataAccess;
using Rest.Products.Exceptions;
using Rest.Products.Models.Category;
using Rest.Products.Models.Product;

namespace Rest.Products.Services;

public class CategoryService : ICategoryService
{
    private readonly IMongoCollection<Category> _categories;
    private readonly IMongoCollection<Product> _products;

    public CategoryService(IMongoContext context)
    {
        _categories = context.Collection<Category>();
        _products = context.Collection<Product>();
    }

    public async Task<IEnumerable<Category>> GetAll(int? page, int? size)
    {
        var filter = Builders<Category>.Filter.Empty;

        var query = _categories.Find(filter);

        if (page is > 0 && size.HasValue)
        {
            var skip = (page - 1) * size;
            query = query
                .Skip(skip.Value)
                .Limit(size.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Category> Get(Guid id)
    {
        return await _categories.Find(x => x.Id == id).FirstOrDefaultAsync() ??
            throw new EntityNotFoundException(id);
    }

    public async Task<Category> Create(CreateCategory category)
    {
        var newCategory = new Category(Guid.NewGuid(), category.Name, category.Description);
        await _categories.InsertOneAsync(newCategory);
        return newCategory;
    }

    public async Task<Category> Update(Guid id, UpdateCategory category)
    {
        var newCategory = new Category(id, category.Name, category.Description);
        var result = await _categories.ReplaceOneAsync(x => x.Id == id, newCategory);

        if (result.ModifiedCount <= 0)
		{
            throw new EntityNotFoundException(id);
        }

        return newCategory;
    }

    public async Task Delete(Guid id)
    {
        var deleteCategoryTask = _categories.DeleteOneAsync(x => x.Id == id);
        var deleteProductsTask = _products.DeleteManyAsync(x => x.CategoryId == id);

        await Task.WhenAll(deleteCategoryTask, deleteProductsTask);

        var deletedCategories = deleteCategoryTask.Result;
        
        if (deletedCategories.DeletedCount <= 0)
		{
            throw new EntityNotFoundException(id);
		}
    }
}
