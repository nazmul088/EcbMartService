using Microsoft.EntityFrameworkCore;
using EcbMartService.Models;

namespace EcbMartService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
