using Xunit;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Collection definition to ensure all integration tests run sequentially
/// and share the same database fixture.
/// This prevents test conflicts when multiple tests try to reset/seed the database.
/// </summary>
[CollectionDefinition(nameof(DatabaseCollection))]
public class DatabaseCollection : ICollectionFixture<TestDatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
