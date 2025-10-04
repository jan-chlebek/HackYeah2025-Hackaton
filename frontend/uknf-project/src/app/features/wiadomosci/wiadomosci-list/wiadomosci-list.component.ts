import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

interface Message {
  id: number;
  identyfikator: string;
  sygnaturaSprawy: string;
  podmiot: string;
  statusWiadomosci: string;
  priorytet: string;
  dataPrzeslaniaPodmiotu: string;
  uzytkownik: string;
  wiadomoscUzytkownika: string;
  dataPrzeslaniaUKNF: string;
  pracownikUKNF: string;
  wiadomoscPracownikaUKNF: string;
}

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
  // Search filter
  searchTerm: string = '';
  
  // Sort state
  sortColumn: SortColumn | null = null;
  sortDirection: SortDirection = null;
  
  // Column filters
  identyfikatorFilter: string = '';
  sygnaturaSprawyFilter: string = '';
  podmiotFilter: string = '';
  statusFilter: string = '';
  priorytetFilter: string = '';
  dataPrzeslaniaPodmiotuFilter: string = '';
  uzytkownikFilter: string = '';
  wiadomoscUzytkownikaFilter: string = '';
  dataPrzeslaniaUKNFFilter: string = '';
  pracownikUKNFFilter: string = '';
  wiadomoscPracownikaUKNFFilter: string = '';
  
  // Pagination
  currentPage: number = 1;
  itemsPerPage: number = 10;
  totalItems: number = 0;
  
  // Data
  messages: Message[] = [
    {
      id: 1,
      identyfikator: '2024/System14/5',
      sygnaturaSprawy: '001/2025',
      podmiot: 'Testowy podmiot',
      statusWiadomosci: 'Oczekuje na odpowiedź UKNF',
      priorytet: 'Średni',
      dataPrzeslaniaPodmiotu: '2025-06-14 12:43:12',
      uzytkownik: 'Jan Kowalski',
      wiadomoscUzytkownika: 'Testowa wiadomość...',
      dataPrzeslaniaUKNF: '',
      pracownikUKNF: '',
      wiadomoscPracownikaUKNF: ''
    },
    {
      id: 2,
      identyfikator: '2024/System14/5',
      sygnaturaSprawy: '001/2025',
      podmiot: 'Testowy podmiot',
      statusWiadomosci: 'Oczekuje na odpowiedź UKNF',
      priorytet: 'Średni',
      dataPrzeslaniaPodmiotu: '2025-06-14 12:43:12',
      uzytkownik: 'Jan Kowalski',
      wiadomoscUzytkownika: 'Testowa wiadomość...',
      dataPrzeslaniaUKNF: '',
      pracownikUKNF: '',
      wiadomoscPracownikaUKNF: ''
    },
    {
      id: 3,
      identyfikator: '2024/System14/5',
      sygnaturaSprawy: '001/2025',
      podmiot: 'Testowy podmiot',
      statusWiadomosci: 'Oczekuje na odpowiedź UKNF',
      priorytet: 'Średni',
      dataPrzeslaniaPodmiotu: '2025-06-14 12:43:12',
      uzytkownik: 'Jan Kowalski',
      wiadomoscUzytkownika: 'Testowa wiadomość...',
      dataPrzeslaniaUKNF: '',
      pracownikUKNF: '',
      wiadomoscPracownikaUKNF: ''
    },
    {
      id: 4,
      identyfikator: '2024/System14/5',
      sygnaturaSprawy: '001/2025',
      podmiot: 'Testowy podmiot',
      statusWiadomosci: 'Oczekuje na odpowiedź UKNF',
      priorytet: 'Średni',
      dataPrzeslaniaPodmiotu: '2025-06-14 12:43:12',
      uzytkownik: 'Jan Kowalski',
      wiadomoscUzytkownika: 'Testowa wiadomość...',
      dataPrzeslaniaUKNF: '',
      pracownikUKNF: '',
      wiadomoscPracownikaUKNF: ''
    },
    {
      id: 5,
      identyfikator: '2024/System14/5',
      sygnaturaSprawy: '001/2025',
      podmiot: 'Testowy podmiot',
      statusWiadomosci: 'Oczekuje na odpowiedź UKNF',
      priorytet: 'Średni',
      dataPrzeslaniaPodmiotu: '2025-06-14 12:43:12',
      uzytkownik: 'Jan Kowalski',
      wiadomoscUzytkownika: 'Testowa wiadomość...',
      dataPrzeslaniaUKNF: '',
      pracownikUKNF: '',
      wiadomoscPracownikaUKNF: ''
    },
    {
      id: 6,
      identyfikator: '2024/System14/5',
      sygnaturaSprawy: '001/2025',
      podmiot: 'Testowy podmiot',
      statusWiadomosci: 'Oczekuje na odpowiedź UKNF',
      priorytet: 'Średni',
      dataPrzeslaniaPodmiotu: '2025-06-14 12:43:12',
      uzytkownik: 'Jan Kowalski',
      wiadomoscUzytkownika: 'Testowa wiadomość...',
      dataPrzeslaniaUKNF: '',
      pracownikUKNF: '',
      wiadomoscPracownikaUKNF: ''
    },
    {
      id: 7,
      identyfikator: '2024/System14/5',
      sygnaturaSprawy: '001/2025',
      podmiot: 'Testowy podmiot',
      statusWiadomosci: 'Oczekuje na odpowiedź UKNF',
      priorytet: 'Średni',
      dataPrzeslaniaPodmiotu: '2025-06-14 12:43:12',
      uzytkownik: 'Jan Kowalski',
      wiadomoscUzytkownika: 'Testowa wiadomość...',
      dataPrzeslaniaUKNF: '',
      pracownikUKNF: '',
      wiadomoscPracownikaUKNF: ''
    },
    {
      id: 8,
      identyfikator: '2024/System14/5',
      sygnaturaSprawy: '001/2025',
      podmiot: 'Testowy podmiot',
      statusWiadomosci: 'Oczekuje na odpowiedź UKNF',
      priorytet: 'Średni',
      dataPrzeslaniaPodmiotu: '2025-06-14 12:43:12',
      uzytkownik: 'Jan Kowalski',
      wiadomoscUzytkownika: 'Testowa wiadomość...',
      dataPrzeslaniaUKNF: '',
      pracownikUKNF: '',
      wiadomoscPracownikaUKNF: ''
    }
  ];
  
  filteredMessages: Message[] = [];
  
  constructor(private router: Router) {}
  
  ngOnInit(): void {
    this.applyFilters();
  }
  
  applyFilters(): void {
    this.filteredMessages = this.messages.filter(msg => {
      const matchesSearch = !this.searchTerm || 
        Object.values(msg).some(val => 
          String(val).toLowerCase().includes(this.searchTerm.toLowerCase())
        );
      
      return matchesSearch &&
        (!this.identyfikatorFilter || msg.identyfikator.includes(this.identyfikatorFilter)) &&
        (!this.sygnaturaSprawyFilter || msg.sygnaturaSprawy.includes(this.sygnaturaSprawyFilter)) &&
        (!this.podmiotFilter || msg.podmiot.toLowerCase().includes(this.podmiotFilter.toLowerCase())) &&
        (!this.statusFilter || msg.statusWiadomosci === this.statusFilter) &&
        (!this.priorytetFilter || msg.priorytet === this.priorytetFilter) &&
        (!this.uzytkownikFilter || msg.uzytkownik.toLowerCase().includes(this.uzytkownikFilter.toLowerCase()));
    });
    
    this.applySorting();
    this.totalItems = this.filteredMessages.length;
  }
  
  applySorting(): void {
    if (this.sortColumn && this.sortDirection) {
      this.filteredMessages.sort((a, b) => {
        const aValue = a[this.sortColumn!];
        const bValue = b[this.sortColumn!];
        
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
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredMessages.slice(startIndex, startIndex + this.itemsPerPage);
  }
  
  getTotalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }
  
  changePage(page: number): void {
    if (page >= 1 && page <= this.getTotalPages()) {
      this.currentPage = page;
    }
  }
  
  onItemsPerPageChange(): void {
    this.currentPage = 1;
    this.applyFilters();
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
}
