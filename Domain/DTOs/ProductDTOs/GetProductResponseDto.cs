namespace Domain.DTOs.ProductDTOs;

public class GetProductResponseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}