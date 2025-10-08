namespace EcbMartService.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PhoneNumber { get; set; }
        public string? AuthToken { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string? dateOfBirth { get; set; }
        
        // Navigation property for delivery addresses
        public ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();
    }
}