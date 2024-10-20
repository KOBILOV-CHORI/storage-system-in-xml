using Domain.DTOs.ProductDTOs;

namespace Infrastructure.Services.ProductServices;

public interface IProductService
{
    Task<List<GetProductResponseDto>> GetAllProductsAsync();
    Task<GetProductResponseDto> CreateProductAsync(CreateProductRequestDto product);
    Task<GetProductResponseDto> UpdateProductAsync(UpdateProductRequestDto product);
    Task<GetProductResponseDto> GetProductByIdAsync(int id);
    Task<bool> DeleteProductAsync(int id);

    Task<List<GetProductResponseDto>> GetProductsByCategoryIdAsync(int categoryId, string sortBy,
        string sortOrder);

    Task<List<GetProductResponseDto>> GetProductsByMaxAsync(int maxQuantity);
    Task<List<GetProductDetailsDto>> GetProductDetailsAsync(int id);
    Task<List<GetProductDetailsDto>> GetProductDetailsPaginationAsync(int pageNumber, int pageSize);
    Task<List<GetProductResponseDto>> GetMostOrderedProductsAsync(int minOrders);
}