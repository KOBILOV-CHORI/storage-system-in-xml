namespace Domain.DTOs.CategoryDTOs;

public class CategoryWithCountProductDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int CountOfProducts { get; set; }
}