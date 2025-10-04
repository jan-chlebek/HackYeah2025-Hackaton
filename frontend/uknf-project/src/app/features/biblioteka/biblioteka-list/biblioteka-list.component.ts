import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

interface BibliotecFile {
  id: number;
  dataAktualizacji: string;
  nazwaPliku: string;
  okresSprawozdawczy: string;
  selected?: boolean;
}

type SortColumn = 'dataAktualizacji' | 'nazwaPliku' | 'okresSprawozdawczy';
type SortDirection = 'asc' | 'desc' | null;

@Component({
  selector: 'app-biblioteka-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './biblioteka-list.component.html',
  styleUrl: './biblioteka-list.component.css'
})
export class BibliotekaListComponent implements OnInit {
  // Search filter
  searchTerm: string = '';
  
  // Sort state
  sortColumn: SortColumn | null = null;
  sortDirection: SortDirection = null;
  
  // Filter columns
  dataAktualizacjiFilter: string = '';
  nazwaPlikiuFilter: string = '';
  okresSprawozdawczyFilter: string = '';
  
  // Pagination
  currentPage: number = 1;
  itemsPerPage: number = 10;
  totalItems: number = 0;
  
  // Data
  files: BibliotecFile[] = [
    { id: 1, dataAktualizacji: '2024-09-14', nazwaPliku: 'Plik_w_repozytorium_01.xlsx', okresSprawozdawczy: 'Kwartal', selected: false },
    { id: 2, dataAktualizacji: '2024-09-14', nazwaPliku: 'Plik_w_repozytorium_02.xlsx', okresSprawozdawczy: '', selected: false },
    { id: 3, dataAktualizacji: '2024-09-14', nazwaPliku: 'Plik_w_repozytorium_03.xlsx', okresSprawozdawczy: '', selected: false },
    { id: 4, dataAktualizacji: '2024-09-14', nazwaPliku: 'Plik_w_repozytorium_04.xlsx', okresSprawozdawczy: 'Rok', selected: false },
    { id: 5, dataAktualizacji: '2024-09-14', nazwaPliku: 'Plik_w_repozytorium_05.xlsx', okresSprawozdawczy: 'Kwartal', selected: false },
    { id: 6, dataAktualizacji: '2024-09-14', nazwaPliku: 'Plik_w_repozytorium_06.xlsx', okresSprawozdawczy: 'Kwartal', selected: false },
    { id: 7, dataAktualizacji: '2024-09-14', nazwaPliku: 'Plik_w_repozytorium_07.xlsx', okresSprawozdawczy: 'Kwartal', selected: false },
    { id: 8, dataAktualizacji: '2024-09-14', nazwaPliku: 'Plik_w_repozytorium_08.xlsx', okresSprawozdawczy: 'Rok', selected: false }
  ];
  
  filteredFiles: BibliotecFile[] = [];
  
  constructor(private router: Router) {}
  
  ngOnInit(): void {
    this.applyFilters();
  }
  
  applyFilters(): void {
    this.filteredFiles = this.files.filter(file => {
      const matchesSearch = !this.searchTerm || 
        file.nazwaPliku.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        file.okresSprawozdawczy.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      const matchesDataAktualizacji = !this.dataAktualizacjiFilter || 
        file.dataAktualizacji.includes(this.dataAktualizacjiFilter);
      
      const matchesNazwaPliku = !this.nazwaPlikiuFilter || 
        file.nazwaPliku.toLowerCase().includes(this.nazwaPlikiuFilter.toLowerCase());
      
      const matchesOkresSprawozdawczy = !this.okresSprawozdawczyFilter || 
        file.okresSprawozdawczy === this.okresSprawozdawczyFilter;
      
      return matchesSearch && matchesDataAktualizacji && matchesNazwaPliku && matchesOkresSprawozdawczy;
    });
    
    this.applySorting();
    this.totalItems = this.filteredFiles.length;
  }
  
  applySorting(): void {
    if (this.sortColumn && this.sortDirection) {
      this.filteredFiles.sort((a, b) => {
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
  
  getPaginatedFiles(): BibliotecFile[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredFiles.slice(startIndex, startIndex + this.itemsPerPage);
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
  
  toggleSelection(file: BibliotecFile): void {
    file.selected = !file.selected;
  }
  
  toggleAllSelection(): void {
    const allSelected = this.getPaginatedFiles().every(f => f.selected);
    this.getPaginatedFiles().forEach(f => f.selected = !allSelected);
  }
  
  getSelectedFiles(): BibliotecFile[] {
    return this.files.filter(f => f.selected);
  }
  
  // Modal state
  showAddModal: boolean = false;
  
  // Add file form
  addFileForm = {
    archiwalny: false,
    nazwaPliku: '',
    okresSprawozdawczy: '',
    dataAktualizacji: '',
    zalacznik: null as File | null,
    zalacznikName: '',
    adresaciPowiadomien: '',
    adresatGrupa1: '',
    adresatGrupa2: '',
    adresatGrupa3: ''
  };
  
  // Action handlers
  dodajFile(): void {
    this.showAddModal = true;
    this.resetAddFileForm();
  }
  
  closeAddModal(): void {
    this.showAddModal = false;
    this.resetAddFileForm();
  }
  
  resetAddFileForm(): void {
    this.addFileForm = {
      archiwalny: false,
      nazwaPliku: '',
      okresSprawozdawczy: '',
      dataAktualizacji: '',
      zalacznik: null,
      zalacznikName: '',
      adresaciPowiadomien: '',
      adresatGrupa1: '',
      adresatGrupa2: '',
      adresatGrupa3: ''
    };
  }
  
  onFileSelect(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.addFileForm.zalacznik = file;
      this.addFileForm.zalacznikName = file.name;
    }
  }
  
  przegladajZalacznik(): void {
    const fileInput = document.getElementById('fileUpload') as HTMLInputElement;
    fileInput?.click();
  }
  
  usunZalacznikModal(): void {
    this.addFileForm.zalacznik = null;
    this.addFileForm.zalacznikName = '';
  }
  
  historiaZalacznik(): void {
    console.log('Historia załącznika');
  }
  
  pobierzZalacznikModal(): void {
    if (this.addFileForm.zalacznik) {
      console.log('Pobierz załącznik:', this.addFileForm.zalacznikName);
    }
  }
  
  podgladZalacznik(): void {
    if (this.addFileForm.zalacznik) {
      console.log('Podgląd załącznika:', this.addFileForm.zalacznikName);
    }
  }
  
  wybierzAction(): void {
    console.log('Wybierz');
  }
  
  utworzIDodaj(): void {
    console.log('Utwórz i dodaj');
  }
  
  podgladAction(): void {
    console.log('Podgląd');
  }
  
  edycjaAction(): void {
    console.log('Edycja');
  }
  
  usunAction(): void {
    console.log('Usuń');
  }
  
  anulujAction(): void {
    this.closeAddModal();
  }
  
  zapiszIWyslij(): void {
    console.log('Zapisz i wyślij', this.addFileForm);
    // Add validation and save logic here
    this.closeAddModal();
  }
  
  zapiszIZamknij(): void {
    console.log('Zapisz i zamknij', this.addFileForm);
    // Add validation and save logic here
    this.closeAddModal();
  }
  
  modyfikujFile(): void {
    const selected = this.getSelectedFiles();
    if (selected.length === 0) {
      alert('Proszę wybrać plik do modyfikacji');
      return;
    }
    if (selected.length > 1) {
      alert('Proszę wybrać tylko jeden plik do modyfikacji');
      return;
    }
    console.log('Modyfikuj plik:', selected[0]);
    this.router.navigate(['/biblioteka', selected[0].id]);
  }
  
  podgladFile(): void {
    const selected = this.getSelectedFiles();
    if (selected.length === 0) {
      alert('Proszę wybrać plik do podglądu');
      return;
    }
    if (selected.length > 1) {
      alert('Proszę wybrać tylko jeden plik do podglądu');
      return;
    }
    console.log('Podgląd pliku:', selected[0]);
    this.router.navigate(['/biblioteka', selected[0].id]);
  }
  
  usunFile(): void {
    const selected = this.getSelectedFiles();
    if (selected.length === 0) {
      alert('Proszę wybrać pliki do usunięcia');
      return;
    }
    if (confirm(`Czy na pewno chcesz usunąć ${selected.length} plik(ów)?`)) {
      selected.forEach(file => {
        const index = this.files.indexOf(file);
        if (index > -1) {
          this.files.splice(index, 1);
        }
      });
      this.applyFilters();
      console.log('Usunięto pliki:', selected);
    }
  }
  
  historiaFile(): void {
    const selected = this.getSelectedFiles();
    if (selected.length === 0) {
      alert('Proszę wybrać plik do wyświetlenia historii');
      return;
    }
    if (selected.length > 1) {
      alert('Proszę wybrać tylko jeden plik do wyświetlenia historii');
      return;
    }
    console.log('Historia pliku:', selected[0]);
  }
  
  eksportujFiles(): void {
    const selected = this.getSelectedFiles();
    if (selected.length === 0) {
      alert('Proszę wybrać pliki do eksportu');
      return;
    }
    console.log('Eksportuj pliki:', selected);
    // Implement export logic
  }
}
