using AutoMapper;
using pos_backend.Models.DTOs;
using pos_backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using pos_backend.Database;

namespace pos_backend.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _productsCollection;
        private readonly IMapper _mapper;

        public ProductService(IOptions<MongoDBSettings> settings, IMongoClient mongoClient, IMapper mapper)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _productsCollection = database.GetCollection<Product>(settings.Value.ProductsCollectionName);
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            List<Product> products = await _productsCollection.Find(p => true).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(string id)
        {
            Product product = await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            Product newProduct = _mapper.Map<Product>(productDto);
            await _productsCollection.InsertOneAsync(newProduct);
            return _mapper.Map<ProductDto>(newProduct);
        }

        public async Task<ProductDto?> UpdateProductAsync(string id, ProductDto productDto)
        {
            Product existingProduct = await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (existingProduct == null)
                return null;

            existingProduct.Name = productDto.Name;
            existingProduct.Price = productDto.Price;
            existingProduct.Description = productDto.Description;

            ReplaceOneResult result = await _productsCollection.ReplaceOneAsync(p => p.Id == id, existingProduct);

            return result.IsAcknowledged ? _mapper.Map<ProductDto>(existingProduct) : null;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            DeleteResult result = await _productsCollection.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByLocations(Location[] locations)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.AnyIn(p => p.Location, locations);

            List<Product> existingProducts = await _productsCollection.Find(filter).ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(existingProducts);
        }
    }
}
