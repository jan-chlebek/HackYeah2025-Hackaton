using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.DTOs.Users;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for managing user accounts
/// </summary>
public class UserManagementService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHashingService _passwordHasher;

    public UserManagementService(ApplicationDbContext context, IPasswordHashingService passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Get all users with pagination and filtering
    /// </summary>
    public async Task<(List<UserListItemResponse> Users, int TotalCount)> GetUsersAsync(
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        bool? isActive = null,
        long? supervisedEntityId = null)
    {
        var query = _context.Users
            .Include(u => u.SupervisedEntity)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.Email.Contains(searchTerm) ||
                u.FirstName.Contains(searchTerm) ||
                u.LastName.Contains(searchTerm));
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        if (supervisedEntityId.HasValue)
        {
            query = query.Where(u => u.SupervisedEntityId == supervisedEntityId.Value);
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserListItemResponse
            {
                Id = u.Id,
                Email = u.Email,
                FullName = $"{u.FirstName} {u.LastName}",
                IsActive = u.IsActive,
                SupervisedEntityName = u.SupervisedEntity != null ? u.SupervisedEntity.Name : null,
                RoleNames = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            })
            .ToListAsync();

        return (users, totalCount);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    public async Task<UserResponse?> GetUserByIdAsync(long id)
    {
        var user = await _context.Users
            .Include(u => u.SupervisedEntity)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            LastPasswordChangeAt = user.LastPasswordChangeAt,
            RequirePasswordChange = user.RequirePasswordChange,
            IsLocked = user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow,
            LockedUntil = user.LockedUntil,
            SupervisedEntityId = user.SupervisedEntityId,
            SupervisedEntityName = user.SupervisedEntity?.Name,
            Roles = user.UserRoles.Select(ur => new RoleInfo
            {
                Id = ur.RoleId,
                Name = ur.Role.Name,
                AssignedAt = ur.AssignedAt
            }).ToList()
        };
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new InvalidOperationException($"User with email {request.Email} already exists");
        }

        // Verify roles exist
        var roles = await _context.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync();

        if (roles.Count != request.RoleIds.Count)
        {
            throw new InvalidOperationException("One or more specified roles do not exist");
        }

        // Verify entity exists if specified
        if (request.SupervisedEntityId.HasValue)
        {
            var entityExists = await _context.SupervisedEntities
                .AnyAsync(e => e.Id == request.SupervisedEntityId.Value);

            if (!entityExists)
            {
                throw new InvalidOperationException($"Supervised entity with ID {request.SupervisedEntityId} does not exist");
            }
        }

        var now = DateTime.UtcNow;

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            SupervisedEntityId = request.SupervisedEntityId,
            IsActive = true,
            RequirePasswordChange = request.RequirePasswordChange,
            CreatedAt = now,
            UpdatedAt = now,
            LastPasswordChangeAt = now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assign roles
        foreach (var role in roles)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedAt = now
            });
        }

        // Add password to history
        _context.PasswordHistories.Add(new PasswordHistory
        {
            UserId = user.Id,
            PasswordHash = user.PasswordHash,
            CreatedAt = now
        });

        await _context.SaveChangesAsync();

        return (await GetUserByIdAsync(user.Id))!;
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    public async Task<UserResponse> UpdateUserAsync(long id, UpdateUserRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {id} not found");
        }

        // Verify roles exist
        var roles = await _context.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync();

        if (roles.Count != request.RoleIds.Count)
        {
            throw new InvalidOperationException("One or more specified roles do not exist");
        }

        // Verify entity exists if specified
        if (request.SupervisedEntityId.HasValue)
        {
            var entityExists = await _context.SupervisedEntities
                .AnyAsync(e => e.Id == request.SupervisedEntityId.Value);

            if (!entityExists)
            {
                throw new InvalidOperationException($"Supervised entity with ID {request.SupervisedEntityId} does not exist");
            }
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        user.IsActive = request.IsActive;
        user.SupervisedEntityId = request.SupervisedEntityId;
        user.UpdatedAt = DateTime.UtcNow;

        // Update roles
        _context.UserRoles.RemoveRange(user.UserRoles);
        foreach (var role in roles)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return (await GetUserByIdAsync(id))!;
    }

    /// <summary>
    /// Delete a user (soft delete by setting IsActive = false)
    /// </summary>
    public async Task<bool> DeleteUserAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Set password for a user
    /// </summary>
    public async Task SetPasswordAsync(long userId, SetPasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        // TODO: Validate password against password policy

        var newHash = _passwordHasher.HashPassword(request.NewPassword);

        user.PasswordHash = newHash;
        user.LastPasswordChangeAt = DateTime.UtcNow;
        user.RequirePasswordChange = request.RequireChangeOnLogin;
        user.UpdatedAt = DateTime.UtcNow;

        // Add to password history
        _context.PasswordHistories.Add(new PasswordHistory
        {
            UserId = userId,
            PasswordHash = newHash,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Activate a user account
    /// </summary>
    public async Task<bool> ActivateUserAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Deactivate a user account
    /// </summary>
    public async Task<bool> DeactivateUserAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Unlock a locked user account
    /// </summary>
    public async Task<bool> UnlockUserAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        user.LockedUntil = null;
        user.FailedLoginAttempts = 0;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
}
