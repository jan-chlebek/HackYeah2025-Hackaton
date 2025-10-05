import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FaqService } from '../../../services/faq.service';
import { FaqQuestion } from '../../../models/faq.model';

@Component({
  selector: 'app-faq-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="faq-container">
      <div class="faq-header">
        <h1>Baza Wiedzy FAQ</h1>
        <p class="faq-subtitle">Najczęściej zadawane pytania i odpowiedzi</p>
      </div>

      <div *ngIf="loading" class="loading-state">
        <i class="pi pi-spinner pi-spin"></i>
        <p>Ładowanie pytań...</p>
      </div>

      <div *ngIf="error" class="error-state">
        <i class="pi pi-exclamation-triangle"></i>
        <p>{{ error }}</p>
      </div>

      <div *ngIf="!loading && !error && questions.length === 0" class="empty-state">
        <i class="pi pi-inbox"></i>
        <p>Brak opublikowanych pytań.</p>
      </div>

      <div *ngIf="!loading && !error && questions.length > 0" class="faq-list">
        <div *ngFor="let question of questions" class="faq-item">
          <div class="faq-question">
            <div class="faq-meta">
              <span class="faq-category">{{ question.category }}</span>
              <span class="faq-stats">
                <i class="pi pi-eye"></i> {{ question.viewCount }}
                <span *ngIf="question.averageRating" class="rating">
                  <i class="pi pi-star-fill"></i> {{ question.averageRating | number: '1.1-1' }}
                </span>
              </span>
            </div>
            <h3 class="faq-title">{{ question.title }}</h3>
            <p class="faq-content">{{ question.content }}</p>
          </div>

          <div *ngIf="question.answerContent" class="faq-answer">
            <div class="answer-header">
              <i class="pi pi-check-circle"></i>
              <span>Odpowiedź</span>
              <span *ngIf="question.answeredAt" class="answer-date">
                {{ question.answeredAt | date: 'dd.MM.yyyy' }}
              </span>
            </div>
            <div class="answer-content" [innerHTML]="question.answerContent"></div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .faq-container {
      padding: 2rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .faq-header {
      margin-bottom: 2rem;
    }

    .faq-header h1 {
      color: #003366;
      font-size: 2rem;
      font-weight: 600;
      margin-bottom: 0.5rem;
    }

    .faq-subtitle {
      color: #666;
      font-size: 1rem;
    }

    .loading-state,
    .error-state,
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3rem;
      text-align: center;
      color: #666;
    }

    .loading-state i,
    .error-state i,
    .empty-state i {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .error-state {
      color: #d32f2f;
    }

    .faq-list {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .faq-item {
      background: white;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
    }

    .faq-question {
      padding: 1.5rem;
      background: #f8f9fa;
    }

    .faq-meta {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.75rem;
      font-size: 0.875rem;
    }

    .faq-category {
      background: #003366;
      color: white;
      padding: 0.25rem 0.75rem;
      border-radius: 4px;
      font-weight: 500;
    }

    .faq-stats {
      display: flex;
      align-items: center;
      gap: 1rem;
      color: #666;
    }

    .faq-stats i {
      margin-right: 0.25rem;
    }

    .rating {
      color: #ffa726;
      font-weight: 500;
    }

    .faq-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: #003366;
      margin-bottom: 0.5rem;
    }

    .faq-content {
      color: #333;
      line-height: 1.6;
      margin: 0;
    }

    .faq-answer {
      padding: 1.5rem;
      border-top: 2px solid #4caf50;
    }

    .answer-header {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 1rem;
      color: #4caf50;
      font-weight: 600;
    }

    .answer-header i {
      font-size: 1.25rem;
    }

    .answer-date {
      margin-left: auto;
      font-size: 0.875rem;
      color: #666;
      font-weight: normal;
    }

    .answer-content {
      color: #333;
      line-height: 1.6;
    }

    .answer-content p {
      margin: 0 0 1rem 0;
    }

    .answer-content p:last-child {
      margin-bottom: 0;
    }

    @media (max-width: 768px) {
      .faq-container {
        padding: 1rem;
      }

      .faq-header h1 {
        font-size: 1.5rem;
      }

      .faq-meta {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
      }

      .faq-stats {
        font-size: 0.75rem;
      }
    }
  `]
})
export class FaqListComponent implements OnInit {
  questions: FaqQuestion[] = [];
  loading = false;
  error: string | null = null;

  constructor(private faqService: FaqService) {}

  ngOnInit(): void {
    this.loadQuestions();
  }

  private loadQuestions(): void {
    this.loading = true;
    this.error = null;

    this.faqService.getPublishedQuestions().subscribe({
      next: (response) => {
        this.questions = response.items;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading FAQ questions:', err);
        this.error = 'Nie udało się załadować pytań. Spróbuj ponownie później.';
        this.loading = false;
      }
    });
  }
}
