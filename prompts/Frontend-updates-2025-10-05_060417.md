# Prompt: Fix Attachments Display in Messages

**Date:** 2025-10-05  
**Branch:** Frontend-updates  
**Timestamp:** 2025-10-05_060417

## User Request

> Załączniki are not correctly displayed. Endpoint http://localhost:5000/api/v1/messages/2 returns attachments, but they're not showing correctly. Fix this.

## Problem Identified

The attachments section in the messages list component was displaying **hardcoded attachment names** instead of fetching and displaying the actual attachments from the API response:

```html
<!-- Old hardcoded version -->
<div class="attachment-item">
  <span>Zalącznik_pierwszy.pdf</span>
  ...
</div>
<div class="attachment-item">
  <span>Zalącznik_drugi.pdf</span>
  ...
</div>
<div class="attachment-item">
  <span>Zalącznik_trzeci.txt</span>
  ...
</div>
```

## Solution Implemented

### 1. Updated Messages List HTML (messages-list.component.html)

Replaced hardcoded attachments with dynamic data binding:

```html
<div class="attachments-list">
  <!-- Show message when no attachments -->
  <div *ngIf="!selectedMessage?.attachments || selectedMessage.attachments.length === 0" 
       class="no-attachments">
    <span class="text-muted">Brak załączników</span>
  </div>
  
  <!-- Display actual attachments from API -->
  <div *ngFor="let attachment of selectedMessage?.attachments" class="attachment-item">
    <span>{{ attachment.fileName }}</span>
    <button 
      pButton 
      type="button" 
      icon="pi pi-download" 
      class="p-button-text p-button-sm"
      (click)="downloadMessageAttachment(attachment)"
      [attr.aria-label]="'Pobierz ' + attachment.fileName"
    ></button>
  </div>
</div>
```

**Key Changes:**
- Uses `*ngFor` to iterate over `selectedMessage.attachments`
- Displays actual filename from API: `{{ attachment.fileName }}`
- Shows "Brak załączników" when no attachments exist
- Each attachment has its own download button with click handler
- Added ARIA label for accessibility

### 2. Updated Messages List Component (messages-list.component.ts)

#### Added MessageAttachment Import
```typescript
import { MessageService, Message, MessageFilters, MessageAttachment } from '../../../services/message.service';
```

#### Enhanced viewMessageDetails Method
```typescript
viewMessageDetails(message: Message): void {
  // Fetch full message details including attachments from API
  this.messageService.getMessageById(message.id).subscribe({
    next: (response: any) => {
      // Handle wrapped response or direct message object
      if (response.data && Array.isArray(response.data)) {
        this.selectedMessage = response.data[0] || null;
      } else if (response.attachments) {
        // Direct message object with attachments
        this.selectedMessage = response;
      } else {
        // Fallback to original message
        this.selectedMessage = { ...message };
      }
      
      // Set default priority if not set
      if (this.selectedMessage && !this.selectedMessage.priorytet) {
        this.selectedMessage.priorytet = 'Średni';
      }
      
      this.showDetailsDialog = true;
    },
    error: (error) => {
      console.error('Error loading message details:', error);
      // Fallback to using message from list
      this.selectedMessage = { ...message };
      this.showDetailsDialog = true;
    }
  });
}
```

**Why This Change:**
- The list view might not include full attachment data
- Fetches complete message details from `/api/v1/messages/{id}` when opening details
- Ensures attachments array is populated before displaying the dialog
- Handles both wrapped (`{data: [...]}`) and direct message object responses
- Includes error fallback

#### Added downloadMessageAttachment Method
```typescript
downloadMessageAttachment(attachment: MessageAttachment): void {
  if (!this.selectedMessage) return;
  
  console.log('Downloading attachment:', attachment);
  this.messageService.downloadAttachment(this.selectedMessage.id, attachment.id).subscribe({
    next: (blob) => {
      // Create temporary URL for blob
      const url = window.URL.createObjectURL(blob);
      
      // Create temporary anchor element to trigger download
      const a = document.createElement('a');
      a.href = url;
      a.download = attachment.fileName;
      document.body.appendChild(a);
      a.click();
      
      // Clean up
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    },
    error: (error) => {
      console.error('Error downloading attachment:', error);
      alert('Nie udało się pobrać załącznika. Spróbuj ponownie.');
    }
  });
}
```

**Download Flow:**
1. Calls `messageService.downloadAttachment(messageId, attachmentId)`
2. Receives blob from API
3. Creates temporary object URL
4. Programmatically triggers download
5. Cleans up resources (removes anchor, revokes URL)

### 3. Added CSS Styling (messages-list.component.css)

```css
.no-attachments {
  text-align: center;
  padding: 1rem;
  color: var(--uknf-medium-gray);
  font-style: italic;
}

.text-muted {
  color: var(--uknf-medium-gray);
  opacity: 0.8;
}
```

**Purpose:**
- Styles the "Brak załączników" message
- Uses muted colors to indicate empty state
- Maintains consistency with UI design

## API Response Structure

### GET /api/v1/messages/{id}

```json
{
  "attachments": [
    {
      "id": 1,
      "fileName": "raport_kwartalny_Q4_2024.pdf",
      "fileSize": 39,
      "contentType": "application/pdf",
      "uploadedAt": "2025-10-04T02:39:35.965161Z"
    }
  ],
  "id": 2,
  "subject": "Raport kwartalny - dostarczone",
  "body": "W załączeniu przesyłamy...",
  "sender": {...},
  "recipient": {...},
  "status": 1,
  "attachmentCount": 1,
  ...
}
```

## Before vs. After

### Before
- ❌ Hardcoded attachment names ("Zalącznik_pierwszy.pdf", etc.)
- ❌ Always showed 3 attachments regardless of actual data
- ❌ Download buttons did nothing
- ❌ No indication when message has no attachments

### After
- ✅ Displays actual attachments from API
- ✅ Shows correct number of attachments (0, 1, 2+)
- ✅ Download buttons functional with blob download
- ✅ Shows "Brak załączników" when no attachments exist
- ✅ Proper error handling

## Testing Scenarios

1. **Message with Attachments:**
   - Open message with 1 attachment → Shows "raport_kwartalny_Q4_2024.pdf"
   - Click download → File downloads successfully

2. **Message without Attachments:**
   - Open message with no attachments → Shows "Brak załączników"

3. **Multiple Attachments:**
   - Open message with multiple attachments → All attachments listed
   - Each has individual download button

4. **Error Handling:**
   - Network failure → Falls back to list message data
   - Download failure → Shows Polish error alert

## Files Modified

1. `frontend/uknf-project/src/app/features/messages/messages-list/messages-list.component.html`
2. `frontend/uknf-project/src/app/features/messages/messages-list/messages-list.component.ts`
3. `frontend/uknf-project/src/app/features/messages/messages-list/messages-list.component.css`

## Technical Notes

- **MessageService** already had `downloadAttachment()` method - reused it
- **MessageAttachment** interface already existed in service - imported it
- Component now fetches full message details on dialog open, not just using list data
- Blob download approach works for all file types (PDF, Excel, Word, ZIP, etc.)
- Proper resource cleanup prevents memory leaks

## Dependencies

- Angular HttpClient (already in use)
- RxJS operators (already in use)
- MessageService with downloadAttachment method (already implemented)
- Browser Blob API (native)
- DOM manipulation for download trigger (native)
