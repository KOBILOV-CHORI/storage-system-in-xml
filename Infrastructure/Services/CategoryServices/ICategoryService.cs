using Domain.DTOs.CategoryDTOs;

namespace Infrastructure.Services.CategoryServices;

public interface ICategoryService
{
    Task<List<GetCategoryResponseDto>> GetAllCategoriesAsync();
    Task<GetCategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto category);
    Task<GetCategoryResponseDto> UpdateCategoryAsync(UpdateCategoryRequestDto category);
    Task<GetCategoryResponseDto> GetCategoryByIdAsync(int id);
    Task<bool> DeleteCategoryAsync(int id);
    Task<List<CategoryWithCountProductDto>> GetCategoryWithCountProductAsync();
}