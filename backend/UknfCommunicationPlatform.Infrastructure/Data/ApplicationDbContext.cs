using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.Entities;
using UserRoleEnum = UknfCommunicationPlatform.Core.Enums.UserRole;

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

    // Core entities
    public DbSet<SupervisedEntity> SupervisedEntities { get; set; }
    public DbSet<User> Users { get; set; }

    // Communication Module entities
    public DbSet<Report> Reports { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    public DbSet<Case> Cases { get; set; }
    public DbSet<CaseDocument> CaseDocuments { get; set; }
    public DbSet<CaseHistory> CaseHistories { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<AnnouncementAttachment> AnnouncementAttachments { get; set; }
    public DbSet<AnnouncementRead> AnnouncementReads { get; set; }
    public DbSet<AnnouncementRecipient> AnnouncementRecipients { get; set; }
    public DbSet<AnnouncementHistory> AnnouncementHistories { get; set; }
    public DbSet<FileLibrary> FileLibraries { get; set; }
    public DbSet<FileLibraryPermission> FileLibraryPermissions { get; set; }
    public DbSet<FaqQuestion> FaqQuestions { get; set; }
    public DbSet<FaqRating> FaqRatings { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<ContactGroup> ContactGroups { get; set; }
    public DbSet<ContactGroupMember> ContactGroupMembers { get; set; }

    // Admin module entities
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<PasswordPolicy> PasswordPolicies { get; set; }
    public DbSet<PasswordHistory> PasswordHistories { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entities FIRST (before snake_case conversion)
        ConfigureEntities(modelBuilder);

        // Then apply snake_case naming convention to all entities automatically
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convert table names to snake_case (but specific ToTable() calls will override)
            if (entity.GetTableName() != null)
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()!));
            }

            // Convert all column names to snake_case
            foreach (var property in entity.GetProperties())
            {
                // Only apply snake_case if column name hasn't been explicitly set
                var currentColumnName = property.GetColumnName();
                if (currentColumnName == property.Name || currentColumnName == ToSnakeCase(property.Name))
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
                
                // Convert enum properties to strings automatically
                if (property.ClrType.IsEnum)
                {
                    property.SetProviderClrType(typeof(string));
                }
            }

            // Convert primary key names to snake_case
            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName() ?? string.Empty));
            }

            // Convert foreign key constraint names to snake_case
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(ToSnakeCase(foreignKey.GetConstraintName() ?? string.Empty));
            }

            // Convert index names to snake_case
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName() ?? string.Empty));
            }
        }
    }

    private void ConfigureEntities(ModelBuilder modelBuilder)
    {
        // SupervisedEntity configuration
        modelBuilder.Entity<SupervisedEntity>(entity =>
        {
            entity.ToTable("entities");
            entity.HasKey(e => e.Id);
            
            // Explicit mappings where C# property name differs from desired PostgreSQL column name
            entity.Property(e => e.UKNFCode).HasColumnName("uknf_code").IsRequired().HasMaxLength(250);
            entity.Property(e => e.Name).HasColumnName("entity_name").IsRequired().HasMaxLength(500);
            entity.Property(e => e.RegistryNumber).HasColumnName("uknf_registry_number").HasMaxLength(100);
            entity.Property(e => e.Status).HasColumnName("entity_status").HasMaxLength(250);
            entity.Property(e => e.Category).HasColumnName("entity_category").HasMaxLength(500);
            entity.Property(e => e.Sector).HasColumnName("entity_sector").HasMaxLength(500);
            entity.Property(e => e.Subsector).HasColumnName("entity_subsector").HasMaxLength(500);
            
            // Other constraints (snake_case handled automatically)
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(250);
            entity.Property(e => e.LEI).HasMaxLength(20);
            entity.Property(e => e.NIP).HasMaxLength(10);
            entity.Property(e => e.KRS).HasMaxLength(10);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(500);
            
            entity.HasIndex(e => e.UKNFCode).IsUnique();
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            
            // Explicit mappings where C# property name differs from desired PostgreSQL column name
            entity.Property(e => e.PESEL).HasColumnName("pesel_masked").HasMaxLength(250);
            entity.Property(e => e.LastPasswordChangeAt).HasColumnName("password_changed_at");
            
            // Other constraints (snake_case handled automatically)
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(250);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(250);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            // Role enum conversion handled globally in snake_case loop
            
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
            entity.HasIndex(e => e.ThreadId);
            entity.HasIndex(e => new { e.SenderId, e.SentAt });
            entity.HasIndex(e => new { e.RecipientId, e.IsRead });

            entity.HasOne(e => e.Sender)
                  .WithMany(u => u.SentMessages)
                  .HasForeignKey(e => e.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Recipient)
                  .WithMany(u => u.ReceivedMessages)
                  .HasForeignKey(e => e.RecipientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ParentMessage)
                  .WithMany(m => m.Replies)
                  .HasForeignKey(e => e.ParentMessageId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RelatedEntity)
                  .WithMany()
                  .HasForeignKey(e => e.RelatedEntityId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RelatedReport)
                  .WithMany()
                  .HasForeignKey(e => e.RelatedReportId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RelatedCase)
                  .WithMany(c => c.Messages)
                  .HasForeignKey(e => e.RelatedCaseId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // MessageAttachment configuration
        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.ToTable("message_attachments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Message)
                  .WithMany(m => m.Attachments)
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UploadedBy)
                  .WithMany()
                  .HasForeignKey(e => e.UploadedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Case configuration
        modelBuilder.Entity<Case>(entity =>
        {
            entity.ToTable("cases");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.CaseNumber).IsUnique();
            entity.HasIndex(e => new { e.SupervisedEntityId, e.Status });
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.SupervisedEntity)
                  .WithMany()
                  .HasForeignKey(e => e.SupervisedEntityId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Handler)
                  .WithMany()
                  .HasForeignKey(e => e.HandlerId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CaseDocument configuration
        modelBuilder.Entity<CaseDocument>(entity =>
        {
            entity.ToTable("case_documents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Case)
                  .WithMany(c => c.Documents)
                  .HasForeignKey(e => e.CaseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UploadedBy)
                  .WithMany()
                  .HasForeignKey(e => e.UploadedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CaseHistory configuration
        modelBuilder.Entity<CaseHistory>(entity =>
        {
            entity.ToTable("case_histories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChangeType).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.CaseId, e.ChangedAt });

            entity.HasOne(e => e.Case)
                  .WithMany(c => c.History)
                  .HasForeignKey(e => e.CaseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ChangedBy)
                  .WithMany()
                  .HasForeignKey(e => e.ChangedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Announcement configuration
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.ToTable("announcements");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.IsPublished, e.PublishedAt });
            entity.HasIndex(e => e.ExpiresAt);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // AnnouncementAttachment configuration
        modelBuilder.Entity<AnnouncementAttachment>(entity =>
        {
            entity.ToTable("announcement_attachments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Announcement)
                  .WithMany(a => a.Attachments)
                  .HasForeignKey(e => e.AnnouncementId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AnnouncementRead configuration
        modelBuilder.Entity<AnnouncementRead>(entity =>
        {
            entity.ToTable("announcement_reads");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.HasIndex(e => new { e.AnnouncementId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.ReadAt);

            entity.HasOne(e => e.Announcement)
                  .WithMany(a => a.ReadConfirmations)
                  .HasForeignKey(e => e.AnnouncementId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AnnouncementRecipient configuration
        modelBuilder.Entity<AnnouncementRecipient>(entity =>
        {
            entity.ToTable("announcement_recipients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RecipientType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PodmiotType).HasMaxLength(100);
            entity.HasIndex(e => new { e.AnnouncementId, e.RecipientType });

            entity.HasOne(e => e.Announcement)
                  .WithMany(a => a.Recipients)
                  .HasForeignKey(e => e.AnnouncementId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SupervisedEntity)
                  .WithMany()
                  .HasForeignKey(e => e.SupervisedEntityId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AnnouncementHistory configuration
        modelBuilder.Entity<AnnouncementHistory>(entity =>
        {
            entity.ToTable("announcement_histories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChangeType).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.AnnouncementId, e.ChangedAt });

            entity.HasOne(e => e.Announcement)
                  .WithMany(a => a.History)
                  .HasForeignKey(e => e.AnnouncementId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ChangedBy)
                  .WithMany()
                  .HasForeignKey(e => e.ChangedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // FileLibrary configuration
        modelBuilder.Entity<FileLibrary>(entity =>
        {
            entity.ToTable("file_libraries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Version).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.Category, e.IsCurrentVersion });
            entity.HasIndex(e => e.UploadedAt);

            entity.HasOne(e => e.ParentFile)
                  .WithMany(f => f.Versions)
                  .HasForeignKey(e => e.ParentFileId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UploadedBy)
                  .WithMany()
                  .HasForeignKey(e => e.UploadedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // FileLibraryPermission configuration
        modelBuilder.Entity<FileLibraryPermission>(entity =>
        {
            entity.ToTable("file_library_permissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PermissionType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RoleName).HasMaxLength(100);
            entity.Property(e => e.PodmiotType).HasMaxLength(100);
            entity.HasIndex(e => new { e.FileLibraryId, e.PermissionType });

            entity.HasOne(e => e.FileLibrary)
                  .WithMany(f => f.Permissions)
                  .HasForeignKey(e => e.FileLibraryId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SupervisedEntity)
                  .WithMany()
                  .HasForeignKey(e => e.SupervisedEntityId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // FaqQuestion configuration
        modelBuilder.Entity<FaqQuestion>(entity =>
        {
            entity.ToTable("faq_questions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AnonymousName).HasMaxLength(250);
            entity.Property(e => e.AnonymousEmail).HasMaxLength(500);
            entity.HasIndex(e => new { e.Status, e.Category });
            entity.HasIndex(e => e.SubmittedAt);
            entity.HasIndex(e => e.PublishedAt);

            entity.HasOne(e => e.SubmittedBy)
                  .WithMany()
                  .HasForeignKey(e => e.SubmittedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.AnsweredBy)
                  .WithMany()
                  .HasForeignKey(e => e.AnsweredByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // FaqRating configuration
        modelBuilder.Entity<FaqRating>(entity =>
        {
            entity.ToTable("faq_ratings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rating).IsRequired();
            entity.HasIndex(e => new { e.FaqQuestionId, e.UserId }).IsUnique();

            entity.HasOne(e => e.FaqQuestion)
                  .WithMany(f => f.Ratings)
                  .HasForeignKey(e => e.FaqQuestionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Contact configuration
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("contacts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Position).HasMaxLength(250);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.Department).HasMaxLength(250);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => new { e.SupervisedEntityId, e.IsPrimary });

            entity.HasOne(e => e.SupervisedEntity)
                  .WithMany()
                  .HasForeignKey(e => e.SupervisedEntityId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ContactGroup configuration
        modelBuilder.Entity<ContactGroup>(entity =>
        {
            entity.ToTable("contact_groups");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(250);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ContactGroupMember configuration
        modelBuilder.Entity<ContactGroupMember>(entity =>
        {
            entity.ToTable("contact_group_members");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ContactGroupId, e.ContactId }).IsUnique();

            entity.HasOne(e => e.ContactGroup)
                  .WithMany(g => g.Members)
                  .HasForeignKey(e => e.ContactGroupId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Contact)
                  .WithMany(c => c.ContactGroupMemberships)
                  .HasForeignKey(e => e.ContactId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Resource).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique();
        });

        // RolePermission configuration (many-to-many)
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permissions");
            entity.HasKey(e => new { e.RoleId, e.PermissionId });

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(e => e.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // UserRole configuration (many-to-many)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // PasswordPolicy configuration
        modelBuilder.Entity<PasswordPolicy>(entity =>
        {
            entity.ToTable("password_policies");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.UpdatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // PasswordHistory configuration
        modelBuilder.Entity<PasswordHistory>(entity =>
        {
            entity.ToTable("password_histories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => new { e.UserId, e.CreatedAt });

            entity.HasOne(e => e.User)
                  .WithMany(u => u.PasswordHistories)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.UserId, e.Timestamp });
            entity.HasIndex(e => new { e.Resource, e.Action });

            entity.HasOne(e => e.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CreatedByIp).HasMaxLength(50);
            entity.Property(e => e.RevokedByIp).HasMaxLength(50);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.RevocationReason).HasMaxLength(500);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => new { e.UserId, e.ExpiresAt });

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    } // End of ConfigureEntities

    /// <summary>
    /// Converts a PascalCase or camelCase string to snake_case
    /// Example: FirstName -> first_name, UserId -> user_id
    /// </summary>
    private static string ToSnakeCase(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input ?? string.Empty;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLowerInvariant(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            char currentChar = input[i];
            
            if (char.IsUpper(currentChar))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(currentChar));
            }
            else
            {
                result.Append(currentChar);
            }
        }

        return result.ToString();
    }
}
