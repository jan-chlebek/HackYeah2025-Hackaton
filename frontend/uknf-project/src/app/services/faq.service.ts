import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FaqQuestion, FaqListResponse } from '../models/faq.model';
import { environment } from '../../environments/environment';

/**
 * Service for managing FAQ questions and answers
 */
@Injectable({
  providedIn: 'root'
})
export class FaqService {
  private readonly apiUrl = `${environment.apiUrl}/faq`;

  constructor(private http: HttpClient) {}

  /**
   * Get all FAQ questions
   */
  getAllQuestions(page: number = 1, pageSize: number = 20): Observable<FaqListResponse> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<FaqListResponse>(this.apiUrl, { params });
  }

  /**
   * Get a single FAQ question by ID
   */
  getQuestionById(id: number): Observable<FaqQuestion> {
    return this.http.get<FaqQuestion>(`${this.apiUrl}/${id}`);
  }

  /**
   * Submit a new FAQ question
   */
  submitQuestion(question: string, answer?: string): Observable<FaqQuestion> {
    return this.http.post<FaqQuestion>(this.apiUrl, { question, answer });
  }
}
