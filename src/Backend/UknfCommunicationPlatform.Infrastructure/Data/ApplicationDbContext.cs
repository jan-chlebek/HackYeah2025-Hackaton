using Microsoft.EntityFrameworkCore;

namespace UknfCommunicationPlatform.Infrastructure.Data;

/// <summary>
/// Application database context for PostgreSQL
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure your entities here
        // Example:
        // modelBuilder.Entity<YourEntity>()
        //     .ToTable("your_table_name")
        //     .HasKey(e => e.Id);
    }
}
