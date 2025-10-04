# Message Attachments API Implementation

## Overview

This document describes the implementation of message attachment functionality for the UKNF Communication Platform. The implementation allows users to:
1. **Upload multiple files when creating a message** (0 or more attachments allowed)
2. **Download individual attachments** from messages they have access to

## Key Design Principles

### Atomic Message-Attachment Relationship
- **Attachments cannot exist without a message** - they are created in the same transaction as the parent message
- **Each attachment belongs to exactly one message** - enforced by foreign key `MessageId` in `MessageAttachment` entity
- **Cascade behavior** - when a message is deleted, its attachments are also removed (configured in database schema)

### Security
- Users can only download attachments from messages they are participants in (sender or recipient)
- Access control is verified on every download request
- File size limits: 50MB per file, 100MB total request size

## Changes Made

### 1. CreateMessageRequest DTO
**File**: `backend/UknfCommunicationPlatform.Core/DTOs/Messages/CreateMessageRequest.cs`

**Changes**:
- Added `Microsoft.AspNetCore.Http` using directive for `IFormFile`
- Added `Attachments` property:
  ```csharp
  /// <summary>
  /// Optional file attachments (multiple files allowed, can be empty)
  /// </summary>
  public List<IFormFile>? Attachments { get; set; }
  ```

### 2. MessagesController - CreateMessage Endpoint
**File**: `backend/UknfCommunicationPlatform.Api/Controllers/v1/MessagesController.cs`

**Changes**:
- Changed parameter binding from `[FromBody]` to `[FromForm]` to support multipart/form-data
- Added `[RequestSizeLimit(100_000_000)]` attribute (100MB max)
- Added attachment validation:
  - Maximum 50MB per file
  - No empty files allowed
- Updated documentation to reflect attachment support

**Endpoint**: `POST /api/v1/messages`

**Example cURL**:
```bash
curl -X POST http://localhost:5000/api/v1/messages \
  -F "Subject=Test Message" \
  -F "Body=This is a test message with attachments" \
  -F "RecipientId=2" \
  -F "SendImmediately=true" \
  -F "Attachments=@/path/to/file1.pdf" \
  -F "Attachments=@/path/to/file2.xlsx"
```

### 3. MessagesController - Download Attachment Endpoint
**File**: `backend/UknfCommunicationPlatform.Api/Controllers/v1/MessagesController.cs`

**New Endpoint**: `GET /api/v1/messages/{messageId}/attachments/{attachmentId}/download`

**Features**:
- Verifies user has access to the message
- Returns 404 if attachment not found or access denied
- Returns file with proper content-type and filename
- Logs download activity

**Example cURL**:
```bash
curl -O http://localhost:5000/api/v1/messages/123/attachments/456/download
```

### 4. MessageService - CreateMessageAsync Method
**File**: `backend/UknfCommunicationPlatform.Infrastructure/Services/MessageService.cs`

**Changes**:
- Enhanced to process attachments atomically with message creation
- Attachments are created in the same database transaction context
- For each uploaded file:
  1. Read file content into memory
  2. Create `MessageAttachment` entity with:
     - Foreign key `MessageId` (ensures attachment belongs to message)
     - File metadata (name, size, content-type)
     - Binary content stored as byte array
     - Upload timestamp
     - Uploader user ID
  3. Save to `message_attachments` table
- Loads attachment collection after save for response
- Logs attachment count for audit trail

### 5. MessageService - GetAttachmentAsync Method
**File**: `backend/UknfCommunicationPlatform.Infrastructure/Services/MessageService.cs`

**New Method**: `GetAttachmentAsync(long messageId, long attachmentId, long userId)`

**Security Flow**:
1. Verify user has access to the message (is sender or recipient)
2. Verify message is not cancelled
3. Retrieve attachment ensuring it belongs to the specified message
4. Log download activity
5. Return attachment or null if access denied

**Method Signature**:
```csharp
public async Task<MessageAttachment?> GetAttachmentAsync(
    long messageId, 
    long attachmentId, 
    long userId)
```

## Database Schema

### MessageAttachment Entity
**Table**: `message_attachments`

**Columns**:
- `id` (bigint, PK) - Unique identifier
- `message_id` (bigint, FK) - Parent message ID (NOT NULL)
- `file_name` (varchar) - Original filename
- `file_size` (bigint) - Size in bytes
- `content_type` (varchar) - MIME type
- `file_content` (bytea) - Binary file content
- `uploaded_at` (timestamp) - Upload timestamp
- `uploaded_by_user_id` (bigint, FK) - User who uploaded

**Relationships**:
- Many-to-One with `messages` table (cascade on delete)
- Many-to-One with `users` table (uploader)

## API Response Updates

### MessageResponse / MessageDetailResponse
The attachment information is returned in message responses:

```json
{
  "id": 123,
  "subject": "Test Message",
  "body": "Message content",
  "attachmentCount": 2,
  ...
}
```

### MessageDetailResponse (includes full attachment list)
```json
{
  "id": 123,
  "subject": "Test Message",
  "body": "Message content",
  "attachments": [
    {
      "id": 456,
      "fileName": "document.pdf",
      "fileSize": 1024000,
      "contentType": "application/pdf",
      "uploadedAt": "2025-10-05T12:34:56Z"
    }
  ],
  ...
}
```

## Swagger Documentation

Both endpoints are fully documented in Swagger/OpenAPI:

1. **POST /api/v1/messages** - Shows multipart/form-data support with file uploads
2. **GET /api/v1/messages/{messageId}/attachments/{attachmentId}/download** - Shows file download response

Access Swagger UI at: `http://localhost:5000/swagger`

## Testing

### Test Results
- ✅ All 142 unit tests passed
- ✅ All 20 integration tests passed
- ✅ Build successful with 0 errors

### Manual Testing Checklist
- [ ] Create message without attachments (should work)
- [ ] Create message with 1 attachment
- [ ] Create message with multiple attachments
- [ ] Try to upload file > 50MB (should fail with 400)
- [ ] Try to upload empty file (should fail with 400)
- [ ] Download attachment as message sender
- [ ] Download attachment as message recipient
- [ ] Try to download attachment from message you're not part of (should fail with 404)
- [ ] Verify attachment count in message list response
- [ ] Verify attachment details in message detail response

## Usage Examples

### Creating a Message with Attachments (Frontend)

```typescript
const formData = new FormData();
formData.append('Subject', 'Monthly Report');
formData.append('Body', 'Please find the attached reports.');
formData.append('RecipientId', '123');
formData.append('SendImmediately', 'true');

// Add multiple files
files.forEach(file => {
  formData.append('Attachments', file);
});

const response = await fetch('/api/v1/messages', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  },
  body: formData
});
```

### Downloading an Attachment (Frontend)

```typescript
async function downloadAttachment(messageId: number, attachmentId: number, fileName: string) {
  const response = await fetch(
    `/api/v1/messages/${messageId}/attachments/${attachmentId}/download`,
    {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    }
  );
  
  if (response.ok) {
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
  }
}
```

## Security Considerations

1. **File Size Limits**: Prevents DoS attacks through large file uploads
2. **Access Control**: Users can only access attachments from their own messages
3. **Atomic Operations**: Attachments are created with messages in a transaction
4. **Audit Trail**: All downloads are logged with user ID, attachment ID, and timestamp
5. **Content-Type Validation**: Could be enhanced to whitelist allowed file types
6. **Virus Scanning**: Integration point available for future virus scanning service

## Future Enhancements

1. **Virus Scanning**: Integrate with antivirus service before allowing downloads
2. **File Type Restrictions**: Whitelist/blacklist specific file extensions
3. **Storage Backend**: Move from database BLOB to cloud storage (Azure Blob, S3)
4. **Compression**: Automatically compress large files
5. **Thumbnails**: Generate previews for images/PDFs
6. **Attachment Versioning**: Track if attachment is replaced
7. **Bulk Download**: ZIP multiple attachments together
8. **Direct Upload to Cloud**: Upload directly to cloud storage from browser

## Migration Notes

### Existing Data
- Existing messages without attachments continue to work
- `AttachmentCount` will be 0 for messages without attachments
- No data migration required

### Backward Compatibility
- Old API clients can still create messages without attachments
- The `Attachments` field is optional in the request
- Response schema is backward compatible (added fields only)

## Performance Considerations

1. **Database Size**: Storing files as BLOBs increases database size
   - Consider moving to external storage for production
   - Monitor database growth

2. **Memory Usage**: Files are loaded into memory during upload/download
   - 50MB limit per file prevents excessive memory usage
   - Consider streaming for larger files in future

3. **Query Performance**: Include attachment count without loading BLOB data
   - Uses `COUNT()` aggregation in queries
   - BLOB data only loaded on explicit download

## Compliance & Audit

- All attachment uploads are logged with timestamp and user ID
- All downloads are logged for audit trail
- File metadata (name, size, type) is preserved
- Original filenames are maintained for user experience
