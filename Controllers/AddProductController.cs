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
    }
}
