using ContactManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}
