using Domain.DTOs.SupplierDTOs;
using Infrastructure.Services.SupplierServices;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GetSupplierResponseDto), 201)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequestDto supplier)
    {
         var createdSupplier = await _supplierService.CreateSupplierAsync(supplier);
        return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.Id }, createdSupplier);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetSupplierResponseDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetSupplierById(int id)
    {
        var supplier = await _supplierService.GetSupplierByIdAsync(id);

        if (supplier == null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetSupplierResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAllSuppliers()
    {
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        return Ok(suppliers);
    }

    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateSupplier([FromBody] UpdateSupplierRequestDto updatedSupplier)
    {
        var result = await _supplierService.UpdateSupplierAsync(updatedSupplier);

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
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var result = await _supplierService.DeleteSupplierAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    [HttpGet("min-product-quantity")]
    [ProducesResponseType(typeof(List<GetSupplierResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetSuppliersByCountProductAsync([FromQuery]int minQuantity)
    {
        var suppliers = await _supplierService.GetSuppliersByCountProductAsync(minQuantity);
        return Ok(suppliers);
    }
}