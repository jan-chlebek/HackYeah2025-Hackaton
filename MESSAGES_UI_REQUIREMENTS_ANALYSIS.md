# Messages Backend - UI Requirements Analysis

**Date**: October 5, 2025
**Branch**: krzys
**Feature**: Messages System Backend
**Sources**:
- `.requirements/UI_SCREENS_SUMMARY.md` (Screens 05, 06, 07)
- `.requirements/DETAILS_UKNF_Prompt2Code2.md` (Polish + English sections)

---

## 📋 Executive Summary

The Messages system is a **two-way communication channel** similar to email between UKNF employees and supervised entity users. It supports:
- **Threaded conversations** with reply/forward capabilities
- **Attachments** (files uploaded with messages)
- **Advanced filtering** (date, sender, type, priority, read status)
- **Read tracking** with timestamps
- **Priority levels** (High, Normal, Low)
- **Message types** (System, Notifications, Entity correspondence, Internal)
- **Star/favorite** marking
- **Archive** functionality

---

## 🎨 UI Screens Analysis

### Screen 05: Wiadomości (Messages List)

**Layout**: Split view (40% list / 60% preview)

#### Left Panel - Message List

**Filter Tabs**:
```
- Wszystkie (All)
- Nieprzeczytane (Unread) - with badge count
- Oznaczone gwiazdką (Starred)
- Wysłane (Sent)
- Archiwum (Archive)
```

**Each Message List Item Shows**:
- ☑️ Checkbox (for bulk selection)
- ⭐ Star icon (toggle favorite)
- **Sender name** (bold if unread)
- **Subject line** (bold if unread)
- **Preview text** (first ~100 chars of body)
- **Timestamp** (relative or absolute)
- 📎 Attachment icon (if has attachments)
- 🔵 Unread indicator (blue dot or bold)

**Visual States**:
- Unread: bold text, blue accent
- Read: normal weight
- Selected: highlighted background
- Hover: subtle background change

**Pagination**: Bottom of list with Previous/Next/Page numbers

#### Right Panel - Message Preview

**Message Header**:
- **From**: Sender name + email
- **To**: Recipient(s) - expandable if many
- **Subject**: Full subject
- **Date/Time**: Full timestamp

**Action Buttons**:
- Odpowiedz (Reply)
- Odpowiedz wszystkim (Reply all)
- Przekaż (Forward)
- Usuń (Delete)
- Więcej (More) - dropdown:
  - Oznacz jako nieprzeczytane
  - Dodaj gwiazdkę
  - Przenieś do archiwum
  - Drukuj

**Message Body**:
- Full formatted text
- Embedded images support
- Proper spacing

**Attachments Section**:
- File icon, name, size
- Download button per file
- "Pobierz wszystkie jako ZIP" option

**Quick Reply Box**: Collapsed, expands on click

---

### Screen 06: Wiadomości filtrowanie (Messages with Filters)

**Layout**: Three columns (20% filters / 30% list / 50% preview)

#### Advanced Filter Panel

**1. Status**
```
☐ Nieprzeczytane (Unread)
☐ Przeczytane (Read)
☐ Oznaczone gwiazdką (Starred)
```

**2. Data (Date)**
```
○ Ostatnie 7 dni (Last 7 days)
○ Ostatnie 30 dni (Last 30 days)
○ Zakres dat (Date range)
  📅 Od (From): [date picker]
  📅 Do (To): [date picker]
```

**3. Nadawca (Sender)**
```
[Autocomplete/dropdown]
- Recently used senders
- "Wszyscy" (All) option
```

**4. Typ wiadomości (Message Type)**
```
☐ Wiadomości systemowe (System messages)
☐ Powiadomienia (Notifications)
☐ Wiadomości od podmiotów (Messages from entities)
☐ Korespondencja wewnętrzna (Internal correspondence)
```

**5. Załączniki (Attachments)**
```
☐ Z załącznikami (With attachments)
☐ Bez załączników (Without attachments)
```

**6. Priorytet (Priority)**
```
☐ Wysoki (High)
☐ Normalny (Normal)
☐ Niski (Low)
```

**Filter Actions**:
- [Zastosuj filtry] (Apply filters) - button
- [Wyczyść filtry] (Clear filters) - link

**Active Filters Display**:
- Shown as chips/tags above message list
- Each chip has X to remove
- "Znaleziono X wiadomości" (Found X messages) counter

**Collapsible**: Toggle button to hide/show filter panel

---

### Screen 07: Wiadomości szczegóły (Message Details)

**Layout**: Full-page message thread view

#### Header
- Breadcrumb: Wiadomości > [Subject]
- Back button to list

#### Primary Message

**Sender Info**:
- Avatar/icon
- Sender name (bold, large)
- Sender email
- Organization/role badge
- Timestamp (right aligned)

**Action Bar**:
- Odpowiedz (Reply)
- Odpowiedz wszystkim (Reply all)
- Przekaż (Forward)
- ⭐ Star/favorite toggle
- ⋮ More actions:
  - Oznacz jako nieprzeczytane
  - Drukuj
  - Pobierz jako PDF
  - Zgłoś nadużycie
  - Przenieś do archiwum

**Message Metadata**:
- **Do**: Recipient list (expandable)
- **DW** (CC): CC recipients
- **Temat** (Subject)
- **Data**: Full timestamp with timezone

**Message Body**:
- Full HTML content
- Formatting (bold, italic, lists, links)
- Quoted text collapsed with "Pokaż cytowany tekst"
- Clickable links
- Styled signatures

**Attachments Section**:
- Card/box per attachment
- Large file icons by type
- Filename + size
- Download + Preview buttons
- "Pobierz wszystkie" (ZIP) option
- Virus scan status indicator

#### Thread View (Reply History)

**Thread Display**:
- Previous messages in thread
- Collapsed by default (headers only)
- Click to expand individual messages
- Visual threading (indent/connectors)
- Chronological order

**Each Thread Item**:
- Mini avatar
- Sender name
- Timestamp
- First line preview
- Expand/collapse icon

#### Reply Compose Box

**Quick Reply**:
- Expands on "Odpowiedz" click
- Rich text editor
- Formatting toolbar (bold, italic, lists, links)
- Attachment button
- [Wyślij] (Send) button
- [Anuluj] (Cancel) button

**Full Compose**:
- To/CC/BCC fields
- Subject (editable)
- Full WYSIWYG editor
- Template selector (optional)
- Signature selector
- Send options:
  - Priority
  - Read receipt request
  - Delivery notification

#### Sidebar (Optional)

**Message Metadata**:
- Related case number (if linked)
- Tags/categories
- Folder location
- Message ID

**Related Messages**:
- Same thread messages
- Same subject messages
- Same sender messages

---

## 📊 Backend Data Requirements from UI

### Message Entity Fields

**Core Fields**:
```csharp
public long Id { get; set; }
public string Subject { get; set; }
public string Body { get; set; } // HTML content
public string? BodyPlainText { get; set; } // For search/preview
public DateTime CreatedAt { get; set; }
public DateTime UpdatedAt { get; set; }
```

**Sender/Recipient**:
```csharp
public long SenderId { get; set; }
public User Sender { get; set; }
public List<MessageRecipient> Recipients { get; set; } // To
public List<MessageRecipient> CcRecipients { get; set; } // CC
public List<MessageRecipient> BccRecipients { get; set; } // BCC
```

**Status & Categorization**:
```csharp
public MessageType Type { get; set; } // System, Notification, Entity, Internal
public MessagePriority Priority { get; set; } // High, Normal, Low
public bool IsDraft { get; set; }
public bool IsArchived { get; set; }
public DateTime? ArchivedAt { get; set; }
```

**Threading**:
```csharp
public long? ParentMessageId { get; set; }
public Message? ParentMessage { get; set; }
public long? ThreadRootId { get; set; } // Root message of thread
public Message? ThreadRoot { get; set; }
public List<Message> Replies { get; set; }
```

**Attachments**:
```csharp
public List<MessageAttachment> Attachments { get; set; }
public bool HasAttachments { get; set; }
```

**Read Tracking** (per recipient):
```csharp
// In MessageRecipient entity:
public bool IsRead { get; set; }
public DateTime? ReadAt { get; set; }
public bool IsStarred { get; set; }
```

**Related Entities**:
```csharp
public long? RelatedCaseId { get; set; } // Link to Case/Access Request
public Case? RelatedCase { get; set; }
public long? RelatedEntityId { get; set; } // Link to Supervised Entity
public SupervisedEntity? RelatedEntity { get; set; }
```

**Metadata**:
```csharp
public string? Tags { get; set; } // JSON array
public string? Category { get; set; }
public bool RequireReadReceipt { get; set; }
public bool RequireDeliveryNotification { get; set; }
```

---

### Message Enums

**MessageType**:
```csharp
public enum MessageType
{
    System = 1,           // Wiadomości systemowe
    Notification = 2,     // Powiadomienia
    EntityCorrespondence = 3, // Wiadomości od podmiotów
    Internal = 4          // Korespondencja wewnętrzna
}
```

**MessagePriority**:
```csharp
public enum MessagePriority
{
    Low = 1,     // Niski
    Normal = 2,  // Normalny (default)
    High = 3     // Wysoki
}
```

---

## 🔌 API Endpoints Required by UI

### Currently Implemented (8 endpoints) ✅

```
GET    /api/v1/messages                           - List with pagination
GET    /api/v1/messages/{id}                      - Get details
POST   /api/v1/messages                           - Create with attachments
POST   /api/v1/messages/{id}/read                 - Mark as read
POST   /api/v1/messages/read-multiple             - Bulk mark read
GET    /api/v1/messages/unread-count              - Count unread
GET    /api/v1/messages/stats                     - Statistics
GET    /api/v1/messages/{messageId}/attachments/{attachmentId}/download - Download
```

### Missing Endpoints (6 endpoints) ❌

```
POST   /api/v1/messages/{id}/reply                - Reply to message
POST   /api/v1/messages/{id}/forward              - Forward message
POST   /api/v1/messages/{id}/star                 - Toggle star/favorite
POST   /api/v1/messages/{id}/archive              - Archive message
DELETE /api/v1/messages/{id}                      - Delete draft
GET    /api/v1/messages/{id}/thread               - Get full thread
POST   /api/v1/messages/send-bulk                 - Send to multiple recipients
GET    /api/v1/messages/export                    - Export to CSV/Excel
```

---

## 📥 Request DTOs Required

### CreateMessageRequest ✅ (Already exists)
```csharp
public class CreateMessageRequest
{
    public List<long> RecipientIds { get; set; }
    public List<long>? CcRecipientIds { get; set; }
    public List<long>? BccRecipientIds { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public MessagePriority Priority { get; set; } = MessagePriority.Normal;
    public MessageType Type { get; set; }
    public List<IFormFile>? Attachments { get; set; }
    public long? RelatedCaseId { get; set; }
    public long? RelatedEntityId { get; set; }
    public bool RequireReadReceipt { get; set; }
    public bool IsDraft { get; set; }
}
```

### MessageFilterRequest ✅ (Exists but may need expansion)
```csharp
public class MessageFilterRequest
{
    // Status filters (from Screen 06)
    public bool? IsUnread { get; set; }
    public bool? IsRead { get; set; }
    public bool? IsStarred { get; set; }
    public bool? IsArchived { get; set; }

    // Date filters
    public MessageDateFilter? DateFilter { get; set; } // Last7Days, Last30Days, Custom
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    // Sender filter
    public long? SenderId { get; set; }
    public List<long>? SenderIds { get; set; }

    // Type filters (checkboxes)
    public List<MessageType>? Types { get; set; }

    // Attachment filter
    public bool? HasAttachments { get; set; }

    // Priority filter
    public List<MessagePriority>? Priorities { get; set; }

    // Search
    public string? SearchTerm { get; set; }

    // Related entities
    public long? RelatedCaseId { get; set; }
    public long? RelatedEntityId { get; set; }

    // Folder/tab
    public MessageFolder? Folder { get; set; } // Inbox, Sent, Archive
}

public enum MessageDateFilter
{
    Last7Days = 1,
    Last30Days = 2,
    Custom = 3
}

public enum MessageFolder
{
    All = 0,
    Inbox = 1,
    Unread = 2,
    Starred = 3,
    Sent = 4,
    Archive = 5
}
```

### ReplyMessageRequest ❌ (NEW)
```csharp
public class ReplyMessageRequest
{
    public long ParentMessageId { get; set; }
    public bool ReplyAll { get; set; } // Reply vs Reply All
    public string Body { get; set; }
    public List<IFormFile>? Attachments { get; set; }
    public MessagePriority? Priority { get; set; } // Inherit or override
}
```

### ForwardMessageRequest ❌ (NEW)
```csharp
public class ForwardMessageRequest
{
    public long OriginalMessageId { get; set; }
    public List<long> RecipientIds { get; set; }
    public List<long>? CcRecipientIds { get; set; }
    public string? AdditionalMessage { get; set; } // Comment before forwarded message
    public List<IFormFile>? AdditionalAttachments { get; set; }
}
```

### BulkSendMessageRequest ❌ (NEW)
```csharp
public class BulkSendMessageRequest
{
    public string Subject { get; set; }
    public string Body { get; set; }
    public MessagePriority Priority { get; set; }

    // Recipient selection (at least one required)
    public List<long>? RecipientUserIds { get; set; }
    public List<long>? RecipientEntityIds { get; set; } // All users of these entities
    public List<string>? RecipientEntityTypes { get; set; } // All users of entity types
    public List<long>? RecipientContactGroupIds { get; set; } // Contact groups

    public List<IFormFile>? Attachments { get; set; }
    public bool RequireReadReceipt { get; set; }
}
```

---

## 📤 Response DTOs Required

### MessageListItemResponse ✅ (Exists, may need fields)
```csharp
public class MessageListItemResponse
{
    public long Id { get; set; }
    public string Subject { get; set; }
    public string BodyPreview { get; set; } // First ~100 chars
    public long SenderId { get; set; }
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public string RelativeTimestamp { get; set; } // "2 hours ago"

    // UI-specific flags
    public bool IsRead { get; set; }
    public bool IsStarred { get; set; }
    public bool HasAttachments { get; set; }
    public int AttachmentCount { get; set; }
    public MessagePriority Priority { get; set; }
    public MessageType Type { get; set; }

    // Polish UI labels
    public string PriorityPolish { get; set; }
    public string TypePolish { get; set; }
}
```

### MessageDetailResponse ✅ (Exists, may need fields)
```csharp
public class MessageDetailResponse
{
    public long Id { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; } // Full HTML
    public string BodyPlainText { get; set; } // For accessibility

    // Sender info
    public long SenderId { get; set; }
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string? SenderOrganization { get; set; }
    public string? SenderRole { get; set; }

    // Recipients
    public List<MessageRecipientResponse> ToRecipients { get; set; }
    public List<MessageRecipientResponse> CcRecipients { get; set; }
    // BCC not shown to recipients

    // Metadata
    public MessagePriority Priority { get; set; }
    public MessageType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    // Attachments
    public List<MessageAttachmentResponse> Attachments { get; set; }

    // Threading
    public long? ParentMessageId { get; set; }
    public long? ThreadRootId { get; set; }
    public int ReplyCount { get; set; }

    // Related entities
    public long? RelatedCaseId { get; set; }
    public string? RelatedCaseNumber { get; set; }
    public long? RelatedEntityId { get; set; }
    public string? RelatedEntityName { get; set; }

    // Flags
    public bool IsStarred { get; set; }
    public bool IsArchived { get; set; }
    public bool IsDraft { get; set; }
    public bool RequireReadReceipt { get; set; }

    // Polish labels
    public string PriorityPolish { get; set; }
    public string TypePolish { get; set; }
    public string StatusPolish { get; set; }
}
```

### MessageThreadResponse ❌ (NEW)
```csharp
public class MessageThreadResponse
{
    public long ThreadRootId { get; set; }
    public string ThreadSubject { get; set; }
    public int MessageCount { get; set; }
    public List<MessageThreadItemResponse> Messages { get; set; }
}

public class MessageThreadItemResponse
{
    public long Id { get; set; }
    public long? ParentMessageId { get; set; }
    public string Subject { get; set; }
    public string BodyPreview { get; set; }
    public string SenderName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public bool HasAttachments { get; set; }
    public int Level { get; set; } // Indentation level in thread
}
```

### MessageStatsResponse ✅ (Exists)
```csharp
public class MessageStatsResponse
{
    public int TotalMessages { get; set; }
    public int UnreadMessages { get; set; }
    public int StarredMessages { get; set; }
    public int ArchivedMessages { get; set; }
    public int DraftMessages { get; set; }
    public int SentMessages { get; set; }

    // By priority
    public int HighPriorityUnread { get; set; }

    // Recent activity
    public int MessagesLast7Days { get; set; }
    public int MessagesLast30Days { get; set; }
}
```

### MessageRecipientResponse
```csharp
public class MessageRecipientResponse
{
    public long UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Organization { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
```

---

## 🎯 UI Feature Requirements Summary

### Message List Features (Screen 05)

**Required Backend Support**:
- [x] Pagination
- [x] Unread count badge
- [x] Preview text generation (first 100 chars)
- [x] Relative timestamps ("2 hours ago")
- [x] Attachment indicator
- [ ] Star/favorite toggle
- [ ] Bulk selection operations
- [ ] Archive function
- [x] Read/Unread status

**Filter Tabs**:
- [ ] All messages
- [x] Unread only (IsUnread filter exists)
- [ ] Starred only
- [ ] Sent messages (need SenderId = currentUser)
- [ ] Archive (IsArchived filter)

### Advanced Filtering (Screen 06)

**Required Backend Support**:
- [x] Date range filtering
- [x] Sender filtering
- [ ] Message type filtering (System, Notification, Entity, Internal)
- [ ] Attachment filtering (with/without)
- [ ] Priority filtering (High/Normal/Low)
- [x] Combined filters (AND logic)
- [x] Result count
- [ ] Active filter display (frontend handles)

### Message Details & Threading (Screen 07)

**Required Backend Support**:
- [x] Full message display
- [x] Attachment download
- [ ] Thread navigation (parent/replies)
- [ ] Reply functionality
- [ ] Forward functionality
- [ ] Print/PDF export
- [ ] Archive toggle
- [ ] Star toggle
- [x] Mark as unread
- [ ] Related case linking
- [ ] HTML rendering support
- [ ] Quote collapse (frontend handles)

### Compose & Reply

**Required Backend Support**:
- [x] Create message with attachments
- [x] Save as draft
- [ ] Reply (quote original message)
- [ ] Reply All (include all To + CC)
- [ ] Forward (include attachments)
- [ ] Multiple recipients (To, CC, BCC)
- [ ] Priority setting
- [ ] Read receipt request
- [ ] Rich text / HTML storage

### Bulk Operations

**Required Backend Support**:
- [x] Mark multiple as read
- [ ] Delete multiple drafts
- [ ] Archive multiple
- [ ] Star multiple
- [ ] Move to folder
- [ ] Bulk send to groups

---

## 📝 Polish UI Field Mappings

**Message List** (Screen 05):
```
Wiadomości → Messages
Nowa wiadomość → New Message
Nieprzeczytane → Unread
Oznaczone gwiazdką → Starred
Wysłane → Sent
Archiwum → Archive
Od → From
Temat → Subject
Data → Date
Odpowiedz → Reply
Odpowiedz wszystkim → Reply All
Przekaż → Forward
Usuń → Delete
Więcej → More
```

**Filter Labels** (Screen 06):
```
Filtruj → Filter
Status → Status
Przeczytane → Read
Data → Date
Ostatnie 7 dni → Last 7 days
Ostatnie 30 dni → Last 30 days
Zakres dat → Date range
Od → From
Do → To
Nadawca → Sender
Typ wiadomości → Message Type
Wiadomości systemowe → System messages
Powiadomienia → Notifications
Wiadomości od podmiotów → Messages from entities
Korespondencja wewnętrzna → Internal correspondence
Załączniki → Attachments
Z załącznikami → With attachments
Bez załączników → Without attachments
Priorytet → Priority
Wysoki → High
Normalny → Normal
Niski → Low
Zastosuj filtry → Apply filters
Wyczyść filtry → Clear filters
Znaleziono X wiadomości → Found X messages
```

**Message Details** (Screen 07):
```
Do → To
DW → CC (Polish: Do Wiadomości)
Temat → Subject
Pokaż cytowany tekst → Show quoted text
Pobierz wszystkie → Download all
Wyślij → Send
Anuluj → Cancel
Oznacz jako nieprzeczytane → Mark as unread
Drukuj → Print
Pobierz jako PDF → Download as PDF
Zgłoś nadużycie → Report abuse
Przenieś do archiwum → Move to archive
```

---

## ✅ Implementation Checklist

### Phase 1: Complete Missing Core Features (4-6 hours)

- [ ] **Star/Favorite Toggle**
  - [ ] Add endpoint: `POST /api/v1/messages/{id}/star`
  - [ ] Update MessageRecipient entity with IsStarred field
  - [ ] Update service method: `ToggleStarAsync()`
  - [ ] Add integration test

- [ ] **Archive Functionality**
  - [ ] Add endpoint: `POST /api/v1/messages/{id}/archive`
  - [ ] Update Message entity with IsArchived + ArchivedAt
  - [ ] Update service method: `ArchiveMessageAsync()`
  - [ ] Add integration test

- [ ] **Delete Drafts**
  - [ ] Add endpoint: `DELETE /api/v1/messages/{id}`
  - [ ] Update service method: `DeleteDraftAsync()` (only allow drafts)
  - [ ] Add integration test

### Phase 2: Threading & Reply (6-8 hours)

- [ ] **Reply Functionality**
  - [ ] Add endpoint: `POST /api/v1/messages/{id}/reply`
  - [ ] Create ReplyMessageRequest DTO
  - [ ] Update service method: `ReplyToMessageAsync()`
  - [ ] Auto-quote original message in body
  - [ ] Handle Reply vs Reply All logic
  - [ ] Add integration tests (2-3 tests)

- [ ] **Forward Functionality**
  - [ ] Add endpoint: `POST /api/v1/messages/{id}/forward`
  - [ ] Create ForwardMessageRequest DTO
  - [ ] Update service method: `ForwardMessageAsync()`
  - [ ] Copy attachments to new message
  - [ ] Add integration test

- [ ] **Thread Navigation**
  - [ ] Add endpoint: `GET /api/v1/messages/{id}/thread`
  - [ ] Create MessageThreadResponse DTO
  - [ ] Update service method: `GetMessageThreadAsync()`
  - [ ] Build hierarchical thread structure
  - [ ] Add integration test

### Phase 3: Advanced Filtering (2-3 hours)

- [ ] **Expand MessageFilterRequest**
  - [ ] Add Type filtering (List<MessageType>)
  - [ ] Add Priority filtering (List<MessagePriority>)
  - [ ] Add HasAttachments boolean
  - [ ] Add Folder enum (All, Inbox, Sent, etc.)

- [ ] **Update GetMessagesAsync()**
  - [ ] Apply Type filters
  - [ ] Apply Priority filters
  - [ ] Apply Attachment filters
  - [ ] Apply Folder logic
  - [ ] Add integration tests for each filter

### Phase 4: Bulk Operations (3-4 hours)

- [ ] **Bulk Send**
  - [ ] Add endpoint: `POST /api/v1/messages/send-bulk`
  - [ ] Create BulkSendMessageRequest DTO
  - [ ] Update service method: `SendBulkMessageAsync()`
  - [ ] Support recipient selection by:
    - [ ] User IDs
    - [ ] Entity IDs (all users of entity)
    - [ ] Entity Types (all users of type)
    - [ ] Contact Groups
  - [ ] Add integration test

- [ ] **Bulk Archive**
  - [ ] Add endpoint: `POST /api/v1/messages/archive-multiple`
  - [ ] Update service method: `ArchiveMultipleAsync()`
  - [ ] Add integration test

- [ ] **Bulk Star**
  - [ ] Add endpoint: `POST /api/v1/messages/star-multiple`
  - [ ] Update service method: `StarMultipleAsync()`
  - [ ] Add integration test

- [ ] **Bulk Delete**
  - [ ] Add endpoint: `DELETE /api/v1/messages/delete-multiple`
  - [ ] Update service method: `DeleteMultipleDraftsAsync()`
  - [ ] Add integration test

### Phase 5: Export & Reporting (2-3 hours)

- [ ] **CSV/Excel Export**
  - [ ] Add endpoint: `GET /api/v1/messages/export`
  - [ ] Support query params (format, filters)
  - [ ] Generate CSV with key fields
  - [ ] Generate Excel with formatting (optional)
  - [ ] Add integration test

### Phase 6: Integration Tests (3-4 hours)

Write comprehensive integration tests for all endpoints:

- [ ] **Core CRUD** (already covered?)
  - [ ] GetMessages_WithFilters_ReturnsFiltered
  - [ ] GetMessageById_ReturnsDetails
  - [ ] CreateMessage_CreatesSuccessfully
  - [ ] CreateMessage_WithAttachments_StoresFiles

- [ ] **Read Tracking** (already covered?)
  - [ ] MarkAsRead_UpdatesReadStatus
  - [ ] MarkMultipleAsRead_UpdatesAll
  - [ ] GetUnreadCount_ReturnsCorrectCount

- [ ] **Star/Archive** (NEW)
  - [ ] ToggleStar_MarksAsFavorite
  - [ ] ToggleStar_UnmarksWhenAlreadyStarred
  - [ ] ArchiveMessage_MovesToArchive
  - [ ] GetMessages_WithArchivedFilter_ReturnsOnlyArchived

- [ ] **Threading** (NEW)
  - [ ] ReplyToMessage_CreatesChildMessage
  - [ ] ReplyAll_IncludesAllRecipients
  - [ ] ForwardMessage_CopiesContent
  - [ ] GetThread_ReturnsFullConversation

- [ ] **Filtering** (NEW)
  - [ ] GetMessages_ByType_ReturnsOnlyType
  - [ ] GetMessages_ByPriority_ReturnsOnlyPriority
  - [ ] GetMessages_WithAttachments_ReturnsOnlyWithFiles
  - [ ] GetMessages_CombinedFilters_AppliesAll

- [ ] **Bulk Operations** (NEW)
  - [ ] SendBulk_ToMultipleUsers_CreatesAll
  - [ ] ArchiveMultiple_ArchivesAll
  - [ ] StarMultiple_StarsAll
  - [ ] DeleteMultiple_DeletesOnlyDrafts

---

## ⏱️ Time Estimates

| Phase | Description | Hours |
|-------|-------------|-------|
| 1 | Complete Missing Core (Star, Archive, Delete) | 4-6 |
| 2 | Threading & Reply | 6-8 |
| 3 | Advanced Filtering | 2-3 |
| 4 | Bulk Operations | 3-4 |
| 5 | Export & Reporting | 2-3 |
| 6 | Integration Tests | 3-4 |
| **TOTAL** | **Full Messages Backend** | **20-28 hours** |

**MVP** (8-10 hours):
- Phase 1: Star, Archive, Delete (4-6h)
- Phase 2: Reply & Thread (4-6h)
- Skip advanced filtering, bulk ops, export for MVP

---

## 🔍 Current Status vs Requirements

### ✅ Implemented (60% complete)
- Basic message list with pagination
- Message details view
- Create message with attachments
- Mark as read (single + bulk)
- Unread count
- Message stats
- Attachment download
- Basic filtering (date, sender, read status)

### ❌ Missing (40% remaining)
- Star/favorite toggle
- Archive functionality
- Delete drafts
- Reply (with quote)
- Forward (with attachments)
- Thread navigation
- Type filtering (System, Notification, etc.)
- Priority filtering
- Attachment filtering
- Bulk send to groups
- Bulk archive/star/delete
- CSV/Excel export
- Related case linking (entity exists but not used)

---

## 📊 Priority Matrix

**High Priority** (Core UX features):
1. Reply functionality (Screen 07 primary action)
2. Star/favorite (Screen 05 & 06 visible everywhere)
3. Archive (Screen 05 & 06 folder/tab)
4. Thread view (Screen 07 conversation context)

**Medium Priority** (Efficiency features):
5. Forward (Screen 07 action bar)
6. Advanced filtering - Type, Priority (Screen 06 explicit UI)
7. Delete drafts (cleanup functionality)

**Low Priority** (Nice-to-have):
8. Bulk send (admin convenience)
9. Bulk operations (archive/star multiple)
10. Export (reporting, not core UX)

---

**Created**: October 5, 2025
**Status**: Analysis complete, ready for implementation
**Next Steps**: Implement Phase 1 (Star, Archive, Delete) → Phase 2 (Reply, Thread)
