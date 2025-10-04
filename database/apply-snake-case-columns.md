# Snake Case Column Mapping Guide

This document lists the required column name mappings to align the backend with the PostgreSQL database script.

## Already Updated

✅ **SupervisedEntity** → `entities` table  
✅ **User** → `users` table

## Column Mapping Reference

All C# properties should map to snake_case columns. Use `.HasColumnName()` in ApplicationDbContext.

### Common Patterns

| C# Property | PostgreSQL Column |
|-------------|-------------------|
| `Id` | `id` |
| `FirstName` | `first_name` |
| `LastName` | `last_name` |
| `CreatedAt` | `created_at` |
| `UpdatedAt` | `updated_at` |
| `IsActive` | `is_active` |
| `UserId` | `user_id` |
| `EntityId` | `entity_id` |
| `SupervisedEntityId` | `entity_id` (note: matches database script) |

### Report Entity Mapping

**C# Class**: `Report`  
**Table**: `reports`

```csharp
entity.Property(e => e.Id).HasColumnName("id");
entity.Property(e => e.SupervisedEntityId).HasColumnName("entity_id");
entity.Property(e => e.RegistryId).HasColumnName("registry_id");
entity.Property(e => e.ReportNumber).HasColumnName("report_identifier");
entity.Property(e => e.ReportName).HasColumnName("report_name");
entity.Property(e => e.ReportPeriod).HasColumnName("report_period");
entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
entity.Property(e => e.FileName).HasColumnName("file_name");
entity.Property(e => e.FilePath).HasColumnName("file_path");
entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
entity.Property(e => e.ValidationReportPath).HasColumnName("validation_report_path");
entity.Property(e => e.ValidationDate).HasColumnName("validation_date");
entity.Property(e => e.ChallengeDescription).HasColumnName("challenge_description");
entity.Property(e => e.SubmittedByUserId).HasColumnName("submitted_by_user_id");
entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
entity.Property(e => e.IsCorrection).HasColumnName("is_correction");
entity.Property(e => e.OriginalReportId).HasColumnName("corrects_report_id");
entity.Property(e => e.CreatedAt).HasColumnName("created_at");
entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
```

### Message Entity Mapping

**C# Class**: `Message`  
**Table**: `messages`

```csharp
entity.Property(e => e.Id).HasColumnName("id");
entity.Property(e => e.Subject).HasColumnName("subject");
entity.Property(e => e.Content).HasColumnName("content");
entity.Property(e => e.Context).HasColumnName("context").HasConversion<string>();
entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
entity.Property(e => e.SenderId).HasColumnName("created_by_user_id");
entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
entity.Property(e => e.ParentMessageId).HasColumnName("parent_thread_id");
entity.Property(e => e.ThreadId).HasColumnName("message_id");
entity.Property(e => e.RelatedEntityId).HasColumnName("entity_id");
entity.Property(e => e.RelatedReportId).HasColumnName("related_report_id");
entity.Property(e => e.RelatedCaseId).HasColumnName("related_case_id");
entity.Property(e => e.SentAt).HasColumnName("created_at");
entity.Property(e => e.IsRead).HasColumnName("is_read");
entity.Property(e => e.ReadAt).HasColumnName("read_at");
```

### Case Entity Mapping

**C# Class**: `Case`  
**Table**: `cases`

```csharp
entity.Property(e => e.Id).HasColumnName("id");
entity.Property(e => e.CaseNumber).HasColumnName("case_number");
entity.Property(e => e.Title).HasColumnName("title");
entity.Property(e => e.Description).HasColumnName("description");
entity.Property(e => e.SupervisedEntityId).HasColumnName("entity_id");
entity.Property(e => e.Category).HasColumnName("category").HasConversion<string>();
entity.Property(e => e.Priority).HasColumnName("priority").HasConversion<string>();
entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
entity.Property(e => e.HandlerId).HasColumnName("assigned_to_user_id");
entity.Property(e => e.OpenedAt).HasColumnName("opened_at");
entity.Property(e => e.ClosedAt).HasColumnName("closed_at");
entity.Property(e => e.CreatedAt).HasColumnName("created_at");
entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
```

### Announcement Entity Mapping

**C# Class**: `Announcement`  
**Table**: `announcements`

```csharp
entity.Property(e => e.Id).HasColumnName("id");
entity.Property(e => e.Title).HasColumnName("title");
entity.Property(e => e.Content).HasColumnName("content");
entity.Property(e => e.Category).HasColumnName("category");
entity.Property(e => e.Priority).HasColumnName("priority").HasConversion<string>();
entity.Property(e => e.RequiresConfirmation).HasColumnName("requires_confirmation");
entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
entity.Property(e => e.IsPublished).HasColumnName("is_published");
entity.Property(e => e.CreatedByUserId).HasColumnName("published_by_user_id");
entity.Property(e => e.PublishedAt).HasColumnName("published_at");
entity.Property(e => e.CreatedAt).HasColumnName("created_at");
entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
```

### FileLibrary Entity Mapping

**C# Class**: `FileLibrary`  
**Table**: `library_files`

```csharp
entity.Property(e => e.Id).HasColumnName("id");
entity.Property(e => e.Name).HasColumnName("file_name");
entity.Property(e => e.Description).HasColumnName("description");
entity.Property(e => e.FileName).HasColumnName("file_path");
entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
entity.Property(e => e.ContentType).HasColumnName("file_type");
entity.Property(e => e.Category).HasColumnName("category");
entity.Property(e => e.ReportPeriod).HasColumnName("report_period");
entity.Property(e => e.TemplateUpdatedDate).HasColumnName("template_updated_date");
entity.Property(e => e.Version).HasColumnName("version_status").HasConversion<string>();
entity.Property(e => e.IsCurrentVersion).HasColumnName("is_public");
entity.Property(e => e.UploadedByUserId).HasColumnName("uploaded_by_user_id");
entity.Property(e => e.UploadedAt).HasColumnName("uploaded_at");
entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
```

### Contact Entity Mapping

**C# Class**: `Contact`  
**Table**: `contacts`

```csharp
entity.Property(e => e.Id).HasColumnName("id");
entity.Property(e => e.Name).HasColumnName("first_name"); // Split name or adjust entity
entity.Property(e => e.Email).HasColumnName("email");
entity.Property(e => e.Phone).HasColumnName("phone");
entity.Property(e => e.SupervisedEntityId).HasColumnName("entity_id");
entity.Property(e => e.Position).HasColumnName("user_id"); // May need adjustment
entity.Property(e => e.CreatedAt).HasColumnName("created_at");
```

### Role & Permission Entities

**C# Class**: `Role`  
**Table**: `roles` (no change - backend roles are different from database script)

**C# Class**: `Permission`  
**Table**: `permissions` (no change)

**C# Class**: `UserRole`  
**Table**: `user_roles`

```csharp
entity.Property(e => e.UserId).HasColumnName("user_id");
entity.Property(e => e.RoleId).HasColumnName("role_id");
entity.Property(e => e.AssignedAt).HasColumnName("granted_at");
entity.Property(e => e.AssignedByUserId).HasColumnName("granted_by_user_id");
```

### AuditLog Entity Mapping

**C# Class**: `AuditLog`  
**Table**: `audit_log` (singular in database script)

```csharp
entity.Property(e => e.Id).HasColumnName("id");
entity.Property(e => e.UserId).HasColumnName("user_id");
entity.Property(e => e.Action).HasColumnName("action");
entity.Property(e => e.Resource).HasColumnName("entity_type");
entity.Property(e => e.ResourceId).HasColumnName("entity_id");
entity.Property(e => e.OldValue).HasColumnName("old_value");
entity.Property(e => e.NewValue).HasColumnName("new_value");
entity.Property(e => e.IpAddress).HasColumnName("ip_address");
entity.Property(e => e.UserAgent).HasColumnName("user_agent");
entity.Property(e => e.Timestamp).HasColumnName("performed_at");
```

### RefreshToken Entity Mapping

**C# Class**: `RefreshToken`  
**Table**: `refresh_tokens`

```csharp
entity.Property(e => e.Id).HasColumnName("id");
entity.Property(e => e.UserId).HasColumnName("user_id");
entity.Property(e => e.Token).HasColumnName("token");
entity.Property(e => e.CreatedAt).HasColumnName("created_at");
entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
entity.Property(e => e.CreatedByIp).HasColumnName("created_by_ip");
entity.Property(e => e.RevokedByIp).HasColumnName("revoked_by_ip");
entity.Property(e => e.ReplacedByToken).HasColumnName("replaced_by_token");
entity.Property(e => e.RevocationReason).HasColumnName("revocation_reason");
```

## Implementation Strategy

### Option 1: Manual Update (Tedious but Safe)
Copy the column mappings above and add them one entity at a time to `ApplicationDbContext.cs`.

### Option 2: Global Snake Case Convention (Recommended)
Override `OnModelCreating` to apply snake_case automatically:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Apply snake_case to all properties automatically
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        foreach (var property in entity.GetProperties())
        {
            property.SetColumnName(ToSnakeCase(property.Name));
        }
        
        foreach (var key in entity.GetKeys())
        {
            key.SetName(ToSnakeCase(key.GetName()));
        }
        
        foreach (var foreignKey in entity.GetForeignKeys())
        {
            foreignKey.SetConstraintName(ToSnakeCase(foreignKey.GetConstraintName()));
        }
    }

    // Then configure specific entities...
}

private static string ToSnakeCase(string? input)
{
    if (string.IsNullOrEmpty(input)) return input ?? string.Empty;
    
    var result = new StringBuilder();
    result.Append(char.ToLowerInvariant(input[0]));
    
    for (int i = 1; i < input.Length; i++)
    {
        if (char.IsUpper(input[i]))
        {
            result.Append('_');
            result.Append(char.ToLowerInvariant(input[i]));
        }
        else
        {
            result.Append(input[i]);
        }
    }
    
    return result.ToString();
}
```

### Option 3: Incremental Migration
1. Keep PascalCase for now
2. Create a new migration that renames columns
3. Deploy gradually

## Next Steps

1. Choose implementation strategy (recommend Option 2)
2. Update `ApplicationDbContext.cs`
3. Create new migration: `dotnet ef migrations add SnakeCaseColumns`
4. Review generated migration SQL
5. Apply to database: `dotnet ef database update`

## Testing

After applying changes:

```sql
-- Verify column names match
SELECT column_name 
FROM information_schema.columns 
WHERE table_name = 'users'
ORDER BY ordinal_position;

-- Should return: id, first_name, last_name, email, etc.
```

## Rollback Plan

If issues occur:
```bash
# Rollback last migration
dotnet ef database update PreviousMigrationName

# Remove migration files
dotnet ef migrations remove
```
