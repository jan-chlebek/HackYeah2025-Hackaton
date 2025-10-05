using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.DTOs.Entities;
using UknfCommunicationPlatform.Core.DTOs.Users;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for managing supervised entities
/// </summary>
public class EntityManagementService
{
    private readonly ApplicationDbContext _context;

    public EntityManagementService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all entities with pagination and filtering
    /// </summary>
    public async Task<(List<EntityListItemResponse> Entities, int TotalCount)> GetEntitiesAsync(
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? entityType = null,
        bool? isActive = null)
    {
        var query = _context.SupervisedEntities.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Name.Contains(searchTerm) ||
                (e.NIP != null && e.NIP.Contains(searchTerm)) ||
                (e.REGON != null && e.REGON.Contains(searchTerm)) ||
                (e.KRS != null && e.KRS.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(e => e.EntityType == entityType);
        }

        if (isActive.HasValue)
        {
            query = query.Where(e => e.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var entities = await query
            .OrderBy(e => e.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EntityListItemResponse
            {
                Id = e.Id,
                Name = e.Name,
                EntityType = e.EntityType,
                NIP = e.NIP,
                REGON = e.REGON,
                City = e.City,
                IsActive = e.IsActive,
                UserCount = e.Users.Count,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();

        return (entities, totalCount);
    }

    /// <summary>
    /// Get entity by ID
    /// </summary>
    public async Task<EntityResponse?> GetEntityByIdAsync(long id)
    {
        var entity = await _context.SupervisedEntities
            .Include(e => e.Users)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
            return null;

        // Get report count separately since we removed the navigation property
        var reportCount = await _context.Reports.CountAsync(r => r.SubmittedBy.SupervisedEntityId == id);

        return new EntityResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            EntityType = entity.EntityType,
            UknfCode = entity.UKNFCode,
            LEI = entity.LEI,
            NIP = entity.NIP,
            REGON = entity.REGON,
            KRS = entity.KRS,
            Street = entity.Street,
            BuildingNumber = entity.BuildingNumber,
            ApartmentNumber = entity.ApartmentNumber,
            PostalCode = entity.PostalCode,
            City = entity.City,
            Country = entity.Country,
            Phone = entity.Phone,
            Email = entity.Email,
            Website = entity.Website,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            UserCount = entity.Users.Count,
            ReportCount = reportCount
        };
    }

    /// <summary>
    /// Create a new supervised entity
    /// </summary>
    public async Task<EntityResponse> CreateEntityAsync(CreateEntityRequest request)
    {
        // Check if entity with same NIP or REGON already exists
        if (!string.IsNullOrWhiteSpace(request.NIP))
        {
            if (await _context.SupervisedEntities.AnyAsync(e => e.NIP == request.NIP))
            {
                throw new InvalidOperationException($"Entity with NIP {request.NIP} already exists");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.REGON))
        {
            if (await _context.SupervisedEntities.AnyAsync(e => e.REGON == request.REGON))
            {
                throw new InvalidOperationException($"Entity with REGON {request.REGON} already exists");
            }
        }

        var now = DateTime.UtcNow;

        // Generate UKNF Code (simple sequential for now)
        var maxCode = await _context.SupervisedEntities
            .Where(e => e.UKNFCode != null)
            .Select(e => e.UKNFCode)
            .ToListAsync();

        var nextNumber = 1;
        if (maxCode.Any())
        {
            var numbers = maxCode
                .Where(c => c != null && c.StartsWith("UKNF"))
                .Select(c =>
                {
                    var numPart = c!.Substring(4);
                    return int.TryParse(numPart, out var num) ? num : 0;
                })
                .Where(n => n > 0);

            if (numbers.Any())
            {
                nextNumber = numbers.Max() + 1;
            }
        }

        var entity = new SupervisedEntity
        {
            Name = request.Name,
            EntityType = request.EntityType,
            UKNFCode = $"UKNF{nextNumber:D6}",
            LEI = request.LEI,
            NIP = request.NIP,
            REGON = request.REGON,
            KRS = request.KRS,
            Street = request.Street,
            BuildingNumber = request.BuildingNumber,
            ApartmentNumber = request.ApartmentNumber,
            PostalCode = request.PostalCode,
            City = request.City,
            Country = request.Country,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.SupervisedEntities.Add(entity);
        await _context.SaveChangesAsync();

        return (await GetEntityByIdAsync(entity.Id))!;
    }

    /// <summary>
    /// Update an existing supervised entity
    /// </summary>
    public async Task<EntityResponse> UpdateEntityAsync(long id, UpdateEntityRequest request)
    {
        var entity = await _context.SupervisedEntities.FindAsync(id);
        if (entity == null)
        {
            throw new InvalidOperationException($"Entity with ID {id} not found");
        }

        // Check for duplicate NIP/REGON (excluding current entity)
        if (!string.IsNullOrWhiteSpace(request.NIP) && request.NIP != entity.NIP)
        {
            if (await _context.SupervisedEntities.AnyAsync(e => e.NIP == request.NIP && e.Id != id))
            {
                throw new InvalidOperationException($"Entity with NIP {request.NIP} already exists");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.REGON) && request.REGON != entity.REGON)
        {
            if (await _context.SupervisedEntities.AnyAsync(e => e.REGON == request.REGON && e.Id != id))
            {
                throw new InvalidOperationException($"Entity with REGON {request.REGON} already exists");
            }
        }

        entity.Name = request.Name;
        entity.EntityType = request.EntityType;
        entity.LEI = request.LEI;
        entity.NIP = request.NIP;
        entity.REGON = request.REGON;
        entity.KRS = request.KRS;
        entity.Street = request.Street;
        entity.BuildingNumber = request.BuildingNumber;
        entity.ApartmentNumber = request.ApartmentNumber;
        entity.PostalCode = request.PostalCode;
        entity.City = request.City;
        entity.Country = request.Country;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.Website = request.Website;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetEntityByIdAsync(id))!;
    }

    /// <summary>
    /// Delete a supervised entity (soft delete)
    /// </summary>
    public async Task<bool> DeleteEntityAsync(long id)
    {
        var entity = await _context.SupervisedEntities.FindAsync(id);
        if (entity == null)
            return false;

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Get users belonging to an entity
    /// </summary>
    public async Task<List<UserListItemResponse>> GetEntityUsersAsync(long entityId)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.SupervisedEntityId == entityId)
            .Select(u => new UserListItemResponse
            {
                Id = u.Id,
                Email = u.Email,
                FullName = $"{u.FirstName} {u.LastName}",
                IsActive = u.IsActive,
                RoleNames = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            })
            .ToListAsync();
    }
}
