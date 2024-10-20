using Infrastructure.Services.CategoryServices;
using Infrastructure.Services.OrderServices;
using Infrastructure.Services.ProductServices;
using Infrastructure.Services.SupplierServices;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public static class Extension
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICategoryService, CategoryService>();
    }
}