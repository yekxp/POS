using pos_backend.Models;
using pos_backend.Models.DTOs;

namespace pos_backend.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        Task<ProductDto?> GetProductByIdAsync(string id);
        
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        
        Task<ProductDto?> UpdateProductAsync(string id, ProductDto productDto);
        
        Task<bool> DeleteProductAsync(string id);

        Task<IEnumerable<ProductDto>> GetProductsByLocations(Location[] locations);
    }

}
