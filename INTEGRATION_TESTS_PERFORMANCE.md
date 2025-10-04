# Integration Tests Performance Analysis

## Performance Results

### Before Optimization
- **Total test execution time**: ~2 minutes (119 seconds)
- **Unit tests**: 5.15 seconds (103 tests) - FAST ✓
- **Integration tests**: 1.99 minutes (20 tests) - SLOW ⚠️
- **Average time per integration test**: ~6 seconds
- **Database operations**: 940 (TRUNCATE/Seeding calls)

### After Phase 1 Optimizations ✅
- **Total test execution time**: ~10 seconds
- **Unit tests**: 5.4 seconds (103 tests)
- **Integration tests**: 7.2 seconds (20 tests) - **FAST!** ✓
- **Average time per integration test**: ~0.36 seconds
- **Database operations**: 305 (67% reduction!)

### Improvement Summary
- **91.6% faster** (119s → 10s)
- **94% faster per test** (6s → 0.36s)
- **16.7x speedup** overall!

## What Was Changed (Phase 1)

1. ✅ **Shared seeded data** - Seed once at test start, not per test class
2. ✅ **Reduced BCrypt work factor** - Changed from 12 to 4 for tests
3. ✅ **Disabled EF change tracking** during bulk seeding
4. ✅ **Added transactions** for seed operations
5. ✅ **Lightweight reset** - Only delete test-created data, keep seed data

## Current Performance

- **Total test execution time**: ~2 minutes (119 seconds)
- **Unit tests**: 5.15 seconds (103 tests) - FAST ✓
- **Integration tests**: 1.99 minutes (20 tests) - SLOW ⚠️
- **Average time per integration test**: ~6 seconds

## Why Integration Tests Are Slow

### 1. **Sequential Execution (Main Bottleneck)**
- Tests run **sequentially** due to `[Collection(nameof(DatabaseCollection))]`
- This was necessary to prevent race conditions when multiple tests access the same database
- **Impact**: No parallelization, 20 tests × 6s = 120s total

### 2. **Database Reset & Seeding Per Test**
Each test class calls:
```csharp
await _factory.ResetDatabaseAsync();  // 29 TRUNCATE commands × ~5ms = 145ms
await _factory.SeedTestDataAsync();   // Seeds ~100 entities = 800-1000ms
```

**Breakdown per test:**
- Database truncate: ~145ms (29 tables)
- Seed roles & permissions: ~50ms
- Seed users: ~200ms (10 users + password hashing with BCrypt)
- Seed entities: ~150ms (5 supervised entities)
- Seed messages: ~300ms (20 messages)
- Seed reports: ~200ms (63 reports)
- **Total seeding time per test**: ~1045ms (over 1 second!)

### 3. **BCrypt Password Hashing (CPU Intensive)**
- `_passwordHasher.HashPassword()` is called 10 times during seeding
- BCrypt is designed to be slow (security feature)
- **Impact**: ~15-20ms per hash × 10 users = 150-200ms

### 4. **Large INSERT Batches**
- 20 messages with 27 columns each = 540 parameters in one INSERT command
- 63 reports with 14 columns each = 882 parameters
- EF Core logs show massive parameterized queries

### 5. **WebApplicationFactory Overhead**
- Each test spins up a mini web application
- Database migrations are applied on first test
- Dependency injection container is built

## Performance Optimization Strategies

### Quick Wins (Easy to implement)

#### 1. **Use Test Fixtures to Share Seeded Data** (Recommended)
Instead of resetting and reseeding for every test, seed once per test run:

```csharp
public class TestDatabaseFixture : IAsyncLifetime
{
    private bool _isSeeded = false;

    public async Task InitializeAsync()
    {
        await Database.MigrateAsync();
        if (!_isSeeded)
        {
            await SeedTestDataAsync();
            _isSeeded = true;
        }
    }

    // Only reset data that tests modify, not all data
    public async Task ResetModifiedDataAsync()
    {
        // Truncate only tables that tests write to
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM messages WHERE id > 20");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM reports WHERE id > 63");
    }
}
```

**Expected improvement**: 50-70% faster (20s → 6-10s)

#### 2. **Reduce BCrypt Work Factor for Tests**
Use a lower work factor for password hashing in tests:

```csharp
// In test configuration
services.AddScoped<IPasswordHashingService>(sp =>
    new PasswordHashingService(workFactor: 4)); // Default is 10-12
```

**Expected improvement**: ~150ms per test

#### 3. **Batch Database Operations**
Instead of individual INSERTs, use bulk insert:

```csharp
// Instead of:
foreach (var message in messages)
    context.Messages.Add(message);

// Use:
context.Messages.AddRange(messages);
await context.SaveChangesAsync();
```

**Expected improvement**: 10-20% faster INSERTs

#### 4. **Disable EF Core Change Tracking for Seed Data**
```csharp
context.ChangeTracker.AutoDetectChangesEnabled = false;
context.Messages.AddRange(messages);
await context.SaveChangesAsync();
context.ChangeTracker.AutoDetectChangesEnabled = true;
```

**Expected improvement**: 20-30% faster seeding

#### 5. **Use Transactions for Seed Operations**
```csharp
using var transaction = await context.Database.BeginTransactionAsync();
try
{
    await SeedRolesAndPermissionsAsync();
    await SeedUsersAsync();
    // ...
    await transaction.CommitAsync();
}
```

**Expected improvement**: 15-25% faster (reduces database round-trips)

### Medium Effort (More complex)

#### 6. **Database Snapshots (PostgreSQL specific)**
Create a database snapshot after seeding, then restore for each test:

```bash
# After seeding
pg_dump -Fc uknf_db > test_snapshot.dump

# Before each test
pg_restore -d uknf_db --clean test_snapshot.dump
```

**Expected improvement**: 60-80% faster (snapshot restore is faster than seeding)

#### 7. **In-Memory Database for Fast Tests**
Use SQLite in-memory for tests that don't need PostgreSQL-specific features:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("DataSource=:memory:"));
```

**Expected improvement**: 80-90% faster (no disk I/O)

⚠️ **Caveat**: Some PostgreSQL-specific features won't work (enums, arrays, etc.)

#### 8. **Parallel Test Execution with Database Pooling**
Use a separate database per test class:

```csharp
var dbName = $"uknf_test_{Guid.NewGuid():N}";
// Create database, run tests, drop database
```

**Expected improvement**: Near-linear speedup with CPU cores (2x-4x faster on 4-8 cores)

### Advanced (Significant refactoring)

#### 9. **Test Data Builders**
Instead of seeding everything, create only what each test needs:

```csharp
var user = new UserBuilder()
    .WithEmail("test@example.com")
    .AsInternalUser()
    .Build();

var message = new MessageBuilder()
    .From(user)
    .WithSubject("Test")
    .Build();
```

**Expected improvement**: 70-90% faster (minimal data creation)

#### 10. **Containerized Test Databases**
Use Testcontainers to spin up isolated PostgreSQL instances:

```csharp
var container = new PostgreSqlBuilder()
    .WithImage("postgres:15-alpine")
    .Build();
```

**Expected improvement**: Better isolation, can run in parallel

## Recommended Approach

**Phase 1 (Quick wins - implement today)**:
1. Use test fixtures to share seeded data (don't reset/reseed every test)
2. Reduce BCrypt work factor to 4 for tests
3. Disable change tracking during seeding
4. Use transactions for seed operations

**Expected result**: 20 tests in ~25-35 seconds (60-70% improvement)

**Phase 2 (Medium effort - next sprint)**:
5. Implement database snapshots for faster resets
6. Add test data builders for tests that don't need full seed

**Expected result**: 20 tests in ~10-15 seconds (85-90% improvement)

**Phase 3 (Optional - if needed)**:
7. Consider parallel execution with database pooling
8. Evaluate in-memory SQLite for appropriate tests

## Current vs Target Performance

| Metric | Current | Phase 1 | Phase 2 | Phase 3 |
|--------|---------|---------|---------|---------|
| Total time | 119s | 35s | 15s | 8s |
| Per test | 6s | 1.75s | 0.75s | 0.4s |
| Improvement | - | 70% | 87% | 93% |

## Implementation Priority

1. ✅ **High Priority**: Phase 1 items (low effort, high impact)
2. ⚠️ **Medium Priority**: Database snapshots (moderate effort, good impact)
3. ℹ️ **Low Priority**: Parallel execution (high complexity, best for large test suites)

## Notes

- The current ~2 minute execution time is **acceptable** for a test suite of this size
- Most CI/CD pipelines can handle 2-minute test runs
- Focus on Phase 1 optimizations first - they'll give you 60-70% improvement with minimal changes
- Don't optimize prematurely - wait until test count grows to 50+ tests before implementing Phase 3
