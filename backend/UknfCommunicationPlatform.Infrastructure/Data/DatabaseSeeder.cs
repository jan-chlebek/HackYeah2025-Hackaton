using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Services;
using CoreUserRole = UknfCommunicationPlatform.Core.Enums.UserRole;
using EntityUserRole = UknfCommunicationPlatform.Core.Entities.UserRole;

namespace UknfCommunicationPlatform.Infrastructure.Data;

/// <summary>
/// Seeds the database with initial development data
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHashingService _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IPasswordHashingService passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with sample data
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Check if data already exists
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            await SeedRolesAndPermissionsAsync();
            await SeedUsersAsync();
            await SeedSupervisedEntitiesAsync();
            await SeedMessagesAsync();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            throw;
        }
    }

    private async Task SeedRolesAndPermissionsAsync()
    {
        _logger.LogInformation("Seeding roles and permissions...");

        var permissions = new List<Permission>
        {
            new Permission { Name = "users.read", Resource = "users", Action = "read", Description = "Can view user information" },
            new Permission { Name = "users.write", Resource = "users", Action = "write", Description = "Can create and edit users" },
            new Permission { Name = "users.delete", Resource = "users", Action = "delete", Description = "Can delete users" },
            new Permission { Name = "entities.read", Resource = "entities", Action = "read", Description = "Can view supervised entities" },
            new Permission { Name = "entities.write", Resource = "entities", Action = "write", Description = "Can create and edit entities" },
            new Permission { Name = "messages.read", Resource = "messages", Action = "read", Description = "Can view messages" },
            new Permission { Name = "messages.write", Resource = "messages", Action = "write", Description = "Can send and manage messages" },
            new Permission { Name = "reports.read", Resource = "reports", Action = "read", Description = "Can view reports" },
            new Permission { Name = "reports.write", Resource = "reports", Action = "write", Description = "Can submit and manage reports" }
        };

        await _context.Permissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();

        var roles = new List<Role>
        {
            new Role { Name = "Administrator", Description = "Full system access", IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Name = "InternalUser", Description = "UKNF internal staff", IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Name = "Supervisor", Description = "UKNF supervisor", IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Name = "ExternalUser", Description = "Supervised entity representative", IsSystemRole = true, CreatedAt = DateTime.UtcNow }
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();

        // Assign all permissions to Administrator role
        var adminRole = roles.First(r => r.Name == "Administrator");
        var rolePermissions = permissions.Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionId = p.Id
        }).ToList();

        await _context.RolePermissions.AddRangeAsync(rolePermissions);
        await _context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        _logger.LogInformation("Seeding users...");

        var adminRole = await _context.Roles.FirstAsync(r => r.Name == "Administrator");
        var internalRole = await _context.Roles.FirstAsync(r => r.Name == "InternalUser");

        var users = new List<User>
        {
            new User
            {
                FirstName = "Admin",
                LastName = "UKNF",
                Email = "admin@uknf.gov.pl",
                Phone = "+48123456789",
                PasswordHash = _passwordHasher.HashPassword("Admin123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@uknf.gov.pl",
                Phone = "+48234567890",
                PasswordHash = _passwordHasher.HashPassword("User123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Assign roles to users
        var userRoles = new List<EntityUserRole>
        {
            new EntityUserRole { UserId = users[0].Id, RoleId = adminRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[1].Id, RoleId = internalRole.Id, AssignedAt = DateTime.UtcNow }
        };

        await _context.UserRoles.AddRangeAsync(userRoles);
        await _context.SaveChangesAsync();
    }

    private async Task SeedSupervisedEntitiesAsync()
    {
        _logger.LogInformation("Seeding supervised entities...");

        var externalRole = await _context.Roles.FirstAsync(r => r.Name == "ExternalUser");

        var entities = new List<SupervisedEntity>
        {
            new SupervisedEntity
            {
                Name = "PKO Bank Polski S.A.",
                NIP = "5250006644",
                REGON = "000010016",
                KRS = "0000026438",
                EntityType = "Bank",
                UKNFCode = "BANK001",
                Street = "Puławska",
                BuildingNumber = "15",
                City = "Warszawa",
                PostalCode = "02-515",
                Country = "Polska",
                Email = "kontakt@pkobp.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "Bank Pekao S.A.",
                NIP = "5260000668",
                REGON = "014749240",
                KRS = "0000014843",
                EntityType = "Bank",
                UKNFCode = "BANK002",
                Street = "Grzybowska",
                BuildingNumber = "53/57",
                City = "Warszawa",
                PostalCode = "00-844",
                Country = "Polska",
                Email = "kontakt@pekao.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "PZU S.A.",
                NIP = "5250002750",
                REGON = "000006479",
                KRS = "0000009831",
                EntityType = "Insurance",
                UKNFCode = "INS001",
                Street = "Aleje Jerozolimskie",
                BuildingNumber = "44",
                City = "Warszawa",
                PostalCode = "00-024",
                Country = "Polska",
                Email = "kontakt@pzu.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        await _context.SupervisedEntities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Create external users for each entity
        var externalUsers = new List<User>();
        var externalUserRoles = new List<EntityUserRole>();

        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            var user = new User
            {
                FirstName = "Przedstawiciel",
                LastName = entity.Name,
                Email = entity.Email,
                Phone = $"+4856789{i:D4}",
                PasswordHash = _passwordHasher.HashPassword("External123!"),
                SupervisedEntityId = entity.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            externalUsers.Add(user);
        }

        await _context.Users.AddRangeAsync(externalUsers);
        await _context.SaveChangesAsync();

        foreach (var user in externalUsers)
        {
            externalUserRoles.Add(new EntityUserRole
            {
                UserId = user.Id,
                RoleId = externalRole.Id,
                AssignedAt = DateTime.UtcNow
            });
        }

        await _context.UserRoles.AddRangeAsync(externalUserRoles);
        await _context.SaveChangesAsync();
    }

    private async Task SeedMessagesAsync()
    {
        _logger.LogInformation("Seeding messages...");

        var internalUser = await _context.Users.FirstAsync(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var externalUser = await _context.Users.FirstAsync(u => u.SupervisedEntityId != null);

        var messages = new List<Message>
        {
            new Message
            {
                Subject = "Prośba o przesłanie raportu kwartalnego",
                Body = "Uprzejmie prosimy o przesłanie raportu finansowego za IV kwartał 2024 roku zgodnie z wymogami regulacyjnymi.",
                SenderId = internalUser.Id,
                RecipientId = externalUser.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Sent,
                SentAt = DateTime.UtcNow.AddDays(-5),
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddDays(-4)
            },
            new Message
            {
                Subject = "Raport kwartalny - dostarczone",
                Body = "W załączeniu przesyłamy żądany raport finansowy za IV kwartał 2024.",
                SenderId = externalUser.Id,
                RecipientId = internalUser.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Inbox,
                SentAt = DateTime.UtcNow.AddDays(-3),
                IsRead = false
            },
            new Message
            {
                Subject = "Harmonogram kontroli",
                Body = "Informujemy o zaplanowanej kontroli w dniach 20-22 stycznia 2025.",
                SenderId = internalUser.Id,
                RecipientId = externalUser.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Sent,
                SentAt = DateTime.UtcNow.AddDays(-1),
                IsRead = false
            }
        };

        await _context.Messages.AddRangeAsync(messages);
        await _context.SaveChangesAsync();
    }
}
