# Testing Guide - Backend Tests

This document provides comprehensive information about running backend tests for the UKNF Communication Platform.

## Quick Start

### Run All Backend Tests
```bash
./run-tests.sh
```

### Run Specific Test Suites
```bash
# All backend tests (default)
./run-tests-backend.sh

# Quick unit tests only (fast feedback)
./run-tests-quick.sh
```

## Test Scripts

### Main Test Script: `run-tests.sh`

The main test runner script executes all backend tests (unit and integration).

#### Options

| Option | Description |
|--------|-------------|
| `--unit-only` | Run only unit tests |
| `--integration-only` | Run only integration tests |
| `--coverage` | Generate code coverage reports |
| `--verbose` | Show detailed test output |
| `--help` | Display help information |

#### Examples

```bash
# Run all backend tests
./run-tests.sh

# Run only unit tests (fast)
./run-tests.sh --unit-only

# Run integration tests with verbose output
./run-tests.sh --integration-only --verbose

# Run all tests with coverage
./run-tests.sh --coverage
```

### Helper Scripts

#### `run-tests-backend.sh`
Convenience script for running all backend tests. Same as running `./run-tests.sh`.

```bash
./run-tests-backend.sh
./run-tests-backend.sh --coverage
```

#### `run-tests-quick.sh`
Runs only unit tests for quick feedback during development (skips slower integration tests).

```bash
./run-tests-quick.sh
```

## Test Structure

### Backend Tests (.NET/C#)

#### Unit Tests
- **Location**: `backend/UknfCommunicationPlatform.Tests.Unit/`
- **Framework**: xUnit
- **Purpose**: Test individual components in isolation
- **Dependencies**: No external services required

#### Integration Tests
- **Location**: `backend/UknfCommunicationPlatform.Tests.Integration/`
- **Framework**: xUnit
- **Purpose**: Test component interactions and database operations
- **Dependencies**: Requires PostgreSQL database (Docker container)

**Note**: Integration tests require the development environment to be running:
```bash
./dev-start.sh
```

## Current Test Status

### Test Count Summary
- **Unit Tests**: 67 tests (ALL PASSING ✅)
- **Integration Tests**: 1 test (ALL PASSING ✅)
- **Total**: 68 backend tests

### Unit Test Breakdown
- Authorization (15 tests)
  - Role Authorization: 4 tests
  - Permission Authorization: 4 tests
  - Entity Ownership: 7 tests
- JWT Service: 9 tests
- Auth Service: 13 tests
- User Management: 10 tests
- Entity Management: 9 tests
- Password Hashing: 11 tests

## Code Coverage

### Generating Coverage Reports

Add the `--coverage` flag to any test command:

```bash
./run-tests.sh --coverage
```

### Coverage Report Locations

After running tests with coverage:

- **Backend Unit Tests**: `backend/UknfCommunicationPlatform.Tests.Unit/coverage/`
- **Backend Integration Tests**: `backend/UknfCommunicationPlatform.Tests.Integration/coverage/`

### Viewing Coverage Reports

#### Backend (OpenCover format)
The backend generates OpenCover XML coverage files. You can:

1. Use a coverage viewer tool
2. Generate HTML reports with ReportGenerator:
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:backend/**/coverage/coverage.opencover.xml -targetdir:coverage-report
```

## Continuous Integration

### Running Tests in CI/CD

The test scripts are designed to work in CI/CD pipelines:

```yaml
# GitHub Actions example
- name: Run All Tests
  run: ./run-tests.sh --coverage

# Azure Pipelines example
- script: ./run-tests.sh --coverage
  displayName: 'Run Tests with Coverage'
```

### Exit Codes

- **0**: All tests passed
- **1**: One or more tests failed
- **Other**: Script error or interrupted

## Best Practices

### During Development

1. **Quick feedback loop**: Use `./run-tests-quick.sh` for rapid testing
2. **Before committing**: Run `./run-tests.sh` to ensure all tests pass
3. **Integration tests**: Ensure dev environment is running with `./dev-start.sh`

### Writing Tests

#### Backend (C#)

```csharp
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Services
{
    public class MessageServiceTests
    {
        [Fact]
        public async Task SendMessage_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var service = new MessageService();

            // Act
            var result = await service.SendMessageAsync(/* params */);

            // Assert
            Assert.True(result.Success);
        }
    }
}
```

#### Frontend (TypeScript)

```typescript
import { TestBed } from '@angular/core/testing';
import { MessageService } from './message.service';

describe('MessageService', () => {
  let service: MessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MessageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should send message successfully', async () => {
    const result = await service.sendMessage(/* params */);
    expect(result.success).toBe(true);
  });
});
```

**Note**: Frontend tests are not currently included in the test scripts. Run frontend tests separately:
```bash
cd frontend/uknf-project
npm test
```

## Troubleshooting

### Integration Tests Fail: Database Not Running

**Problem**: Integration tests are skipped or fail because PostgreSQL is not running.

**Solution**: Start the development environment:
```bash
./dev-start.sh
```

### No Tests Found

**Problem**: Script reports "No tests found" or shows 0 tests.

**Solution**:
1. Check that test projects have actual test files with `[Fact]` or `[Theory]` attributes
2. Verify project builds successfully: `dotnet build backend/UknfCommunicationPlatform.Tests.Unit/`
3. Run tests manually: `dotnet test backend/UknfCommunicationPlatform.Tests.Unit/ --verbosity normal`

### Permission Denied

**Problem**: Cannot execute test scripts.

**Solution**: Make scripts executable:
```bash
chmod +x run-tests.sh run-tests-backend.sh run-tests-quick.sh
```

### Tests Pass Locally But Fail in CI

**Possible causes**:
1. Environment differences (ensure Docker/PostgreSQL configured in CI)
2. Timing issues (add appropriate waits/retries for database connections)
3. Missing dependencies (check CI configuration has .NET 9.0 SDK)

## Test Metrics

### Coverage Targets

| Component | Target Coverage | Current Status |
|-----------|----------------|----------------|
| Backend Core | ≥ 80% | 🟡 In Progress |
| Backend Infrastructure | ≥ 70% | 🟡 In Progress |
| Backend API Controllers | ≥ 60% | 🟡 In Progress |

### Test Performance

| Test Suite | Count | Duration | Status |
|------------|-------|----------|--------|
| Backend Unit Tests | 67 | ~5 seconds | ✅ All Passing |
| Backend Integration Tests | 1 | <1 second | ✅ All Passing |
| **Total** | **68** | **~6 seconds** | **✅ All Passing** |

## Additional Resources

- [xUnit Documentation](https://xunit.net/)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Entity Framework Core Testing](https://learn.microsoft.com/en-us/ef/core/testing/)

## Maintenance

### Adding New Tests

1. **Backend**: Add test classes to appropriate test project (Unit or Integration)
2. **Run tests**: Verify with `./run-tests.sh`
3. **Coverage**: Check with `./run-tests.sh --coverage`

### Updating Test Configuration

- **Backend**: Modify `.csproj` files in test projects
- **Scripts**: Modify `run-tests.sh` as needed for new requirements
