# Database Load Summary

## Database Information
- **Database Name:** `uknf_platform`
- **PostgreSQL Version:** 16-alpine
- **Container:** `hackyeah2025-hackaton-postgres-1`
- **Date Loaded:** October 4, 2025

## Scripts Executed

### 1. init-schema.sql ✅
- Created complete database schema with 40+ tables
- Set up all indexes, triggers, and constraints
- Established relationships between entities

### 2. seed-data.sql ✅
- Initial test data loaded
- 5 base entities
- 8 base users
- Foundational records for testing

### 3. additional-seed-data-fixed.sql ✅
- Extended test data loaded
- Additional entities, users, and records
- Comprehensive cross-sectoral data

## Current Database Statistics

| Table Name | Record Count | Description |
|------------|--------------|-------------|
| **Entities** | 15 | Supervised entities across multiple financial sectors |
| **Users** | 18 | UKNF employees (7) + Entity users (11) |
| **Reports** | 10 | Quarterly and monthly financial reports |
| **Messages** | 8 | Communication threads between UKNF and entities |
| **Message Threads** | 12 | Individual messages within conversations |
| **Cases** | 8 | Administrative cases and applications |
| **Announcements** | 11 | System-wide announcements and bulletins |
| **FAQ Questions** | 7 | Frequently asked questions |
| **FAQ Answers** | 7 | Answers to FAQ questions |
| **Contact Groups** | 7 | Contact groups for organized communication |
| **Audit Log** | 10 | Audit trail of system actions |

## Entity Breakdown by Sector

### Financial Institutions (15 total)

1. **Lending Institutions (5)**
   - Pożyczka Plus S.A. (IP-001)
   - Kredyt Szybki Sp. z o.o. (IP-002)
   - Finanse Domowe S.A. (IP-003)
   - Bank Testowy S.A. (BANK-001)
   - Chwilówka Express Sp. z o.o. (IP-004)

2. **Insurance Companies (2)**
   - Bezpieczne Ubezpieczenia S.A. (TU-001)
   - Życie Plus TU S.A. (TU-002)

3. **Investment Funds (2)**
   - Kapitalny TFI S.A. (TFI-001)
   - Globalne Inwestycje TFI S.A. (TFI-002)

4. **Payment Institutions (2)**
   - Płatności Online Sp. z o.o. (IPT-001)
   - Swift Pay Sp. z o.o. (IPT-002)

5. **Brokerage Houses (1)**
   - Makler Expert Dom Maklerski S.A. (DM-001)

6. **E-Money Institutions (1)**
   - E-Money Fast Sp. z o.o. (IPE-001)

7. **Pension Funds (1)**
   - Emerytury Bezpieczne PTE S.A. (PTE-001)

8. **Credit Unions (1)**
   - SKOK Wspólnota (SKOK-001)

## User Distribution

### Internal Users (UKNF Employees) - 7 users
- System administrators
- Department heads
- Case handlers
- Reviewers

### External Users (Entity Representatives) - 11 users
- Entity administrators (with full permissions)
- Entity employees (with limited permissions)
- Distributed across all entity types

## Report Status Distribution

- **validation_success**: 4 reports (validated successfully)
- **validation_errors**: 2 reports (failed validation)
- **in_progress**: 2 reports (currently being processed)
- **submitted**: 1 report (awaiting validation)
- **challenged_by_uknf**: 1 report (questioned by UKNF)

## Message/Case Status

### Messages
- **awaiting_uknf_response**: 3 messages
- **awaiting_user_response**: 3 messages
- **closed**: 2 messages

### Cases
- **in_progress**: 3 cases
- **new**: 2 cases
- **requires_completion**: 1 case
- **completed**: 2 cases

## Testing Scenarios Covered

✅ **Multi-sector entities** - Insurance, investment, payments, lending, etc.
✅ **Cross-border entities** - Some entities marked as cross-border
✅ **User permissions** - Different role combinations (admin, employee, internal, external)
✅ **Report workflows** - Various statuses (submitted, validated, errors, challenged)
✅ **Message threads** - Complete conversations with multiple replies
✅ **Case management** - Different priorities and statuses
✅ **Announcements** - System-wide communications
✅ **FAQ system** - Questions with answers
✅ **Contact groups** - Organized communication groups
✅ **Audit trail** - System action logging

## Connection Details

### Docker Container
```bash
# Connect to PostgreSQL
docker exec -it hackyeah2025-hackaton-postgres-1 psql -U uknf_user -d uknf_platform
```

### Connection String
```
Host=localhost;Port=5432;Database=uknf_platform;Username=uknf_user;Password=uknf_dev_password_2024
```

### Environment Variables
- **POSTGRES_USER:** uknf_user
- **POSTGRES_DB:** uknf_platform
- **POSTGRES_PASSWORD:** uknf_dev_password_2024

## Next Steps

1. ✅ **Database Schema Created**
2. ✅ **Initial Data Loaded**
3. ✅ **Additional Test Data Loaded**
4. ⏳ **Apply EF Core Migration** (backend alignment)
   ```powershell
   cd src\Backend\UknfCommunicationPlatform.Infrastructure
   dotnet ef database update --startup-project ..\UknfCommunicationPlatform.Api
   ```
5. ⏳ **Test Backend API Endpoints** (CRUD operations)
6. ⏳ **Integration Testing** (E2E scenarios)
7. ⏳ **Performance Testing** (load testing with full dataset)

## Data Quality Notes

- All entities have realistic Polish addresses and contact details
- NIP and KRS numbers follow Polish formatting conventions
- Email addresses match entity domains
- Phone numbers use Polish format (+48)
- PESEL is masked for privacy (last 4 digits only)
- All passwords are hashed (placeholder hash used for test data)
- Timestamps use realistic dates (2024-2025 range)

## Known Issues/Warnings

⚠️ Some duplicate entries attempted during loading (expected behavior):
- Entities with codes TU-001 (already existed from partial run)
- Users k.kowalska@uknf.gov.pl, a.malinowska@bezpieczneub.pl
- Contact group "Instytucje ubezpieczeniowe"

These duplicates were safely ignored by PostgreSQL's unique constraints.

---

**Database Ready for Development and Testing! 🎉**
