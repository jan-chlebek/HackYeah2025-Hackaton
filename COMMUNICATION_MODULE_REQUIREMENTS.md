# Communication Module - Requirements & Implementation Plan

**Branch:** krzys
**Date:** 2025-10-04
**Status:** Phase 1 Complete (Data Layer)

## 📋 Executive Summary

The **Communication Module (Moduł Komunikacyjny)** is the primary module of the UKNF Communication Platform. It enables secure, two-way communication between UKNF staff and supervised financial entities, manages regulatory reporting, maintains document libraries, handles administrative cases, and provides information through FAQ and bulletin board systems.

**Current Backend Status:** 33% (Data Layer Complete, API Layer Pending)
**Priority:** CRITICAL (Core platform functionality)

---

## 🎯 Module Objectives

Per requirements (DETAILS_UKNF_Prompt2Code2.md, RULES_UKNF_Prompt2Code2.md):

1. **Reports Management** - Handle acceptance of reports submitted by supervised entities with validation feedback
2. **Two-Way Messaging** - Enable communication between internal and external users with attachments
3. **File Library** - Maintain local repository performing library functions with versioning and permissions
4. **Case Management** - Handle and track administrative cases concerning supervised entities
5. **Bulletin Board** - Broadcast announcements to selected groups with read confirmations
6. **Contact Management** - Maintain addressees, contact groups, and contacts registry
7. **FAQ System** - Database of questions and answers in FAQ convention
8. **Entity Data Management** - Update information on supervised entities

---

## 🏗️ Required Backend Components

### Phase 1: Data Layer ✅ COMPLETE

#### Enums (5 created)
- ✅ **MessageStatus.cs** - 7 states (Draft, Sent, Read, AwaitingUknfResponse, AwaitingUserResponse, Closed, Cancelled)
- ✅ **MessageFolder.cs** - 6 folders (Inbox, Sent, Drafts, Reports, Cases, Applications)
- ✅ **CaseStatus.cs** - 7 states (New, InProgress, AwaitingUknfResponse, AwaitingUserResponse, Resolved, Closed, Cancelled)
- ✅ **AnnouncementPriority.cs** - 3 levels (Low, Medium, High)
- ✅ **FaqQuestionStatus.cs** - 5 states (Submitted, InProgress, Answered, Published, Rejected)

#### Core Entities (17 total)
- ✅ **Message.cs** (enhanced) - Two-way messaging with threads and attachments
- ✅ **MessageAttachment.cs** - File attachments for messages
- ✅ **Case.cs** - Administrative case management
- ✅ **CaseDocument.cs** - Documents attached to cases
- ✅ **CaseHistory.cs** - Case audit trail
- ✅ **Announcement.cs** - Bulletin board messages
- ✅ **AnnouncementAttachment.cs** - Files attached to announcements
- ✅ **AnnouncementRead.cs** - Read confirmation tracking
- ✅ **AnnouncementRecipient.cs** - Flexible recipient targeting
- ✅ **AnnouncementHistory.cs** - Announcement change tracking
- ✅ **FileLibrary.cs** - Document repository with versioning
- ✅ **FileLibraryPermission.cs** - Granular access control
- ✅ **FaqQuestion.cs** - Q&A database
- ✅ **FaqRating.cs** - User ratings for answers
- ✅ **Contact.cs** - Contact registry
- ✅ **ContactGroup.cs** - Contact groups
- ✅ **ContactGroupMember.cs** - Group membership

#### Existing Entities (Reused)
- ✅ **Report.cs** - Already implemented in prior phase
- ✅ **SupervisedEntity.cs** - Already implemented in core

#### Database Configuration
- ✅ **ApplicationDbContext.cs** - 23 DbSets added, relationships configured
- ✅ **Migration** - CommunicationModuleEntities migration created

---

### Phase 2: DTOs & Validation ⏳ PENDING

#### Request DTOs (Needed: ~40)
**Messages:**
- [ ] SendMessageRequest
- [ ] ReplyToMessageRequest
- [ ] MessageFilterRequest

**Cases:**
- [ ] CreateCaseRequest
- [ ] UpdateCaseRequest
- [ ] AddCaseDocumentRequest
- [ ] CaseFilterRequest

**Announcements:**
- [ ] CreateAnnouncementRequest
- [ ] UpdateAnnouncementRequest
- [ ] PublishAnnouncementRequest
- [ ] ConfirmReadRequest

**File Library:**
- [ ] UploadFileRequest
- [ ] UpdateFileMetadataRequest
- [ ] ManagePermissionsRequest
- [ ] FileFilterRequest

**FAQ:**
- [ ] SubmitQuestionRequest
- [ ] AnswerQuestionRequest
- [ ] RateAnswerRequest
- [ ] FaqFilterRequest

**Contacts:**
- [ ] CreateContactRequest
- [ ] CreateContactGroupRequest
- [ ] AddContactToGroupRequest

**Reports:**
- [ ] SubmitReportRequest (exists)
- [ ] UpdateReportStatusRequest

#### Response DTOs (Needed: ~40)
- [ ] MessageResponse, MessageListResponse
- [ ] CaseResponse, CaseListResponse, CaseHistoryResponse
- [ ] AnnouncementResponse, AnnouncementListResponse, ReadStatsResponse
- [ ] FileLibraryResponse, FileVersionResponse
- [ ] FaqQuestionResponse, FaqListResponse
- [ ] ContactResponse, ContactGroupResponse
- [ ] ReportResponse (exists)

#### Validation
- [ ] FluentValidation rules for all request DTOs
- [ ] File upload validation (size, type, virus scanning hooks)
- [ ] Business rule validation (permissions, status transitions)

---

### Phase 3: Service Layer ⏳ PENDING

#### Services (Needed: 8)
- [ ] **IMessagingService** / **MessagingService**
  - SendMessage, ReplyToMessage, GetMessages, GetThread
  - CancelMessage, MarkAsRead, DeleteDraft
  - GetUnreadCount, GetMessagesByFolder

- [ ] **ICaseManagementService** / **CaseManagementService**
  - CreateCase, UpdateCase, AssignHandler
  - AddDocument, AddMessage, UpdateStatus
  - GetCaseHistory, CancelCase, ResolveCase

- [ ] **IAnnouncementService** / **AnnouncementService**
  - CreateAnnouncement, UpdateAnnouncement
  - PublishAnnouncement, UnpublishAnnouncement
  - ConfirmRead, GetReadStats, GetAnnouncements

- [ ] **IFileLibraryService** / **FileLibraryService**
  - UploadFile, UpdateFileMetadata, DeleteFile
  - GetFile, DownloadFile, GetVersionHistory
  - ManagePermissions, CheckAccess

- [ ] **IFaqService** / **FaqService**
  - SubmitQuestion, AnswerQuestion
  - PublishQuestion, UnpublishQuestion
  - RateAnswer, GetQuestions, SearchQuestions

- [ ] **IContactService** / **ContactService**
  - CreateContact, UpdateContact, DeleteContact
  - CreateGroup, AddToGroup, RemoveFromGroup
  - GetContacts, GetGroups, BulkImport

- [ ] **IReportService** / **ReportService** (partial - enhance existing)
  - SubmitReport, ValidateReport, GetReport
  - RejectReport, CorrectReport, ArchiveReport
  - GetMissingReports

- [ ] **IEntityDataService** / **EntityDataService**
  - GetEditableFields, ConfirmData
  - RequestChange, VerifyChange, ApproveChange

---

### Phase 4: API Controllers ⏳ PENDING

#### Controllers (Needed: 8)
- [ ] **MessagesController** (`/api/v1/messages`)
- [ ] **CasesController** (`/api/v1/cases`)
- [ ] **AnnouncementsController** (`/api/v1/announcements`)
- [ ] **FileLibraryController** (`/api/v1/library/files`)
- [ ] **FaqController** (`/api/v1/faq`)
- [ ] **ContactsController** (`/api/v1/contacts`, `/api/v1/contact-groups`)
- [ ] **ReportsController** (exists - enhance)
- [ ] **PodmiotyController** (`/api/v1/podmioty` - data updater)

---

## 📡 API Endpoints Specification

### Phase 4A: Messages API (11 endpoints) ⏳

| Method | Endpoint | Status | Description |
|--------|----------|--------|-------------|
| GET | `/api/v1/messages` | ⏳ | List messages with filters |
| GET | `/api/v1/messages/{id}` | ⏳ | Get message details + thread |
| POST | `/api/v1/messages` | ⏳ | Send new message |
| POST | `/api/v1/messages/{id}/reply` | ⏳ | Reply to message |
| POST | `/api/v1/messages/{id}/cancel` | ⏳ | Cancel message (UKNF only) |
| DELETE | `/api/v1/messages/{id}` | ⏳ | Delete draft |
| POST | `/api/v1/messages/{id}/mark-read` | ⏳ | Mark as read |
| GET | `/api/v1/messages/unread-count` | ⏳ | Get unread counts |
| GET | `/api/v1/messages/threads/{threadId}` | ⏳ | Get full thread |
| POST | `/api/v1/messages/bulk-send` | ⏳ | Send to multiple recipients |
| GET | `/api/v1/messages/export` | ⏳ | Export messages (CSV) |

### Phase 4B: Cases API (12 endpoints) ⏳

| Method | Endpoint | Status | Description |
|--------|----------|--------|-------------|
| GET | `/api/v1/cases` | ⏳ | List cases with filters |
| GET | `/api/v1/cases/{id}` | ⏳ | Get case details |
| POST | `/api/v1/cases` | ⏳ | Create new case |
| PUT | `/api/v1/cases/{id}` | ⏳ | Update case |
| DELETE | `/api/v1/cases/{id}` | ⏳ | Delete case (drafts only) |
| POST | `/api/v1/cases/{id}/cancel` | ⏳ | Cancel case |
| POST | `/api/v1/cases/{id}/resolve` | ⏳ | Mark case as resolved |
| POST | `/api/v1/cases/{id}/close` | ⏳ | Close case |
| POST | `/api/v1/cases/{id}/messages` | ⏳ | Add message to case |
| POST | `/api/v1/cases/{id}/documents` | ⏳ | Upload document |
| GET | `/api/v1/cases/{id}/history` | ⏳ | Get change history |
| POST | `/api/v1/cases/{id}/assign` | ⏳ | Assign handler |

### Phase 4C: Announcements API (11 endpoints) ⏳

| Method | Endpoint | Status | Description |
|--------|----------|--------|-------------|
| GET | `/api/v1/announcements` | ⏳ | List announcements |
| GET | `/api/v1/announcements/{id}` | ⏳ | Get announcement details |
| POST | `/api/v1/announcements` | ⏳ | Create announcement |
| PUT | `/api/v1/announcements/{id}` | ⏳ | Update announcement |
| DELETE | `/api/v1/announcements/{id}` | ⏳ | Delete announcement |
| POST | `/api/v1/announcements/{id}/publish` | ⏳ | Publish announcement |
| POST | `/api/v1/announcements/{id}/unpublish` | ⏳ | Unpublish announcement |
| POST | `/api/v1/announcements/{id}/confirm-read` | ⏳ | Confirm reading |
| GET | `/api/v1/announcements/{id}/read-stats` | ⏳ | Get read statistics |
| GET | `/api/v1/announcements/{id}/history` | ⏳ | Get change history |
| POST | `/api/v1/announcements/bulk-notify` | ⏳ | Send notifications |

### Phase 4D: File Library API (10 endpoints) ⏳

| Method | Endpoint | Status | Description |
|--------|----------|--------|-------------|
| GET | `/api/v1/library/files` | ⏳ | List files with filters |
| GET | `/api/v1/library/files/{id}` | ⏳ | Get file metadata |
| POST | `/api/v1/library/files` | ⏳ | Upload file |
| PUT | `/api/v1/library/files/{id}` | ⏳ | Update metadata |
| DELETE | `/api/v1/library/files/{id}` | ⏳ | Delete file |
| GET | `/api/v1/library/files/{id}/download` | ⏳ | Download file |
| GET | `/api/v1/library/files/{id}/versions` | ⏳ | Get version history |
| POST | `/api/v1/library/files/{id}/new-version` | ⏳ | Upload new version |
| POST | `/api/v1/library/files/{id}/permissions` | ⏳ | Manage permissions |
| GET | `/api/v1/library/categories` | ⏳ | List file categories |

### Phase 4E: FAQ API (10 endpoints) ⏳

| Method | Endpoint | Status | Description |
|--------|----------|--------|-------------|
| GET | `/api/v1/faq/questions` | ⏳ | List questions with filters |
| GET | `/api/v1/faq/questions/{id}` | ⏳ | Get question + answer |
| POST | `/api/v1/faq/questions` | ⏳ | Submit question |
| PUT | `/api/v1/faq/questions/{id}` | ⏳ | Update question (UKNF) |
| DELETE | `/api/v1/faq/questions/{id}` | ⏳ | Delete question (UKNF) |
| POST | `/api/v1/faq/questions/{id}/answer` | ⏳ | Add/update answer (UKNF) |
| POST | `/api/v1/faq/questions/{id}/publish` | ⏳ | Publish Q&A |
| POST | `/api/v1/faq/questions/{id}/unpublish` | ⏳ | Unpublish Q&A |
| POST | `/api/v1/faq/questions/{id}/rate` | ⏳ | Rate answer (1-5 stars) |
| GET | `/api/v1/faq/analytics` | ⏳ | Get FAQ analytics |

### Phase 4F: Contacts API (12 endpoints) ⏳

| Method | Endpoint | Status | Description |
|--------|----------|--------|-------------|
| GET | `/api/v1/contacts` | ⏳ | List contacts |
| GET | `/api/v1/contacts/{id}` | ⏳ | Get contact details |
| POST | `/api/v1/contacts` | ⏳ | Create contact |
| PUT | `/api/v1/contacts/{id}` | ⏳ | Update contact |
| DELETE | `/api/v1/contacts/{id}` | ⏳ | Delete contact |
| POST | `/api/v1/contacts/bulk-import` | ⏳ | Bulk import (CSV) |
| GET | `/api/v1/contact-groups` | ⏳ | List contact groups |
| GET | `/api/v1/contact-groups/{id}` | ⏳ | Get group + members |
| POST | `/api/v1/contact-groups` | ⏳ | Create group |
| PUT | `/api/v1/contact-groups/{id}` | ⏳ | Update group |
| DELETE | `/api/v1/contact-groups/{id}` | ⏳ | Delete group |
| POST | `/api/v1/contact-groups/{id}/members` | ⏳ | Add/remove members |

### Phase 4G: Reports API (8 endpoints - Enhanced) ✅/⏳

| Method | Endpoint | Status | Description |
|--------|----------|--------|-------------|
| GET | `/api/v1/reports` | ✅ | List reports (implemented) |
| GET | `/api/v1/reports/{id}` | ✅ | Get report details |
| POST | `/api/v1/reports` | ✅ | Upload report |
| PUT | `/api/v1/reports/{id}/status` | ✅ | Update status |
| POST | `/api/v1/reports/{id}/validate` | ⏳ | Trigger validation |
| GET | `/api/v1/reports/{id}/validation-report` | ⏳ | Download validation |
| POST | `/api/v1/reports/{id}/correct` | ⏳ | Submit correction |
| GET | `/api/v1/reports/missing` | ⏳ | List missing reports |

### Phase 4H: Podmioty API (6 endpoints - Data Updater) ⏳

| Method | Endpoint | Status | Description |
|--------|----------|--------|-------------|
| GET | `/api/v1/podmioty` | ⏳ | List podmioty |
| GET | `/api/v1/podmioty/{id}` | ⏳ | Get podmiot details |
| GET | `/api/v1/podmioty/{id}/editable-fields` | ⏳ | Get editable fields |
| POST | `/api/v1/podmioty/{id}/confirm-data` | ⏳ | Confirm data accuracy |
| POST | `/api/v1/podmioty/{id}/request-change` | ⏳ | Request data change |
| POST | `/api/v1/podmioty/{id}/verify-change` | ⏳ | Verify change (UKNF) |

**Total API Endpoints: 80** (3 implemented, 77 pending)

---

## 🔐 Authorization Matrix

### Role-Based Access
| Endpoint Category | SystemAdmin | UKNFEmployee | EntityAdmin | EntityEmployee |
|-------------------|-------------|--------------|-------------|----------------|
| Messages (Send) | ✅ | ✅ | ✅ | ✅ |
| Messages (Cancel) | ✅ | ✅ | ❌ | ❌ |
| Cases (Create) | ✅ | ✅ | ✅ | ✅ |
| Cases (Assign) | ✅ | ✅ | ❌ | ❌ |
| Announcements (Create) | ✅ | ✅ | ❌ | ❌ |
| Announcements (Read) | ✅ | ✅ | ✅ | ✅ |
| Library (Upload) | ✅ | ✅ | ❌ | ❌ |
| Library (Download) | Permission-based | Permission-based | Permission-based | Permission-based |
| FAQ (Submit) | ✅ | ✅ | ✅ | ✅ (+ anonymous) |
| FAQ (Answer) | ✅ | ✅ | ❌ | ❌ |
| Contacts (Manage) | ✅ | ✅ | ❌ | ❌ |
| Reports (Submit) | ✅ | ✅ | ✅ | ✅ |
| Reports (Validate) | ✅ | ✅ | ❌ | ❌ |
| Podmioty (Request Change) | ✅ | ✅ | ✅ | ✅ |
| Podmioty (Approve Change) | ✅ | ✅ | ❌ | ❌ |

---

## 📊 Implementation Phases

### ✅ Phase 1: Data Layer (COMPLETE)
- **Duration:** 1 day
- **Status:** ✅ 100% Complete
- **Deliverables:**
  - ✅ 5 enums created
  - ✅ 17 entities created
  - ✅ Database configuration updated
  - ✅ Migration created
  - ✅ Build successful

### ⏳ Phase 2: DTOs & Validation (PENDING)
- **Estimated Duration:** 2 days
- **Status:** 0% Complete
- **Deliverables:**
  - [ ] ~40 Request DTOs
  - [ ] ~40 Response DTOs
  - [ ] FluentValidation rules
  - [ ] Pagination helpers

### ⏳ Phase 3: Service Layer (PENDING)
- **Estimated Duration:** 4 days
- **Status:** 0% Complete
- **Deliverables:**
  - [ ] 8 service interfaces
  - [ ] 8 service implementations
  - [ ] Business logic validation
  - [ ] Unit tests

### ⏳ Phase 4: API Controllers (PENDING)
- **Estimated Duration:** 5 days
- **Status:** ~4% Complete (3/80 endpoints)
- **Deliverables:**
  - [ ] 8 controllers
  - [ ] 80 endpoints total
  - [ ] Swagger documentation
  - [ ] Integration tests

### ⏳ Phase 5: Advanced Features (PENDING)
- **Estimated Duration:** 3 days
- **Status:** 0% Complete
- **Deliverables:**
  - [ ] File virus scanning integration
  - [ ] SPAM detection
  - [ ] Email notifications
  - [ ] Background job processing
  - [ ] Report validation service integration

---

## 🧪 Testing Requirements

### Unit Tests
- [ ] Entity validation tests
- [ ] Service layer business logic tests
- [ ] DTO mapping tests
- [ ] Permission check tests

### Integration Tests
- [ ] API endpoint tests (all 80)
- [ ] Database transaction tests
- [ ] File upload/download tests
- [ ] Authentication/authorization tests

### Performance Tests
- [ ] Message threading with 1000+ messages
- [ ] File library with 10000+ files
- [ ] Announcement targeting 5000+ users
- [ ] Report list with pagination

---

## 📈 Success Criteria

### Phase 1 (Data Layer) ✅
- [x] All entities created with proper relationships
- [x] Database migration generated successfully
- [x] Build completes without errors
- [x] EF Core configuration complete

### Phase 2 (DTOs) ⏳
- [ ] All request/response DTOs created
- [ ] Validation rules implemented
- [ ] AutoMapper profiles configured
- [ ] Unit tests for validation

### Phase 3 (Services) ⏳
- [ ] All service interfaces and implementations
- [ ] Business rules enforced
- [ ] 80%+ unit test coverage
- [ ] Error handling standardized

### Phase 4 (APIs) ⏳
- [ ] All 80 endpoints functional
- [ ] Swagger documentation complete
- [ ] Integration tests passing
- [ ] Authorization working correctly

### Phase 5 (Advanced) ⏳
- [ ] File scanning operational
- [ ] Notifications working
- [ ] Background jobs processing
- [ ] Performance benchmarks met

---

## 🚀 Deployment Checklist

- [ ] Apply database migration
- [ ] Seed test data
- [ ] Configure file storage paths
- [ ] Set up background job queue
- [ ] Configure email SMTP settings
- [ ] Set up virus scanning service
- [ ] Configure rate limiting
- [ ] Enable audit logging
- [ ] Test all integrations
- [ ] Document API usage

---

## 📝 Documentation Requirements

- [x] COMMUNICATION_MODULE_STATUS.md created
- [x] COMMUNICATION_MODULE_REQUIREMENTS.md created
- [ ] API usage guide
- [ ] User manual (UKNF staff)
- [ ] User manual (Entity users)
- [ ] Troubleshooting guide
- [ ] Admin guide
- [ ] Integration guide (external systems)

---

**Document Version:** 1.0
**Last Updated:** 2025-10-04
**Author:** Development Team
**Status:** Living Document
