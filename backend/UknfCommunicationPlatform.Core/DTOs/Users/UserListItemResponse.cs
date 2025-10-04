namespace UknfCommunicationPlatform.Core.DTOs.Users;

/// <summary>
/// Simplified user information for list views
/// </summary>
public class UserListItemResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? SupervisedEntityName { get; set; }
    public List<string> RoleNames { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
