using Domain.DTOs.OrderDTOs;

namespace Infrastructure.Services.OrderServices;

public interface IOrderService
{
    Task<List<GetOrderResponseDto>> GetAllOrdersAsync();
    Task<GetOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto product);
    Task<GetOrderResponseDto> UpdateOrderAsync(UpdateOrderRequestDto product);
    Task<GetOrderResponseDto> GetOrderByIdAsync(int id);
    Task<bool> DeleteOrderAsync(int id);
    Task<List<GetOrderResponseDto>> GetOrdersBySupplier(int supplierId, string status);
    Task<List<GetOrderResponseDto>> GetOrdersByDate(DateTime startDate, DateTime endDate);
    Task<List<GetOrderResponseDto>> GetOrdersPaginationAsync(int pageNumber, int pageSize);

}