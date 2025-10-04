import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService, Message, MessageResponse } from '../../../services/message.service';

type SortColumn = 'identyfikator' | 'sygnaturaSprawy' | 'podmiot' | 'statusWiadomosci' | 'priorytet' | 
  'dataPrzeslaniaPodmiotu' | 'uzytkownik' | 'wiadomoscUzytkownika' | 'dataPrzeslaniaUKNF' | 
  'pracownikUKNF' | 'wiadomoscPracownikaUKNF';
type SortDirection = 'asc' | 'desc' | null;

@Component({
  selector: 'app-wiadomosci-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './wiadomosci-list.component.html',
  styleUrl: './wiadomosci-list.component.css'
})
export class WiadomosciListComponent implements OnInit {
  private messageService = inject(MessageService);
  private router = inject(Router);
  
  // Search filter
  searchTerm: string = '';
  
  // Sort state
  sortColumn: SortColumn | null = null;
  sortDirection: SortDirection = null;
  
  // Filter visibility
  showFilters: boolean = false;
  
  // Column filters
  identyfikatorFilter: string = '';
  sygnaturaSprawyFilter: string = '';
  podmiotFilter: string = '';
  statusFilter: string = '';
  priorytetFilter: string = '';
  dataPrzeslaniaPodmiotuFromFilter: string = '';
  dataPrzeslaniaPodmiotuToFilter: string = '';
  dataPrzeslaniaUKNFFromFilter: string = '';
  dataPrzeslaniaUKNFToFilter: string = '';
  pracownikUKNFFilter: string = '';
  
  // Checkbox filters
  mojePodmiotyFilter: boolean = false;
  wymaganaOdpowiedzUKNFFilter: boolean = false;
  
  // Available options
  priorytetOptions = ['Wszystkie', 'Wysoki', 'Średni', 'Niski'];
  statusOptions = ['Wszystkie', 'Oczekuje na odpowiedź UKNF', 'Oczekuje na odpowiedź podmiotu', 'Odpowiedziano', 'Zamknięte'];
  pracownikOptions = ['Wszystkie'];
  
  // Pagination
  currentPage: number = 1;
  itemsPerPage: number = 10;
  totalItems: number = 0;
  totalPages: number = 1;
  
  // Data
  messages: Message[] = [];
  filteredMessages: Message[] = [];
  isLoading: boolean = false;
  errorMessage: string = '';
  
  ngOnInit(): void {
    this.loadMessages();
  }
  
  loadMessages(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.messageService.getMessages(this.currentPage, this.itemsPerPage).subscribe({
      next: (response: MessageResponse) => {
        this.messages = response.data;
        this.totalItems = response.pagination.totalCount;
        this.totalPages = response.pagination.totalPages;
        this.currentPage = response.pagination.page;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading messages:', error);
        this.errorMessage = 'Failed to load messages. Please try again.';
        this.isLoading = false;
      }
    });
  }
  
  applyFilters(): void {
    this.filteredMessages = this.messages.filter(msg => {
      const matchesSearch = !this.searchTerm || 
        Object.values(msg).some(val => 
          String(val).toLowerCase().includes(this.searchTerm.toLowerCase())
        );
      
      const matchesIdentyfikator = !this.identyfikatorFilter || 
        (msg.identyfikator && msg.identyfikator.toLowerCase().includes(this.identyfikatorFilter.toLowerCase()));
      
      const matchesSygnatura = !this.sygnaturaSprawyFilter || 
        (msg.sygnaturaSprawy && msg.sygnaturaSprawy.toLowerCase().includes(this.sygnaturaSprawyFilter.toLowerCase()));
      
      const matchesPodmiot = !this.podmiotFilter || 
        (msg.podmiot && msg.podmiot.toLowerCase().includes(this.podmiotFilter.toLowerCase()));
      
      const matchesStatus = !this.statusFilter || this.statusFilter === 'Wszystkie' || 
        msg.statusWiadomosci === this.statusFilter;
      
      const matchesPriorytet = !this.priorytetFilter || this.priorytetFilter === 'Wszystkie' || 
        msg.priorytet === this.priorytetFilter;
      
      const matchesPracownik = !this.pracownikUKNFFilter || this.pracownikUKNFFilter === 'Wszystkie' || 
        (msg.pracownikUKNF && msg.pracownikUKNF.toLowerCase().includes(this.pracownikUKNFFilter.toLowerCase()));
      
      // Date filters
      const matchesDatePodmiotuFrom = !this.dataPrzeslaniaPodmiotuFromFilter || 
        (msg.dataPrzeslaniaPodmiotu && new Date(msg.dataPrzeslaniaPodmiotu) >= new Date(this.dataPrzeslaniaPodmiotuFromFilter));
      
      const matchesDatePodmiotuTo = !this.dataPrzeslaniaPodmiotuToFilter || 
        (msg.dataPrzeslaniaPodmiotu && new Date(msg.dataPrzeslaniaPodmiotu) <= new Date(this.dataPrzeslaniaPodmiotuToFilter));
      
      const matchesDateUKNFFrom = !this.dataPrzeslaniaUKNFFromFilter || 
        (msg.dataPrzeslaniaUKNF && new Date(msg.dataPrzeslaniaUKNF) >= new Date(this.dataPrzeslaniaUKNFFromFilter));
      
      const matchesDateUKNFTo = !this.dataPrzeslaniaUKNFToFilter || 
        (msg.dataPrzeslaniaUKNF && new Date(msg.dataPrzeslaniaUKNF) <= new Date(this.dataPrzeslaniaUKNFToFilter));
      
      return matchesSearch &&
        matchesIdentyfikator &&
        matchesSygnatura &&
        matchesPodmiot &&
        matchesStatus &&
        matchesPriorytet &&
        matchesPracownik &&
        matchesDatePodmiotuFrom &&
        matchesDatePodmiotuTo &&
        matchesDateUKNFFrom &&
        matchesDateUKNFTo;
    });
    
    this.applySorting();
  }
  
  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }
  
  clearFilters(): void {
    this.identyfikatorFilter = '';
    this.sygnaturaSprawyFilter = '';
    this.podmiotFilter = '';
    this.statusFilter = '';
    this.priorytetFilter = '';
    this.dataPrzeslaniaPodmiotuFromFilter = '';
    this.dataPrzeslaniaPodmiotuToFilter = '';
    this.dataPrzeslaniaUKNFFromFilter = '';
    this.dataPrzeslaniaUKNFToFilter = '';
    this.pracownikUKNFFilter = '';
    this.mojePodmiotyFilter = false;
    this.wymaganaOdpowiedzUKNFFilter = false;
    this.applyFilters();
  }
  
  applySorting(): void {
    if (this.sortColumn && this.sortDirection) {
      this.filteredMessages.sort((a, b) => {
        let aValue: any;
        let bValue: any;
        
        switch(this.sortColumn) {
          case 'identyfikator':
            aValue = a.identyfikator || '';
            bValue = b.identyfikator || '';
            break;
          case 'sygnaturaSprawy':
            aValue = a.sygnaturaSprawy || '';
            bValue = b.sygnaturaSprawy || '';
            break;
          case 'podmiot':
            aValue = a.podmiot || '';
            bValue = b.podmiot || '';
            break;
          case 'statusWiadomosci':
            aValue = a.statusWiadomosci || '';
            bValue = b.statusWiadomosci || '';
            break;
          case 'priorytet':
            aValue = a.priorytet || '';
            bValue = b.priorytet || '';
            break;
          case 'dataPrzeslaniaPodmiotu':
            aValue = a.dataPrzeslaniaPodmiotu ? new Date(a.dataPrzeslaniaPodmiotu).getTime() : 0;
            bValue = b.dataPrzeslaniaPodmiotu ? new Date(b.dataPrzeslaniaPodmiotu).getTime() : 0;
            break;
          case 'uzytkownik':
            aValue = a.uzytkownik || '';
            bValue = b.uzytkownik || '';
            break;
          case 'wiadomoscUzytkownika':
            aValue = a.wiadomoscUzytkownika || '';
            bValue = b.wiadomoscUzytkownika || '';
            break;
          case 'dataPrzeslaniaUKNF':
            aValue = a.dataPrzeslaniaUKNF ? new Date(a.dataPrzeslaniaUKNF).getTime() : 0;
            bValue = b.dataPrzeslaniaUKNF ? new Date(b.dataPrzeslaniaUKNF).getTime() : 0;
            break;
          case 'pracownikUKNF':
            aValue = a.pracownikUKNF || '';
            bValue = b.pracownikUKNF || '';
            break;
          case 'wiadomoscPracownikaUKNF':
            aValue = a.wiadomoscPracownikaUKNF || '';
            bValue = b.wiadomoscPracownikaUKNF || '';
            break;
          default:
            return 0;
        }
        
        if (aValue < bValue) return this.sortDirection === 'asc' ? -1 : 1;
        if (aValue > bValue) return this.sortDirection === 'asc' ? 1 : -1;
        return 0;
      });
    }
  }
  
  sortBy(column: SortColumn): void {
    if (this.sortColumn === column) {
      if (this.sortDirection === 'asc') {
        this.sortDirection = 'desc';
      } else if (this.sortDirection === 'desc') {
        this.sortDirection = null;
        this.sortColumn = null;
      } else {
        this.sortDirection = 'asc';
      }
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.applyFilters();
  }
  
  getSortIcon(column: SortColumn): string {
    if (this.sortColumn !== column) return '↕';
    if (this.sortDirection === 'asc') return '↑';
    if (this.sortDirection === 'desc') return '↓';
    return '↕';
  }
  
  getPaginatedMessages(): Message[] {
    return this.filteredMessages;
  }
  
  getTotalPages(): number {
    return this.totalPages;
  }
  
  changePage(page: number): void {
    if (page >= 1 && page <= this.getTotalPages()) {
      this.currentPage = page;
      this.loadMessages();
    }
  }
  
  onItemsPerPageChange(): void {
    this.currentPage = 1;
    this.loadMessages();
  }
  
  getPageNumbers(): number[] {
    const totalPages = this.getTotalPages();
    const pages: number[] = [];
    const maxVisible = 5;
    
    if (totalPages <= maxVisible) {
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      const start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
      const end = Math.min(totalPages, start + maxVisible - 1);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
    }
    
    return pages;
  }
  
  getStartIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage + 1;
  }
  
  getEndIndex(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.totalItems);
  }
  
  viewMessageDetails(message: Message): void {
    this.router.navigate(['/wiadomosci', message.id]);
  }
  
  formatDate(dateString: string | null | undefined): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleString('pl-PL', { 
      year: 'numeric', 
      month: '2-digit', 
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
