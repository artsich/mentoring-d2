using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rest.Products.Models.Filter;
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
    [ProducesResponseType(typeof(ProductFilter), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll([FromQuery] ProductFilter filter)
    {
        var result = await _productService.GetAll(filter.Page, filter.Size, filter.CategoryIds);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<Product>>> Get(Guid id)
    {
        var result = await _productService.Get(id);
        return result is null ? NotFound() : Ok(result);
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
        await _productService.Delete(id);
        return NoContent();
    }
}