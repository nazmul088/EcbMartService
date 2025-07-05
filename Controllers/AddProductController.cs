using Microsoft.AspNetCore.Mvc;
using EcbMartService.Data;
using EcbMartService.Models;

namespace EcbMartService.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class AddProductController : ControllerBase
    {
        private readonly AddProductDbContext _dbContext;

        public AddProductController(AddProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddProduct([FromBody] AddProduct product)
        {
            if (product == null)
            {
                return BadRequest("Product data is required.");
            }

            product.createdAt = DateTime.UtcNow;
            product.updatedAt = DateTime.UtcNow;

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return Ok(new { message = "Product added successfully", product });
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _dbContext.Products.ToList();
            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateProduct(Guid id, [FromBody] AddProduct updatedProduct)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.id == id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            product.name = updatedProduct.name;
            product.description = updatedProduct.description;
            product.price = updatedProduct.price;
            product.quantity = updatedProduct.quantity;
            product.category = updatedProduct.category;
            product.svgImage = updatedProduct.svgImage;
            product.updatedAt = DateTime.UtcNow;

            _dbContext.SaveChanges();

            return Ok(new { message = "Product updated successfully", product });
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.id == id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return Ok(new { message = "Product deleted successfully" });
        }

    }
}
