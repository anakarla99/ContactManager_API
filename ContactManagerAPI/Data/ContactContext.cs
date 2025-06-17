using Microsoft.EntityFrameworkCore;
using ContactManagerApi.Models;

namespace ContactManagerApi.Data
{
    public class ContactContext : DbContext
    {
        public ContactContext(DbContextOptions<ContactContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Validación adicional para User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Validación adicional para Contact
            modelBuilder.Entity<Contact>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Contact>()
                .Property(c => c.DateOfBirth)
                .HasColumnType("date"); // opcional, si quieres limitar precisión
        }
    }
}
