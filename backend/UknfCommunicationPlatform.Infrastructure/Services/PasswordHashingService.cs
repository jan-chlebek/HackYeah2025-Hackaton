using BCrypt.Net;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for password hashing and verification using BCrypt
/// </summary>
public class PasswordHashingService : IPasswordHashingService
{
    private readonly int _workFactor;

    /// <summary>
    /// Creates a new PasswordHashingService with default work factor
    /// </summary>
    public PasswordHashingService() : this(12)
    {
    }

    /// <summary>
    /// Creates a new PasswordHashingService with specified work factor
    /// </summary>
    /// <param name="workFactor">BCrypt work factor (higher = more secure but slower). Use 4 for tests, 12 for production.</param>
    public PasswordHashingService(int workFactor)
    {
        _workFactor = workFactor;
    }

    /// <summary>
    /// Hash a plain text password
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
    }

    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hash">Password hash</param>
    /// <returns>True if password matches hash</returns>
    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
