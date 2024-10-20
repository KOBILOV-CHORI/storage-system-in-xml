using System.Xml;
using System.Xml.Linq;
using Domain.DTOs.OrderDTOs;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.OrderServices;

public class OrderService : IOrderService
{
    private readonly string? _pathData;

    public OrderService(IConfiguration configuration)
    {
        _pathData = configuration.GetConnectionString(XmlElements.PathData);

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(XmlElements.Source, new XElement(XmlElements.Orders))
            );
            if (_pathData != null) doc.Save(_pathData);
        }
        else
        {
            XDocument doc = XDocument.Load(_pathData);
            if (doc.Root == null || doc.Root.Name != XmlElements.Source)
            {
                doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(XmlElements.Source, new XElement(XmlElements.Orders))
                );
                doc.Save(_pathData);
            }
            else
            {
                var groupsElement = doc.Root.Element(XmlElements.Orders);
                if (groupsElement == null)
                {
                    groupsElement = new XElement(XmlElements.Orders);
                    doc.Root.Add(groupsElement);
                    doc.Save(_pathData);
                }
            }
        }
    }

    public async Task<GetOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto order)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        int newId = doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
            .Max(x => (int?)x.Element(XmlElements.Id)) ?? 0;
        newId++;
        XElement newOrder = new XElement(XmlElements.Order,
            new XElement(XmlElements.Id, newId),
            new XElement(XmlElements.Quantity, order.Quantity),
            new XElement(XmlElements.OrderDate, DateTime.UtcNow),
            new XElement(XmlElements.Status, order.Status),
            new XElement(XmlElements.ProductId, order.ProductId),
            new XElement(XmlElements.SupplierId, order.SupplierId)
        );
        doc.Root.Element(XmlElements.Orders).Add(newOrder);
        using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
        {
            await doc.SaveAsync(writer, CancellationToken.None);
        }

        return new GetOrderResponseDto()
        {
            Id = newId,
            Quantity = order.Quantity,
            Status = order.Status,
            OrderDate = DateTime.Parse(newOrder.Element(XmlElements.OrderDate).Value),
            ProductId = order.ProductId,
            SupplierId = order.SupplierId
        };
    }

    public async Task<List<GetOrderResponseDto>> GetAllOrdersAsync()
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var orders = doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
            .Select(x => new GetOrderResponseDto()
            {
                Id = (int)x.Element(XmlElements.Id),
                Quantity = (int)x.Element(XmlElements.Quantity),
                Status = (string)x.Element(XmlElements.Status),
                OrderDate = DateTime.Parse(x.Element(XmlElements.OrderDate).Value),
                ProductId = (int)x.Element(XmlElements.ProductId),
                SupplierId = (int)x.Element(XmlElements.SupplierId),
            }).ToList();

        return orders;
    }

    public async Task<GetOrderResponseDto> UpdateOrderAsync(UpdateOrderRequestDto order)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var orderToUpdate = doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == order.Id);

        if (orderToUpdate != null)
        {
            orderToUpdate.SetElementValue(XmlElements.Quantity, order.Quantity);
            orderToUpdate.SetElementValue(XmlElements.Status, order.Status);
            orderToUpdate.SetElementValue(XmlElements.ProductId, order.ProductId);
            orderToUpdate.SetElementValue(XmlElements.SupplierId, order.SupplierId);
            using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
            {
                await doc.SaveAsync(writer, CancellationToken.None);
            }

            return new GetOrderResponseDto()
            {
                Id = order.Id,
                Quantity = order.Quantity,
                Status = order.Status,
                OrderDate = order.OrderDate,
                ProductId = order.ProductId,
                SupplierId = order.SupplierId
            };
        }

        return null;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var orderToDelete = doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == id);

        if (orderToDelete != null)
        {
            orderToDelete.Remove();
            using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
            {
                await doc.SaveAsync(writer, CancellationToken.None);
            }

            return true;
        }

        return false;
    }

    public async Task<GetOrderResponseDto> GetOrderByIdAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var order = doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == id);
        if (order != null)
        {
            return new GetOrderResponseDto()
            {
                Id = (int)order.Element(XmlElements.Id),
                Quantity = (int)order.Element(XmlElements.Quantity),
                Status = (string)order.Element(XmlElements.Status),
                OrderDate = DateTime.Parse(order.Element(XmlElements.OrderDate).Value),
                ProductId = (int)order.Element(XmlElements.ProductId),
                SupplierId = (int)order.Element(XmlElements.SupplierId)
            };
        }

        return null;
    }
    public async Task<List<GetOrderResponseDto>> GetOrdersBySupplier(int supplierId, string status)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var orders = doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
            .Where(e => (int)e.Element(XmlElements.SupplierId) == supplierId && (string)e.Element(XmlElements.Status) == status)
            .Select(x => new GetOrderResponseDto()
            {
                Id = (int)x.Element(XmlElements.Id),
                Quantity = (int)x.Element(XmlElements.Quantity),
                Status = (string)x.Element(XmlElements.Status),
                OrderDate = DateTime.Parse(x.Element(XmlElements.OrderDate).Value),
                ProductId = (int)x.Element(XmlElements.ProductId),
                SupplierId = (int)x.Element(XmlElements.SupplierId),
            }).ToList();

        return orders;
    }
    public async Task<List<GetOrderResponseDto>> GetOrdersByDate(DateTime startDate, DateTime endDate)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var orders = doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
            .Where(e => DateTime.Parse(e.Element(XmlElements.OrderDate).Value) >= startDate && DateTime.Parse(e.Element(XmlElements.OrderDate).Value) <= endDate)
            .Select(x => new GetOrderResponseDto()
            {
                Id = (int)x.Element(XmlElements.Id),
                Quantity = (int)x.Element(XmlElements.Quantity),
                Status = (string)x.Element(XmlElements.Status),
                OrderDate = DateTime.Parse(x.Element(XmlElements.OrderDate).Value),
                ProductId = (int)x.Element(XmlElements.ProductId),
                SupplierId = (int)x.Element(XmlElements.SupplierId),
            }).ToList();

        return orders;
    }
    public async Task<List<GetOrderResponseDto>> GetOrdersPaginationAsync(int pageNumber, int pageSize)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var orders = doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
            .Skip((pageNumber-1)*pageSize)
            .Take(pageSize)
            .Select(x => new GetOrderResponseDto()
            {
                Id = (int)x.Element(XmlElements.Id),
                Quantity = (int)x.Element(XmlElements.Quantity),
                Status = (string)x.Element(XmlElements.Status),
                OrderDate = DateTime.Parse(x.Element(XmlElements.OrderDate).Value),
                ProductId = (int)x.Element(XmlElements.ProductId),
                SupplierId = (int)x.Element(XmlElements.SupplierId),
            }).ToList();

        return orders;
    }
}