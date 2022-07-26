using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rest.Products.Models.Category;

namespace Rest.Products.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    public CategoriesController(ILogger<CategoriesController> logger)
    {
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(uint), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<Category>>> Get(int? page, int? size)
    {
        var result = await Task.FromResult(Enumerable.Empty<Category>());
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(CreateCategory), (int)HttpStatusCode.BadRequest)]
    public ActionResult<Category> Post(CreateCategory newCategory)
    {
        var result = new Category(Guid.NewGuid(), newCategory.Name, newCategory.Description);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(UpdateCategory), (int)HttpStatusCode.BadRequest)]
    public ActionResult<Category> Update(Guid id, UpdateCategory category)
    {
        var result = new Category(id, category.Name, category.Description);
        return Ok();
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(Guid))]
    public IActionResult Delete(Guid id)
    {
        return NoContent();
    }
}
