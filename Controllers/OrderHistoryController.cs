using Microsoft.AspNetCore.Mvc;
using EcbMartService.Data;
using EcbMartService.Models;
using Microsoft.EntityFrameworkCore;

namespace EcbMartService.Controllers
{
    [ApiController]
    [Route("api/order-history")]
    public class OrderHistoryController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;

        public OrderHistoryController(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet("all")]
        public IActionResult GetAllOrdersSummary()
        {
            //Get all orders and group by customerOrderId and get the total price from CustomerOrder table
            var orders = _dbContext.CustomerOrders
                .GroupBy(o => o.id)
                .Select(g => new OrderHistory
                {
                    orderId = g.Key.ToString(),
                    orderDate = g.First().orderDate.ToString(),
                    orderStatus = g.First().orderStatus.ToString(),
                    orderTotal = g.Sum(o => o.total).ToString()
                });

            return Ok(orders);
        }

        [HttpGet("{id:guid}")]
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
                order.orderStatus,
                Items = order.Items.Select(i => new
                {
                    i.id,
                    i.productId,
                    i.productName,
                    i.productPrice,
                    i.productImage,
                    i.quantity
                    // DO NOT include i.customerOrder here!
                })
            });
        }
    }
}