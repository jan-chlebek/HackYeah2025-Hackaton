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
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Check if data already exists
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Disable change tracking for better performance during bulk inserts
            var originalAutoDetectChanges = _context.ChangeTracker.AutoDetectChangesEnabled;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            await SeedRolesAndPermissionsAsync();
            await SeedUsersAsync();
            await SeedSupervisedEntitiesAsync();

            // Save users and entities first, as messages and reports depend on them
            await _context.SaveChangesAsync();

            await SeedMessagesAsync();
            await SeedReportsAsync();
            await SeedCasesAsync();
            await SeedAnnouncementsAsync();
            await SeedFileLibrariesAsync();
            await SeedFaqQuestionsAsync();
            await SeedContactsAsync();
            await SeedPasswordPolicyAsync();

            await _context.SaveChangesAsync();

            // Re-enable change tracking
            _context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;

            await transaction.CommitAsync();
            _logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
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
            // Administrators
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
                FirstName = "Katarzyna",
                LastName = "Administratorska",
                Email = "k.administratorska@uknf.gov.pl",
                Phone = "+48123456790",
                PasswordHash = _passwordHasher.HashPassword("Admin123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            // Internal Users
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
                FirstName = "Piotr",
                LastName = "Wiśniewski",
                Email = "piotr.wisniewski@uknf.gov.pl",
                Phone = "+48456789012",
                PasswordHash = _passwordHasher.HashPassword("User123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FirstName = "Marek",
                LastName = "Dąbrowski",
                Email = "marek.dabrowski@uknf.gov.pl",
                Phone = "+48567890123",
                PasswordHash = _passwordHasher.HashPassword("User123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FirstName = "Tomasz",
                LastName = "Lewandowski",
                Email = "tomasz.lewandowski@uknf.gov.pl",
                Phone = "+48678901234",
                PasswordHash = _passwordHasher.HashPassword("User123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FirstName = "Krzysztof",
                LastName = "Zieliński",
                Email = "krzysztof.zielinski@uknf.gov.pl",
                Phone = "+48789012345",
                PasswordHash = _passwordHasher.HashPassword("User123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            // Supervisors
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
                FirstName = "Magdalena",
                LastName = "Szymańska",
                Email = "magdalena.szymanska@uknf.gov.pl",
                Phone = "+48890123456",
                PasswordHash = _passwordHasher.HashPassword("Supervisor123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FirstName = "Michał",
                LastName = "Woźniak",
                Email = "michal.wozniak@uknf.gov.pl",
                Phone = "+48901234567",
                PasswordHash = _passwordHasher.HashPassword("Supervisor123!"),
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
            new EntityUserRole { UserId = users[1].Id, RoleId = adminRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[2].Id, RoleId = internalRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[3].Id, RoleId = internalRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[4].Id, RoleId = internalRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[5].Id, RoleId = internalRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[6].Id, RoleId = internalRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[7].Id, RoleId = supervisorRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[8].Id, RoleId = supervisorRole.Id, AssignedAt = DateTime.UtcNow },
            new EntityUserRole { UserId = users[9].Id, RoleId = supervisorRole.Id, AssignedAt = DateTime.UtcNow }
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
            // Banks
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
            },
            new SupervisedEntity
            {
                Name = "Bank Millennium S.A.",
                NIP = "5260369496",
                REGON = "930229369",
                KRS = "0000010186",
                EntityType = "Bank",
                UKNFCode = "BANK006",
                Street = "Stanisława Żaryna",
                BuildingNumber = "2A",
                City = "Warszawa",
                PostalCode = "02-593",
                Country = "Polska",
                Email = "kontakt@bankmillennium.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "BNP Paribas Bank Polska S.A.",
                NIP = "5260255516",
                REGON = "142537384",
                KRS = "0000011571",
                EntityType = "Bank",
                UKNFCode = "BANK007",
                Street = "Suwak",
                BuildingNumber = "3",
                City = "Warszawa",
                PostalCode = "02-676",
                Country = "Polska",
                Email = "kontakt@bnpparibas.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "Bank Handlowy w Warszawie S.A.",
                NIP = "5260203821",
                REGON = "000028168",
                KRS = "0000000001",
                EntityType = "Bank",
                UKNFCode = "BANK008",
                Street = "Senatorska",
                BuildingNumber = "16",
                City = "Warszawa",
                PostalCode = "00-923",
                Country = "Polska",
                Email = "kontakt@citihandlowy.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            // Insurance Companies
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
                Name = "WARTA S.A.",
                NIP = "5270007625",
                REGON = "000001525",
                KRS = "0000014766",
                EntityType = "Insurance",
                UKNFCode = "INS003",
                Street = "Chmielna",
                BuildingNumber = "85/87",
                City = "Warszawa",
                PostalCode = "00-805",
                Country = "Polska",
                Email = "kontakt@warta.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "ERGO Hestia S.A.",
                NIP = "5830026457",
                REGON = "190067320",
                KRS = "0000024529",
                EntityType = "Insurance",
                UKNFCode = "INS004",
                Street = "Hestii",
                BuildingNumber = "1",
                City = "Sopot",
                PostalCode = "81-731",
                Country = "Polska",
                Email = "kontakt@ergohestia.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "Generali T.U. S.A.",
                NIP = "5260007625",
                REGON = "015770190",
                KRS = "0000030993",
                EntityType = "Insurance",
                UKNFCode = "INS005",
                Street = "Postępu",
                BuildingNumber = "15B",
                City = "Warszawa",
                PostalCode = "02-676",
                Country = "Polska",
                Email = "kontakt@generali.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            // Investment Funds
            new SupervisedEntity
            {
                Name = "PKO TFI S.A.",
                NIP = "5262683988",
                REGON = "010316020",
                KRS = "0000019308",
                EntityType = "InvestmentFund",
                UKNFCode = "FUND001",
                Street = "Puławska",
                BuildingNumber = "15",
                City = "Warszawa",
                PostalCode = "02-515",
                Country = "Polska",
                Email = "kontakt@pkotfi.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "PZU TFI S.A.",
                NIP = "5260266060",
                REGON = "012546352",
                KRS = "0000019090",
                EntityType = "InvestmentFund",
                UKNFCode = "FUND002",
                Street = "Aleje Jerozolimskie",
                BuildingNumber = "44",
                City = "Warszawa",
                PostalCode = "00-024",
                Country = "Polska",
                Email = "kontakt@pzutfi.pl",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new SupervisedEntity
            {
                Name = "Investor TFI S.A.",
                NIP = "5262649651",
                REGON = "010267813",
                KRS = "0000019308",
                EntityType = "InvestmentFund",
                UKNFCode = "FUND003",
                Street = "Puławska",
                BuildingNumber = "2",
                City = "Warszawa",
                PostalCode = "02-566",
                Country = "Polska",
                Email = "kontakt@investor.pl",
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

        // Use actual users that were created in SeedUsersAsync
        var internalUser1 = internalUsers.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var internalUser2 = internalUsers.First(u => u.Email == "piotr.wisniewski@uknf.gov.pl");
        var internalUser3 = internalUsers.First(u => u.Email == "marek.dabrowski@uknf.gov.pl");
        var internalUser4 = internalUsers.First(u => u.Email == "tomasz.lewandowski@uknf.gov.pl");

        var messages = new List<Message>();

        // Create 20 messages with varied conversations
        for (int i = 0; i < 20; i++)
        {
            var externalUser = externalUsers[i % externalUsers.Count];
            var internalUser = internalUsers[i % internalUsers.Count];
            var daysAgo = i / 2 + 1;
            bool isFromInternal = i % 3 != 2; // Most messages from internal users

            var messageSubjects = new[]
            {
                "Prośba o przesłanie raportu kwartalnego",
                "Raport kwartalny - dostarczone",
                "Harmonogram kontroli",
                "Wyjaśnienie dotyczące raportu ryzyka",
                "Re: Wyjaśnienie dotyczące raportu ryzyka",
                "Zaproszenie na szkolenie",
                "Pytanie dotyczące wymogów kapitałowych",
                "Potwierdzenie otrzymania dokumentacji",
                "Prośba o uzupełnienie danych",
                "Re: Prośba o uzupełnienie danych",
                "Nowe wytyczne UKNF",
                "Konsultacje dotyczące strategii biznesowej",
                "Weryfikacja compliance",
                "Terminarz raportowania 2025",
                "Kwestie dotyczące płynności finansowej",
                "Aktualizacja polityki bezpieczeństwa",
                "Spotkanie w sprawie audytu",
                "Dokumentacja procesu KYC",
                "Zmiana danych kontaktowych",
                "Potwierdzenie rejestracji nowego produktu"
            };

            var messageBodies = new[]
            {
                "Uprzejmie prosimy o przesłanie raportu finansowego za IV kwartał 2024 roku zgodnie z wymogami regulacyjnymi.",
                "W załączeniu przesyłamy żądany raport finansowy za IV kwartał 2024.",
                "Informujemy o zaplanowanej kontroli w dniach 20-22 stycznia 2025.",
                "Prosimy o wyjaśnienie rozbieżności w raporcie ryzyka za Q3 2024, w szczególności w sekcji dotyczącej ryzyka operacyjnego.",
                "Przekazujemy szczegółowe wyjaśnienie rozbieżności. Załączamy poprawiony raport.",
                "Zapraszamy na szkolenie dotyczące nowych wymogów raportowania, które odbędzie się 15 lutego 2025.",
                "Czy możemy uzyskać wyjaśnienie dotyczące nowych wymogów kapitałowych wprowadzonych w grudniu 2024?",
                "Potwierdzamy otrzymanie kompletnej dokumentacji dotyczącej procesu due diligence.",
                "W związku z prowadzoną analizą prosimy o uzupełnienie danych za okres ostatnich 6 miesięcy.",
                "Przekazujemy uzupełnione dane zgodnie z Państwa prośbą.",
                "Informujemy o nowych wytycznych dotyczących raportowania ryzyka operacyjnego, obowiązujących od 1 marca 2025.",
                "Prosimy o możliwość umówienia konsultacji dotyczących planowanej restrukturyzacji.",
                "W ramach procesu weryfikacji compliance prosimy o dostarczenie dokumentacji polityki AML/CFT.",
                "Przesyłamy szczegółowy terminarz raportowania dla podmiotów nadzorowanych na rok 2025.",
                "Prosimy o przedstawienie planu zapewnienia płynności w kontekście aktualnej sytuacji rynkowej.",
                "Informujemy o konieczności aktualizacji polityki bezpieczeństwa IT zgodnie z nowymi wymogami RODO.",
                "Uprzejmie prosimy o potwierdzenie udziału w spotkaniu dotyczącym przygotowań do audytu.",
                "Prosimy o uzupełnienie dokumentacji procesu KYC dla nowych klientów korporacyjnych.",
                "Informujemy o zmianie danych kontaktowych naszego przedstawiciela ds. regulacji.",
                "Potwierdzamy rejestrację nowego produktu inwestycyjnego zgodnie z otrzymaną dokumentacją."
            };

            var statusWiadomosciOptions = new[] { "Odpowiedziano", "Oczekuje na odpowiedź UKNF", "Oczekuje na odpowiedź podmiotu", "Nowa" };
            var priorytetOptions = new[] { "Wysoki", "Średni", "Niski" };
            var sygnaturaSprawyOptions = new[] { "001/2025", "002/2025", "003/2025", "004/2025", "005/2025" };

            Message message;
            if (isFromInternal)
            {
                message = new Message
                {
                    Subject = messageSubjects[i],
                    Body = messageBodies[i],
                    SenderId = internalUser.Id,
                    RecipientId = externalUser.Id,
                    Status = MessageStatus.Sent,
                    Folder = MessageFolder.Sent,
                    SentAt = DateTime.UtcNow.AddDays(-daysAgo),
                    IsRead = i % 4 != 0,
                    ReadAt = i % 4 != 0 ? DateTime.UtcNow.AddDays(-daysAgo).AddHours(6) : null
                };
            }
            else
            {
                message = new Message
                {
                    Subject = messageSubjects[i],
                    Body = messageBodies[i],
                    SenderId = externalUser.Id,
                    RecipientId = internalUser.Id,
                    Status = MessageStatus.Sent,
                    Folder = MessageFolder.Inbox,
                    SentAt = DateTime.UtcNow.AddDays(-daysAgo),
                    IsRead = i % 3 != 0,
                    ReadAt = i % 3 != 0 ? DateTime.UtcNow.AddDays(-daysAgo).AddHours(3) : null
                };
            }

            messages.Add(message);
        }

        await _context.Messages.AddRangeAsync(messages);
        await _context.SaveChangesAsync();
    }

    private async Task SeedReportsAsync()
    {
        _logger.LogInformation("Seeding reports...");

        var externalUsers = await _context.Users.Where(u => u.SupervisedEntityId != null).ToListAsync();

        if (!externalUsers.Any())
        {
            _logger.LogWarning("No external users found. Skipping report seeding.");
            return;
        }

        var reports = new List<Report>();
        var reportStatuses = new[] 
        { 
            ReportStatus.Submitted, 
            ReportStatus.ValidationSuccessful, 
            ReportStatus.ValidationErrors, 
            ReportStatus.QuestionedByUKNF,
            ReportStatus.Draft 
        };
        
        var periods = new[] 
        { 
            ReportingPeriod.Q1, 
            ReportingPeriod.Q2, 
            ReportingPeriod.Q3, 
            ReportingPeriod.Q4 
        };

        // Generate fake XLSX file content (ZIP header for valid XLSX)
        var fakeXlsxContent = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00 };

        int reportCounter = 1;
        
        // Create 5-7 reports per user to ensure minimum 5 total
        int reportsPerUser = Math.Max(5, (int)Math.Ceiling(25.0 / externalUsers.Count));
        
        foreach (var user in externalUsers.Take(5)) // Take first 5 users to avoid too many reports
        {
            for (int j = 0; j < reportsPerUser; j++)
            {
                var status = reportStatuses[j % reportStatuses.Length];
                var period = periods[j % periods.Length];
                var daysAgo = 30 + (j * 15);
                var year = 2024 + (j / 4); // Some reports from 2024, some from 2025

                var report = new Report
                {
                    ReportNumber = $"RPT-{year}-{reportCounter:D4}",
                    FileName = $"raport_kwartalny_{period}_{year}.xlsx",
                    FileContent = fakeXlsxContent,
                    ReportingPeriod = period,
                    Status = status,
                    SubmittedAt = DateTime.UtcNow.AddDays(-daysAgo),
                    SubmittedByUserId = user.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-daysAgo),
                    UpdatedAt = DateTime.UtcNow.AddDays(-daysAgo + 2)
                };

                reports.Add(report);
                reportCounter++;
            }
        }

        await _context.Reports.AddRangeAsync(reports);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Seeded {Count} reports", reports.Count);
    }

    private async Task SeedCasesAsync()
    {
        _logger.LogInformation("Seeding cases...");

        var entities = await _context.SupervisedEntities.Take(5).ToListAsync();
        var internalUsers = await _context.Users.Where(u => u.SupervisedEntityId == null).Take(3).ToListAsync();
        var externalUsers = await _context.Users.Where(u => u.SupervisedEntityId != null).Take(5).ToListAsync();

        var cases = new List<Case>();
        var caseCategories = new[] { "RegistrationDataChange", "PersonnelChange", "EntitySummons", "SystemPermissions", "Reporting" };
        var casePriorities = new[] { 1, 2, 3, 2, 3 }; // 1=Low, 2=Medium, 3=High
        var caseStatuses = new[] { CaseStatus.New, CaseStatus.InProgress, CaseStatus.AwaitingUserResponse, CaseStatus.InProgress, CaseStatus.Resolved };

        for (int i = 0; i < 5; i++)
        {
            var caseEntity = new Case
            {
                CaseNumber = $"CASE-2025-{(i + 1):D4}",
                Title = $"Sprawa {caseCategories[i]} - {entities[i].Name}",
                Description = $"Opis sprawy dotyczącej {caseCategories[i].ToLower()}. Wymaga weryfikacji i akceptacji przez UKNF.",
                Category = caseCategories[i],
                Priority = casePriorities[i],
                Status = caseStatuses[i],
                SupervisedEntityId = entities[i].Id,
                HandlerId = i < 3 ? internalUsers[i].Id : null,
                CreatedByUserId = externalUsers[i].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-20 + i * 4),
                UpdatedAt = DateTime.UtcNow.AddDays(-10 + i * 2)
            };
            cases.Add(caseEntity);
        }

        await _context.Cases.AddRangeAsync(cases);
        await _context.SaveChangesAsync();

        // Seed CaseDocuments (at least 5)
        var caseDocuments = new List<CaseDocument>();
        for (int i = 0; i < 5; i++)
        {
            caseDocuments.Add(new CaseDocument
            {
                CaseId = cases[i % cases.Count].Id,
                DocumentName = $"Dokument sprawy {i + 1}",
                FileName = $"case_document_{i + 1}.pdf",
                FilePath = $"/cases/documents/case_document_{i + 1}.pdf",
                ContentType = "application/pdf",
                FileSize = 50000 + i * 10000,
                UploadedByUserId = externalUsers[i % externalUsers.Count].Id,
                UploadedAt = DateTime.UtcNow.AddDays(-15 + i * 3)
            });
        }
        await _context.CaseDocuments.AddRangeAsync(caseDocuments);
        await _context.SaveChangesAsync();

        // Seed CaseHistory (at least 5)
        var caseHistories = new List<CaseHistory>();
        for (int i = 0; i < 5; i++)
        {
            caseHistories.Add(new CaseHistory
            {
                CaseId = cases[i % cases.Count].Id,
                ChangeType = i % 2 == 0 ? "StatusChange" : "DocumentAdded",
                OldStatus = i % 2 == 0 ? CaseStatus.New : null,
                NewStatus = i % 2 == 0 ? CaseStatus.InProgress : null,
                ChangedByUserId = internalUsers[i % internalUsers.Count].Id,
                ChangedAt = DateTime.UtcNow.AddDays(-12 + i * 2),
                Description = $"Zmiana w sprawie: {(i % 2 == 0 ? "status zaktualizowany" : "dodano dokument")}"
            });
        }
        await _context.CaseHistories.AddRangeAsync(caseHistories);
        await _context.SaveChangesAsync();
    }

    private async Task SeedAnnouncementsAsync()
    {
        _logger.LogInformation("Seeding announcements...");

        var internalUsers = await _context.Users.Where(u => u.SupervisedEntityId == null).Take(3).ToListAsync();
        var entities = await _context.SupervisedEntities.Take(5).ToListAsync();
        var externalUsers = await _context.Users.Where(u => u.SupervisedEntityId != null).Take(5).ToListAsync();

        var announcements = new List<Announcement>();
        var priorities = new[] { AnnouncementPriority.High, AnnouncementPriority.Medium, AnnouncementPriority.Low, AnnouncementPriority.Medium, AnnouncementPriority.High };
        var categories = new[] { "System", "Reporting", "Training", "Legal", "General" };

        for (int i = 0; i < 5; i++)
        {
            var announcement = new Announcement
            {
                Title = $"Komunikat {categories[i]} - {i + 1}",
                Content = $"<p>Treść komunikatu dotyczącego <strong>{categories[i]}</strong>.</p><p>Prosimy o zapoznanie się z załączonymi informacjami.</p>",
                Category = categories[i],
                Priority = priorities[i],
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-30 + i * 6),
                ExpiresAt = DateTime.UtcNow.AddDays(30 + i * 10),
                CreatedByUserId = internalUsers[i % internalUsers.Count].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-35 + i * 6),
                UpdatedAt = DateTime.UtcNow.AddDays(-30 + i * 6)
            };
            announcements.Add(announcement);
        }

        await _context.Announcements.AddRangeAsync(announcements);
        await _context.SaveChangesAsync();

        // Seed AnnouncementRecipients (at least 5)
        var recipients = new List<AnnouncementRecipient>();
        for (int i = 0; i < 5; i++)
        {
            recipients.Add(new AnnouncementRecipient
            {
                AnnouncementId = announcements[i].Id,
                RecipientType = i % 2 == 0 ? "Entity" : "User",
                SupervisedEntityId = i % 2 == 0 ? entities[i % entities.Count].Id : null,
                UserId = i % 2 != 0 ? externalUsers[i % externalUsers.Count].Id : null
            });
        }
        await _context.AnnouncementRecipients.AddRangeAsync(recipients);
        await _context.SaveChangesAsync();

        // Seed AnnouncementReads (at least 5)
        var reads = new List<AnnouncementRead>();
        for (int i = 0; i < 5; i++)
        {
            reads.Add(new AnnouncementRead
            {
                AnnouncementId = announcements[i % announcements.Count].Id,
                UserId = externalUsers[i % externalUsers.Count].Id,
                ReadAt = DateTime.UtcNow.AddDays(-25 + i * 5),
                IpAddress = $"192.168.1.{100 + i}"
            });
        }
        await _context.AnnouncementReads.AddRangeAsync(reads);
        await _context.SaveChangesAsync();

        // Seed AnnouncementAttachments (at least 5)
        var attachments = new List<AnnouncementAttachment>();
        for (int i = 0; i < 5; i++)
        {
            attachments.Add(new AnnouncementAttachment
            {
                AnnouncementId = announcements[i % announcements.Count].Id,
                FileName = $"announcement_attachment_{i + 1}.pdf",
                FilePath = $"/announcements/attachments/announcement_attachment_{i + 1}.pdf",
                ContentType = "application/pdf",
                FileSize = 100000 + i * 20000,
                UploadedAt = DateTime.UtcNow.AddDays(-32 + i * 6)
            });
        }
        await _context.AnnouncementAttachments.AddRangeAsync(attachments);
        await _context.SaveChangesAsync();

        // Seed AnnouncementHistories (at least 5)
        var histories = new List<AnnouncementHistory>();
        for (int i = 0; i < 5; i++)
        {
            histories.Add(new AnnouncementHistory
            {
                AnnouncementId = announcements[i % announcements.Count].Id,
                ChangeType = i % 3 == 0 ? "Created" : i % 3 == 1 ? "Published" : "Updated",
                ChangedByUserId = internalUsers[i % internalUsers.Count].Id,
                ChangedAt = DateTime.UtcNow.AddDays(-33 + i * 6),
                Description = $"Komunikat został {(i % 3 == 0 ? "utworzony" : i % 3 == 1 ? "opublikowany" : "zaktualizowany")}"
            });
        }
        await _context.AnnouncementHistories.AddRangeAsync(histories);
        await _context.SaveChangesAsync();
    }

    private async Task SeedFileLibrariesAsync()
    {
        _logger.LogInformation("Seeding file libraries...");

        var internalUsers = await _context.Users.Where(u => u.SupervisedEntityId == null).Take(3).ToListAsync();
        var entities = await _context.SupervisedEntities.Take(5).ToListAsync();

        var files = new List<FileLibrary>();
        var categories = new[] { "Templates", "Guidelines", "LegalDocuments", "Reports", "Training" };

        for (int i = 0; i < 5; i++)
        {
            files.Add(new FileLibrary
            {
                Name = $"Dokument {categories[i]} {i + 1}",
                Description = $"Opis dokumentu kategorii {categories[i]}",
                FileName = $"file_{categories[i].ToLower()}_{i + 1}.xlsx",
                FileSize = 150000 + i * 30000,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Category = categories[i],
                FileContent = new byte[] { 0x50, 0x4B, 0x03, 0x04 }, // ZIP header (XLSX is ZIP)
                UploadedByUserId = internalUsers[i % internalUsers.Count].Id,
                UploadedAt = DateTime.UtcNow.AddDays(-60 + i * 12)
            });
        }

        await _context.FileLibraries.AddRangeAsync(files);
        await _context.SaveChangesAsync();

        // Seed FileLibraryPermissions (at least 5)
        var permissions = new List<FileLibraryPermission>();
        for (int i = 0; i < 5; i++)
        {
            permissions.Add(new FileLibraryPermission
            {
                FileLibraryId = files[i].Id,
                PermissionType = i % 2 == 0 ? "SupervisedEntity" : "User",
                SupervisedEntityId = i % 2 == 0 ? entities[i % entities.Count].Id : null,
                UserId = i % 2 != 0 ? (await _context.Users.Where(u => u.SupervisedEntityId != null).Skip(i).FirstOrDefaultAsync())?.Id : null
            });
        }
        await _context.FileLibraryPermissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();
    }

    private async Task SeedFaqQuestionsAsync()
    {
        _logger.LogInformation("Seeding FAQ questions...");

        var questions = new List<FaqQuestion>
        {
            new FaqQuestion
            {
                Question = "Jak mogę zalogować się do systemu?",
                Answer = "<p>Aby zalogować się do systemu, należy:</p><ol><li>Przejść na stronę logowania</li><li>Wprowadzić swój login i hasło</li><li>Kliknąć przycisk \"Zaloguj\"</li></ol><p>W przypadku problemów z logowaniem, skontaktuj się z administratorem systemu.</p>"
            },
            new FaqQuestion
            {
                Question = "Jak często należy składać raporty kwartalne?",
                Answer = "<p>Raporty kwartalne należy składać cztery razy w roku:</p><ul><li>Q1 - do 30 kwietnia</li><li>Q2 - do 31 lipca</li><li>Q3 - do 31 października</li><li>Q4 - do 31 stycznia roku następnego</li></ul><p>Zaleca się składanie raportów z wyprzedzeniem, aby uniknąć problemów technicznych w ostatnim dniu.</p>"
            },
            new FaqQuestion
            {
                Question = "Jakie formaty plików są akceptowane przy wysyłaniu raportów?",
                Answer = "<p>System akceptuje wyłącznie pliki w formacie <strong>XLSX</strong> (Microsoft Excel).</p><p>Pliki w innych formatach (XLS, CSV, PDF) nie będą przyjmowane. Upewnij się, że plik został zapisany w odpowiednim formacie przed przesłaniem.</p>"
            },
            new FaqQuestion
            {
                Question = "Jak mogę zmienić dane mojego podmiotu nadzorowanego?",
                Answer = "<p>Aby zmienić dane podmiotu nadzorowanego:</p><ol><li>Przejdź do zakładki \"Administracja\"</li><li>Wybierz \"Podmioty nadzorowane\"</li><li>Znajdź swój podmiot na liście i kliknij \"Edytuj\"</li><li>Wprowadź zmiany i zapisz</li></ol><p>Zmiany krytycznych danych (np. NIP, REGON) wymagają zatwierdzenia przez pracownika UKNF.</p>"
            },
            new FaqQuestion
            {
                Question = "Gdzie mogę znaleźć archiwalne raporty?",
                Answer = "<p>Archiwalne raporty znajdują się w zakładce <strong>\"Biblioteka plików\"</strong>.</p><p>Możesz filtrować raporty według:</p><ul><li>Okresu sprawozdawczego (kwartał)</li><li>Daty złożenia</li><li>Statusu walidacji</li></ul><p>Każdy raport można pobrać w formacie XLSX.</p>"
            },
            new FaqQuestion
            {
                Question = "Co zrobić w przypadku problemów technicznych?",
                Answer = "<p>W przypadku problemów technicznych:</p><ol><li>Sprawdź sekcję FAQ - większość problemów jest tam opisana</li><li>Skontaktuj się z helpdesk: <a href=\"mailto:helpdesk@uknf.gov.pl\">helpdesk@uknf.gov.pl</a></li><li>W pilnych sprawach zadzwoń: +48 22 262 5000</li></ol><p>Helpdesk jest dostępny od poniedziałku do piątku w godzinach 8:00-16:00.</p>"
            },
            new FaqQuestion
            {
                Question = "Jak długo przechowywane są wiadomości w systemie?",
                Answer = "<p>Wiadomości w systemie są przechowywane zgodnie z wymogami prawnymi:</p><ul><li>Wiadomości robocze: <strong>5 lat</strong></li><li>Wiadomości urzędowe: <strong>10 lat</strong></li><li>Wiadomości dotyczące postępowań: <strong>do zakończenia postępowania + 10 lat</strong></li></ul><p>Po upływie okresu archiwizacji wiadomości są automatycznie usuwane.</p>"
            },
            new FaqQuestion
            {
                Question = "Czy mogę dodać załączniki do wiadomości?",
                Answer = "<p>Tak, do każdej wiadomości możesz dodać załączniki.</p><p>Ograniczenia:</p><ul><li>Maksymalny rozmiar pojedynczego pliku: <strong>25 MB</strong></li><li>Maksymalna liczba załączników: <strong>10</strong></li><li>Dozwolone formaty: PDF, XLSX, DOCX, ZIP, JPG, PNG</li></ul><p>Pliki o innych rozszerzeniach lub przekraczające limity zostaną odrzucone.</p>"
            }
        };

        await _context.FaqQuestions.AddRangeAsync(questions);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {questions.Count} FAQ questions");
    }

    private async Task SeedContactsAsync()
    {
        _logger.LogInformation("Seeding contacts...");

        var entities = await _context.SupervisedEntities.Take(5).ToListAsync();
        var internalUsers = await _context.Users.Where(u => u.SupervisedEntityId == null).Take(3).ToListAsync();

        var contacts = new List<Contact>();
        for (int i = 0; i < 5; i++)
        {
            contacts.Add(new Contact
            {
                Name = $"Kontakt {i + 1}",
                Email = $"contact{i + 1}@{entities[i].Name.Replace(" ", "").Replace(".", "").ToLower()}.pl",
                Phone = $"+4822555{100 + i:D4}",
                Mobile = $"+48600{100 + i:D6}",
                Position = i % 3 == 0 ? "Dyrektor" : i % 3 == 1 ? "Kierownik" : "Specjalista",
                Department = i % 2 == 0 ? "Dział Sprawozdawczości" : "Dział Compliance",
                IsPrimary = i == 0,
                SupervisedEntityId = entities[i].Id,
                CreatedByUserId = internalUsers[i % internalUsers.Count].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-100 + i * 20),
                UpdatedAt = DateTime.UtcNow.AddDays(-90 + i * 18)
            });
        }

        await _context.Contacts.AddRangeAsync(contacts);
        await _context.SaveChangesAsync();

        // Seed ContactGroups (at least 5)
        var groups = new List<ContactGroup>();
        for (int i = 0; i < 5; i++)
        {
            groups.Add(new ContactGroup
            {
                Name = $"Grupa kontaktów {i + 1}",
                Description = $"Opis grupy kontaktów nr {i + 1}",
                CreatedByUserId = internalUsers[i % internalUsers.Count].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-80 + i * 15)
            });
        }
        await _context.ContactGroups.AddRangeAsync(groups);
        await _context.SaveChangesAsync();

        // Seed ContactGroupMembers (at least 5)
        var members = new List<ContactGroupMember>();
        for (int i = 0; i < 5; i++)
        {
            members.Add(new ContactGroupMember
            {
                ContactGroupId = groups[i % groups.Count].Id,
                ContactId = contacts[i].Id,
                AddedAt = DateTime.UtcNow.AddDays(-70 + i * 14)
            });
        }
        await _context.ContactGroupMembers.AddRangeAsync(members);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPasswordPolicyAsync()
    {
        _logger.LogInformation("Seeding password policy...");

        // Seed only one password policy
        var policy = new PasswordPolicy
        {
            MinLength = 8,
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireSpecialChar = true,
            ExpirationDays = 90,
            HistoryCount = 5,
            MaxFailedAttempts = 5,
            LockoutDurationMinutes = 30,
            UpdatedAt = DateTime.UtcNow.AddDays(-30)
        };

        await _context.PasswordPolicies.AddAsync(policy);
        await _context.SaveChangesAsync();

        // Seed PasswordHistories (at least 5)
        var users = await _context.Users.Take(5).ToListAsync();
        var histories = new List<PasswordHistory>();
        for (int i = 0; i < 5; i++)
        {
            histories.Add(new PasswordHistory
            {
                UserId = users[i].Id,
                PasswordHash = _passwordHasher.HashPassword($"OldPassword{i}!"),
                CreatedAt = DateTime.UtcNow.AddDays(-60 + i * 10)
            });
        }
        await _context.PasswordHistories.AddRangeAsync(histories);
        await _context.SaveChangesAsync();
    }
}
