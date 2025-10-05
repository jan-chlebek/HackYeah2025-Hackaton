# Cases/Access Requests (Wnioski) - Implementation Plan

**Date**: October 5, 2025
**Branch**: krzys
**Feature**: Cases Management & Access Requests System

---

## 📋 Overview

**Cases (Wnioski)** represent access requests from external users to gain permissions for specific supervised entities. This is a critical part of the Authentication & Authorization module.

---

## 🎯 Requirements Summary

### What is a Case/Access Request?
- External users register and automatically get a "Working" status access request
- Users fill in details, select entity, choose permissions (checkboxes)
- Submit for approval (status changes to "New")
- UKNF employees approve/reject/block requests
- Communication possible between user and UKNF employee within the request

### Key Statuses
1. **Working** - Draft, not yet submitted
2. **New** - Submitted, awaiting approval
3. **Accepted** - All permissions granted
4. **Blocked** - Rejected/blocked
5. **Updated** - Modified, awaiting re-approval

---

## 🗄️ Database Schema Updates Needed

### Current Case Entity - NEEDS EXPANSION

**Missing Fields for Access Requests**:
```csharp
// Permissions requested (as JSON or separate table)
public string? RequestedPermissions { get; set; } // JSON: ["Reporting", "Cases", "EntityAdmin"]

// Applicant information (from User, but captured at request time)
public string ApplicantFirstName { get; set; }
public string ApplicantLastName { get; set; }
public string ApplicantPeselMasked { get; set; } // Last 4 digits visible
public string ApplicantPhone { get; set; }
public string ApplicantEmail { get; set; }

// Entity email for notifications
public string? EntityNotificationEmail { get; set; }

// Approval information
public long? ApprovedByUserId { get; set; }
public DateTime? ApprovedAt { get; set; }
public User? ApprovedBy { get; set; }

// Block information
public long? BlockedByUserId { get; set; }
public DateTime? BlockedAt { get; set; }
public string? BlockReason { get; set; }
public User? BlockedBy { get; set; }

// Submission tracking
public DateTime? SubmittedAt { get; set; } // When status changed from Working to New
```

### Alternative: Separate AccessRequest Entity

Consider if Cases and AccessRequests should be separate entities:
- **Cases** = General administrative cases (investigations, audits)
- **AccessRequests** = Specific type for permission requests

**Recommendation**: Keep unified as Cases with a `CaseType` field:
```csharp
public CaseType Type { get; set; } // AccessRequest, Investigation, Audit, etc.
```

---

## 📦 Implementation Phases

### Phase 1: Entity & Database Updates (2-3 hours)

#### 1.1 Update Case Entity
**File**: `backend/UknfCommunicationPlatform.Core/Entities/Case.cs`

**Add fields**:
- [ ] Requested permissions (JSON or enum flags)
- [ ] Applicant snapshot data
- [ ] Entity notification email
- [ ] Approval tracking (approver, approval date)
- [ ] Block tracking (blocker, block date, reason)
- [ ] Submission tracking
- [ ] Case type (AccessRequest, Investigation, etc.)

#### 1.2 Update CaseStatus Enum
**File**: `backend/UknfCommunicationPlatform.Core/Enums/CaseStatus.cs`

**Ensure statuses match requirements**:
```csharp
public enum CaseStatus
{
    Working = 0,      // Draft
    New = 1,          // Submitted
    InProgress = 2,   // Being processed
    Accepted = 3,     // Approved
    Blocked = 4,      // Rejected
    Updated = 5,      // Modified, needs re-approval
    Resolved = 6,     // Completed
    Closed = 7        // Archived
}
```

#### 1.3 Create CaseType Enum
**File**: `backend/UknfCommunicationPlatform.Core/Enums/CaseType.cs`

```csharp
public enum CaseType
{
    AccessRequest = 1,
    Investigation = 2,
    Audit = 3,
    Compliance = 4,
    General = 5
}
```

#### 1.4 Create Permission Enum
**File**: `backend/UknfCommunicationPlatform.Core/Enums/RequestedPermission.cs`

```csharp
[Flags]
public enum RequestedPermission
{
    None = 0,
    Reporting = 1 << 0,        // 1
    Cases = 1 << 1,            // 2
    EntityAdmin = 1 << 2,      // 4
    DocumentLibrary = 1 << 3,  // 8
    Announcements = 1 << 4     // 16
}
```

#### 1.5 Create Migration
```bash
cd backend/UknfCommunicationPlatform.Api
dotnet ef migrations add AddCaseAccessRequestFields --project ../UknfCommunicationPlatform.Infrastructure
dotnet ef database update --project ../UknfCommunicationPlatform.Infrastructure
```

---

### Phase 2: DTOs (2-3 hours)

#### 2.1 Request DTOs
**Directory**: `backend/UknfCommunicationPlatform.Core/DTOs/Cases/`

**CreateCaseRequest.cs**:
```csharp
public class CreateCaseRequest
{
    public CaseType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public long SupervisedEntityId { get; set; }
    public int Priority { get; set; } = 3; // Default medium

    // For Access Requests
    public RequestedPermission? RequestedPermissions { get; set; }
    public string? EntityNotificationEmail { get; set; }
}
```

**UpdateCaseRequest.cs**:
```csharp
public class UpdateCaseRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public long? SupervisedEntityId { get; set; }
    public int? Priority { get; set; }
    public RequestedPermission? RequestedPermissions { get; set; }
    public string? EntityNotificationEmail { get; set; }
}
```

**CaseFilterRequest.cs**:
```csharp
public class CaseFilterRequest
{
    public CaseType? Type { get; set; }
    public CaseStatus? Status { get; set; }
    public long? SupervisedEntityId { get; set; }
    public long? HandlerId { get; set; }
    public string? Category { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
```

**SubmitCaseRequest.cs**:
```csharp
// For changing Working -> New
public class SubmitCaseRequest
{
    public string? Comments { get; set; }
}
```

**ApproveCaseRequest.cs**:
```csharp
public class ApproveCaseRequest
{
    public string? Comments { get; set; }
}
```

**BlockCaseRequest.cs**:
```csharp
public class BlockCaseRequest
{
    public string BlockReason { get; set; } = string.Empty;
    public string? Comments { get; set; }
}
```

#### 2.2 Response DTOs

**CaseResponse.cs**:
```csharp
public class CaseResponse
{
    public long Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public CaseType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public CaseStatus Status { get; set; }
    public int Priority { get; set; }

    // Entity info
    public long SupervisedEntityId { get; set; }
    public string SupervisedEntityName { get; set; } = string.Empty;

    // Applicant info (for Access Requests)
    public string? ApplicantName { get; set; }
    public string? ApplicantEmail { get; set; }
    public string? ApplicantPhone { get; set; }
    public string? ApplicantPeselMasked { get; set; }

    // Permissions (for Access Requests)
    public RequestedPermission? RequestedPermissions { get; set; }
    public List<string>? RequestedPermissionsList { get; set; }
    public string? EntityNotificationEmail { get; set; }

    // Handlers
    public long? HandlerId { get; set; }
    public string? HandlerName { get; set; }
    public long CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;

    // Approval/Block info
    public long? ApprovedByUserId { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public long? BlockedByUserId { get; set; }
    public string? BlockedByName { get; set; }
    public DateTime? BlockedAt { get; set; }
    public string? BlockReason { get; set; }

    // Dates
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    // Cancellation
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    // Polish UI fields
    public string StatusPolish { get; set; } = string.Empty;
    public string TypePolish { get; set; } = string.Empty;

    // Counts
    public int DocumentCount { get; set; }
    public int MessageCount { get; set; }
}
```

**CaseListItemResponse.cs** - Simplified for lists

**CaseDetailResponse.cs** - With documents and history included

---

### Phase 3: Service Layer (6-8 hours)

#### 3.1 Create CaseService
**File**: `backend/UknfCommunicationPlatform.Infrastructure/Services/CaseService.cs`

**Methods to implement**:
```csharp
// Core CRUD
public async Task<(List<CaseListItemResponse>, int totalCount)> GetCasesAsync(
    long userId, CaseFilterRequest filters, int page, int pageSize)

public async Task<CaseDetailResponse?> GetCaseByIdAsync(long caseId, long userId)

public async Task<CaseResponse> CreateCaseAsync(long userId, CreateCaseRequest request)

public async Task<CaseResponse> UpdateCaseAsync(long caseId, long userId, UpdateCaseRequest request)

public async Task<bool> DeleteCaseAsync(long caseId, long userId) // Drafts only

// State transitions
public async Task<CaseResponse> SubmitCaseAsync(long caseId, long userId, SubmitCaseRequest request)

public async Task<CaseResponse> ApproveCaseAsync(long caseId, long userId, ApproveCaseRequest request)

public async Task<CaseResponse> BlockCaseAsync(long caseId, long userId, BlockCaseRequest request)

public async Task<CaseResponse> CancelCaseAsync(long caseId, long userId, string reason)

public async Task<CaseResponse> ResolveCaseAsync(long caseId, long userId, string resolution)

public async Task<CaseResponse> CloseCaseAsync(long caseId, long userId)

// Assignment
public async Task<CaseResponse> AssignCaseAsync(long caseId, long handlerId, long assignedBy)

// Documents
public async Task<CaseDocumentResponse> UploadDocumentAsync(
    long caseId, long userId, IFormFile file, string description)

public async Task<CaseDocument?> GetDocumentAsync(long documentId, long userId)

public async Task<bool> DeleteDocumentAsync(long documentId, long userId)

// History
public async Task<List<CaseHistoryResponse>> GetCaseHistoryAsync(long caseId, long userId)

// Messages (conversation within case)
public async Task<List<CaseMessageResponse>> GetCaseMessagesAsync(long caseId, long userId)

public async Task<CaseMessageResponse> AddCaseMessageAsync(
    long caseId, long userId, string message)

// Statistics
public async Task<CaseStatsResponse> GetCaseStatsAsync(long userId)
```

**Business Logic**:
- Access control (creator, handler, entity users can view)
- State machine validation (Working -> New -> Accepted/Blocked)
- Automatic case number generation
- History tracking for all state changes
- Snapshot applicant data on creation
- Permission flag manipulation

---

### Phase 4: API Controller (4-5 hours)

#### 4.1 Create CasesController
**File**: `backend/UknfCommunicationPlatform.Api/Controllers/v1/CasesController.cs`

**Endpoints** (12 total):
```csharp
[ApiController]
[Route("api/v1/cases")]
[Authorize]
public class CasesController : ControllerBase
{
    // Core CRUD
    [HttpGet]
    public async Task<ActionResult<PagedResult<CaseListItemResponse>>> GetCases(
        [FromQuery] CaseFilterRequest filters,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CaseDetailResponse>> GetCaseById(long id)

    [HttpPost]
    public async Task<ActionResult<CaseResponse>> CreateCase(
        [FromBody] CreateCaseRequest request)

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CaseResponse>> UpdateCase(
        long id,
        [FromBody] UpdateCaseRequest request)

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteCase(long id) // Drafts only

    // State transitions
    [HttpPost("{id:long}/submit")]
    public async Task<ActionResult<CaseResponse>> SubmitCase(
        long id,
        [FromBody] SubmitCaseRequest request)

    [HttpPost("{id:long}/approve")]
    [Authorize(Roles = "UKNF")]
    public async Task<ActionResult<CaseResponse>> ApproveCase(
        long id,
        [FromBody] ApproveCaseRequest request)

    [HttpPost("{id:long}/block")]
    [Authorize(Roles = "UKNF")]
    public async Task<ActionResult<CaseResponse>> BlockCase(
        long id,
        [FromBody] BlockCaseRequest request)

    [HttpPost("{id:long}/cancel")]
    public async Task<ActionResult<CaseResponse>> CancelCase(
        long id,
        [FromBody] CancelCaseRequest request)

    [HttpPost("{id:long}/resolve")]
    [Authorize(Roles = "UKNF")]
    public async Task<ActionResult<CaseResponse>> ResolveCase(
        long id,
        [FromBody] ResolveCaseRequest request)

    [HttpPost("{id:long}/close")]
    [Authorize(Roles = "UKNF")]
    public async Task<ActionResult> CloseCase(long id)

    [HttpPost("{id:long}/assign")]
    [Authorize(Roles = "UKNF")]
    public async Task<ActionResult<CaseResponse>> AssignCase(
        long id,
        [FromBody] AssignCaseRequest request)

    // Documents
    [HttpPost("{id:long}/documents")]
    public async Task<ActionResult<CaseDocumentResponse>> UploadDocument(
        long id,
        IFormFile file,
        [FromForm] string? description)

    [HttpGet("{id:long}/documents/{documentId:long}")]
    public async Task<IActionResult> DownloadDocument(long id, long documentId)

    [HttpDelete("{id:long}/documents/{documentId:long}")]
    public async Task<ActionResult> DeleteDocument(long id, long documentId)

    // History
    [HttpGet("{id:long}/history")]
    public async Task<ActionResult<List<CaseHistoryResponse>>> GetCaseHistory(long id)

    // Messages
    [HttpGet("{id:long}/messages")]
    public async Task<ActionResult<List<CaseMessageResponse>>> GetCaseMessages(long id)

    [HttpPost("{id:long}/messages")]
    public async Task<ActionResult<CaseMessageResponse>> AddCaseMessage(
        long id,
        [FromBody] AddCaseMessageRequest request)

    // Statistics
    [HttpGet("stats")]
    public async Task<ActionResult<CaseStatsResponse>> GetCaseStats()
}
```

---

### Phase 5: Integration Tests (3-4 hours)

#### 5.1 Create CasesControllerTests
**File**: `backend/UknfCommunicationPlatform.Tests.Integration/Controllers/CasesControllerTests.cs`

**Test Scenarios** (20+ tests):
- [ ] GetCases_WithFilters_ReturnsFilteredResults
- [ ] GetCases_ByType_ReturnsOnlyAccessRequests
- [ ] GetCases_ByStatus_ReturnsCorrectCases
- [ ] GetCaseById_WithValidId_ReturnsDetails
- [ ] GetCaseById_AsUnauthorized_Returns403
- [ ] CreateCase_AccessRequest_CreatesWithWorkingStatus
- [ ] CreateCase_WithPermissions_StoresCorrectly
- [ ] UpdateCase_InWorkingStatus_Succeeds
- [ ] UpdateCase_InNewStatus_Fails
- [ ] DeleteCase_InWorkingStatus_Succeeds
- [ ] DeleteCase_InNewStatus_Fails
- [ ] SubmitCase_ChangesStatusToNew
- [ ] SubmitCase_RecordsSubmissionDate
- [ ] ApproveCase_AsUKNF_ChangesStatusToAccepted
- [ ] ApproveCase_AsEntity_Returns403
- [ ] BlockCase_AsUKNF_ChangesStatusToBlocked
- [ ] BlockCase_StoresBlockReason
- [ ] CancelCase_AsCreator_Succeeds
- [ ] AssignCase_AsUKNF_AssignsHandler
- [ ] UploadDocument_AddsToCase
- [ ] GetCaseHistory_ReturnsAllChanges
- [ ] AddCaseMessage_CreatesMessage
- [ ] GetCaseMessages_ReturnsConversation

---

### Phase 6: Database Seeding (1-2 hours)

#### 6.1 Update DatabaseSeeder
**File**: `backend/UknfCommunicationPlatform.Infrastructure/Data/DatabaseSeeder.cs`

**Add sample cases**:
- 3-5 Access Requests in different statuses
- Include Working, New, Accepted, Blocked examples
- Link to existing users and entities
- Add sample documents and messages

---

## 📁 File Structure Summary

```
backend/
├── UknfCommunicationPlatform.Core/
│   ├── Entities/
│   │   └── Case.cs (UPDATE - add access request fields)
│   ├── Enums/
│   │   ├── CaseStatus.cs (UPDATE - ensure all statuses)
│   │   ├── CaseType.cs (NEW)
│   │   └── RequestedPermission.cs (NEW - flags enum)
│   └── DTOs/
│       └── Cases/ (NEW DIRECTORY)
│           ├── CreateCaseRequest.cs
│           ├── UpdateCaseRequest.cs
│           ├── CaseFilterRequest.cs
│           ├── SubmitCaseRequest.cs
│           ├── ApproveCaseRequest.cs
│           ├── BlockCaseRequest.cs
│           ├── CancelCaseRequest.cs
│           ├── ResolveCaseRequest.cs
│           ├── AssignCaseRequest.cs
│           ├── CaseResponse.cs
│           ├── CaseListItemResponse.cs
│           ├── CaseDetailResponse.cs
│           ├── CaseDocumentResponse.cs
│           ├── CaseHistoryResponse.cs
│           ├── CaseMessageResponse.cs
│           └── CaseStatsResponse.cs
├── UknfCommunicationPlatform.Infrastructure/
│   ├── Services/
│   │   └── CaseService.cs (NEW - full implementation)
│   ├── Data/
│   │   └── DatabaseSeeder.cs (UPDATE - add case seeding)
│   └── Migrations/
│       └── [timestamp]_AddCaseAccessRequestFields.cs (GENERATED)
├── UknfCommunicationPlatform.Api/
│   └── Controllers/
│       └── v1/
│           └── CasesController.cs (NEW - 18+ endpoints)
└── UknfCommunicationPlatform.Tests.Integration/
    └── Controllers/
        └── CasesControllerTests.cs (NEW - 20+ tests)
```

---

## ⏱️ Time Estimates

| Phase | Task | Hours |
|-------|------|-------|
| 1 | Entity & Database Updates | 2-3 |
| 2 | DTOs | 2-3 |
| 3 | Service Layer | 6-8 |
| 4 | API Controller | 4-5 |
| 5 | Integration Tests | 3-4 |
| 6 | Database Seeding | 1-2 |
| **TOTAL** | **Full Implementation** | **18-25 hours** |

---

## 🎯 Priority Implementation Order

### MVP (8-10 hours):
1. Entity updates + migration (2h)
2. Basic DTOs (1h)
3. Core service methods (CRUD + Submit + Approve) (3-4h)
4. Core API endpoints (GET, POST, Submit, Approve) (2-3h)

### Full Feature (18-25 hours):
1. Complete all DTOs (2-3h)
2. All service methods (6-8h)
3. All API endpoints (4-5h)
4. Integration tests (3-4h)
5. Database seeding (1-2h)

---

## 📝 Notes

- **Access Requests vs General Cases**: Consider keeping unified with CaseType enum
- **Permissions Storage**: Using Flags enum allows bitwise operations
- **Applicant Snapshot**: Store applicant data at creation time (not reference User) for audit trail
- **State Machine**: Implement strict validation for status transitions
- **History Tracking**: Log all state changes, assignments, approvals automatically
- **Communication**: Messages within cases create audit trail of conversation

---

**Created**: October 5, 2025
**Status**: Ready for implementation
