namespace UknfCommunicationPlatform.Core.DTOs.Users;

/// <summary>
/// Request to set a user's password
/// </summary>
public class SetPasswordRequest
{
    /// <summary>
    /// New password (will be validated against password policy)
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// If true, user must change this password on next login
    /// </summary>
    public bool RequireChangeOnLogin { get; set; } = false;
}
