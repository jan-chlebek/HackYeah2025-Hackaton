#!/bin/bash

###############################################################################
# UKNF Communication Platform - Run Backend Tests Script
###############################################################################
# This script runs backend tests (unit and integration)
# Usage: ./run-tests.sh [options]
# Options:
#   --unit-only       Run only unit tests
#   --integration-only Run only integration tests
#   --coverage        Run with code coverage
#   --verbose         Verbose output
#   --help            Show this help message
###############################################################################

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Default options
RUN_UNIT=true
RUN_INTEGRATION=true
COVERAGE=false
VERBOSE=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --unit-only)
            RUN_INTEGRATION=false
            shift
            ;;
        --integration-only)
            RUN_UNIT=false
            shift
            ;;
        --coverage)
            COVERAGE=true
            shift
            ;;
        --verbose)
            VERBOSE=true
            shift
            ;;
        --help)
            echo "Usage: ./run-tests.sh [options]"
            echo ""
            echo "Options:"
            echo "  --unit-only          Run only unit tests"
            echo "  --integration-only   Run only integration tests"
            echo "  --coverage           Run with code coverage"
            echo "  --verbose            Verbose output"
            echo "  --help               Show this help message"
            exit 0
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Print header
echo -e "${BLUE}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  UKNF Communication Platform - Backend Test Runner        ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════════╝${NC}"
echo ""

# Track test results
BACKEND_UNIT_RESULT=0
BACKEND_INTEGRATION_RESULT=0
UNIT_PASSED=0
UNIT_FAILED=0
INTEGRATION_PASSED=0
INTEGRATION_FAILED=0

###############################################################################
# Backend Tests
###############################################################################
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${BLUE}  Backend Tests (.NET)${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo ""

cd backend

    # Backend Unit Tests
    if [ "$RUN_UNIT" = true ]; then
        echo -e "${YELLOW}▶ Running Backend Unit Tests...${NC}"
        echo ""

        if [ "$COVERAGE" = true ]; then
            COVERAGE_ARGS="/p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage/"
        else
            COVERAGE_ARGS=""
        fi

        if [ "$VERBOSE" = true ]; then
            VERBOSE_ARGS="--verbosity detailed"
        else
            VERBOSE_ARGS="--verbosity normal"
        fi

        if dotnet test UknfCommunicationPlatform.Tests.Unit/UknfCommunicationPlatform.Tests.Unit.csproj \
            --configuration Release \
            $VERBOSE_ARGS \
            $COVERAGE_ARGS \
            --logger "console;verbosity=normal" 2>&1 | tee /tmp/backend-unit-test.log; then

            # Extract test counts from output
            UNIT_TOTAL=$(grep -oP "Total tests: \K\d+" /tmp/backend-unit-test.log | tail -1 || echo "0")
            UNIT_PASSED=$(grep -oP "Passed: \K\d+" /tmp/backend-unit-test.log | tail -1 || echo "0")
            UNIT_FAILED=$(grep -oP "Failed: \K\d+" /tmp/backend-unit-test.log | tail -1 || echo "0")

            # Default to 0 if empty
            UNIT_TOTAL=${UNIT_TOTAL:-0}
            UNIT_PASSED=${UNIT_PASSED:-0}
            UNIT_FAILED=${UNIT_FAILED:-0}

            if [ "$UNIT_FAILED" -eq 0 ] && [ "$UNIT_TOTAL" -gt 0 ]; then
                BACKEND_UNIT_RESULT=0
                echo ""
                echo -e "${GREEN}✓ Backend Unit Tests PASSED (${UNIT_PASSED}/${UNIT_TOTAL} tests)${NC}"
            elif [ "$UNIT_TOTAL" -eq 0 ]; then
                BACKEND_UNIT_RESULT=3
                echo ""
                echo -e "${YELLOW}⚠ No unit tests found!${NC}"
            else
                BACKEND_UNIT_RESULT=1
                echo ""
                echo -e "${RED}✗ Backend Unit Tests FAILED (${UNIT_FAILED}/${UNIT_TOTAL} tests failed)${NC}"
            fi
        else
            BACKEND_UNIT_RESULT=1
            echo ""
            echo -e "${RED}✗ Backend Unit Tests FAILED (test execution error)${NC}"
        fi
        echo ""
    fi

    # Backend Integration Tests
    if [ "$RUN_INTEGRATION" = true ]; then
        echo -e "${YELLOW}▶ Running Backend Integration Tests...${NC}"
        echo ""

        # Check if PostgreSQL is running (required for integration tests)
        if docker ps | grep -q uknf-postgres-dev; then
            echo -e "${GREEN}✓ PostgreSQL container is running${NC}"
            echo ""

            if [ "$COVERAGE" = true ]; then
                COVERAGE_ARGS="/p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage/"
            else
                COVERAGE_ARGS=""
            fi

            if [ "$VERBOSE" = true ]; then
                VERBOSE_ARGS="--verbosity detailed"
            else
                VERBOSE_ARGS="--verbosity normal"
            fi

            if dotnet test UknfCommunicationPlatform.Tests.Integration/UknfCommunicationPlatform.Tests.Integration.csproj \
                --configuration Release \
                $VERBOSE_ARGS \
                $COVERAGE_ARGS \
                --logger "console;verbosity=normal" 2>&1 | tee /tmp/backend-integration-test.log; then

                # Extract test counts from output
                INTEGRATION_TOTAL=$(grep -oP "Total tests: \K\d+" /tmp/backend-integration-test.log | tail -1 || echo "0")
                INTEGRATION_PASSED=$(grep -oP "Passed: \K\d+" /tmp/backend-integration-test.log | tail -1 || echo "0")
                INTEGRATION_FAILED=$(grep -oP "Failed: \K\d+" /tmp/backend-integration-test.log | tail -1 || echo "0")

                # Default to 0 if empty
                INTEGRATION_TOTAL=${INTEGRATION_TOTAL:-0}
                INTEGRATION_PASSED=${INTEGRATION_PASSED:-0}
                INTEGRATION_FAILED=${INTEGRATION_FAILED:-0}

                if [ "$INTEGRATION_FAILED" -eq 0 ] && [ "$INTEGRATION_TOTAL" -gt 0 ]; then
                    BACKEND_INTEGRATION_RESULT=0
                    echo ""
                    echo -e "${GREEN}✓ Backend Integration Tests PASSED (${INTEGRATION_PASSED}/${INTEGRATION_TOTAL} tests)${NC}"
                elif [ "$INTEGRATION_TOTAL" -eq 0 ]; then
                    BACKEND_INTEGRATION_RESULT=3
                    echo ""
                    echo -e "${YELLOW}⚠ No integration tests found!${NC}"
                else
                    BACKEND_INTEGRATION_RESULT=1
                    echo ""
                    echo -e "${RED}✗ Backend Integration Tests FAILED (${INTEGRATION_FAILED}/${INTEGRATION_TOTAL} tests failed)${NC}"
                fi
            else
                BACKEND_INTEGRATION_RESULT=1
                echo ""
                echo -e "${RED}✗ Backend Integration Tests FAILED (test execution error)${NC}"
            fi
        else
            echo -e "${YELLOW}⚠ PostgreSQL container not running - skipping integration tests${NC}"
            echo -e "${YELLOW}  Tip: Run ./dev-start.sh to start the development environment${NC}"
            BACKEND_INTEGRATION_RESULT=2  # Skipped
        fi
        echo ""
    fi

cd ..

###############################################################################
# Test Summary
###############################################################################
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${BLUE}  Test Summary${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo ""

TOTAL_TESTS=$((UNIT_PASSED + UNIT_FAILED + INTEGRATION_PASSED + INTEGRATION_FAILED))
TOTAL_PASSED=$((UNIT_PASSED + INTEGRATION_PASSED))
TOTAL_FAILED=$((UNIT_FAILED + INTEGRATION_FAILED))

if [ "$RUN_UNIT" = true ]; then
    if [ $BACKEND_UNIT_RESULT -eq 0 ]; then
        echo -e "  Backend Unit Tests:        ${GREEN}✓ PASSED${NC} (${UNIT_PASSED}/${UNIT_PASSED} tests)"
    elif [ $BACKEND_UNIT_RESULT -eq 3 ]; then
        echo -e "  Backend Unit Tests:        ${YELLOW}⚠ NO TESTS FOUND${NC}"
    else
        echo -e "  Backend Unit Tests:        ${RED}✗ FAILED${NC} (${UNIT_FAILED} failed, ${UNIT_PASSED} passed)"
    fi
fi

if [ "$RUN_INTEGRATION" = true ]; then
    if [ $BACKEND_INTEGRATION_RESULT -eq 0 ]; then
        echo -e "  Backend Integration Tests: ${GREEN}✓ PASSED${NC} (${INTEGRATION_PASSED}/${INTEGRATION_PASSED} tests)"
    elif [ $BACKEND_INTEGRATION_RESULT -eq 2 ]; then
        echo -e "  Backend Integration Tests: ${YELLOW}⊘ SKIPPED${NC} (PostgreSQL not running)"
    elif [ $BACKEND_INTEGRATION_RESULT -eq 3 ]; then
        echo -e "  Backend Integration Tests: ${YELLOW}⚠ NO TESTS FOUND${NC}"
    else
        echo -e "  Backend Integration Tests: ${RED}✗ FAILED${NC} (${INTEGRATION_FAILED} failed, ${INTEGRATION_PASSED} passed)"
    fi
fi

echo ""
echo -e "  Total Tests:               ${TOTAL_TESTS}"
echo -e "  Passed:                    ${GREEN}${TOTAL_PASSED}${NC}"
if [ $TOTAL_FAILED -gt 0 ]; then
    echo -e "  Failed:                    ${RED}${TOTAL_FAILED}${NC}"
else
    echo -e "  Failed:                    ${TOTAL_FAILED}"
fi
echo ""

# Coverage report location
if [ "$COVERAGE" = true ]; then
    echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${BLUE}  Coverage Reports${NC}"
    echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo ""
    if [ "$RUN_UNIT" = true ]; then
        echo -e "  Backend Unit:       ${YELLOW}backend/UknfCommunicationPlatform.Tests.Unit/coverage/${NC}"
    fi
    if [ "$RUN_INTEGRATION" = true ]; then
        echo -e "  Backend Integration: ${YELLOW}backend/UknfCommunicationPlatform.Tests.Integration/coverage/${NC}"
    fi
    echo ""
fi

# Exit with appropriate code
TOTAL_FAILURES=0
if [ $BACKEND_UNIT_RESULT -eq 1 ]; then
    TOTAL_FAILURES=$((TOTAL_FAILURES + 1))
fi
if [ $BACKEND_INTEGRATION_RESULT -eq 1 ]; then
    TOTAL_FAILURES=$((TOTAL_FAILURES + 1))
fi

# Warn if no tests were run
if [ $BACKEND_UNIT_RESULT -eq 3 ] && [ $BACKEND_INTEGRATION_RESULT -eq 3 ]; then
    echo -e "${RED}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${RED}║  ERROR: No tests found in any test project!               ║${NC}"
    echo -e "${RED}╚════════════════════════════════════════════════════════════╝${NC}"
    exit 1
fi

if [ $TOTAL_FAILURES -gt 0 ]; then
    echo -e "${RED}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${RED}║  Some tests FAILED - please review the output above       ║${NC}"
    echo -e "${RED}╚════════════════════════════════════════════════════════════╝${NC}"
    exit 1
else
    echo -e "${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${GREEN}║  All tests PASSED! ✓                                       ║${NC}"
    echo -e "${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"
    exit 0
fi
