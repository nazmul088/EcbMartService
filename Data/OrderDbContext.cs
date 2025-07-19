using Microsoft.EntityFrameworkCore;
using EcbMartService.Models;

namespace EcbMartService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<AddProduct> Products { get; set; }
    }
}
