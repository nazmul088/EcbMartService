namespace EcbMartService.Models
{
    public class AddProduct
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public required string name { get; set; }
        public string? description { get; set; }
        public required decimal price { get; set; }
        public required int quantity { get; set; }
        public required string category { get; set; }
        public required string imageUrl { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }
}
