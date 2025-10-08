using Microsoft.AspNetCore.Mvc;
using EcbMartService.Data;
using EcbMartService.Models;
using Microsoft.EntityFrameworkCore;
using EcbMartService.Utils;
using System.Security.Claims;

namespace EcbMartService.Controllers
{
    [ApiController]
    [Route("api/order-history")]
    [RequireAuth]
    public class OrderHistoryController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly UserDbContext _userDbContext;

        public OrderHistoryController(OrderDbContext dbContext, UserDbContext userDbContext)
        {
            _dbContext = dbContext;
            _userDbContext = userDbContext;
        }
        
        [HttpGet("all")]
        public IActionResult GetAllOrdersSummary()
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new { message = "User identity missing" });
            }

            // Get the user from the database
            var user = _userDbContext.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            // Get orders for the specific user only
            var orders = _dbContext.CustomerOrders
                .Where(o => o.customerId == user.Id)
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
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new { message = "User identity missing" });
            }

            // Get the user from the database
            var user = _userDbContext.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            // Get order for the specific user only
            var order = _dbContext.CustomerOrders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.id == id && o.customerId == user.Id);

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