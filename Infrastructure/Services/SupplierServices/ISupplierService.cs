using Domain.DTOs.SupplierDTOs;

namespace Infrastructure.Services.SupplierServices;

public interface ISupplierService
{
    Task<List<GetSupplierResponseDto>> GetAllSuppliersAsync();
    Task<GetSupplierResponseDto> CreateSupplierAsync(CreateSupplierRequestDto supplier);
    Task<GetSupplierResponseDto> UpdateSupplierAsync(UpdateSupplierRequestDto supplier);
    Task<GetSupplierResponseDto> GetSupplierByIdAsync(int id);
    Task<bool> DeleteSupplierAsync(int id);
    Task<List<GetSupplierResponseDto>> GetSuppliersByCountProductAsync(int minQuantity);
}