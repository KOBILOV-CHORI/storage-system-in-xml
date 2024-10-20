namespace Domain.DTOs.OrderDTOs;

public class CreateOrderRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int SupplierId { get; set; }
    public string? Status { get; set; }
}