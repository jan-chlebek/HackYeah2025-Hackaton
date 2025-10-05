# UKNF Platform - Backend Only Priority Analysis

**Date**: October 5, 2025  
**Branch**: krzys  
**Focus**: Backend API Implementation (excluding frontend)

---

## 🎯 Executive Summary - Backend Only

### Current Backend Status
- ✅ **Infrastructure**: Database, entities, migrations, seeding - **100%**
- ✅ **Testing**: 222/222 tests passing (188 unit + 34 integration)
- ✅ **Admin Module**: 17 API endpoints implemented (33% complete)
- ⚠️ **Communication Module**: Only 3 report endpoints (4% complete)
- 🚨 **Security**: Authorization completely disabled

### Backend Work Remaining
**Total API Endpoints**:
- ✅ Implemented: 20 endpoints (17 admin + 3 reports)
- ⏳ Pending: 81 endpoints
- **Total**: 101 API endpoints needed

---

## 🚨 CRITICAL PRIORITY - Security (IMMEDIATE)

### Re-enable Authorization System
**Current State**: 🚨 **ALL AUTHORIZATION DISABLED**

**Files to Fix**:

1. **Program.cs** (Line ~176)
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// app.UseAuthentication();
// app.UseAuthorization();
```

2. **Controllers with Disabled Auth**:
   - `EntitiesController.cs` - Class level [Authorize]
   - `MessagesController.cs` - Class level [Authorize]
   - `AnnouncementsController.cs` - Class + 3 method level [Authorize]
   - `AuthController.cs` - 5 method level [Authorize]

3. **Hardcoded User IDs to Remove**:
   - `MessagesController.GetCurrentUserId()` - Falls back to user ID 2
   - `AnnouncementsController.GetCurrentUserId()` - Falls back to user ID 2
   - `AuthController` - Multiple hardcoded user ID references

**Tasks**:
- [ ] Uncomment authentication/authorization middleware
- [ ] Uncomment all [Authorize] attributes
- [ ] Remove GetCurrentUserId() fallback methods
- [ ] Use ICurrentUserService properly
- [ ] Update integration tests to authenticate
- [ ] Test all endpoints with JWT tokens
- [ ] Verify role-based access (UKNF vs Entity users)

**Estimated Time**: 6-8 hours
**Impact**: Without this, system is completely insecure

---

## 📊 BACKEND API IMPLEMENTATION STATUS

### ✅ COMPLETED APIs (20 endpoints)

#### Admin - Users Management (10 endpoints) ✅
- ✅ `GET /api/v1/users` - List with pagination
- ✅ `GET /api/v1/users/{id}` - Get details
- ✅ `POST /api/v1/users` - Create user
- ✅ `PUT /api/v1/users/{id}` - Update user
- ✅ `DELETE /api/v1/users/{id}` - Soft delete
- ✅ `POST /api/v1/users/{id}/set-password`
- ✅ `POST /api/v1/users/{id}/reset-password`
- ✅ `POST /api/v1/users/{id}/activate`
- ✅ `POST /api/v1/users/{id}/deactivate`
- ✅ `POST /api/v1/users/{id}/unlock`

#### Admin - Entities Management (7 endpoints) ✅
- ✅ `GET /api/v1/entities` - List with pagination
- ✅ `GET /api/v1/entities/{id}` - Get details
- ✅ `POST /api/v1/entities` - Create entity
- ✅ `PUT /api/v1/entities/{id}` - Update entity
- ✅ `DELETE /api/v1/entities/{id}` - Soft delete
- ✅ `GET /api/v1/entities/{id}/users` - List entity users
- ✅ `GET /api/v1/entities/{id}/reports-count` - (via details endpoint)

#### Communication - Reports (3 endpoints) ✅
- ✅ `GET /api/v1/reports` - List with filters
- ✅ `GET /api/v1/reports/{id}` - Get details
- ✅ `POST /api/v1/reports` - Upload XLSX

---

### 🚧 HIGH PRIORITY - Core Communication APIs (38 endpoints)

These are the "preferowane funkcjonalności" from requirements:

#### 1. Messages System (11 endpoints) - **TOP PRIORITY**
**Status**: Entities exist, service NOT implemented, 0 APIs

**Service Layer Needed**:
- `MessageService.cs` with business logic:
  - Send message with attachments
  - Reply to message (thread handling)
  - Cancel message (UKNF only)
  - Mark as read/unread
  - Get message thread
  - Filter by read status, sender/recipient
  - Bulk send to groups
  - Export to CSV

**API Endpoints**:
- [ ] `GET /api/v1/messages` - List with filters (read/unread, sent/received, by entity)
- [ ] `GET /api/v1/messages/{id}` - Get message details with full thread
- [ ] `POST /api/v1/messages` - Send new message with optional attachments
- [ ] `POST /api/v1/messages/{id}/reply` - Reply to message (creates thread)
- [ ] `POST /api/v1/messages/{id}/cancel` - Cancel message (UKNF only, soft delete)
- [ ] `DELETE /api/v1/messages/{id}` - Delete draft (only drafts, hard delete)
- [ ] `POST /api/v1/messages/{id}/mark-read` - Mark message as read
- [ ] `GET /api/v1/messages/unread-count` - Get count of unread messages
- [ ] `GET /api/v1/messages/threads/{threadId}` - Get all messages in thread
- [ ] `POST /api/v1/messages/bulk-send` - Send to multiple recipients/groups
- [ ] `GET /api/v1/messages/export` - Export messages to CSV

**DTOs Needed**:
- `CreateMessageRequest` (Subject, Body, RecipientId, RelatedEntityId, Attachments)
- `MessageResponse` (full details with sender, recipient, entity, attachments, thread info)
- `MessageListItemResponse` (simplified for list view)
- `MessageThreadResponse` (thread with all messages)
- `BulkSendRequest` (RecipientIds[], GroupIds[], Subject, Body)

**Estimated Time**: 14-18 hours

---

#### 2. Reports System - Complete (5 remaining endpoints)
**Status**: 3 of 8 endpoints done, service partially implemented

**Service Layer Updates Needed**:
- Async validation workflow
- Generate validation report
- Handle corrections
- Detect missing reports by entity/period

**API Endpoints**:
- [x] ✅ `GET /api/v1/reports` - List with filters
- [x] ✅ `GET /api/v1/reports/{id}` - Get details
- [x] ✅ `POST /api/v1/reports` - Upload XLSX
- [x] ✅ `PUT /api/v1/reports/{id}/status` - Update validation status
- [ ] `POST /api/v1/reports/{id}/validate` - Trigger async validation
- [ ] `GET /api/v1/reports/{id}/validation-report` - Download validation PDF/XLSX
- [ ] `POST /api/v1/reports/{id}/correct` - Submit correction (new version)
- [ ] `GET /api/v1/reports/missing` - List missing reports by entity/period

**DTOs Needed**:
- `TriggerValidationRequest`
- `ValidationReportResponse`
- `SubmitCorrectionRequest`
- `MissingReportResponse`

**Estimated Time**: 8-10 hours

---

#### 3. Announcements System (6 endpoints)
**Status**: ✅ All implemented BUT authorization disabled

**Service Layer**: ✅ Complete (`AnnouncementService.cs`)
**API Controller**: ✅ Complete BUT needs auth re-enabled

**API Endpoints** (All exist, need security fix):
- [x] ✅ `GET /api/v1/announcements` - List with pagination & filters
- [x] ✅ `GET /api/v1/announcements/{id}` - Get details with read status
- [x] ✅ `POST /api/v1/announcements` - Create (UKNF only) ⚠️ Auth disabled
- [x] ✅ `PUT /api/v1/announcements/{id}` - Update (UKNF only) ⚠️ Auth disabled
- [x] ✅ `DELETE /api/v1/announcements/{id}` - Delete (UKNF only) ⚠️ Auth disabled
- [x] ✅ `POST /api/v1/announcements/{id}/read` - Mark as read

**Tasks**:
- [ ] Re-enable [Authorize] on class level
- [ ] Re-enable [Authorize(Roles = "UKNF")] on POST/PUT/DELETE
- [ ] Remove GetCurrentUserId() fallback
- [ ] Test with proper JWT tokens

**Estimated Time**: 2-3 hours (just auth fixes)

---

#### 4. File Library (10 endpoints)
**Status**: Entities exist, NO service, NO APIs

**Service Layer Needed**:
- `FileLibraryService.cs` with:
  - File upload with chunking (large files)
  - Permission management (role/podmiot/user based)
  - Search and metadata filtering
  - Bulk download (ZIP creation)
  - Storage abstraction (local/Azure Blob)

**API Endpoints**:
- [ ] `GET /api/v1/files` - List files with filters (by folder, type, permissions)
- [ ] `GET /api/v1/files/{id}` - Get file metadata
- [ ] `POST /api/v1/files` - Upload file (chunked upload support)
- [ ] `PUT /api/v1/files/{id}` - Update metadata
- [ ] `DELETE /api/v1/files/{id}` - Delete file
- [ ] `GET /api/v1/files/{id}/download` - Download file
- [ ] `POST /api/v1/files/{id}/permissions` - Set/update permissions
- [ ] `GET /api/v1/files/search` - Search files by name, tags, metadata
- [ ] `POST /api/v1/files/bulk-download` - Download multiple files as ZIP
- [ ] `GET /api/v1/files/statistics` - Storage usage statistics

**DTOs Needed**:
- `UploadFileRequest` (File, FolderId, Metadata, Permissions)
- `FileResponse` (full metadata with permissions)
- `FileListItemResponse` (simplified for list)
- `SetPermissionsRequest` (PermissionType, RoleNames[], UserIds[], EntityIds[])
- `FileSearchRequest` (Name, Tags, DateRange, FileType)

**Estimated Time**: 16-20 hours

---

#### 5. Cases Management (12 endpoints)
**Status**: Entities exist, NO service, NO APIs

**Service Layer Needed**:
- `CaseService.cs` with:
  - Case lifecycle (draft → open → in progress → resolved → closed)
  - Document attachments
  - Case assignment to UKNF users
  - History tracking
  - Message integration

**API Endpoints**:
- [ ] `GET /api/v1/cases` - List cases with filters (status, entity, assigned to)
- [ ] `GET /api/v1/cases/{id}` - Get case details with documents & history
- [ ] `POST /api/v1/cases` - Create new case
- [ ] `PUT /api/v1/cases/{id}` - Update case details
- [ ] `DELETE /api/v1/cases/{id}` - Delete case (drafts only)
- [ ] `POST /api/v1/cases/{id}/cancel` - Cancel case
- [ ] `POST /api/v1/cases/{id}/resolve` - Mark case as resolved
- [ ] `POST /api/v1/cases/{id}/close` - Close case
- [ ] `POST /api/v1/cases/{id}/messages` - Add message to case
- [ ] `POST /api/v1/cases/{id}/documents` - Upload document to case
- [ ] `GET /api/v1/cases/{id}/history` - Get case change history
- [ ] `POST /api/v1/cases/{id}/assign` - Assign case to UKNF user

**DTOs Needed**:
- `CreateCaseRequest` (Title, Description, EntityId, Priority)
- `UpdateCaseRequest`
- `CaseResponse` (full details with documents, messages, history)
- `CaseListItemResponse`
- `AssignCaseRequest` (AssignedToUserId)
- `CaseHistoryResponse`

**Estimated Time**: 18-22 hours

---

### 🎯 MEDIUM PRIORITY - Supporting APIs (26 endpoints)

#### 6. Contacts Management (5 endpoints)
**Status**: Entities exist, seeding done, NO service, NO APIs

**Service Layer Needed**:
- `ContactService.cs` for CRUD operations

**API Endpoints**:
- [ ] `GET /api/v1/contacts` - List contacts with filters
- [ ] `GET /api/v1/contacts/{id}` - Get contact details
- [ ] `POST /api/v1/contacts` - Create contact
- [ ] `PUT /api/v1/contacts/{id}` - Update contact
- [ ] `DELETE /api/v1/contacts/{id}` - Delete contact

**Estimated Time**: 4-6 hours

---

#### 7. Contact Groups (3 endpoints)
**Status**: Entities exist, seeding done, NO service, NO APIs

**Service Layer Needed**:
- `ContactGroupService.cs` for group management

**API Endpoints**:
- [ ] `GET /api/v1/contact-groups` - List groups
- [ ] `POST /api/v1/contact-groups` - Create group
- [ ] `POST /api/v1/contact-groups/{id}/members` - Add/remove members

**Estimated Time**: 3-4 hours

---

#### 8. FAQ System (6 endpoints)
**Status**: Entities exist, seeding done, basic API exists, needs expansion

**Service Layer Updates Needed**:
- Add rating functionality
- Search and filtering
- Category management

**API Endpoints**:
- [x] ✅ `GET /api/v1/faq` - List questions (basic exists)
- [ ] `GET /api/v1/faq/{id}` - Get question details
- [ ] `POST /api/v1/faq` - Create question (UKNF only)
- [ ] `PUT /api/v1/faq/{id}` - Update question (UKNF only)
- [ ] `DELETE /api/v1/faq/{id}` - Delete question (UKNF only)
- [ ] `POST /api/v1/faq/{id}/rate` - Rate answer (helpful/not helpful)
- [ ] `GET /api/v1/faq/search` - Search questions

**Estimated Time**: 6-8 hours

---

#### 9. Admin - Roles Management (8 endpoints)
**Status**: Entities exist, NO service, NO APIs

**Service Layer Needed**:
- `RoleManagementService.cs` with:
  - CRUD for roles
  - Permission assignment
  - User assignment

**API Endpoints**:
- [ ] `GET /api/v1/roles` - List all roles
- [ ] `GET /api/v1/roles/{id}` - Get role with permissions
- [ ] `POST /api/v1/roles` - Create custom role (admin only)
- [ ] `PUT /api/v1/roles/{id}` - Update role (custom only)
- [ ] `DELETE /api/v1/roles/{id}` - Delete role (custom, not system)
- [ ] `POST /api/v1/roles/{id}/assign` - Assign role to users
- [ ] `POST /api/v1/roles/{id}/revoke` - Revoke role from users
- [ ] `GET /api/v1/roles/{id}/users` - List users with this role

**Estimated Time**: 8-10 hours

---

#### 10. Admin - Password Policy (3 endpoints)
**Status**: Entity exists, NO service, NO APIs

**Service Layer Needed**:
- `PasswordPolicyService.cs` for policy management

**API Endpoints**:
- [ ] `GET /api/v1/password-policy` - Get current policy
- [ ] `PUT /api/v1/password-policy` - Update policy (admin only)
- [ ] `POST /api/v1/password-policy/force-change` - Force password change for users

**Estimated Time**: 3-4 hours

---

#### 11. Admin - Audit Logs (3 endpoints)
**Status**: Entity exists, NO service, NO APIs

**Service Layer Needed**:
- `AuditService.cs` for logging and querying

**API Endpoints**:
- [ ] `GET /api/v1/audit` - List audit logs with filters
- [ ] `GET /api/v1/audit/{id}` - Get audit log entry
- [ ] `GET /api/v1/audit/export` - Export logs to CSV

**Estimated Time**: 4-6 hours

---

### 🆕 ADDITIONAL FEATURES (17 endpoints)

#### 12. Authentication Enhancements (5 endpoints)
**Status**: Basic auth exists, needs external user features

**Service Layer Updates Needed**:
- External user registration workflow
- Access request management
- Multi-podmiot session selection

**API Endpoints**:
- [x] ✅ `POST /api/v1/auth/login` - Login (exists)
- [x] ✅ `POST /api/v1/auth/refresh` - Refresh token (exists)
- [x] ✅ `POST /api/v1/auth/logout` - Logout (exists)
- [ ] `POST /api/v1/auth/register` - External user registration
- [ ] `GET /api/v1/auth/access-requests` - List access requests (UKNF admin)
- [ ] `POST /api/v1/auth/access-requests/{id}/approve` - Approve access
- [ ] `POST /api/v1/auth/access-requests/{id}/reject` - Reject access
- [ ] `POST /api/v1/auth/select-entity` - Select active entity for session

**Estimated Time**: 10-12 hours

---

#### 13. Entity CSV Import (1 endpoint)
**Status**: Planned for Admin Sprint 3

**API Endpoint**:
- [ ] `POST /api/v1/entities/import` - CSV import with validation

**Estimated Time**: 4-6 hours

---

#### 14. Advanced Announcement Features (5 endpoints)
**Status**: Basic announcements done, missing advanced features

**API Endpoints**:
- [ ] `POST /api/v1/announcements/{id}/publish` - Publish announcement
- [ ] `POST /api/v1/announcements/{id}/unpublish` - Unpublish
- [ ] `GET /api/v1/announcements/{id}/read-stats` - Read statistics
- [ ] `GET /api/v1/announcements/{id}/history` - Change history
- [ ] `POST /api/v1/announcements/bulk-notify` - Send email notifications

**Estimated Time**: 6-8 hours

---

## 📊 BACKEND WORK BREAKDOWN

### Summary by Priority

| Priority | Category | Endpoints | Estimated Hours |
|----------|----------|-----------|----------------|
| 🚨 CRITICAL | Security Fix | N/A | 6-8h |
| ⭐ HIGH | Messages | 11 | 14-18h |
| ⭐ HIGH | Reports (complete) | 5 | 8-10h |
| ⭐ HIGH | Announcements (auth fix) | 0 new | 2-3h |
| ⭐ HIGH | File Library | 10 | 16-20h |
| ⭐ HIGH | Cases | 12 | 18-22h |
| 🎯 MEDIUM | Contacts | 5 | 4-6h |
| 🎯 MEDIUM | Contact Groups | 3 | 3-4h |
| 🎯 MEDIUM | FAQ (complete) | 6 | 6-8h |
| 🎯 MEDIUM | Roles Management | 8 | 8-10h |
| 🎯 MEDIUM | Password Policy | 3 | 3-4h |
| 🎯 MEDIUM | Audit Logs | 3 | 4-6h |
| 🆕 ADDITIONAL | Auth Enhancements | 5 | 10-12h |
| 🆕 ADDITIONAL | Entity Import | 1 | 4-6h |
| 🆕 ADDITIONAL | Advanced Announcements | 5 | 6-8h |
| **TOTAL** | **All Backend** | **81** | **113-145h** |

---

## 🚀 RECOMMENDED BACKEND IMPLEMENTATION PLAN

### Week 1: Security + Messages (24-29 hours)
**Day 1-2**: Security Foundation
- [ ] Re-enable authorization (6-8h)
- [ ] Test all existing endpoints with auth

**Day 3-5**: Messages System
- [ ] MessageService implementation (14-18h)
- [ ] 11 API endpoints
- [ ] Integration tests
- [ ] Attachment handling

### Week 2: Reports + Announcements + File Library (26-33 hours)
**Day 1-2**: Complete Reports
- [ ] Finish reports service (8-10h)
- [ ] 5 remaining endpoints
- [ ] Validation workflow

**Day 3**: Announcements
- [ ] Fix authorization (2-3h)

**Day 4-5**: File Library
- [ ] FileLibraryService (16-20h)
- [ ] 10 API endpoints
- [ ] Chunked upload

### Week 3: Cases + Contacts + FAQ (31-40 hours)
**Day 1-3**: Cases Management
- [ ] CaseService (18-22h)
- [ ] 12 API endpoints
- [ ] Integration tests

**Day 4**: Contacts
- [ ] ContactService + API (4-6h)
- [ ] ContactGroupService + API (3-4h)

**Day 5**: FAQ
- [ ] Complete FAQ system (6-8h)

### Week 4: Admin Features (32-40 hours)
**Day 1-2**: Roles & Permissions
- [ ] RoleManagementService (8-10h)
- [ ] 8 API endpoints

**Day 3**: Password Policy & Audit
- [ ] PasswordPolicyService (3-4h)
- [ ] AuditService (4-6h)

**Day 4-5**: Additional Features
- [ ] Auth enhancements (10-12h)
- [ ] Entity import (4-6h)
- [ ] Advanced announcements (6-8h)

---

## ✅ BACKEND-ONLY COMPLETION CHECKLIST

### Phase 1: Security (CRITICAL)
- [ ] Re-enable authentication middleware
- [ ] Re-enable all [Authorize] attributes
- [ ] Remove hardcoded user ID fallbacks
- [ ] Update integration tests
- [ ] Test all endpoints with JWT

### Phase 2: Core Communication APIs
- [ ] Messages System (11 endpoints + service)
- [ ] Reports completion (5 endpoints)
- [ ] Announcements auth fix
- [ ] File Library (10 endpoints + service)
- [ ] Cases Management (12 endpoints + service)

### Phase 3: Supporting Features
- [ ] Contacts (5 endpoints + service)
- [ ] Contact Groups (3 endpoints + service)
- [ ] FAQ completion (6 endpoints)

### Phase 4: Admin Completion
- [ ] Roles Management (8 endpoints + service)
- [ ] Password Policy (3 endpoints + service)
- [ ] Audit Logs (3 endpoints + service)

### Phase 5: Enhancements
- [ ] Auth enhancements (5 endpoints)
- [ ] Entity CSV import (1 endpoint)
- [ ] Advanced announcements (5 endpoints)

---

## 📈 TESTING REQUIREMENTS

For each new service/controller, add:
- **Unit Tests**: Test service business logic (aim for 80%+ coverage)
- **Integration Tests**: Test API endpoints end-to-end
- **Current Status**: 222/222 tests passing ✅

**Estimated Testing Time**: Add 30-40% to development time
- Total Backend Dev: 113-145h
- Testing Time: 34-58h
- **Total with Tests**: 147-203h (4-5 weeks)

---

## 💡 KEY BACKEND INSIGHTS

1. **Foundation is Solid**: Database, entities, migrations all complete
2. **Security is Urgent**: Must re-enable before any production use
3. **Messages is Critical**: Top priority feature, currently 0% API done
4. **Good Test Coverage**: 222 tests give confidence for changes
5. **Entities Ready**: All database entities exist, just need services/APIs

---

## 🎯 IMMEDIATE NEXT STEPS (Backend Only)

**TODAY** (6-8 hours):
1. Re-enable authorization system
2. Test existing 20 endpoints with proper JWT auth

**THIS WEEK** (24-29 hours):
1. Implement MessageService
2. Create 11 message endpoints
3. Add integration tests
4. Test attachment handling

**NEXT WEEK** (26-33 hours):
1. Complete Reports (5 endpoints)
2. Fix Announcements auth (2-3h)
3. Implement File Library (16-20h)

---

**Total Backend Work (API only)**: 113-145 hours (3-4 weeks)
**With Testing**: 147-203 hours (4-5 weeks)
**Current Backend Progress**: 20 of 101 endpoints (20%)
