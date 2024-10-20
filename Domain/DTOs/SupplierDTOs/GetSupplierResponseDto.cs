namespace Domain.DTOs.SupplierDTOs;

public class GetSupplierResponseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}