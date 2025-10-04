using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.Entities;

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

    public DbSet<SupervisedEntity> SupervisedEntities { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SupervisedEntity configuration
        modelBuilder.Entity<SupervisedEntity>(entity =>
        {
            entity.ToTable("supervised_entities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UKNFCode).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.UKNFCode).IsUnique();
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(250);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(250);
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.SupervisedEntity)
                  .WithMany(s => s.Users)
                  .HasForeignKey(e => e.SupervisedEntityId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Report configuration
        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("reports");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReportNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.ReportNumber).IsUnique();

            entity.HasOne(e => e.SupervisedEntity)
                  .WithMany(s => s.Reports)
                  .HasForeignKey(e => e.SupervisedEntityId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SubmittedBy)
                  .WithMany(u => u.Reports)
                  .HasForeignKey(e => e.SubmittedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OriginalReport)
                  .WithMany(r => r.Corrections)
                  .HasForeignKey(e => e.OriginalReportId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Message configuration
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(e => e.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Recipient)
                  .WithMany()
                  .HasForeignKey(e => e.RecipientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
