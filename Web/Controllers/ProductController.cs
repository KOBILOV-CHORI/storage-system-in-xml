using Domain.DTOs.ProductDTOs;
using Infrastructure.Services.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GetProductResponseDto), 201)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto product)
    {
        var createdProduct = await _productService.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetProductResponseDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetProductResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequestDto updatedProduct)
    {
        var result = await _productService.UpdateProductAsync(updatedProduct);

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
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("products-by-category-id")]
    [ProducesResponseType(typeof(List<GetProductResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetProductsByCategoryIdAsync([FromQuery]int categoryId, string sortBy, string sortOrder)
    {
        var products = await _productService.GetProductsByCategoryIdAsync(categoryId, sortBy, sortOrder);
        return Ok(products);
    }

    [HttpGet("max-quantity")]
    [ProducesResponseType(typeof(List<GetProductResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetProductsByMaxQuantity([FromQuery]int maxQuantity)
    {
        var products = await _productService.GetProductsByMaxAsync(maxQuantity);
        return Ok(products);
    }
    [HttpGet("product-details")]
    [ProducesResponseType(typeof(List<GetProductResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetProductDetailsAsync([FromQuery]int id)
    {
        var products = await _productService.GetProductDetailsAsync(id);
        return Ok(products);
    }
    [HttpGet("product-details-pagination")]
    [ProducesResponseType(typeof(List<GetProductResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetProductDetailsPaginationAsync([FromQuery]int pageNumber, int pageSize)
    {
        var products = await _productService.GetProductDetailsPaginationAsync(pageNumber, pageSize);
        return Ok(products);
    }
    [HttpGet("most-ordered")]
    [ProducesResponseType(typeof(List<GetProductResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetMostOrderedProductsAsync([FromQuery]int minOrders)
    {
        var products = await _productService.GetMostOrderedProductsAsync(minOrders);
        return Ok(products);
    }
}