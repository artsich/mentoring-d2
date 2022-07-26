using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rest.Products.Models.Product;
using Rest.Products.Services;

namespace Rest.Products.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(int? page, int? size, [FromQuery]Guid[]? categoryIds)
    {
        var result = await _productService.GetAll(page, size, categoryIds);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> Get(Guid id)
    {
        var result = await _productService.Get(id);
        return result is null ? BadRequest() : Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(CreateProduct), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Product>> Post(CreateProduct product)
    {
        var result = await _productService.Create(product);
        return CreatedAtRoute(new { id = result.Id }, result);      
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(UpdateProduct), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Product>> Update(Guid id, UpdateProduct product)
    {
        return Ok(await _productService.Update(id, product));
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(Guid))]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await _productService.Delete(id) ? NoContent() : NotFound();
    }
}
