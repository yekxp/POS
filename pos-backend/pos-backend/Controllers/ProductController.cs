using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pos_backend.Models;
using pos_backend.Models.DTOs;
using pos_backend.Services;

namespace pos_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            IEnumerable<ProductDto> products = await _productService.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpGet("locations")]
        [Authorize(Roles = "Manager,CashierBA,CashierKE")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByLocation([FromQuery] Location[] location)
        {
            IEnumerable<ProductDto> products = await _productService.GetProductsByLocations(location);

            if (products is null || !products.Any())
                return NotFound();

            return Ok(products);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager,CashierBA,CashierKE")]
        public async Task<ActionResult<ProductDto>> GetProductById(string id)
        {
            ProductDto product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ProductDto createdProduct = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(string id, [FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ProductDto updatedProduct = await _productService.UpdateProductAsync(id, productDto);
            if (updatedProduct == null)
                return NotFound();

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            bool result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
