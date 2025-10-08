using System.ComponentModel.DataAnnotations;

namespace EcbMartService.Models
{
    public class DeliveryAddress
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid UserId { get; set; }
        
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
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation property
        public User User { get; set; }
    }
}
