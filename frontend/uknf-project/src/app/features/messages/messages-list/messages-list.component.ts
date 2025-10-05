import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { CheckboxModule } from 'primeng/checkbox';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { MessageService, Message, MessageFilters } from '../../../services/message.service';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-messages-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    DatePickerModule,
    CheckboxModule,
    BreadcrumbModule,
    DialogModule,
    TextareaModule
  ],
  templateUrl: './messages-list.component.html',
  styleUrls: ['./messages-list.component.css']
})
export class MessagesListComponent implements OnInit {
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [
    { label: 'Pulpit użytkownika', routerLink: '/dashboard' },
    { label: 'Wnioski o dostęp', routerLink: '/wnioski' },
    { label: 'Biblioteka - repozytorium plików', routerLink: '/biblioteka' },
    { label: 'Wiadomości' }
  ];
  
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  messages: Message[] = [];
  totalRecords = 0;
  loading = false;
  
  // Pagination
  page = 1;
  pageSize = 10;
  pageSizeOptions = [10, 25, 50, 100];

  // Filters
  showFilters = false;
  filters: MessageFilters = {};
  
  // Filter options
  statusOptions = [
    { label: 'Wszystkie', value: '' },
    { label: 'Wysłana', value: 'Wysłana' },
    { label: 'Oczekuje na odpowiedź UKNF', value: 'Oczekuje na odpowiedź UKNF' },
    { label: 'Odpowiedziano', value: 'Odpowiedziano' },
    { label: 'Zamknięta', value: 'Zamknięta' }
  ];
  
  priorityOptions = [
    { label: 'Wszystkie', value: '' },
    { label: 'Niski', value: 'Niski' },
    { label: 'Średni', value: 'Średni' },
    { label: 'Wysoki', value: 'Wysoki' }
  ];

  // Selected message for details modal
  selectedMessage: Message | null = null;
  showDetailsDialog = false;

  ngOnInit(): void {
    // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError in zoneless mode
    setTimeout(() => {
      this.loadMessages();
    }, 0);
  }

  loadMessages(): void {
    this.loading = true;
    console.log('Loading messages with params:', { page: this.page, pageSize: this.pageSize, filters: this.filters });
    
    this.messageService.getMessages(this.page, this.pageSize, this.filters).subscribe({
      next: (response) => {
        console.log('Messages loaded successfully:', response);
        this.messages = response.data || [];
        this.totalRecords = response.pagination?.totalCount || 0;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading messages:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url
        });
        this.messages = [];
        this.totalRecords = 0;
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onPageChange(event: any): void {
    this.page = event.page + 1;
    this.pageSize = event.rows;
    this.loadMessages();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  applyFilters(): void {
    this.page = 1;
    this.loadMessages();
  }

  clearFilters(): void {
    this.filters = {};
    this.page = 1;
    this.loadMessages();
  }

  viewMessageDetails(message: Message): void {
    // Create a copy of the message to avoid mutating the original
    this.selectedMessage = { ...message };
    
    // Set default priority if not set
    if (!this.selectedMessage.priorytet) {
      this.selectedMessage.priorytet = 'Średni';
    }
    
    this.showDetailsDialog = true;
  }

  closeDetailsDialog(): void {
    this.showDetailsDialog = false;
    this.selectedMessage = null;
  }
  
  saveMessage(): void {
    if (this.selectedMessage) {
      console.log('Saving message:', this.selectedMessage);
      // TODO: Call API to save message
      this.messageService.updateMessage(this.selectedMessage.id, this.selectedMessage).subscribe({
        next: (updatedMessage) => {
          console.log('Message updated successfully:', updatedMessage);
          // Update the message in the list
          const index = this.messages.findIndex(m => m.id === updatedMessage.id);
          if (index !== -1) {
            this.messages[index] = updatedMessage;
          }
          this.closeDetailsDialog();
        },
        error: (error) => {
          console.error('Error updating message:', error);
        }
      });
    }
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

  getPriorityClass(priority: string | undefined): string {
    switch(priority?.toLowerCase()) {
      case 'wysoki': return 'priority-high';
      case 'średni': return 'priority-medium';
      case 'niski': return 'priority-low';
      default: return '';
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
}
