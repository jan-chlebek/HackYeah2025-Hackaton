import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

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
  private apiUrl = `${environment.apiUrl}/messages`;

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
      if (filters.identyfikator) {
        params = params.set('identyfikator', filters.identyfikator);
      }
      if (filters.sygnaturaSprawy) {
        params = params.set('sygnaturaSprawy', filters.sygnaturaSprawy);
      }
      if (filters.podmiot) {
        params = params.set('podmiot', filters.podmiot);
      }
      if (filters.statusWiadomosci) {
        params = params.set('statusWiadomosci', filters.statusWiadomosci);
      }
      if (filters.priorytet) {
        params = params.set('priorytet', filters.priorytet);
      }
      if (filters.dataPrzeslaniaPodmiotuFrom) {
        params = params.set('dataPrzeslaniaPodmiotuFrom', filters.dataPrzeslaniaPodmiotuFrom);
      }
      if (filters.dataPrzeslaniaPodmiotuTo) {
        params = params.set('dataPrzeslaniaPodmiotuTo', filters.dataPrzeslaniaPodmiotuTo);
      }
      if (filters.uzytkownik) {
        params = params.set('uzytkownik', filters.uzytkownik);
      }
      if (filters.dataPrzeslaniaUKNFFrom) {
        params = params.set('dataPrzeslaniaUKNFFrom', filters.dataPrzeslaniaUKNFFrom);
      }
      if (filters.dataPrzeslaniaUKNFTo) {
        params = params.set('dataPrzeslaniaUKNFTo', filters.dataPrzeslaniaUKNFTo);
      }
      if (filters.pracownikUKNF) {
        params = params.set('pracownikUKNF', filters.pracownikUKNF);
      }
      if (filters.mojePodmioty !== undefined) {
        params = params.set('mojePodmioty', filters.mojePodmioty.toString());
      }
      if (filters.wymaganaOdpowiedzUKNF !== undefined) {
        params = params.set('wymaganaOdpowiedzUKNF', filters.wymaganaOdpowiedzUKNF.toString());
      }
    }

    if (sortField) {
      params = params.set('sortField', sortField);
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

