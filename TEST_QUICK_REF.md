# Quick Test Reference - Backend Tests

## Run Tests

```bash
# All backend tests
./run-tests.sh

# All backend tests (same as above)
./run-tests-backend.sh

# Quick (unit tests only)
./run-tests-quick.sh
```

## Options

```bash
--unit-only         # Unit tests only
--integration-only  # Integration tests only
--coverage          # Generate coverage reports
--verbose           # Detailed output
--help              # Show help
```

## Examples

```bash
# All tests with coverage
./run-tests.sh --coverage

# Quick unit test feedback loop
./run-tests-quick.sh

# Integration tests only (requires dev environment)
./run-tests.sh --integration-only

# Verbose output
./run-tests.sh --verbose
```

## Requirements

- **Backend Tests**: .NET 9.0 SDK
- **Integration Tests**: Docker (PostgreSQL container via `./dev-start.sh`)

## Current Test Status

- **Unit Tests**: 67 tests ✅ ALL PASSING
- **Integration Tests**: 1 test ✅ ALL PASSING
- **Total**: 68 backend tests

See [TESTING_GUIDE.md](TESTING_GUIDE.md) for complete documentation.
