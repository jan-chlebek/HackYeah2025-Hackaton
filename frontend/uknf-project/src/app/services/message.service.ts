import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

// API Response interfaces matching backend structure
export interface MessageAttachment {
  id: number;
  fileName: string;
  fileSize: number;
  contentType: string;
  uploadedAt: string;
}

export interface MessageUser {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
}

export interface Message {
  id: number;
  subject: string;
  body: string;
  sender: MessageUser;
  recipient: MessageUser;
  status: number;
  folder: number;
  threadId: number | null;
  parentMessageId: number | null;
  isRead: boolean;
  sentAt: string;
  readAt: string | null;
  relatedEntityId: number | null;
  relatedEntityName: string | null;
  relatedReportId: number | null;
  relatedCaseId: number | null;
  attachmentCount: number;
  replyCount: number;
  isCancelled: boolean;
  cancelledAt: string | null;
  attachments?: MessageAttachment[];
  
  // Polish UI fields
  identyfikator?: string;
  sygnaturaSprawy?: string;
  podmiot?: string;
  statusWiadomosci?: string;
  priorytet?: string;
  dataPrzeslaniaPodmiotu?: string;
  uzytkownik?: string;
  wiadomoscUzytkownika?: string;
  dataPrzeslaniaUKNF?: string;
  pracownikUKNF?: string;
  wiadomoscPracownikaUKNF?: string;
}

export interface MessagePagination {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface MessageResponse {
  data: Message[];
  pagination: MessagePagination;
}

export interface MessageFilters {
  identyfikator?: string;
  sygnaturaSprawy?: string;
  podmiot?: string;
  statusWiadomosci?: string;
  priorytet?: string;
  dataPrzeslaniaPodmiotuFrom?: string;
  dataPrzeslaniaPodmiotuTo?: string;
  uzytkownik?: string;
  dataPrzeslaniaUKNFFrom?: string;
  dataPrzeslaniaUKNFTo?: string;
  pracownikUKNF?: string;
  mojePodmioty?: boolean;
  wymaganaOdpowiedzUKNF?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1/messages';

  getMessages(
    page: number = 1, 
    pageSize: number = 10,
    filters?: MessageFilters,
    sortField?: string,
    sortOrder?: 'asc' | 'desc'
  ): Observable<MessageResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (filters) {
      // Normalize empty strings to undefined to avoid sending blank params
      const normalized: Record<string, any> = { ...filters };
      Object.keys(normalized).forEach(k => {
        if (normalized[k] === '' || normalized[k] === null) delete normalized[k];
      });

      // Date normalization (ensure ISO date only if picker returns Date object)
      const normDate = (v: any) => {
        if (!v) return undefined;
        if (v instanceof Date) return v.toISOString();
        return v;
      };

      // Mapping: UI field -> API expected param (adjust if backend uses different naming)
      const map: Record<string, string> = {
        identyfikator: 'identifier',
        sygnaturaSprawy: 'caseSignature',
        podmiot: 'entityName',
        statusWiadomosci: 'status',
        priorytet: 'priority',
        dataPrzeslaniaPodmiotuFrom: 'entitySentFrom',
        dataPrzeslaniaPodmiotuTo: 'entitySentTo',
        uzytkownik: 'userName',
        dataPrzeslaniaUKNFFrom: 'uknfSentFrom',
        dataPrzeslaniaUKNFTo: 'uknfSentTo',
        pracownikUKNF: 'uknfEmployee'
      };

      for (const key of Object.keys(map)) {
        const value = (normalized as any)[key];
        if (value) {
          const finalVal = key.includes('data') ? normDate(value) : value;
          if (finalVal) {
            params = params.set(map[key], finalVal);
          }
        }
      }

      if ((normalized as any)['mojePodmioty'] !== undefined) {
        params = params.set('myEntities', String((normalized as any)['mojePodmioty']));
      }
      if ((normalized as any)['wymaganaOdpowiedzUKNF'] !== undefined) {
        params = params.set('requiresUknfResponse', String((normalized as any)['wymaganaOdpowiedzUKNF']));
      }
    }

    // Debug log (remove in production)
    console.log('[MessageService] getMessages params:', params.toString());

    if (sortField) {
      // Map UI (Polish / view) field names to backend sortable field names
      const sortMap: Record<string, string> = {
        identyfikator: 'id',
        sygnaturaSprawy: 'caseSignature',
        podmiot: 'entityName',
        statusWiadomosci: 'status',
        priorytet: 'priority',
        dataPrzeslaniaPodmiotu: 'sentAt',
        uzytkownik: 'senderName',
        wiadomoscUzytkownika: 'body',
        dataPrzeslaniaUKNF: 'uknfSentAt',
        pracownikUKNF: 'uknfEmployee',
        wiadomoscPracownikaUKNF: 'uknfBody'
      };
      const mapped = sortMap[sortField];
      if (mapped) {
        params = params.set('sortField', mapped);
      } else {
        console.warn('[MessageService] Unsupported sort field (not sending to backend):', sortField);
      }
    }
    if (sortOrder) {
      params = params.set('sortOrder', sortOrder);
    }
    
    return this.http.get<MessageResponse>(this.apiUrl, { params });
  }

  getMessageById(id: number): Observable<Message> {
    return this.http.get<Message>(`${this.apiUrl}/${id}`);
  }

  updateMessage(id: number, updates: Partial<Message>): Observable<Message> {
    return this.http.put<Message>(`${this.apiUrl}/${id}`, updates);
  }

  downloadAttachment(messageId: number, attachmentId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${messageId}/attachments/${attachmentId}/download`, {
      responseType: 'blob'
    });
  }
}
