namespace UknfCommunicationPlatform.Core.Enums;

/// <summary>
/// User roles in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// System Administrator - UKNF employee with full system access
    /// </summary>
    SystemAdministrator = 0,

    /// <summary>
    /// UKNF Employee - Internal user with supervisory functions
    /// </summary>
    UKNFEmployee = 1,

    /// <summary>
    /// Supervised Entity Administrator - External user with admin rights for their entity
    /// </summary>
    EntityAdministrator = 2,

    /// <summary>
    /// Supervised Entity Employee - External user representing a supervised entity
    /// </summary>
    EntityEmployee = 3
}
