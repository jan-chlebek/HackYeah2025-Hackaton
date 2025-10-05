import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FaqQuestion, FaqListResponse, FaqQuestionStatus } from '../models/faq.model';

/**
 * Service for managing FAQ questions and answers
 */
@Injectable({
  providedIn: 'root'
})
export class FaqService {
  private readonly apiUrl = 'http://localhost:5000/api/v1/faq';

  constructor(private http: HttpClient) {}

  /**
   * Get all published FAQ questions
   */
  getPublishedQuestions(pageNumber: number = 1, pageSize: number = 20): Observable<FaqListResponse> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('status', FaqQuestionStatus.Published.toString());
    
    return this.http.get<FaqListResponse>(this.apiUrl, { params });
  }

  /**
   * Get FAQ questions by user (Moje zapytania)
   */
  getMyQuestions(pageNumber: number = 1, pageSize: number = 20): Observable<FaqListResponse> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<FaqListResponse>(`${this.apiUrl}/my-questions`, { params });
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
  submitQuestion(question: Partial<FaqQuestion>): Observable<FaqQuestion> {
    return this.http.post<FaqQuestion>(this.apiUrl, question);
  }

  /**
   * Search FAQ questions
   */
  searchQuestions(searchTerm: string, category?: string): Observable<FaqListResponse> {
    let params = new HttpParams()
      .set('searchTerm', searchTerm)
      .set('status', FaqQuestionStatus.Published.toString());
    
    if (category) {
      params = params.set('category', category);
    }
    
    return this.http.get<FaqListResponse>(`${this.apiUrl}/search`, { params });
  }
}
