# Prompt: Message Details and Library Updates

**Date:** 2025-10-05  
**Branch:** Frontend-updates  
**Timestamp:** 2025-10-05_055039

## User Prompts

### Prompt 1: Simplify Library Upload Dialog
> Dodaj button should only give opportunity to upload file

### Prompt 2: Update Message Details with API Integration
> Let's updated detail for wiadomości it should take information from http://localhost:5000/api/v1/messages/2

### Prompt 3: Add Attachments Support
> Załączniki should come from the response on http://localhost:5000/api/v1/messages/2

### Prompt 4: Simplify Attachment Actions
> Remove from details buttons Usuń and Edytuj, leave only dodaj. Pobierz should be possible only on single lines

## Response Summary

Successfully implemented comprehensive message details view with API integration, attachment support with download functionality, and simplified UI controls across library and messages modules.

## Changes Made

### 1. Message Service Updates (message.service.ts)

- **Added MessageAttachment Interface**:
  ```typescript
  export interface MessageAttachment {
    id: number;
    fileName: string;
    fileSize: number;
    contentType: string;
    uploadedAt: string;
  }
  ```

- **Updated Message Interface**: Added `attachments?: MessageAttachment[]` field to support attachment arrays from API

- **Added downloadAttachment Method**:
  ```typescript
  downloadAttachment(messageId: number, attachmentId: number): Observable<Blob>
  ```
  - Downloads attachment as blob for client-side file download
  - Endpoint: `GET /api/v1/messages/{messageId}/attachments/{attachmentId}/download`

### 2. Message Details Component (message-details.component.ts)

**Complete Rewrite** - Transformed from empty placeholder to full-featured details view:

#### Component Features:
- **Route Parameter Handling**: Reads message ID from URL route parameters
- **API Integration**: Fetches message details from `/api/v1/messages/{id}`
- **Response Handling**: Supports both wrapped (`{data: [...]}`) and direct message object responses
- **Loading States**: Shows spinner during data fetch
- **Error Handling**: Displays user-friendly error messages with retry option

#### UI Sections:
1. **Breadcrumb Navigation**: 
   - Home → Wiadomości → Szczegóły wiadomości

2. **Message Header Card**:
   - Identyfikator (message ID or custom identifier)
   - Sygnatura sprawy (case signature)
   - Status (with color-coded badges)
   - Podmiot (related entity)
   - Subject (temat)

3. **Sender Information Card**:
   - Full name
   - Email address
   - Date sent (sentAt)

4. **Recipient Information Card**:
   - Full name
   - Email address
   - Date read (readAt)

5. **User Message Card**:
   - Message body (wiadomoscUzytkownika or body)
   - Date sent by entity (dataPrzeslaniaPodmiotu)
   - Pre-wrapped text formatting

6. **UKNF Response Card** (conditional):
   - Response message (wiadomoscPracownikaUKNF)
   - UKNF worker name (pracownikUKNF)
   - Response date (dataPrzeslaniaUKNF)
   - Styled with green accent

7. **Attachments Card** (conditional):
   - Lists all attachments from API response
   - Each attachment shows:
     - File type icon (PDF, Excel, Word, generic file)
     - File name
     - File size (formatted: B, KB, MB, GB)
     - Upload date
     - Individual "Pobierz" (Download) button
   - Hover effects for better UX

8. **Action Buttons**:
   - Odpowiedz (Reply) - Primary button
   - Drukuj (Print) - Outlined
   - Powrót (Back) - Outlined, navigates to messages list

#### Helper Methods:
- `formatDate()`: Formats dates to Polish locale (dd.mm.yyyy hh:mm)
- `getStatusText()`: Converts numeric status codes to Polish text
- `getStatusClass()`: Returns CSS class for status badge styling
- `getFileIcon()`: Returns appropriate PrimeNG icon class based on content type
- `formatFileSize()`: Converts bytes to human-readable format
- `downloadAttachment()`: Handles file download via blob URL creation

#### Styling:
- Responsive grid layouts
- Card-based design with PrimeNG components
- Color-coded status badges (sent, pending, answered, closed)
- Attachment list with hover effects
- File type specific icon colors (PDF: red, Excel: green, Word: blue)
- Mobile-responsive with column stacking on small screens

### 3. Messages List Component (messages-list.component.html)

- **Removed Attachment Action Buttons**: 
  - Deleted: "Pobierz", "Edytuj", "Usuń"
  - Kept: Only "Dodaj" button for adding new attachments
  - "Pobierz" functionality remains on individual attachment lines (already in place)

### 4. Library List Component Updates (from earlier prompts)

- **Simplified Main Actions**: Removed "Modyfikuj", "Poglądaj", "Usuń", "Historia", "Eksportuj"
- **Kept Only**: "Dodaj" button for uploading files
- **Removed Unused Methods**: Deleted `viewFileDetails()` and `deleteFile()` methods

## API Response Structure

### Message Details Endpoint: GET /api/v1/messages/{id}

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
  "body": "W załączeniu przesyłamy żądany raport finansowy...",
  "sender": {
    "id": 2,
    "email": "k.administratorska@uknf.gov.pl",
    "firstName": "Katarzyna",
    "lastName": "Administratorska",
    "fullName": "Katarzyna Administratorska"
  },
  "recipient": {
    "id": 12,
    "email": "kontakt@pekao.pl",
    "firstName": "Przedstawiciel",
    "lastName": "Bank Pekao S.A.",
    "fullName": "Przedstawiciel Bank Pekao S.A."
  },
  "status": 1,
  "isRead": true,
  "sentAt": "2025-10-04T02:39:35.965161Z",
  "readAt": "2025-10-04T08:39:35.965161Z",
  "attachmentCount": 1,
  "identyfikator": "2025/System2/2",
  "sygnaturaSprawy": "2025/000002",
  "statusWiadomosci": "Wysłana",
  "pracownikUKNF": "Katarzyna Administratorska",
  "wiadomoscPracownikaUKNF": "W załączeniu przesyłamy..."
}
```

## Technical Implementation Details

### Attachment Download Flow:
1. User clicks "Pobierz" on individual attachment
2. Component calls `messageService.downloadAttachment(messageId, attachmentId)`
3. Service performs GET request with `responseType: 'blob'`
4. Component receives blob response
5. Creates temporary object URL using `window.URL.createObjectURL()`
6. Creates temporary anchor element with download attribute
7. Programmatically clicks anchor to trigger download
8. Cleans up: removes anchor and revokes object URL

### Error Handling:
- Network errors display Polish alert: "Nie udało się pobrać załącznika..."
- Loading failures show retry button
- Missing message shows "Nie znaleziono wiadomości"

### Accessibility:
- ARIA labels on breadcrumbs
- Semantic HTML structure
- Keyboard navigable buttons
- Screen reader friendly labels

## Files Modified

1. `frontend/uknf-project/src/app/services/message.service.ts`
2. `frontend/uknf-project/src/app/features/messages/message-details/message-details.component.ts`
3. `frontend/uknf-project/src/app/features/messages/messages-list/messages-list.component.html`
4. `frontend/uknf-project/src/app/features/library/library-list/library-list.component.html` (from earlier)
5. `frontend/uknf-project/src/app/features/library/library-list/library-list.component.ts` (from earlier)

## Testing Recommendations

### Message Details:
1. Test with various message IDs
2. Verify attachment display and download
3. Test with messages that have no attachments
4. Test with messages that have UKNF responses vs. those without
5. Verify date formatting in Polish locale
6. Test error handling (invalid IDs, network errors)
7. Test navigation (breadcrumb, back button)

### Attachments:
1. Download PDF files
2. Download Excel files (verify content-type handling)
3. Download various file types (Word, ZIP, images)
4. Test with large files
5. Verify filename preservation
6. Test file size formatting (B, KB, MB, GB)
7. Test icon display for different content types

### UI/UX:
1. Test responsive layout on mobile devices
2. Verify card layouts at different screen sizes
3. Test hover effects on attachment items
4. Verify status badge colors
5. Test loading spinner display
6. Test error message display

## Dependencies

- Angular Router (ActivatedRoute, Router)
- PrimeNG Components (Button, Card, Divider, Breadcrumb)
- RxJS (Observable, subscription handling)
- Browser Blob API
- DOM manipulation for download triggers
