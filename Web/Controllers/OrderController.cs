using Domain.DTOs.OrderDTOs;
using Infrastructure.Services.OrderServices;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GetOrderResponseDto), 201)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto order)
    {
         var createdOrder = await _orderService.CreateOrderAsync(order);
        return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetOrderResponseDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetOrderResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequestDto updatedOrder)
    {
        var result = await _orderService.UpdateOrderAsync(updatedOrder);

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
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var result = await _orderService.DeleteOrderAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    [HttpGet("orders-by-supplier")]
    [ProducesResponseType(typeof(List<GetOrderResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetOrdersBySupplier([FromQuery] int supplierId, string status)
    {
        var orders = await _orderService.GetOrdersBySupplier(supplierId, status);
        return Ok(orders);
    }
    [HttpGet("orders-by-date")]
    [ProducesResponseType(typeof(List<GetOrderResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetOrdersByDate([FromQuery]DateTime startDate, DateTime endDate)
    {
        var orders = await _orderService.GetOrdersByDate(startDate, endDate);
        return Ok(orders);
    }
    [HttpGet("orders-pagination")]
    [ProducesResponseType(typeof(List<GetOrderResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetOrdersPaginationAsync([FromQuery]int pageNumber, int pageSize)
    {
        var orders = await _orderService.GetOrdersPaginationAsync(pageNumber, pageSize);
        return Ok(orders);
    }
}