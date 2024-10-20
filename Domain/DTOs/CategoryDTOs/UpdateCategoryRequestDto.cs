namespace Domain.DTOs.CategoryDTOs;

public class UpdateCategoryRequestDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}