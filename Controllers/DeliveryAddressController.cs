using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcbMartService.Models;
using EcbMartService.Utils;
using EcbMartService.Data;
using System.ComponentModel.DataAnnotations;

namespace EcbMartService.Controllers
{
    [ApiController]
    [Route("api/delivery-addresses")]
    [RequireAuth]
    public class DeliveryAddressController : ControllerBase
    {
        private readonly UserDbContext _userDbContext;

        public DeliveryAddressController(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetDeliveryAddresses()
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userDbContext.Users
                .Include(u => u.DeliveryAddresses)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var addresses = user.DeliveryAddresses.Select(da => new
            {
                id = da.Id,
                phoneNumber = da.PhoneNumber,
                addressLine1 = da.AddressLine1,
                addressLine2 = da.AddressLine2,
                city = da.City,
                isDefault = da.IsDefault,
                createdAt = da.CreatedAt
            }).OrderByDescending(a => a.isDefault).ThenBy(a => a.createdAt);

            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeliveryAddress([FromBody] CreateDeliveryAddressRequest request)
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userDbContext.Users
                .Include(u => u.DeliveryAddresses)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // If this is set as default, unset other default addresses
            if (request.IsDefault)
            {
                foreach (var address in user.DeliveryAddresses)
                {
                    address.IsDefault = false;
                }
            }

            var deliveryAddress = new DeliveryAddress
            {
                UserId = user.Id,
                PhoneNumber = request.PhoneNumber,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.UtcNow
            };

            _userDbContext.DeliveryAddresses.Add(deliveryAddress);
            await _userDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDeliveryAddress), new { id = deliveryAddress.Id }, new
            {
                id = deliveryAddress.Id,
                phoneNumber = deliveryAddress.PhoneNumber,
                addressLine1 = deliveryAddress.AddressLine1,
                addressLine2 = deliveryAddress.AddressLine2,
                city = deliveryAddress.City,
                isDefault = deliveryAddress.IsDefault,
                createdAt = deliveryAddress.CreatedAt
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryAddress(Guid id)
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userDbContext.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var address = await _userDbContext.DeliveryAddresses
                .FirstOrDefaultAsync(da => da.Id == id && da.UserId == user.Id);

            if (address == null)
            {
                return NotFound(new { message = "Delivery address not found" });
            }

            return Ok(new
            {
                id = address.Id,
                phoneNumber = address.PhoneNumber,
                addressLine1 = address.AddressLine1,
                addressLine2 = address.AddressLine2,
                city = address.City,
                isDefault = address.IsDefault,
                createdAt = address.CreatedAt,
                updatedAt = address.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeliveryAddress(Guid id, [FromBody] UpdateDeliveryAddressRequest request)
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userDbContext.Users
                .Include(u => u.DeliveryAddresses)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var address = await _userDbContext.DeliveryAddresses
                .FirstOrDefaultAsync(da => da.Id == id && da.UserId == user.Id);

            if (address == null)
            {
                return NotFound(new { message = "Delivery address not found" });
            }

            // If this is set as default, unset other default addresses
            if (request.IsDefault && !address.IsDefault)
            {
                foreach (var addr in user.DeliveryAddresses.Where(a => a.Id != id))
                {
                    addr.IsDefault = false;
                }
            }

            // Update address fields
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                address.PhoneNumber = request.PhoneNumber;
            
            if (!string.IsNullOrEmpty(request.AddressLine1))
                address.AddressLine1 = request.AddressLine1;
            
            address.AddressLine2 = request.AddressLine2;
            
            if (!string.IsNullOrEmpty(request.City))
                address.City = request.City;
            
            address.IsDefault = request.IsDefault;
            address.UpdatedAt = DateTime.UtcNow;

            await _userDbContext.SaveChangesAsync();

            return Ok(new { message = "Delivery address updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeliveryAddress(Guid id)
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userDbContext.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var address = await _userDbContext.DeliveryAddresses
                .FirstOrDefaultAsync(da => da.Id == id && da.UserId == user.Id);

            if (address == null)
            {
                return NotFound(new { message = "Delivery address not found" });
            }

            _userDbContext.DeliveryAddresses.Remove(address);
            await _userDbContext.SaveChangesAsync();

            return Ok(new { message = "Delivery address deleted successfully" });
        }

        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefaultDeliveryAddress(Guid id)
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userDbContext.Users
                .Include(u => u.DeliveryAddresses)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var address = await _userDbContext.DeliveryAddresses
                .FirstOrDefaultAsync(da => da.Id == id && da.UserId == user.Id);

            if (address == null)
            {
                return NotFound(new { message = "Delivery address not found" });
            }

            // Unset all other default addresses
            foreach (var addr in user.DeliveryAddresses)
            {
                addr.IsDefault = false;
            }

            // Set this address as default
            address.IsDefault = true;
            address.UpdatedAt = DateTime.UtcNow;

            await _userDbContext.SaveChangesAsync();

            return Ok(new { message = "Default delivery address updated successfully" });
        }
    }

    // DTOs for delivery address management
    public class CreateDeliveryAddressRequest
    {
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string AddressLine1 { get; set; }

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        public bool IsDefault { get; set; } = false;
    }

    public class UpdateDeliveryAddressRequest
    {
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? AddressLine1 { get; set; }

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        public bool IsDefault { get; set; }
    }
}
