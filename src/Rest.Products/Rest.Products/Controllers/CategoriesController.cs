using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rest.Products.Models.Category;
using Rest.Products.Models.Filter;
using Rest.Products.Services;

namespace Rest.Products.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Category>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(PageFilter), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll([FromQuery]PageFilter filter)
    {
        var result = await _categoryService.GetAll(filter.Page, filter.Size);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Category>>> Get(Guid id)
    {
        var result = await _categoryService.Get(id);
        return result is null ? BadRequest() : Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(CreateCategory), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Category>> Post(CreateCategory category)
    {
        var result = await _categoryService.Create(category);
        return CreatedAtRoute(new { id = result.Id }, result);      
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(UpdateCategory), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Category>> Update(Guid id, UpdateCategory category)
    {
        var result = await _categoryService.Update(id, category);
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(Guid))]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await _categoryService.Delete(id) ? NoContent() : NotFound();
    }
}
