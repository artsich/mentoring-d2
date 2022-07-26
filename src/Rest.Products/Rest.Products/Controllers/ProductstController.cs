using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rest.Products.Models.Category;
using Rest.Products.Models.Product;

namespace Rest.Products.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    public ProductsController(ILogger<ProductsController> logger)
    {
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(uint), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<Product>>> Get(int? page, int? size, [FromQuery]Guid[] categoryIds)
    {
        var result = await Task.FromResult(Enumerable.Empty<Product>());
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(CreateProduct), (int)HttpStatusCode.BadRequest)]
    public ActionResult<Product> Post(CreateProduct product)
    {
        var result = new Product(Guid.NewGuid(), product.Name, new Category(product.CategoryId, "Category name", "Category desc"));
        return Created("", result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(UpdateProduct), (int)HttpStatusCode.BadRequest)]
    public ActionResult<Product> Update(Guid id, UpdateProduct product)
    {
        var result = new Product(id, product.Name, new Category(product.CategoryId, "Category name", "Category desc"));
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(Guid))]
    public IActionResult Delete(Guid id)
    {
        return NoContent();
    }
}
