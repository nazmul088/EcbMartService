using Microsoft.EntityFrameworkCore;
using EcbMartService.Models;

namespace EcbMartService.Data
{
    public class AddProductDbContext : DbContext
    {
        public AddProductDbContext(DbContextOptions<AddProductDbContext> options)
            : base(options) { }

        public DbSet<AddProduct> Products { get; set; }
    }
}
