using System.Xml;
using System.Xml.Linq;
using Domain.DTOs.ProductDTOs;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.ProductServices;

public class ProductService : IProductService
{
    private readonly string? _pathData;

    public ProductService(IConfiguration configuration)
    {
        _pathData = configuration.GetConnectionString(XmlElements.PathData);

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(XmlElements.Source, new XElement(XmlElements.Products))
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
                    new XElement(XmlElements.Source, new XElement(XmlElements.Products))
                );
                doc.Save(_pathData);
            }
            else
            {
                var groupsElement = doc.Root.Element(XmlElements.Products);
                if (groupsElement == null)
                {
                    groupsElement = new XElement(XmlElements.Products);
                    doc.Root.Add(groupsElement);
                    doc.Save(_pathData);
                }
            }
        }
    }

    public async Task<GetProductResponseDto> CreateProductAsync(CreateProductRequestDto product)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        int newId = doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            .Max(x => (int?)x.Element(XmlElements.Id)) ?? 0;
        newId++;
        XElement newProduct = new XElement(XmlElements.Product,
            new XElement(XmlElements.Id, newId),
            new XElement(XmlElements.Name, product.Name),
            new XElement(XmlElements.Description, product.Description),
            new XElement(XmlElements.Price, product.Price),
            new XElement(XmlElements.Quantity, product.Quantity),
            new XElement(XmlElements.CategoryId, product.CategoryId)
        );
        doc.Root.Element(XmlElements.Products).Add(newProduct);
        using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
        {
            await doc.SaveAsync(writer, CancellationToken.None);
        }

        return new GetProductResponseDto()
        {
            Id = newId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            CategoryId = product.CategoryId
        };
    }

    public async Task<List<GetProductResponseDto>> GetAllProductsAsync()
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var products = doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            .Select(x => new GetProductResponseDto()
            {
                Id = (int)x.Element(XmlElements.Id),
                Name = (string)x.Element(XmlElements.Name),
                Description = (string)x.Element(XmlElements.Description),
                Price = (decimal)x.Element(XmlElements.Price),
                Quantity = (int)x.Element(XmlElements.Quantity),
                CategoryId = (int)x.Element(XmlElements.CategoryId)
            }).ToList();

        return products;
    }

    public async Task<GetProductResponseDto> UpdateProductAsync(UpdateProductRequestDto product)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var productToUpdate = doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == product.Id);

        if (productToUpdate != null)
        {
            productToUpdate.SetElementValue(XmlElements.Name, product.Name);
            productToUpdate.SetElementValue(XmlElements.Description, product.Description);
            productToUpdate.SetElementValue(XmlElements.Price, product.Price);
            productToUpdate.SetElementValue(XmlElements.Quantity, product.Quantity);
            productToUpdate.SetElementValue(XmlElements.CategoryId, product.CategoryId);
            using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
            {
                await doc.SaveAsync(writer, CancellationToken.None);
            }

            return new GetProductResponseDto()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId
            };
        }

        return null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var productToDelete = doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == id);

        if (productToDelete != null)
        {
            productToDelete.Remove();
            using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
            {
                await doc.SaveAsync(writer, CancellationToken.None);
            }

            return true;
        }

        return false;
    }

    public async Task<GetProductResponseDto> GetProductByIdAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var product = doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == id);

        if (product != null)
        {
            return new GetProductResponseDto()
            {
                Id = (int)product.Element(XmlElements.Id),
                Name = (string)product.Element(XmlElements.Name),
                Description = (string)product.Element(XmlElements.Description),
                Price = (decimal)product.Element(XmlElements.Price),
                Quantity = (int)product.Element(XmlElements.Quantity),
                CategoryId = (int)product.Element(XmlElements.CategoryId)
            };
        }

        return null;
    }

    public async Task<List<GetProductResponseDto>> GetProductsByCategoryIdAsync(int categoryId, string sortBy,
        string sortOrder)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        List<GetProductResponseDto> products = new List<GetProductResponseDto>();
        switch (sortBy.ToLower())
        {
            case "name":
            case "description":
                if (sortOrder == "desc")
                {
                    products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
                        where (int)p.Element(XmlElements.CategoryId) == categoryId
                        orderby (string)p.Element(sortBy) descending
                        select new GetProductResponseDto()
                        {
                            Id = (int)p.Element(XmlElements.Id),
                            Name = (string)p.Element(XmlElements.Name),
                            Description = (string)p.Element(XmlElements.Description),
                            Price = (decimal)p.Element(XmlElements.Price),
                            Quantity = (int)p.Element(XmlElements.Quantity),
                            CategoryId = (int)p.Element(XmlElements.CategoryId)
                        }).ToList();
                }
                else
                {
                    products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
                        where (int)p.Element(XmlElements.CategoryId) == categoryId
                        orderby (string)p.Element(sortBy)
                        select new GetProductResponseDto()
                        {
                            Id = (int)p.Element(XmlElements.Id),
                            Name = (string)p.Element(XmlElements.Name),
                            Description = (string)p.Element(XmlElements.Description),
                            Price = (decimal)p.Element(XmlElements.Price),
                            Quantity = (int)p.Element(XmlElements.Quantity),
                            CategoryId = (int)p.Element(XmlElements.CategoryId)
                        }).ToList();
                }

                break;
            case "id":
            case "quantity":
                if (sortOrder == "desc")
                {
                    products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
                        where (int)p.Element(XmlElements.CategoryId) == categoryId
                        orderby (int)p.Element(sortBy) descending
                        select new GetProductResponseDto()
                        {
                            Id = (int)p.Element(XmlElements.Id),
                            Name = (string)p.Element(XmlElements.Name),
                            Description = (string)p.Element(XmlElements.Description),
                            Price = (decimal)p.Element(XmlElements.Price),
                            Quantity = (int)p.Element(XmlElements.Quantity),
                            CategoryId = (int)p.Element(XmlElements.CategoryId)
                        }).ToList();
                }
                else
                {
                    products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
                        where (int)p.Element(XmlElements.CategoryId) == categoryId
                        orderby (int)p.Element(sortBy)
                        select new GetProductResponseDto()
                        {
                            Id = (int)p.Element(XmlElements.Id),
                            Name = (string)p.Element(XmlElements.Name),
                            Description = (string)p.Element(XmlElements.Description),
                            Price = (decimal)p.Element(XmlElements.Price),
                            Quantity = (int)p.Element(XmlElements.Quantity),
                            CategoryId = (int)p.Element(XmlElements.CategoryId)
                        }).ToList();
                }

                break;
            case "price":
                if (sortOrder == "desc")
                {
                    products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
                        where (int)p.Element(XmlElements.CategoryId) == categoryId
                        orderby (decimal)p.Element(sortBy) descending
                        select new GetProductResponseDto()
                        {
                            Id = (int)p.Element(XmlElements.Id),
                            Name = (string)p.Element(XmlElements.Name),
                            Description = (string)p.Element(XmlElements.Description),
                            Price = (decimal)p.Element(XmlElements.Price),
                            Quantity = (int)p.Element(XmlElements.Quantity),
                            CategoryId = (int)p.Element(XmlElements.CategoryId)
                        }).ToList();
                }
                else
                {
                    products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
                        where (int)p.Element(XmlElements.CategoryId) == categoryId
                        orderby (decimal)p.Element(sortBy)
                        select new GetProductResponseDto()
                        {
                            Id = (int)p.Element(XmlElements.Id),
                            Name = (string)p.Element(XmlElements.Name),
                            Description = (string)p.Element(XmlElements.Description),
                            Price = (decimal)p.Element(XmlElements.Price),
                            Quantity = (int)p.Element(XmlElements.Quantity),
                            CategoryId = (int)p.Element(XmlElements.CategoryId)
                        }).ToList();
                }

                break;
        }

        return products;
    }

    public async Task<List<GetProductResponseDto>> GetProductsByMaxAsync(int maxQuantity)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var products = doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            .Where(e => (int)e.Element(XmlElements.Quantity) < maxQuantity)
            .Select(x => new GetProductResponseDto()
            {
                Id = (int)x.Element(XmlElements.Id),
                Name = (string)x.Element(XmlElements.Name),
                Description = (string)x.Element(XmlElements.Description),
                Price = (decimal)x.Element(XmlElements.Price),
                Quantity = (int)x.Element(XmlElements.Quantity),
                CategoryId = (int)x.Element(XmlElements.CategoryId)
            }).ToList();

        return products;
    }

    public async Task<List<GetProductDetailsDto>> GetProductDetailsAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            join c in doc.Root.Element(XmlElements.Categories).Elements(XmlElements.Category) 
                on (int)p.Element(XmlElements.CategoryId) equals (int)c.Element(XmlElements.Id)
            join o in doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order) 
                on (int)p.Element(XmlElements.Id) equals (int)o.Element(XmlElements.ProductId)
            join s in doc.Root.Element(XmlElements.Suppliers).Elements(XmlElements.Supplier) 
                on (int)o.Element(XmlElements.SupplierId) equals (int)s.Element(XmlElements.Id)
            where (int)p.Element(XmlElements.Id) == id 
            select new GetProductDetailsDto()
            {
                Id = (int)p.Element(XmlElements.Id),
                Name = (string)p.Element(XmlElements.Name),
                Description = (string)p.Element(XmlElements.Description),
                Price = (decimal)p.Element(XmlElements.Price),
                Quantity = (int)p.Element(XmlElements.Quantity),
                CategoryId = (int)c.Element(XmlElements.Id),
                SupplierId = (int)s.Element(XmlElements.Id),
                Email = (string)s.Element(XmlElements.Email),
                Phone = (string)s.Element(XmlElements.Phone),
                CategoryDescription = (string)c.Element(XmlElements.Description),
                CategoryName = (string)c.Element(XmlElements.Name),
                ContactPerson = (string)c.Element(XmlElements.ContactPerson),
                SupplierName = (string)s.Element(XmlElements.Name)
            }).ToList();

        return products;
    }
    public async Task<List<GetProductDetailsDto>> GetProductDetailsPaginationAsync(int pageNumber, int pageSize)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            join c in doc.Root.Element(XmlElements.Categories).Elements(XmlElements.Category) 
                on (int)p.Element(XmlElements.CategoryId) equals (int)c.Element(XmlElements.Id)
            join o in doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order) 
                on (int)p.Element(XmlElements.Id) equals (int)o.Element(XmlElements.ProductId)
            join s in doc.Root.Element(XmlElements.Suppliers).Elements(XmlElements.Supplier) 
                on (int)o.Element(XmlElements.SupplierId) equals (int)s.Element(XmlElements.Id)
            select new GetProductDetailsDto()
            {
                Id = (int)p.Element(XmlElements.Id),
                Name = (string)p.Element(XmlElements.Name),
                Description = (string)p.Element(XmlElements.Description),
                Price = (decimal)p.Element(XmlElements.Price),
                Quantity = (int)p.Element(XmlElements.Quantity),
                CategoryId = (int)c.Element(XmlElements.Id),
                SupplierId = (int)s.Element(XmlElements.Id),
                Email = (string)s.Element(XmlElements.Email),
                Phone = (string)s.Element(XmlElements.Phone),
                CategoryDescription = (string)c.Element(XmlElements.Description),
                CategoryName = (string)c.Element(XmlElements.Name),
                ContactPerson = (string)c.Element(XmlElements.ContactPerson),
                SupplierName = (string)s.Element(XmlElements.Name)
            }).Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();

        return products;
    }
    public async Task<List<GetProductResponseDto>> GetMostOrderedProductsAsync(int minOrders)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var products = (from p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
            join o in doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order)
                on (int)p.Element(XmlElements.Id) equals (int)o.Element(XmlElements.ProductId)
            group o by p into g
            where g.Count() >= minOrders
            orderby g.Count() descending 
            select new GetProductResponseDto()
            {
                Id = (int)g.Key.Element(XmlElements.Id),
                Name = (string)g.Key.Element(XmlElements.Name),
                Description = (string)g.Key.Element(XmlElements.Description),
                Price = (decimal)g.Key.Element(XmlElements.Price),
                Quantity = (int)g.Key.Element(XmlElements.Quantity),
                CategoryId = (int)g.Key.Element(XmlElements.CategoryId)
            }).ToList();

        return products;
    }
}