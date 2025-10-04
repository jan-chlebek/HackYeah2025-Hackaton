namespace UknfCommunicationPlatform.Core.DTOs.Entities;

/// <summary>
/// Detailed entity information response
/// </summary>
public class EntityResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? UknfCode { get; set; }
    public string? LEI { get; set; }
    public string? NIP { get; set; }
    public string? REGON { get; set; }
    public string? KRS { get; set; }

    // Address
    public string? Street { get; set; }
    public string? BuildingNumber { get; set; }
    public string? ApartmentNumber { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    // Contact
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Statistics
    public int UserCount { get; set; }
    public int ReportCount { get; set; }
}
