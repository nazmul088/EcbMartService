namespace EcbMartService.Models
{
    public class CustomerOrder
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string name { get; set; }
        public string address { get; set; }
        public string mobileNumber { get; set; }
        public string paymentMethod { get; set; }
        public decimal total { get; set; }
        public DateTime orderDate { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid productId { get; set; }

        // Snapshot fields
        public string? productName { get; set; }
        public decimal? productPrice { get; set; }
        public string? productImage { get; set; }
        public int quantity { get; set; }

        // Foreign key to CustomerOrder
        public Guid customerOrderId { get; set; }
        public CustomerOrder? customerOrder { get; set; }
    }
}
