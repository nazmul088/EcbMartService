using Microsoft.AspNetCore.Mvc;
using EcbMartService.Data;
using EcbMartService.Models;
using Microsoft.EntityFrameworkCore;

namespace EcbMartService.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;

        public OrderController(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddOrder([FromBody] CustomerOrder order)
        {
            if (order == null)
            {
                return BadRequest("Order data is required.");
            }

            foreach (var item in order.Items)
            {
                var product = _dbContext.Products.FirstOrDefault(p => p.id == item.productId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {item.id} not found.");
                }
                item.productName = product.name;
                item.productImage = product.svgImage;
                item.productPrice = product.price;
                item.customerOrder = order;
            }

            order.orderDate = DateTime.UtcNow;
            _dbContext.CustomerOrders.Add(order);
            _dbContext.SaveChanges();

            return Ok(new
            {
                message = "Order placed successfully",
                order = new
                {
                    order.id,
                    order.name,
                    order.address,
                    order.mobileNumber,
                    order.paymentMethod,
                    order.total,
                    order.orderDate,
                    Items = order.Items.Select(i => new
                    {
                        i.id,
                        i.productId,
                        i.productName,
                        i.productPrice,
                        i.productImage,
                        i.quantity
                    })
                }
            });
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _dbContext.CustomerOrders
                .Include(o => o.Items)
                .ToList();

            return Ok(orders.Select(order => new
            {
                order.id,
                order.name,
                order.address,
                order.mobileNumber,
                order.paymentMethod,
                order.total,
                order.orderDate,
                Items = order.Items.Select(i => new
                {
                    i.id,
                    i.productId,
                    i.productName,
                    i.productPrice,
                    i.productImage,
                    i.quantity
                })
            }));
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(Guid id)
        {
            var order = _dbContext.CustomerOrders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.id == id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok(new
            {
                order.id,
                order.name,
                order.address,
                order.mobileNumber,
                order.paymentMethod,
                order.total,
                order.orderDate,
                Items = order.Items.Select(i => new
                {
                    i.id,
                    i.productId,
                    i.productName,
                    i.productPrice,
                    i.productImage,
                    i.quantity
                })
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _dbContext.CustomerOrders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.id == id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            _dbContext.CustomerOrders.Remove(order);
            _dbContext.SaveChanges();

            return Ok(new
            {
                message = $"Order with ID {id} deleted successfully."
            });
        }
    }
}
