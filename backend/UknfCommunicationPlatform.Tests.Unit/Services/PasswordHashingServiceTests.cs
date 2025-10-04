using FluentAssertions;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

/// <summary>
/// Unit tests for PasswordHashingService
/// </summary>
public class PasswordHashingServiceTests
{
    private readonly PasswordHashingService _sut;

    public PasswordHashingServiceTests()
    {
        _sut = new PasswordHashingService();
    }

    [Fact]
    public void HashPassword_ShouldReturnValidBCryptHash()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash = _sut.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().StartWith("$2a$"); // BCrypt hash starts with $2a$
        hash.Length.Should().Be(60); // BCrypt hash is always 60 characters
    }

    [Fact]
    public void HashPassword_WithDifferentPasswords_ShouldReturnDifferentHashes()
    {
        // Arrange
        var password1 = "Password1";
        var password2 = "Password2";

        // Act
        var hash1 = _sut.HashPassword(password1);
        var hash2 = _sut.HashPassword(password2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void HashPassword_WithSamePassword_ShouldReturnDifferentHashesDueToSalt()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash1 = _sut.HashPassword(password);
        var hash2 = _sut.HashPassword(password);

        // Assert - BCrypt uses random salt, so same password produces different hashes
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var invalidHash = "invalid_hash";

        // Act
        var result = _sut.VerifyPassword(password, invalidHash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword("", hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var result = _sut.VerifyPassword(password, "");

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("short")]
    [InlineData("verylongpasswordwithmanycharsthatshouldstillwork123456789!@#")]
    [InlineData("Pass!@#$%^&*()")]
    [InlineData("日本語パスワード")]
    public void HashPassword_WithVariousPasswordFormats_ShouldHashSuccessfully(string password)
    {
        // Act
        var hash = _sut.HashPassword(password);
        var isValid = _sut.VerifyPassword(password, hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        isValid.Should().BeTrue();
    }
}
