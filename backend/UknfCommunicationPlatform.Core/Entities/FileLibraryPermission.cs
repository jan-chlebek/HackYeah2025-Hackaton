namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Defines access permissions for library files
/// </summary>
public class FileLibraryPermission
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// File ID
    /// </summary>
    public long FileLibraryId { get; set; }

    /// <summary>
    /// Navigation property - File
    /// </summary>
    public FileLibrary FileLibrary { get; set; } = null!;

    /// <summary>
    /// Permission type: "User", "Role", "SupervisedEntity", "PodmiotType"
    /// </summary>
    public string PermissionType { get; set; } = string.Empty;

    /// <summary>
    /// User ID (if PermissionType is "User")
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Navigation property - User
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Role name (if PermissionType is "Role")
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// Supervised entity ID (if PermissionType is "SupervisedEntity")
    /// </summary>
    public long? SupervisedEntityId { get; set; }

    /// <summary>
    /// Navigation property - Supervised entity
    /// </summary>
    public SupervisedEntity? SupervisedEntity { get; set; }

    /// <summary>
    /// Podmiot type (if PermissionType is "PodmiotType")
    /// </summary>
    public string? PodmiotType { get; set; }

    /// <summary>
    /// Can read/download
    /// </summary>
    public bool CanRead { get; set; } = true;

    /// <summary>
    /// Can modify
    /// </summary>
    public bool CanWrite { get; set; }

    /// <summary>
    /// Can delete
    /// </summary>
    public bool CanDelete { get; set; }
}
