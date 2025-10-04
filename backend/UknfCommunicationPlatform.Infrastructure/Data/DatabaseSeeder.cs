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
            await SeedReportsAsync();

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
        var supervisorRole = await _context.Roles.FirstAsync(r => r.Name == "Supervisor");

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
            },
            new User
            {
                FirstName = "Anna",
                LastName = "Nowak",
                Email = "anna.nowak@uknf.gov.pl",
                Phone = "+48345678901",
                PasswordHash = _passwordHasher.HashPassword("Supervisor123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FirstName = "Piotr",
                LastName = "Wiśniewski",
                Email = "piotr.wisniewski@uknf.gov.pl",
                Phone = "+48456789012",
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
            new EntityUserRole { UserId = users[1].Id, RoleId = internalRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[2].Id, RoleId = supervisorRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[3].Id, RoleId = internalRole.Id, AssignedAt = DateTime.UtcNow }
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
            },
            new SupervisedEntity
            {
                Name = "Allianz Polska S.A.",
                NIP = "5260231971",
                REGON = "012267870",
                KRS = "0000028261",
                EntityType = "Insurance",
                UKNFCode = "INS002",
                Street = "Inflancka",
                BuildingNumber = "4B",
                City = "Warszawa",
                PostalCode = "00-189",
                Country = "Polska",
                Email = "kontakt@allianz.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "ING Bank Śląski S.A.",
                NIP = "6340011111",
                REGON = "270834354",
                KRS = "0000005459",
                EntityType = "Bank",
                UKNFCode = "BANK003",
                Street = "Sokolska",
                BuildingNumber = "34",
                City = "Katowice",
                PostalCode = "40-086",
                Country = "Polska",
                Email = "kontakt@ingbank.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "mBank S.A.",
                NIP = "5260252172",
                REGON = "000026730",
                KRS = "0000025237",
                EntityType = "Bank",
                UKNFCode = "BANK004",
                Street = "Senatorska",
                BuildingNumber = "18",
                City = "Warszawa",
                PostalCode = "00-082",
                Country = "Polska",
                Email = "kontakt@mbank.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "Santander Bank Polska S.A.",
                NIP = "8961512764",
                REGON = "930041341",
                KRS = "0000008580",
                EntityType = "Bank",
                UKNFCode = "BANK005",
                Street = "pl. Władysława Andersa",
                BuildingNumber = "5",
                City = "Warszawa",
                PostalCode = "00-159",
                Country = "Polska",
                Email = "kontakt@santander.pl",
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

        var internalUsers = await _context.Users.Where(u => u.SupervisedEntityId == null).ToListAsync();
        var externalUsers = await _context.Users.Where(u => u.SupervisedEntityId != null).ToListAsync();

        var internalUser1 = internalUsers.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var internalUser2 = internalUsers.First(u => u.Email == "anna.nowak@uknf.gov.pl");
        var externalUser1 = externalUsers.First();
        var externalUser2 = externalUsers.Skip(1).First();

        var messages = new List<Message>
        {
            new Message
            {
                Subject = "Prośba o przesłanie raportu kwartalnego",
                Body = "Uprzejmie prosimy o przesłanie raportu finansowego za IV kwartał 2024 roku zgodnie z wymogami regulacyjnymi.",
                SenderId = internalUser1.Id,
                RecipientId = externalUser1.Id,
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
                SenderId = externalUser1.Id,
                RecipientId = internalUser1.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Inbox,
                SentAt = DateTime.UtcNow.AddDays(-3),
                IsRead = false
            },
            new Message
            {
                Subject = "Harmonogram kontroli",
                Body = "Informujemy o zaplanowanej kontroli w dniach 20-22 stycznia 2025.",
                SenderId = internalUser1.Id,
                RecipientId = externalUser1.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Sent,
                SentAt = DateTime.UtcNow.AddDays(-1),
                IsRead = false
            },
            new Message
            {
                Subject = "Wyjaśnienie dotyczące raportu ryzyka",
                Body = "Prosimy o wyjaśnienie rozbieżności w raporcie ryzyka za Q3 2024, w szczególności w sekcji dotyczącej ryzyka operacyjnego.",
                SenderId = internalUser2.Id,
                RecipientId = externalUser2.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Sent,
                SentAt = DateTime.UtcNow.AddDays(-7),
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddDays(-6)
            },
            new Message
            {
                Subject = "Re: Wyjaśnienie dotyczące raportu ryzyka",
                Body = "Przekazujemy szczegółowe wyjaśnienie rozbieżności. Załączamy poprawiony raport.",
                SenderId = externalUser2.Id,
                RecipientId = internalUser2.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Inbox,
                SentAt = DateTime.UtcNow.AddDays(-6),
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddDays(-5)
            },
            new Message
            {
                Subject = "Zaproszenie na szkolenie",
                Body = "Zapraszamy na szkolenie dotyczące nowych wymogów raportowania, które odbędzie się 15 lutego 2025.",
                SenderId = internalUser1.Id,
                RecipientId = externalUser1.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Sent,
                SentAt = DateTime.UtcNow.AddDays(-2),
                IsRead = false
            },
            new Message
            {
                Subject = "Pytanie dotyczące wymogów kapitałowych",
                Body = "Czy możemy uzyskać wyjaśnienie dotyczące nowych wymogów kapitałowych wprowadzonych w grudniu 2024?",
                SenderId = externalUser2.Id,
                RecipientId = internalUser2.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Inbox,
                SentAt = DateTime.UtcNow.AddHours(-12),
                IsRead = false
            },
            new Message
            {
                Subject = "Potwierdzenie otrzymania dokumentacji",
                Body = "Potwierdzamy otrzymanie kompletnej dokumentacji dotyczącej procesu due diligence.",
                SenderId = internalUser1.Id,
                RecipientId = externalUser1.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Sent,
                SentAt = DateTime.UtcNow.AddDays(-10),
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddDays(-9)
            }
        };

        await _context.Messages.AddRangeAsync(messages);
        await _context.SaveChangesAsync();
    }

    private async Task SeedReportsAsync()
    {
        _logger.LogInformation("Seeding reports...");

        var entities = await _context.SupervisedEntities.ToListAsync();
        var externalUsers = await _context.Users.Where(u => u.SupervisedEntityId != null).ToListAsync();

        var reports = new List<Report>();
        var reportStatuses = new[] { ReportStatus.Submitted, ReportStatus.ValidationSuccessful, ReportStatus.ValidationErrors, ReportStatus.QuestionedByUKNF };
        var reportTypes = new[] { "FinancialReport", "RiskReport", "ComplianceReport", "QuarterlyReport" };
        var periods = new[] { "2024-Q1", "2024-Q2", "2024-Q3", "2024-Q4" };

        int reportCounter = 1;
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            var user = externalUsers.FirstOrDefault(u => u.SupervisedEntityId == entity.Id);

            if (user == null) continue;

            // Create 3-5 reports per entity
            int reportCount = 3 + (i % 3);
            for (int j = 0; j < reportCount; j++)
            {
                var status = reportStatuses[j % reportStatuses.Length];
                var reportType = reportTypes[j % reportTypes.Length];
                var period = periods[j % periods.Length];
                var daysAgo = 30 + (j * 20);

                var report = new Report
                {
                    ReportNumber = $"RPT-{entity.UKNFCode}-{reportCounter:D4}",
                    FileName = $"report_{entity.UKNFCode}_{period}_{reportType}.pdf",
                    FilePath = $"/reports/{entity.UKNFCode}/{period}_{reportType}.pdf",
                    ReportingPeriod = period,
                    ReportType = reportType,
                    Status = status,
                    ErrorDescription = status == ReportStatus.ValidationErrors 
                        ? "Missing required financial data in section 3.2" 
                        : status == ReportStatus.QuestionedByUKNF 
                            ? "Please update the balance sheet figures" 
                            : null,
                    SubmittedAt = DateTime.UtcNow.AddDays(-daysAgo),
                    ValidatedAt = status == ReportStatus.ValidationSuccessful 
                        ? DateTime.UtcNow.AddDays(-daysAgo + 2) 
                        : null,
                    IsCorrection = j > 3,
                    OriginalReportId = j > 3 ? (long?)(reportCounter - 3) : null,
                    SupervisedEntityId = entity.Id,
                    SubmittedByUserId = user.Id
                };

                reports.Add(report);
                reportCounter++;
            }
        }

        await _context.Reports.AddRangeAsync(reports);
        await _context.SaveChangesAsync();
    }
}
