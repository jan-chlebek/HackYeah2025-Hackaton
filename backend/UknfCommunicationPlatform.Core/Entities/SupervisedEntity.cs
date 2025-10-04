namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a supervised entity in the UKNF system
/// </summary>
public class SupervisedEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Entity type in the registry (e.g., "Lending Institution")
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// UKNF code - generated in the Entity Database, non-editable
    /// </summary>
    public string UKNFCode { get; set; } = string.Empty;

    /// <summary>
    /// Entity name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Legal Entity Identifier
    /// </summary>
    public string? LEI { get; set; }

    /// <summary>
    /// Tax Identification Number
    /// </summary>
    public string? NIP { get; set; }

    /// <summary>
    /// National Court Register number
    /// </summary>
    public string? KRS { get; set; }

    /// <summary>
    /// National Business Registry Number (REGON)
    /// </summary>
    public string? REGON { get; set; }

    /// <summary>
    /// Street address
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Building number
    /// </summary>
    public string BuildingNumber { get; set; } = string.Empty;

    /// <summary>
    /// Apartment/Unit number
    /// </summary>
    public string? ApartmentNumber { get; set; }

    /// <summary>
    /// Postal code
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// City
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Website URL
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// UKNF registry entry number
    /// </summary>
    public string? RegistryNumber { get; set; }

    /// <summary>
    /// Entity status (e.g., "Registered", "Removed")
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Entity category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Entity sector
    /// </summary>
    public string? Sector { get; set; }

    /// <summary>
    /// Entity subsector
    /// </summary>
    public string? Subsector { get; set; }

    /// <summary>
    /// Is cross-border entity
    /// </summary>
    public bool IsCrossBorder { get; set; }

    /// <summary>
    /// Is entity active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property - Reports submitted by this entity
    /// </summary>
    public ICollection<Report> Reports { get; set; } = new List<Report>();

    /// <summary>
    /// Navigation property - Users associated with this entity
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();
}
