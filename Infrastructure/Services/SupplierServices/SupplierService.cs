using System.Xml;
using System.Xml.Linq;
using Domain.DTOs.SupplierDTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.SupplierServices;

public class SupplierService : ISupplierService
{
    private readonly string? _pathData;

    public SupplierService(IConfiguration configuration)
    {
        _pathData = configuration.GetConnectionString(XmlElements.PathData);

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(XmlElements.Source, new XElement(XmlElements.Suppliers))
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
                    new XElement(XmlElements.Source, new XElement(XmlElements.Suppliers))
                );
                doc.Save(_pathData);
            }
            else
            {
                var groupsElement = doc.Root.Element(XmlElements.Suppliers);
                if (groupsElement == null)
                {
                    groupsElement = new XElement(XmlElements.Suppliers);
                    doc.Root.Add(groupsElement);
                    doc.Save(_pathData);
                }
            }
        }
    }

    public async Task<GetSupplierResponseDto> CreateSupplierAsync(CreateSupplierRequestDto supplier)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        int newId = doc.Root.Element(XmlElements.Suppliers).Elements(XmlElements.Supplier)
            .Max(x => (int?)x.Element(XmlElements.Id)) ?? 0;
        newId++;
        XElement newSupplier = new XElement(XmlElements.Supplier,
            new XElement(XmlElements.Id, newId),
            new XElement(XmlElements.Name, supplier.Name),
            new XElement(XmlElements.ContactPerson, supplier.ContactPerson),
            new XElement(XmlElements.Email, supplier.Email),
            new XElement(XmlElements.Phone, supplier.Phone)
        );
        doc.Root.Element(XmlElements.Suppliers).Add(newSupplier);
        using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
        {
            await doc.SaveAsync(writer, CancellationToken.None);
        }

        return new GetSupplierResponseDto()
        {
            Id = newId,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            Name = supplier.Name,
            Phone = supplier.Phone
        };
    }

    public async Task<List<GetSupplierResponseDto>> GetAllSuppliersAsync()
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var suppliers = doc.Root.Element(XmlElements.Suppliers).Elements(XmlElements.Supplier)
            .Select(x => new GetSupplierResponseDto()
            {
                Id = (int)x.Element(XmlElements.Id),
                Name = (string)x.Element(XmlElements.Name),
                ContactPerson = (string)x.Element(XmlElements.ContactPerson),
                Email = (string)x.Element(XmlElements.Email),
                Phone = (string)x.Element(XmlElements.Phone)
            }).ToList();

        return suppliers;
    }

    public async Task<GetSupplierResponseDto> UpdateSupplierAsync(UpdateSupplierRequestDto supplier)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var supplierToUpdate = doc.Root.Element(XmlElements.Suppliers).Elements(XmlElements.Supplier)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == supplier.Id);

        if (supplierToUpdate != null)
        {
            supplierToUpdate.SetElementValue(XmlElements.Name, supplier.Name);
            supplierToUpdate.SetElementValue(XmlElements.ContactPerson, supplier.ContactPerson);
            supplierToUpdate.SetElementValue(XmlElements.Email, supplier.Email);
            supplierToUpdate.SetElementValue(XmlElements.Phone, supplier.Phone);
            using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
            {
                await doc.SaveAsync(writer, CancellationToken.None);
            }

            return new GetSupplierResponseDto()
            {
                Id = supplier.Id,
                ContactPerson = supplier.ContactPerson,
                Email = supplier.Email,
                Name = supplier.Name,
                Phone = supplier.Phone
            };
        }

        return null;
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var supplierToDelete = doc.Root.Element(XmlElements.Suppliers).Elements(XmlElements.Supplier)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == id);

        if (supplierToDelete != null)
        {
            supplierToDelete.Remove();
            using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
            {
                await doc.SaveAsync(writer, CancellationToken.None);
            }

            return true;
        }

        return false;
    }

    public async Task<GetSupplierResponseDto> GetSupplierByIdAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var supplier = doc.Root.Element(XmlElements.Suppliers).Elements(XmlElements.Supplier)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == id);

        return new GetSupplierResponseDto()
        {
            Id = (int)supplier.Element(XmlElements.Id),
            Name = (string)supplier.Element(XmlElements.Name),
            ContactPerson = (string)supplier.Element(XmlElements.ContactPerson),
            Email = (string)supplier.Element(XmlElements.Email),
            Phone = (string)supplier.Element(XmlElements.Phone)
        };
    }
    public async Task<List<GetSupplierResponseDto>> GetSuppliersByCountProductAsync(int minQuantity)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var suppliers = (from s in  doc.Root.Element(XmlElements.Suppliers).Elements(XmlElements.Supplier)
            join o in doc.Root.Element(XmlElements.Orders).Elements(XmlElements.Order) 
                on (int)s.Element(XmlElements.Id) equals (int)o.Element(XmlElements.SupplierId)
            join p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product) 
                on (int)o.Element(XmlElements.ProductId) equals (int)p.Element(XmlElements.Id)
            where (int)p.Element(XmlElements.Quantity) < minQuantity 
            select new GetSupplierResponseDto()
            {
                Id = (int)s.Element(XmlElements.Id),
                Name = (string)s.Element(XmlElements.Name),
                ContactPerson = (string)s.Element(XmlElements.ContactPerson),
                Email = (string)s.Element(XmlElements.Email),
                Phone = (string)s.Element(XmlElements.Phone)
            }).ToList();

        return suppliers;
    }
}