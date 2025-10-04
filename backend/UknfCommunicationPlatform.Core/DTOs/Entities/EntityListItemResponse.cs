namespace UknfCommunicationPlatform.Core.DTOs.Entities;

/// <summary>
/// Simplified entity information for list views
/// </summary>
public class EntityListItemResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? NIP { get; set; }
    public string? REGON { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
