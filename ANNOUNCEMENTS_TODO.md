# Announcements Feature - TODO (POSTPONED)

**Status**: ⏸️ POSTPONED - To be implemented later

## Overview
System for UKNF to publish official announcements to supervised entities with read tracking.

## Simplified Requirements (NO Priority, ExpiryDate, IsActive, PublishedDate, Attachments)

### Backend Implementation

#### 1. Database Entities
- [x] `Announcement` - Title, Content, CreatedByUserId, CreatedAt, UpdatedAt
- [x] `AnnouncementReadStatus` - Track which users read which announcements

#### 2. Database Schema
```sql
- announcements (id, title, content, created_by_user_id, created_at, updated_at)
- announcement_read_statuses (id, announcement_id, user_id, read_at)
```

#### 3. DTOs
- [x] `CreateAnnouncementRequest` - Title, Content
- [x] `UpdateAnnouncementRequest` - Title, Content
- [x] `AnnouncementResponse` - Full details with read status
- [x] `AnnouncementListItemResponse` - List view with preview (200 chars)

#### 4. API Endpoints
```
GET    /api/v1/announcements              - List with pagination & filters
GET    /api/v1/announcements/{id}         - Get details
POST   /api/v1/announcements              - Create (UKNF only)
PUT    /api/v1/announcements/{id}         - Update (UKNF only)
DELETE /api/v1/announcements/{id}         - Delete (UKNF only)
POST   /api/v1/announcements/{id}/read    - Mark as read
```

#### 5. Business Logic
- [x] Authorization: UKNF can CRUD, entities can only read
- [x] Filtering: by read/unread status
- [x] Read status tracking (unique per user per announcement)
- [x] Content preview truncation (200 chars)

#### 6. Tests
- [x] Unit tests for AnnouncementService (18 tests planned)
- [x] Integration tests for AnnouncementsController (8 tests planned)

#### 7. Database Seeding
- [x] Sample announcements with Polish content
- [x] Read status examples (60%, 30%, 20%, 0% read rates)

### Frontend Implementation (NOT STARTED)

#### 1. Angular Components
- [ ] `AnnouncementsListComponent` - Display list with filters
- [ ] `AnnouncementDetailComponent` - Full view
- [ ] `AnnouncementCreateComponent` - UKNF only
- [ ] `AnnouncementEditComponent` - UKNF only

#### 2. Features
- [ ] Read/unread indicators
- [ ] Pagination and filtering
- [ ] Search functionality
- [ ] Responsive design with accessibility (WCAG 2.2)

#### 3. Routing
```
/announcements           - List view
/announcements/:id       - Detail view
/announcements/new       - Create (UKNF only)
/announcements/:id/edit  - Edit (UKNF only)
```

## Migration Status
- [ ] Create entities
- [ ] Add to ApplicationDbContext
- [ ] Generate migration
- [ ] Apply migration
- [ ] Register service in Program.cs
- [ ] Add database seeding
- [ ] Run tests

## Notes
- All announcements visible to all users (no recipient targeting)
- Simple design: just title + content
- Read tracking at user level (not Podmiot level)
- No email notifications (future enhancement)
- No rich text editor (plain text for MVP)

## When to Resume
This feature should be implemented after:
1. ✅ Core messaging system is complete
2. ✅ User authentication/authorization is stable
3. Current priority features are delivered

---
**Created**: October 5, 2025
**Planned Start**: TBD
