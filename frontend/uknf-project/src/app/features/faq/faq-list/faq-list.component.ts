import { ChangeDetectorRef, Component, DestroyRef, OnDestroy, OnInit, inject } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FaqService } from '../../../services/faq.service';
import { FaqQuestion, FaqListResponse } from '../../../models/faq.model';

@Component({
  selector: 'app-faq-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="faq-container">
      <div class="faq-header">
        <div class="header-content">
          <div>
            <h1>Baza Wiedzy FAQ</h1>
            <p class="faq-subtitle">Najczęściej zadawane pytania i odpowiedzi</p>
          </div>
          <button
            type="button"
            class="btn-add-question"
            (click)="toggleForm()"
            [class.active]="showForm"
          >
            <i class="pi" [ngClass]="showForm ? 'pi-times' : 'pi-plus'"></i>
            {{ showForm ? 'Anuluj' : 'Zadaj pytanie' }}
          </button>
        </div>
      </div>

      <div *ngIf="showForm" class="add-question-form">
        <h2 class="form-title">
          <i class="pi pi-plus-circle"></i>
          Zadaj nowe pytanie
        </h2>

        <div class="form-group">
          <label for="newQuestion">Pytanie *</label>
          <textarea
            id="newQuestion"
            [(ngModel)]="newQuestion"
            placeholder="Wpisz swoje pytanie..."
            rows="4"
            class="form-control"
            [disabled]="submitting"
          ></textarea>
        </div>

        <div *ngIf="submitError" class="submit-error">
          <i class="pi pi-exclamation-circle"></i>
          {{ submitError }}
        </div>

        <div *ngIf="submitSuccess" class="submit-success">
          <i class="pi pi-check-circle"></i>
          Pytanie zostało pomyślnie dodane!
        </div>

        <div class="form-actions">
          <button
            type="button"
            class="btn-submit"
            (click)="submitNewQuestion()"
            [disabled]="submitting || !newQuestion.trim()"
          >
            <i class="pi" [ngClass]="submitting ? 'pi-spinner pi-spin' : 'pi-send'"></i>
            {{ submitting ? 'Wysyłanie...' : 'Wyślij pytanie' }}
          </button>
          <button
            type="button"
            class="btn-cancel"
            (click)="cancelNewQuestion()"
            [disabled]="submitting"
          >
            Wyczyść
          </button>
        </div>
      </div>

      <div *ngIf="showForm" class="divider"></div>

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
        <div *ngFor="let item of questions" class="faq-item">
          <div class="faq-question">
            <h3 class="faq-title">{{ item.question }}</h3>
          </div>

          <div *ngIf="item.answer" class="faq-answer">
            <div class="answer-header">
              <i class="pi pi-check-circle"></i>
              <span>Odpowiedź</span>
            </div>
            <div class="answer-content" [innerHTML]="getAnswerHtml(item)"></div>
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

    .header-content {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      gap: 2rem;
      position: relative;
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

    .btn-add-question {
      position: absolute;
      top: 0;
      right: 0;
      padding: 0.75rem 1.5rem;
      background: #003366;
      color: #fff;
      border: none;
      border-radius: 4px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s;
      display: flex;
      align-items: center;
      gap: 0.5rem;
      white-space: nowrap;
      z-index: 10;
    }

    .btn-add-question:hover {
      background: #002244;
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0, 51, 102, 0.3);
    }

    .btn-add-question.active {
      background: #d32f2f;
    }

    .btn-add-question.active:hover {
      background: #b71c1c;
    }

    .btn-add-question i {
      font-size: 1.1rem;
    }

    .add-question-form {
      background: #fff;
      border: 2px solid #003366;
      border-radius: 8px;
      padding: 1.5rem;
      margin-bottom: 2rem;
      box-shadow: 0 2px 8px rgba(0, 51, 102, 0.1);
    }

    .form-title {
      color: #003366;
      font-size: 1.25rem;
      font-weight: 600;
      margin-bottom: 1.5rem;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .form-title i {
      font-size: 1.5rem;
    }

    .form-group {
      margin-bottom: 1rem;
    }

    .form-group label {
      display: block;
      color: #333;
      font-weight: 600;
      margin-bottom: 0.5rem;
      font-size: 0.95rem;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ccc;
      border-radius: 4px;
      font-size: 1rem;
      font-family: inherit;
      transition: border-color 0.3s;
      resize: vertical;
    }

    .form-control:focus {
      outline: none;
      border-color: #003366;
      box-shadow: 0 0 0 3px rgba(0, 51, 102, 0.1);
    }

    .form-control:disabled {
      background-color: #f5f5f5;
      cursor: not-allowed;
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      margin-top: 1rem;
    }

    .btn-submit,
    .btn-cancel {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 4px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .btn-submit {
      background: #003366;
      color: #fff;
    }

    .btn-submit:hover:not(:disabled) {
      background: #002244;
      transform: translateY(-1px);
      box-shadow: 0 4px 8px rgba(0, 51, 102, 0.3);
    }

    .btn-submit:disabled {
      background: #ccc;
      cursor: not-allowed;
      transform: none;
    }

    .btn-cancel {
      background: #f5f5f5;
      color: #666;
    }

    .btn-cancel:hover:not(:disabled) {
      background: #e0e0e0;
    }

    .btn-cancel:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .submit-error {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.75rem;
      background: #ffebee;
      color: #c62828;
      border-radius: 4px;
      margin-bottom: 1rem;
      border-left: 4px solid #c62828;
    }

    .submit-error i {
      font-size: 1.25rem;
    }

    .submit-success {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.75rem;
      background: #e8f5e9;
      color: #2e7d32;
      border-radius: 4px;
      margin-bottom: 1rem;
      border-left: 4px solid #2e7d32;
    }

    .submit-success i {
      font-size: 1.25rem;
    }

    .divider {
      height: 2px;
      background: linear-gradient(to right, transparent, #e0e0e0, transparent);
      margin: 2rem 0;
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

      .header-content {
        flex-direction: column;
        align-items: flex-start;
        gap: 1rem;
      }

      .btn-add-question {
        position: static;
        width: 100%;
        justify-content: center;
      }

      .faq-header h1 {
        font-size: 1.5rem;
      }

      .form-actions {
        flex-direction: column-reverse;
      }

      .btn-submit,
      .btn-cancel {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class FaqListComponent implements OnInit, OnDestroy {
  questions: FaqQuestion[] = [];
  loading = false;
  error: string | null = null;

  showForm = false;
  newQuestion = '';
  submitting = false;
  submitError: string | null = null;
  submitSuccess = false;

  private successTimeoutId: ReturnType<typeof setTimeout> | null = null;
  private readonly destroyRef: DestroyRef = inject(DestroyRef);
  private isDestroyed = false;

  constructor(private faqService: FaqService, private readonly cdr: ChangeDetectorRef, private readonly sanitizer: DomSanitizer) {
    this.destroyRef.onDestroy(() => {
      this.isDestroyed = true;
    });
  }

  ngOnInit(): void {
    this.loadQuestions();
  }

  ngOnDestroy(): void {
    if (this.successTimeoutId) {
      clearTimeout(this.successTimeoutId);
      this.successTimeoutId = null;
    }
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.cancelNewQuestion();
    }
    this.detectChangesSafe();
  }

  submitNewQuestion(): void {
    const trimmedQuestion = this.newQuestion.trim();
    if (!trimmedQuestion || this.submitting) {
      return;
    }

    this.submitting = true;
    this.submitError = null;
    this.submitSuccess = false;

    this.faqService
      .submitQuestion(trimmedQuestion)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
      next: () => {
        this.submitting = false;
        this.submitSuccess = true;
        this.newQuestion = '';
        this.loadQuestions();

        if (this.successTimeoutId) {
          clearTimeout(this.successTimeoutId);
        }

        this.successTimeoutId = setTimeout(() => {
          this.submitSuccess = false;
          this.showForm = false;
          this.successTimeoutId = null;
            this.detectChangesSafe();
        }, 2000);
          this.detectChangesSafe();
      },
      error: (err) => {
        console.error('Error submitting FAQ question:', err);
        this.submitting = false;
        this.submitError = 'Nie udało się wysłać pytania. Spróbuj ponownie później.';
          this.detectChangesSafe();
      }
    });
  }

  cancelNewQuestion(): void {
    this.newQuestion = '';
    this.submitError = null;
    this.submitSuccess = false;
    if (this.submitting) {
      this.submitting = false;
    }

    if (this.successTimeoutId) {
      clearTimeout(this.successTimeoutId);
      this.successTimeoutId = null;
    }
    this.detectChangesSafe();
  }

  private loadQuestions(): void {
    this.loading = true;
    this.error = null;
    this.detectChangesSafe();

    this.faqService
      .getAllQuestions()
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.loading = false;
          this.detectChangesSafe();
        })
      )
      .subscribe({
        next: (response: FaqListResponse) => {
          this.questions = response?.items ?? [];
          this.detectChangesSafe();
        },
        error: (err) => {
          console.error('Error loading FAQ questions:', err);
          this.error = 'Nie udało się załadować pytań. Spróbuj ponownie później.';
          this.detectChangesSafe();
        }
      });
  }

  private detectChangesSafe(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }

  /**
   * Returns sanitized HTML for an answer. If backend already stores safe HTML, this prevents Angular from stripping tags.
   * Minimal sanitation: relying on backend to whitelist tags; could be extended with client-side DOMPurify if needed.
   */
  getAnswerHtml(item: FaqQuestion): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(item.answer || '');
  }
}
