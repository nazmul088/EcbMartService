using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcbMartService.Models;
using EcbMartService.Utils;
using EcbMartService.Data;
using System.ComponentModel.DataAnnotations;

namespace EcbMartService.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [RequireAuth]
    public class ProfileController : ControllerBase
    {
        private readonly UserDbContext _userDbContext;

        public ProfileController(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
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

            return Ok(new
            {
                id = user.Id,
                phoneNumber = user.PhoneNumber,
                firstName = user.firstName,
                lastName = user.lastName,
                email = user.email,
                dateOfBirth = user.dateOfBirth,
                deliveryAddresses = user.DeliveryAddresses.Select(da => new
                {
                    id = da.Id,
                    phoneNumber = da.PhoneNumber,
                    addressLine1 = da.AddressLine1,
                    addressLine2 = da.AddressLine2,
                    city = da.City,
                    isDefault = da.IsDefault,
                    createdAt = da.CreatedAt
                })
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
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

            // Update user profile fields
            if (!string.IsNullOrEmpty(request.FirstName))
                user.firstName = request.FirstName;
            
            if (!string.IsNullOrEmpty(request.LastName))
                user.lastName = request.LastName;
            
            if (!string.IsNullOrEmpty(request.Email))
                user.email = request.Email;
            
            if (!string.IsNullOrEmpty(request.DateOfBirth))
                user.dateOfBirth = request.DateOfBirth;

            await _userDbContext.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully" });
        }
    }

    // DTOs for profile management
    public class UpdateProfileRequest
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        public string? DateOfBirth { get; set; }
    }
}
