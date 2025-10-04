namespace UknfCommunicationPlatform.Core.DTOs.Entities;

/// <summary>
/// Request to update a supervised entity
/// </summary>
public class UpdateEntityRequest
{
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
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
}
