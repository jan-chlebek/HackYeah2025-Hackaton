import { Component, OnInit, inject, ChangeDetectorRef, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
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
import { MessageService, Message, MessageFilters, MessageAttachment } from '../../../services/message.service';
import { AuthService } from '../../../services/auth.service';
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
  private authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);
  private platformId = inject(PLATFORM_ID);
  // Flag to prevent PrimeNG table/theme logic from executing during SSR (causing isNaN errors)
  public readonly isBrowser = isPlatformBrowser(this.platformId);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [];
  
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  messages: Message[] = [];
  totalRecords = 0;
  loading = false;
  errorMessage: string | null = null;
  
  // Pagination
  page = 1;
  pageSize = 10;
  pageSizeOptions = [10, 25, 50, 100];

  // Sorting
  sortField: string | null = null;
  sortOrder: 1 | -1 | 0 = 0; // PrimeNG uses 1 (asc) / -1 (desc)

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
    // Build breadcrumb based on permissions
    const items: MenuItem[] = [];
    
    if (this.authService.hasElevatedPermissions()) {
      items.push({ label: 'Wnioski o dostęp', routerLink: '/wnioski' });
    }
    
    items.push({ label: 'Biblioteka - repozytorium plików', routerLink: '/biblioteka' });
    items.push({ label: 'Wiadomości' });
    
    this.breadcrumbItems = items;
    
    // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError in zoneless mode
    setTimeout(() => {
      this.loadMessages();
    }, 0);
  }

  loadMessages(): void {
    this.loading = true;
    console.log('Loading messages with params:', { page: this.page, pageSize: this.pageSize, filters: this.filters });
    
    this.errorMessage = null;
    this.messageService.getMessages(
      this.page,
      this.pageSize,
      this.filters,
      this.sortField || undefined,
      this.sortOrder === 0 ? undefined : (this.sortOrder === 1 ? 'asc' : 'desc')
    ).subscribe({
      next: (response) => {
        console.log('Messages loaded successfully:', response);
        let data = response.data || [];
        this.totalRecords = response.pagination?.totalCount || 0;

        // Client-side sorting fallback if backend ignores sort parameters
        if (this.sortField && this.sortOrder !== 0) {
          const before = JSON.stringify(data.map(d => (d as any)[this.sortField!]).slice(0,5));
          data = [...data].sort((a: any, b: any) => {
            const av = (a as any)[this.sortField!];
            const bv = (b as any)[this.sortField!];
            if (av == null && bv == null) return 0;
            if (av == null) return 1;
            if (bv == null) return -1;
            // Attempt date comparison
            const aDate = Date.parse(av);
            const bDate = Date.parse(bv);
            if (!isNaN(aDate) && !isNaN(bDate)) {
              return (aDate - bDate) * (this.sortOrder as number);
            }
            // Fallback string/number comparison
            if (typeof av === 'number' && typeof bv === 'number') {
              return (av - bv) * (this.sortOrder as number);
            }
            return av.toString().localeCompare(bv.toString(), 'pl', { numeric: true }) * (this.sortOrder as number);
          });
          const after = JSON.stringify(data.map(d => (d as any)[this.sortField!]).slice(0,5));
          console.log('[MessagesList] Applied client-side sort fallback', { field: this.sortField, order: this.sortOrder, before, after });
        }

        this.messages = data;
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
        if (error.status === 401) {
          this.errorMessage = 'Brak autoryzacji – zaloguj się ponownie.';
        } else if (error.status === 403) {
          this.errorMessage = 'Brak uprawnień do wyświetlenia wiadomości.';
        } else {
          this.errorMessage = 'Nie udało się załadować wiadomości. Spróbuj ponownie.';
        }
        this.cdr.markForCheck();
      }
    });
  }

  onPageChange(event: any): void {
    this.page = event.page + 1;
    this.pageSize = event.rows;
    this.loadMessages();
  }

  onSort(event: any): void {
    // event: { field, order }
    this.sortField = event.field;
    this.sortOrder = event.order;
    this.page = 1; // reset to first page on new sort
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
    // Fetch full message details including attachments
    this.messageService.getMessageById(message.id).subscribe({
      next: (response: any) => {
        console.log('Full message details loaded:', response);
        
        // Handle wrapped response with data array or direct message object
        if (response.data && Array.isArray(response.data)) {
          this.selectedMessage = response.data[0] || null;
        } else if (response.attachments) {
          // Direct message object with attachments
          this.selectedMessage = response;
        } else {
          // Fallback to original message
          this.selectedMessage = { ...message };
        }
        
        // Set default priority if not set
        if (this.selectedMessage && !this.selectedMessage.priorytet) {
          this.selectedMessage.priorytet = 'Średni';
        }
        
        this.showDetailsDialog = true;
      },
      error: (error) => {
        console.error('Error loading message details:', error);
        // Fallback to using the message from the list
        this.selectedMessage = { ...message };
        
        if (!this.selectedMessage.priorytet) {
          this.selectedMessage.priorytet = 'Średni';
        }
        
        this.showDetailsDialog = true;
      }
    });
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

  downloadMessageAttachment(attachment: MessageAttachment): void {
    if (!this.selectedMessage) return;
    
    console.log('Downloading attachment:', attachment);
    this.messageService.downloadAttachment(this.selectedMessage.id, attachment.id).subscribe({
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
