# UKNF Platform - Backend-Only Priority Analysis

**Date**: October 5, 2025
**Branch**: krzys
**Focus**: Backend API Implementation Only (No Frontend)

---

## 🎯 Executive Summary - Backend Status

### Current Backend State
- ✅ **Infrastructure**: Excellent (DB, entities, migrations, seeding, testing)
- ✅ **Admin Module**: 33% complete (17 endpoints working)
- ✅ **Communication Module**: Data layer 100%, API layer 4%
- ✅ **Test Coverage**: 222/222 tests passing (188 unit + 34 integration)
- 🚨 **Security**: Authorization completely disabled - **CRITICAL ISSUE**

### Backend Completion Status
```
Backend Implementation Progress:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Database & Entities         ████████████████████ 100% ✅
Testing Infrastructure      ████████████████████ 100% ✅
Admin API Endpoints         ███████░░░░░░░░░░░░░  35% 🚧
Communication API Endpoints █░░░░░░░░░░░░░░░░░░░   5% 🚧
Authentication/Security     ████████████░░░░░░░░  60% ⚠️ (DISABLED)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Overall Backend Completion: ████████░░░░░░░░░░░░░░░░  32%
```

---

## 🚨 CRITICAL PRIORITY 0: Security (MUST DO FIRST)

### Authorization Re-enablement
**Status**: 🚨 DISABLED - Critical security vulnerability
**Estimated Time**: 4-6 hours
**Risk**: System cannot be demoed or deployed without this

#### Files to Fix:

1. **Program.cs** (Line ~176)
```csharp
// Currently DISABLED:
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// app.UseAuthentication();
// app.UseAuthorization();

// MUST UNCOMMENT:
app.UseAuthentication();
app.UseAuthorization();
```

2. **Controllers with Disabled Authorization**:
   - `EntitiesController.cs` - Class-level `[Authorize]` (Line 14)
   - `MessagesController.cs` - Class-level `[Authorize]` (Line 16)
   - `AnnouncementsController.cs` - Class-level + method-level `[Authorize]` (Lines 15, 101, 142, 188)
   - `AuthController.cs` - Multiple method-level `[Authorize]` attributes

3. **Remove Hardcoded User ID Fallbacks**:
   - `MessagesController.GetCurrentUserId()` - Returns hardcoded user ID 2
   - `AnnouncementsController.GetCurrentUserId()` - Returns hardcoded user ID 2
   - `AuthController` - Multiple mock data returns when auth disabled

#### Action Items:
- [ ] Uncomment all `[Authorize]` attributes
- [ ] Remove `GetCurrentUserId()` fallback methods
- [ ] Remove mock data returns in AuthController
- [ ] Update integration tests to include JWT tokens
- [ ] Test all endpoints with real authentication
- [ ] Verify role-based authorization works (UKNF vs Entity users)

**Files Affected**: 5 controllers, Program.cs, integration tests

---

## 📋 HIGH PRIORITY: Complete Communication Module APIs

### Current Communication Module Status

#### ✅ Completed (Data Layer - Phase 1)
- Database entities: Message, Report, Case, Announcement, FileLibrary, Contact, FAQ
- All migrations applied
- Database seeding with Polish test data
- Entity relationships configured

#### 🚧 Partially Complete
**Reports API**: 4 of 8 endpoints (50%)
- [x] ✅ `GET /api/v1/reports` - List with filters
- [x] ✅ `GET /api/v1/reports/{id}` - Get details
- [x] ✅ `POST /api/v1/reports` - Upload XLSX
- [x] ✅ `PUT /api/v1/reports/{id}/status` - Update status
- [ ] ⏳ `POST /api/v1/reports/{id}/validate` - Trigger validation
- [ ] ⏳ `GET /api/v1/reports/{id}/validation-report` - Download report
- [ ] ⏳ `POST /api/v1/reports/{id}/correct` - Submit correction
- [ ] ⏳ `GET /api/v1/reports/missing` - List missing reports

**Announcements API**: 6 of 6 endpoints (100% - but auth disabled!)
- [x] ✅ All endpoints implemented
- [x] ✅ Service layer complete
- [x] ✅ Integration tests passing (34/34)
- [ ] 🚨 Authorization disabled - must re-enable

---

## 🎯 PRIORITY 1: Messages System API (TOP PRIORITY)

**Status**: ⏳ 0% implementation (entities exist, no service/controllers)
**Estimated Time**: 12-16 hours (backend only)
**Business Value**: ⭐⭐⭐⭐⭐ (Core communication feature)

### Implementation Checklist

#### 1. Service Layer (6-8 hours)
Create `MessageService.cs` with methods:

**Core Message Operations**:
- [ ] `GetMessagesAsync(userId, filters, pagination)` - List with filtering
- [ ] `GetMessageByIdAsync(messageId, userId)` - Get details with access check
- [ ] `SendMessageAsync(senderId, createDto)` - Send new message
- [ ] `ReplyToMessageAsync(originalMessageId, senderId, replyDto)` - Reply to message
- [ ] `CancelMessageAsync(messageId, userId)` - Cancel message (UKNF only)
- [ ] `DeleteDraftAsync(messageId, userId)` - Delete draft
- [ ] `MarkAsReadAsync(messageId, userId)` - Mark as read
- [ ] `GetUnreadCountAsync(userId)` - Get unread counts
- [ ] `GetThreadAsync(threadId, userId)` - Get full conversation thread
- [ ] `BulkSendAsync(senderId, recipientIds, messageDto)` - Send to multiple recipients
- [ ] `ExportMessagesAsync(userId, filters)` - Export to CSV

**Attachment Operations**:
- [ ] `UploadAttachmentAsync(messageId, file)` - Upload attachment
- [ ] `GetAttachmentAsync(attachmentId)` - Get attachment metadata
- [ ] `DownloadAttachmentAsync(attachmentId)` - Download file
- [ ] `DeleteAttachmentAsync(attachmentId)` - Delete attachment

**Business Logic**:
- Access control (sender/recipient only)
- Read status tracking
- Thread management (parent-child relationships)
- Attachment validation (file size, types)
- Bulk send with transaction handling
- CSV export formatting

#### 2. DTOs (2 hours)
Create DTOs in `UknfCommunicationPlatform.Core/DTOs/Messages/`:

**Request DTOs**:
- [ ] `CreateMessageRequest` - Subject, Body, RecipientId, RelatedEntityId, Attachments[]
- [ ] `ReplyMessageRequest` - Body, Attachments[]
- [ ] `BulkSendRequest` - Subject, Body, RecipientIds[], RelatedEntityId
- [ ] `MessageFilterRequest` - IsRead, Status, SentDateFrom, SentDateTo, SenderId, RecipientId

**Response DTOs**:
- [ ] `MessageResponse` - Full message with sender, recipient, entity, attachments
- [ ] `MessageListItemResponse` - Simplified for list views
- [ ] `MessageAttachmentResponse` - File metadata
- [ ] `UnreadCountResponse` - Counts by category
- [ ] `MessageThreadResponse` - Message + all replies

**Validation**:
- Required fields validation
- Max length constraints
- Attachment size/type validation
- Recipient validation (must be active user)

#### 3. API Controller (3-4 hours)
Create `MessagesController.cs` with endpoints:

```csharp
[ApiController]
[Route("api/v1/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    // Core endpoints
    [HttpGet]
    public async Task<ActionResult<PagedResult<MessageListItemResponse>>> GetMessages(
        [FromQuery] MessageFilterRequest filters,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20);

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MessageResponse>> GetMessageById(long id);

    [HttpPost]
    public async Task<ActionResult<MessageResponse>> SendMessage(
        [FromBody] CreateMessageRequest request);

    [HttpPost("{id:long}/reply")]
    public async Task<ActionResult<MessageResponse>> ReplyToMessage(
        long id,
        [FromBody] ReplyMessageRequest request);

    [HttpPost("{id:long}/cancel")]
    [Authorize(Roles = "UKNF")]
    public async Task<ActionResult> CancelMessage(long id);

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteDraft(long id);

    [HttpPost("{id:long}/mark-read")]
    public async Task<ActionResult> MarkAsRead(long id);

    [HttpGet("unread-count")]
    public async Task<ActionResult<UnreadCountResponse>> GetUnreadCount();

    [HttpGet("threads/{threadId:long}")]
    public async Task<ActionResult<MessageThreadResponse>> GetThread(long threadId);

    [HttpPost("bulk-send")]
    [Authorize(Roles = "UKNF")]
    public async Task<ActionResult> BulkSend([FromBody] BulkSendRequest request);

    [HttpGet("export")]
    public async Task<IActionResult> ExportMessages(
        [FromQuery] MessageFilterRequest filters);

    // Attachment endpoints
    [HttpPost("{id:long}/attachments")]
    public async Task<ActionResult<MessageAttachmentResponse>> UploadAttachment(
        long id,
        IFormFile file);

    [HttpGet("attachments/{attachmentId:long}")]
    public async Task<IActionResult> DownloadAttachment(long attachmentId);

    [HttpDelete("attachments/{attachmentId:long}")]
    public async Task<ActionResult> DeleteAttachment(long attachmentId);
}
```

#### 4. Integration Tests (2-3 hours)
Create `MessagesControllerTests.cs`:

**Test Coverage**:
- [ ] GetMessages_WithFilters_ReturnsFilteredResults
- [ ] GetMessages_WithPagination_ReturnsCorrectPage
- [ ] GetMessageById_WithValidId_ReturnsDetails
- [ ] GetMessageById_AsRecipient_ReturnsMessage
- [ ] GetMessageById_AsUnauthorizedUser_Returns403
- [ ] SendMessage_WithValidData_CreatesMessage
- [ ] SendMessage_WithAttachments_UploadsFiles
- [ ] ReplyToMessage_WithValidId_CreatesReply
- [ ] CancelMessage_AsUKNF_CancelsMessage
- [ ] CancelMessage_AsEntity_Returns403
- [ ] DeleteDraft_AsOwner_DeletesMessage
- [ ] MarkAsRead_UpdatesReadStatus
- [ ] GetUnreadCount_ReturnsAccurateCount
- [ ] GetThread_ReturnsFullConversation
- [ ] BulkSend_AsUKNF_SendsToMultiple
- [ ] ExportMessages_ReturnsCsvFile

---

## 🎯 PRIORITY 2: Complete Reports API (Remaining Endpoints)

**Status**: ✅ 50% complete (4 of 8 endpoints)
**Estimated Time**: 6-8 hours
**Business Value**: ⭐⭐⭐⭐⭐

### Remaining Implementation

#### 1. Service Layer Methods (3-4 hours)
Add to `ReportService.cs`:

**Validation Workflow**:
- [ ] `TriggerValidationAsync(reportId)` - Async validation job
- [ ] `GetValidationReportAsync(reportId)` - Generate validation report
- [ ] `SubmitCorrectionAsync(reportId, correctionFile)` - Submit corrected report
- [ ] `GetMissingReportsAsync(entityId, period)` - Detect missing reports

**Business Logic**:
- XLSX file validation (structure, data types, required fields)
- Validation report generation (errors, warnings)
- Correction tracking (link to original report)
- Missing report detection (based on submission schedule)
- Background job integration (placeholder for now)

#### 2. API Controller Methods (2 hours)
Add to existing `ReportsController.cs`:

```csharp
[HttpPost("{id:long}/validate")]
[Authorize(Roles = "UKNF")]
public async Task<ActionResult> TriggerValidation(long id);

[HttpGet("{id:long}/validation-report")]
public async Task<IActionResult> GetValidationReport(long id);

[HttpPost("{id:long}/correct")]
public async Task<ActionResult<ReportResponse>> SubmitCorrection(
    long id,
    IFormFile correctionFile);

[HttpGet("missing")]
[Authorize(Roles = "UKNF")]
public async Task<ActionResult<List<MissingReportResponse>>> GetMissingReports(
    [FromQuery] long? entityId,
    [FromQuery] string? period);
```

#### 3. Integration Tests (1-2 hours)
Add to `ReportsControllerTests.cs`:
- [ ] TriggerValidation_AsUKNF_StartsValidation
- [ ] GetValidationReport_ReturnsReport
- [ ] SubmitCorrection_WithValidFile_UpdatesReport
- [ ] GetMissingReports_ReturnsExpectedReports

---

## 🎯 PRIORITY 3: File Library API (Full Implementation)

**Status**: ⏳ 10% complete (entities only)
**Estimated Time**: 12-14 hours
**Business Value**: ⭐⭐⭐⭐

### Implementation Checklist

#### 1. Service Layer (6-7 hours)
Create `FileLibraryService.cs`:

**Core File Operations**:
- [ ] `GetFilesAsync(userId, filters, pagination)` - List with permissions check
- [ ] `GetFileByIdAsync(fileId, userId)` - Get metadata with access check
- [ ] `UploadFileAsync(userId, uploadDto)` - Upload with chunking support
- [ ] `UpdateFileMetadataAsync(fileId, updateDto)` - Update metadata
- [ ] `DeleteFileAsync(fileId, userId)` - Delete with permission check
- [ ] `DownloadFileAsync(fileId, userId)` - Download with access check
- [ ] `SearchFilesAsync(searchQuery, userId)` - Full-text search
- [ ] `BulkDownloadAsync(fileIds, userId)` - Create ZIP archive
- [ ] `GetStatisticsAsync()` - Usage statistics

**Permission Operations**:
- [ ] `SetPermissionsAsync(fileId, permissions)` - Set access rules
- [ ] `CheckPermissionAsync(fileId, userId, action)` - Verify access
- [ ] `GetFilePermissionsAsync(fileId)` - List permissions

**Business Logic**:
- Permission system (Role/Podmiot/User based)
- File chunking for large uploads
- ZIP generation for bulk download
- Virus scanning placeholder
- Storage path management
- Metadata extraction

#### 2. DTOs (2 hours)
- [ ] `UploadFileRequest`
- [ ] `UpdateFileRequest`
- [ ] `FileLibraryResponse`
- [ ] `FilePermissionRequest`
- [ ] `FileSearchRequest`

#### 3. API Controller (3-4 hours)
Create `FileLibraryController.cs` with 10 endpoints

#### 4. Integration Tests (1-2 hours)
Create `FileLibraryControllerTests.cs` with 12+ tests

---

## 🎯 PRIORITY 4: Cases Management API

**Status**: ⏳ 10% complete (entities only)
**Estimated Time**: 14-16 hours
**Business Value**: ⭐⭐⭐⭐

### Implementation Checklist

#### 1. Service Layer (7-8 hours)
Create `CaseService.cs`:

**Core Case Operations**:
- [ ] `GetCasesAsync(userId, filters, pagination)`
- [ ] `GetCaseByIdAsync(caseId, userId)`
- [ ] `CreateCaseAsync(userId, createDto)`
- [ ] `UpdateCaseAsync(caseId, updateDto)`
- [ ] `DeleteCaseAsync(caseId)` - Drafts only
- [ ] `CancelCaseAsync(caseId)`
- [ ] `ResolveCaseAsync(caseId, resolution)`
- [ ] `CloseCaseAsync(caseId)`

**Case Interaction**:
- [ ] `AddMessageToCaseAsync(caseId, message)`
- [ ] `UploadCaseDocumentAsync(caseId, file)`
- [ ] `GetCaseHistoryAsync(caseId)`
- [ ] `AssignCaseAsync(caseId, handlerId)`

**Business Logic**:
- Case lifecycle state machine
- Access control (assigned users only)
- History tracking (audit trail)
- Document management
- Assignment workflow

#### 2. DTOs & Controller (3-4 hours)
- 8 request/response DTOs
- 12 API endpoints

#### 3. Integration Tests (2-3 hours)
- 15+ test scenarios

---

## 🎯 PRIORITY 5: Contacts & Groups API

**Status**: ✅ 80% backend ready (entities + seeding)
**Estimated Time**: 6-8 hours
**Business Value**: ⭐⭐⭐

### Implementation Checklist

#### 1. Service Layer (3-4 hours)
Create `ContactService.cs`:
- [ ] CRUD operations for Contacts
- [ ] CRUD operations for ContactGroups
- [ ] Group member management
- [ ] Search functionality

#### 2. DTOs & Controller (2-3 hours)
- 6 DTOs
- 8 API endpoints

#### 3. Integration Tests (1-2 hours)
- 10+ test scenarios

---

## 🎯 PRIORITY 6: FAQ API Enhancement

**Status**: ✅ 50% complete (basic API exists)
**Estimated Time**: 4-6 hours
**Business Value**: ⭐⭐⭐

### Remaining Implementation

#### 1. Service Layer (2-3 hours)
Add to `FaqService.cs`:
- [ ] Rating system implementation
- [ ] Search with ranking
- [ ] Analytics (most viewed, highest rated)

#### 2. API Enhancement (1-2 hours)
Add endpoints:
- `POST /api/v1/faq/{id}/rate`
- `GET /api/v1/faq/search`
- `GET /api/v1/faq/analytics`

#### 3. Tests (1 hour)
- 5+ test scenarios

---

## 🎯 PRIORITY 7: Admin Module - Sprints 2 & 3

**Status**: 33% complete (Sprint 1 done)
**Estimated Time**: 12-16 hours
**Business Value**: ⭐⭐⭐⭐

### Sprint 2: Roles & Permissions (8-10 hours)

#### 1. Service Layer (4-5 hours)
Create `RoleManagementService.cs`:
- [ ] CRUD for roles
- [ ] Permission assignment
- [ ] User-role assignment
- [ ] Permission checking

#### 2. Controller (2-3 hours)
Create `RolesController.cs`:
- 8 endpoints for roles
- 2 endpoints for permissions

#### 3. Tests (2 hours)
- 12+ test scenarios

### Sprint 3: Audit & Import (4-6 hours)

#### 1. Audit Service (2-3 hours)
- [ ] Log audit events
- [ ] Query audit logs
- [ ] Export functionality

#### 2. CSV Import (2-3 hours)
- [ ] Entity CSV import
- [ ] Validation
- [ ] Error reporting

---

## 📊 BACKEND IMPLEMENTATION SUMMARY

### Total Estimated Time (Backend Only)

| Priority | Feature | Hours | Status |
|----------|---------|-------|--------|
| P0 | Re-enable Authorization | 4-6 | 🚨 CRITICAL |
| P1 | Messages System API | 12-16 | ⏳ Not Started |
| P2 | Complete Reports API | 6-8 | 🚧 50% Done |
| P2 | Announcements (Re-auth) | 2-3 | 🚧 90% Done |
| P3 | File Library API | 12-14 | ⏳ Not Started |
| P4 | Cases Management API | 14-16 | ⏳ Not Started |
| P5 | Contacts & Groups API | 6-8 | ⏳ Not Started |
| P6 | FAQ Enhancement | 4-6 | 🚧 50% Done |
| P7 | Admin Sprints 2-3 | 12-16 | ⏳ Not Started |

**Total Backend Work Remaining**: ~75-95 hours (2-3 weeks with 1 developer)

---

## 🚀 RECOMMENDED BACKEND-ONLY ROADMAP

### Week 1: Security + Core Communication
**Day 1-2** (8-10 hours):
- [ ] Re-enable authorization completely
- [ ] Fix all integration tests for auth
- [ ] Verify role-based access control

**Day 3-5** (16-20 hours):
- [ ] Messages System API complete
  - Service layer
  - DTOs
  - Controller
  - Integration tests

### Week 2: Complete Communication Features
**Day 1-2** (8-10 hours):
- [ ] Complete Reports API (4 endpoints)
- [ ] Re-enable Announcements auth (2-3h)
- [ ] File Library API start (5-6h remaining)

**Day 3-5** (12-16 hours):
- [ ] Complete File Library API
- [ ] Contacts & Groups API

### Week 3: Cases + Admin + Polish
**Day 1-3** (14-18 hours):
- [ ] Cases Management API complete
- [ ] Admin Module Sprint 2 (Roles)

**Day 4-5** (8-10 hours):
- [ ] Admin Module Sprint 3 (Audit, Import)
- [ ] FAQ enhancements
- [ ] Code review and cleanup
- [ ] Update Swagger documentation
- [ ] Performance testing

---

## ✅ IMMEDIATE ACTION PLAN (Next 8 Hours)

### Session 1: Security Fix (4 hours)
1. **Re-enable auth in Program.cs** (15 min)
2. **Remove all `GetCurrentUserId()` fallbacks** (30 min)
3. **Uncomment all `[Authorize]` attributes** (30 min)
4. **Update integration tests** (2 hours)
   - Add JWT token generation helper
   - Update all test methods to authenticate
   - Add role-based auth tests
5. **Run all tests and fix issues** (45 min)

### Session 2: Messages Service Start (4 hours)
1. **Create MessageService.cs skeleton** (30 min)
2. **Implement GetMessagesAsync** (1 hour)
3. **Implement SendMessageAsync** (1.5 hours)
4. **Write unit tests** (1 hour)

---

## 📈 SUCCESS CRITERIA (Backend Only)

### MVP Backend Complete
- [x] ✅ Database & entities (100%)
- [x] ✅ Testing infrastructure (100%)
- [ ] 🚨 Authentication/Authorization (Re-enable - 4-6h)
- [ ] ⏳ Messages API (12-16h)
- [ ] 🚧 Reports API (6-8h to complete)
- [ ] ⏳ Announcements API (Re-enable auth - 2-3h)
- [ ] ⏳ File Library API (12-14h)
- [ ] ⏳ Cases API (14-16h)
- [ ] ⏳ Contacts API (6-8h)
- [ ] 🚧 FAQ API (4-6h to complete)
- [ ] ⏳ Admin Module complete (12-16h)

**Current**: 32% complete
**After Security Fix**: 36% complete
**After Messages + Reports**: 55% complete
**After Week 2**: 75% complete
**After Week 3**: 100% complete ✅

---

## 🎯 KEY METRICS

**Total API Endpoints**:
- ✅ Implemented: 21 endpoints (Admin 17 + Reports 4)
- 🚧 Partially Done: 6 endpoints (Announcements - auth disabled)
- ⏳ Remaining: ~70 endpoints

**Test Coverage**:
- ✅ Current: 222/222 passing (100%)
- Target: Maintain 100% pass rate
- Add: ~50 new integration tests

**Code Quality**:
- All services with comprehensive error handling
- Proper DTOs with validation
- Consistent API patterns
- Full XML documentation
- Swagger documentation updated

---

## 💡 BACKEND-SPECIFIC RECOMMENDATIONS

1. **Focus on API Completeness First**: Get all endpoints working before frontend
2. **Maintain Test Coverage**: Write integration tests for every endpoint
3. **Document Everything**: Update Swagger as you go
4. **Use Consistent Patterns**: Follow existing service/controller patterns
5. **Security First**: Don't add new features until auth is re-enabled
6. **Database Performance**: Add indexes if queries slow down
7. **Error Handling**: Consistent error responses (Problem Details)
8. **Logging**: Comprehensive logging for debugging

---

**Next Step**: Fix authorization (4-6 hours) - Everything else depends on this! 🚨
