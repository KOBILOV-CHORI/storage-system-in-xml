using System.Xml;
using System.Xml.Linq;
using Domain.DTOs.CategoryDTOs;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.CategoryServices;

public class CategoryService : ICategoryService
{
    private readonly string? _pathData;

    public CategoryService(IConfiguration configuration)
    {
        _pathData = configuration.GetConnectionString(XmlElements.PathData);

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(XmlElements.Source, new XElement(XmlElements.Categories))
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
                    new XElement(XmlElements.Source, new XElement(XmlElements.Categories))
                );
                doc.Save(_pathData);
            }
            else
            {
                var groupsElement = doc.Root.Element(XmlElements.Categories);
                if (groupsElement == null)
                {
                    groupsElement = new XElement(XmlElements.Categories);
                    doc.Root.Add(groupsElement);
                    doc.Save(_pathData);
                }
            }
        }
    }

    public async Task<GetCategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto category)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        int newId = doc.Root.Element(XmlElements.Categories).Elements(XmlElements.Category)
            .Max(x => (int?)x.Element(XmlElements.Id)) ?? 0;
        newId++;
        XElement newCategory = new XElement(XmlElements.Category,
            new XElement(XmlElements.Id, newId),
            new XElement(XmlElements.Name, category.Name),
            new XElement(XmlElements.Description, category.Description)
        );
        doc.Root.Element(XmlElements.Categories).Add(newCategory);
        using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
        {
            await doc.SaveAsync(writer, CancellationToken.None);
        }

        return new GetCategoryResponseDto()
        {
            Id = newId,
            Name = category.Name,
            Description = category.Description,
        };
    }

    public async Task<List<GetCategoryResponseDto>> GetAllCategoriesAsync()
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var categories = doc.Root.Element(XmlElements.Categories).Elements(XmlElements.Category)
            .Select(x => new GetCategoryResponseDto()
            {
                Id = (int)x.Element(XmlElements.Id),
                Name = (string)x.Element(XmlElements.Name),
                Description = (string)x.Element(XmlElements.Description)
            }).ToList();

        return categories;
    }

    public async Task<GetCategoryResponseDto> UpdateCategoryAsync(UpdateCategoryRequestDto category)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var categoryToUpdate = doc.Root.Element(XmlElements.Categories).Elements(XmlElements.Category)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == category.Id);

        if (categoryToUpdate != null)
        {
            categoryToUpdate.SetElementValue(XmlElements.Name, category.Name);
            categoryToUpdate.SetElementValue(XmlElements.Description, category.Description);
            using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
            {
                await doc.SaveAsync(writer, CancellationToken.None);
            }

            return new GetCategoryResponseDto()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        return null;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var categoryToDelete = doc.Root.Element(XmlElements.Categories).Elements(XmlElements.Category)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == id);

        if (categoryToDelete != null)
        {
            categoryToDelete.Remove();
            using (var writer = XmlWriter.Create(_pathData, new XmlWriterSettings { Async = true }))
            {
                await doc.SaveAsync(writer, CancellationToken.None);
            }

            return true;
        }

        return false;
    }

    public async Task<GetCategoryResponseDto> GetCategoryByIdAsync(int id)
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var category = doc.Root.Element(XmlElements.Categories).Elements(XmlElements.Category)
            .FirstOrDefault(x => (int)x.Element(XmlElements.Id) == id);
        if (category != null)
        {
            return new GetCategoryResponseDto()
            {
                Id = (int)category.Element(XmlElements.Id),
                Name = (string)category.Element(XmlElements.Name),
                Description = (string)category.Element(XmlElements.Description)
            };
        }
        return null;
    }
    public async Task<List<CategoryWithCountProductDto>> GetCategoryWithCountProductAsync()
    {
        XDocument doc;
        using (var stream = File.OpenRead(_pathData))
        {
            doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        var categories = (from c in doc.Root.Element(XmlElements.Categories).Elements(XmlElements.Category)
            join p in doc.Root.Element(XmlElements.Products).Elements(XmlElements.Product)
                on (int)c.Element(XmlElements.Id) equals (int)p.Element(XmlElements.CategoryId)
            group p by c into g
            select new CategoryWithCountProductDto()
            {
                Id = (int)g.Key.Element(XmlElements.Id),
                Name = (string)g.Key.Element(XmlElements.Name),
                Description = (string)g.Key.Element(XmlElements.Description),
                CountOfProducts = g.Count()
            }).ToList();

        return categories;
    }
}