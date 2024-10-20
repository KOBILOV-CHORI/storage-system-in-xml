namespace Domain.DTOs.OrderDTOs;

public class UpdateOrderRequestDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public int SupplierId { get; set; }
    public string? Status { get; set; }
}