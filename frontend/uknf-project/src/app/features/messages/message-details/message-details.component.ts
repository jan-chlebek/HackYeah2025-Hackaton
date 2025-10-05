import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { MessageService, Message, MessageAttachment } from '../../../services/message.service';

@Component({
  selector: 'app-message-details',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    CardModule,
    DividerModule,
    BreadcrumbModule
  ],
  template: `
    <div class="message-details-page">
      <!-- Breadcrumb Navigation -->
      <p-breadcrumb 
        [model]="breadcrumbItems" 
        [home]="home"
        class="mb-4"
        aria-label="Ścieżka nawigacji"
      ></p-breadcrumb>

      <div class="page-header">
        <h1>Szczegóły wiadomości</h1>
        <button 
          pButton 
          type="button" 
          label="Powrót" 
          icon="pi pi-arrow-left"
          class="p-button-outlined"
          (click)="goBack()"
        ></button>
      </div>

      <div *ngIf="loading" class="loading-container">
        <i class="pi pi-spin pi-spinner" style="font-size: 2rem"></i>
        <p>Ładowanie wiadomości...</p>
      </div>

      <div *ngIf="error" class="error-container">
        <i class="pi pi-exclamation-triangle" style="font-size: 2rem; color: var(--red-500)"></i>
        <p>{{ error }}</p>
        <button 
          pButton 
          type="button" 
          label="Spróbuj ponownie" 
          (click)="loadMessage()"
        ></button>
      </div>

      <div *ngIf="message && !loading && !error" class="message-details-container">
        <!-- Message Header Card -->
        <p-card class="message-card">
          <div class="message-header">
            <div class="header-row">
              <div class="header-field">
                <label>Identyfikator:</label>
                <span class="value">{{ message.identyfikator || message.id }}</span>
              </div>
              <div class="header-field">
                <label>Sygnatura sprawy:</label>
                <span class="value">{{ message.sygnaturaSprawy || '-' }}</span>
              </div>
              <div class="header-field">
                <label>Status:</label>
                <span class="value status-badge" [ngClass]="getStatusClass(message.statusWiadomosci)">
                  {{ message.statusWiadomosci || getStatusText(message.status) }}
                </span>
              </div>
            </div>

            <div class="header-row">
              <div class="header-field">
                <label>Podmiot:</label>
                <span class="value">{{ message.podmiot || message.relatedEntityName || 'Brak powiązania' }}</span>
              </div>
              <div class="header-field">
                <label>Temat:</label>
                <span class="value"><strong>{{ message.subject }}</strong></span>
              </div>
            </div>
          </div>
        </p-card>

        <p-divider></p-divider>

        <!-- Sender Information -->
        <p-card class="info-card">
          <h3>Nadawca</h3>
          <div class="info-grid">
            <div class="info-field">
              <label>Imię i nazwisko:</label>
              <span>{{ message.sender.fullName }}</span>
            </div>
            <div class="info-field">
              <label>Email:</label>
              <span>{{ message.sender.email }}</span>
            </div>
            <div class="info-field">
              <label>Data wysłania:</label>
              <span>{{ formatDate(message.sentAt) }}</span>
            </div>
          </div>
        </p-card>

        <!-- Recipient Information -->
        <p-card class="info-card">
          <h3>Odbiorca</h3>
          <div class="info-grid">
            <div class="info-field">
              <label>Imię i nazwisko:</label>
              <span>{{ message.recipient.fullName }}</span>
            </div>
            <div class="info-field">
              <label>Email:</label>
              <span>{{ message.recipient.email }}</span>
            </div>
            <div class="info-field">
              <label>Data odczytania:</label>
              <span>{{ formatDate(message.readAt) }}</span>
            </div>
          </div>
        </p-card>

        <!-- Message from User -->
        <p-card *ngIf="message.wiadomoscUzytkownika || message.body" class="content-card">
          <h3>Wiadomość użytkownika</h3>
          <div class="message-body">
            {{ message.wiadomoscUzytkownika || message.body }}
          </div>
          <div *ngIf="message.dataPrzeslaniaPodmiotu" class="message-meta">
            <small>Data przesłania: {{ formatDate(message.dataPrzeslaniaPodmiotu) }}</small>
          </div>
        </p-card>

        <!-- Message from UKNF Worker -->
        <p-card *ngIf="message.wiadomoscPracownikaUKNF" class="content-card uknf-message">
          <h3>Odpowiedź UKNF</h3>
          <div class="message-body">
            {{ message.wiadomoscPracownikaUKNF }}
          </div>
          <div class="message-meta">
            <div *ngIf="message.pracownikUKNF">
              <small><strong>Pracownik:</strong> {{ message.pracownikUKNF }}</small>
            </div>
            <div *ngIf="message.dataPrzeslaniaUKNF">
              <small><strong>Data:</strong> {{ formatDate(message.dataPrzeslaniaUKNF) }}</small>
            </div>
          </div>
        </p-card>

        <!-- Attachments -->
        <p-card *ngIf="message.attachments && message.attachments.length > 0" class="attachments-card">
          <h3>Załączniki ({{ message.attachments.length }})</h3>
          <div class="attachments-list">
            <div *ngFor="let attachment of message.attachments" class="attachment-item">
              <div class="attachment-info">
                <i class="pi pi-file" [class]="getFileIcon(attachment.contentType)"></i>
                <div class="attachment-details">
                  <div class="attachment-name">{{ attachment.fileName }}</div>
                  <div class="attachment-meta">
                    <span>{{ formatFileSize(attachment.fileSize) }}</span>
                    <span class="separator">•</span>
                    <span>{{ formatDate(attachment.uploadedAt) }}</span>
                  </div>
                </div>
              </div>
              <button 
                pButton 
                type="button" 
                label="Pobierz"
                icon="pi pi-download"
                class="p-button-sm p-button-outlined"
                (click)="downloadAttachment(attachment)"
              ></button>
            </div>
          </div>
        </p-card>

        <!-- Action Buttons -->
        <div class="action-buttons">
          <button 
            pButton 
            type="button" 
            label="Odpowiedz" 
            icon="pi pi-reply"
            class="p-button-primary"
          ></button>
          <button 
            pButton 
            type="button" 
            label="Drukuj" 
            icon="pi pi-print"
            class="p-button-outlined"
          ></button>
          <button 
            pButton 
            type="button" 
            label="Powrót" 
            icon="pi pi-arrow-left"
            class="p-button-outlined"
            (click)="goBack()"
          ></button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .message-details-page {
      padding: 1.5rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }

    .page-header h1 {
      margin: 0;
      font-size: 1.75rem;
      font-weight: 600;
      color: var(--primary-color);
    }

    .loading-container,
    .error-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3rem;
      gap: 1rem;
      text-align: center;
    }

    .message-details-container {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .message-card,
    .info-card,
    .content-card,
    .attachments-card {
      margin-bottom: 0;
    }

    .message-header {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .header-row {
      display: flex;
      gap: 2rem;
      flex-wrap: wrap;
    }

    .header-field {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
      min-width: 200px;
      flex: 1;
    }

    .header-field label {
      font-size: 0.875rem;
      color: var(--text-color-secondary);
      font-weight: 500;
    }

    .header-field .value {
      font-size: 1rem;
      color: var(--text-color);
    }

    .status-badge {
      display: inline-block;
      padding: 0.25rem 0.75rem;
      border-radius: 4px;
      font-weight: 500;
      font-size: 0.875rem;
    }

    .status-sent {
      background-color: #e3f2fd;
      color: #1976d2;
    }

    .status-pending {
      background-color: #fff3e0;
      color: #f57c00;
    }

    .status-answered {
      background-color: #e8f5e9;
      color: #388e3c;
    }

    .status-closed {
      background-color: #f5f5f5;
      color: #616161;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1rem;
    }

    .info-field {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .info-field label {
      font-size: 0.875rem;
      color: var(--text-color-secondary);
      font-weight: 500;
    }

    .info-field span {
      font-size: 1rem;
      color: var(--text-color);
    }

    .content-card h3 {
      margin-top: 0;
      margin-bottom: 1rem;
      font-size: 1.25rem;
      color: var(--primary-color);
    }

    .message-body {
      white-space: pre-wrap;
      line-height: 1.6;
      padding: 1rem;
      background-color: var(--surface-50);
      border-radius: 4px;
      border-left: 4px solid var(--primary-color);
    }

    .uknf-message .message-body {
      border-left-color: var(--green-500);
      background-color: var(--green-50);
    }

    .message-meta {
      margin-top: 0.75rem;
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .message-meta small {
      color: var(--text-color-secondary);
    }

    .info-text {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      color: var(--text-color-secondary);
      margin: 0;
    }

    .attachments-list {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .attachment-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem;
      border: 1px solid var(--surface-border);
      border-radius: 4px;
      background-color: var(--surface-0);
      transition: background-color 0.2s;
    }

    .attachment-item:hover {
      background-color: var(--surface-50);
    }

    .attachment-info {
      display: flex;
      align-items: center;
      gap: 1rem;
      flex: 1;
    }

    .attachment-info > i {
      font-size: 2rem;
      color: var(--primary-color);
    }

    .attachment-info > i.pi-file-pdf {
      color: #d32f2f;
    }

    .attachment-info > i.pi-file-excel {
      color: #388e3c;
    }

    .attachment-info > i.pi-file-word {
      color: #1976d2;
    }

    .attachment-details {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .attachment-name {
      font-weight: 500;
      color: var(--text-color);
    }

    .attachment-meta {
      font-size: 0.875rem;
      color: var(--text-color-secondary);
      display: flex;
      gap: 0.5rem;
      align-items: center;
    }

    .attachment-meta .separator {
      color: var(--surface-border);
    }

    .action-buttons {
      display: flex;
      gap: 1rem;
      padding-top: 1rem;
      flex-wrap: wrap;
    }

    @media (max-width: 768px) {
      .message-details-page {
        padding: 1rem;
      }

      .header-row {
        flex-direction: column;
        gap: 1rem;
      }

      .info-grid {
        grid-template-columns: 1fr;
      }

      .action-buttons {
        flex-direction: column;
      }

      .action-buttons button {
        width: 100%;
      }
    }
  `]
})
export class MessageDetailsComponent implements OnInit {
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  message: Message | null = null;
  loading = false;
  error: string | null = null;

  breadcrumbItems: MenuItem[] = [
    { label: 'Pulpit użytkownika', routerLink: '/dashboard' },
    { label: 'Wiadomości', routerLink: '/wiadomosci' },
    { label: 'Szczegóły wiadomości' }
  ];

  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadMessage(+id);
      }
    });
  }

  loadMessage(id?: number): void {
    const messageId = id || this.message?.id;
    if (!messageId) {
      this.error = 'Brak identyfikatora wiadomości';
      return;
    }

    this.loading = true;
    this.error = null;

    this.messageService.getMessageById(messageId).subscribe({
      next: (response: any) => {
        console.log('Message loaded:', response);
        
        // Handle wrapped response with data array
        if (response.data && Array.isArray(response.data)) {
          this.message = response.data[0] || null;
          if (!this.message) {
            this.error = 'Nie znaleziono wiadomości';
          }
        } else {
          // Handle direct message object
          this.message = response;
        }
        
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading message:', error);
        this.error = 'Nie udało się załadować wiadomości. Spróbuj ponownie.';
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/wiadomosci']);
  }

  formatDate(date: string | null | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleString('pl-PL', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusText(status: number): string {
    switch(status) {
      case 0: return 'Wersja robocza';
      case 1: return 'Wysłana';
      case 2: return 'Odczytana';
      case 3: return 'Odpowiedziano';
      case 4: return 'Zamknięta';
      case 5: return 'Anulowana';
      default: return 'Nieznany';
    }
  }

  getStatusClass(status: string | undefined): string {
    if (!status) return '';
    const statusLower = status.toLowerCase();
    if (statusLower.includes('oczekuje')) return 'status-pending';
    if (statusLower.includes('odpowiedziano')) return 'status-answered';
    if (statusLower.includes('zamknięta')) return 'status-closed';
    if (statusLower.includes('wysłana')) return 'status-sent';
    return '';
  }

  getFileIcon(contentType: string): string {
    if (contentType.includes('pdf')) return 'pi-file-pdf';
    if (contentType.includes('excel') || contentType.includes('spreadsheet')) return 'pi-file-excel';
    if (contentType.includes('word') || contentType.includes('document')) return 'pi-file-word';
    if (contentType.includes('image')) return 'pi-image';
    if (contentType.includes('zip') || contentType.includes('compressed')) return 'pi-file';
    return 'pi-file';
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  downloadAttachment(attachment: MessageAttachment): void {
    if (!this.message) return;
    
    console.log('Downloading attachment:', attachment);
    this.messageService.downloadAttachment(this.message.id, attachment.id).subscribe({
      next: (blob) => {
        // Create a temporary URL for the blob
        const url = window.URL.createObjectURL(blob);
        
        // Create a temporary anchor element to trigger download
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
}
