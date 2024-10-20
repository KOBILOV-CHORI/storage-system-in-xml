using Domain.DTOs.CategoryDTOs;
using Infrastructure.Services.CategoryServices;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GetCategoryResponseDto), 201)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto category)
    {
         var createdCategory = await _categoryService.CreateCategoryAsync(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetCategoryResponseDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetCategoryResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequestDto updatedCategory)
    {
        var result = await _categoryService.UpdateCategoryAsync(updatedCategory);

        if (result != null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    [HttpGet("category-with-count-products")]
    [ProducesResponseType(typeof(List<GetCategoryResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetCategoryWithCountProductAsync()
    {
        var categories = await _categoryService.GetCategoryWithCountProductAsync();
        return Ok(categories);
    }
}