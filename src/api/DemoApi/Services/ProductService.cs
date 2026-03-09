using DemoApi.Configuration;
using DemoApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DemoApi.Services;

public class ProductService
{
    private readonly IMongoCollection<Product> _products;

    public ProductService(MongoDbService mongoDbService, IOptions<MongoDbSettings> settings)
    {
        _products = mongoDbService.GetCollection<Product>(
            settings.Value.ProductsCollectionName);
    }

    public virtual async Task<List<Product>> GetAllAsync() =>
        await _products.Find(_ => true).ToListAsync();

    public virtual async Task<Product?> GetByIdAsync(string id) =>
        await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
}