namespace UknfCommunicationPlatform.Core.DTOs.Users;

/// <summary>
/// Detailed user information response
/// </summary>
public class UserResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastPasswordChangeAt { get; set; }
    public bool RequirePasswordChange { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedUntil { get; set; }

    // Supervised entity info
    public long? SupervisedEntityId { get; set; }
    public string? SupervisedEntityName { get; set; }

    // Roles
    public List<RoleInfo> Roles { get; set; } = new();
}

/// <summary>
/// Basic role information for UserResponse
/// </summary>
public class RoleInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
}
