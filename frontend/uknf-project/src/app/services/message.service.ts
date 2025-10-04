import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

// API Response interfaces matching backend structure
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

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1/messages';

  getMessages(page: number = 1, pageSize: number = 20): Observable<MessageResponse> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<MessageResponse>(this.apiUrl, { params });
  }

  getMessageById(id: number): Observable<Message> {
    return this.http.get<Message>(`${this.apiUrl}/${id}`);
  }
}
