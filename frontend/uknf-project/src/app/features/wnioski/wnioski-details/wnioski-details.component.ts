import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface Podmiot {
  nazwa: string;
  link: string;
  sprawy: string;
  ignarowaneZesde: string;
  administrator: string;
  status: string;
}

interface Zalacznik {
  nazwa: string;
  typ: string;
}

interface Wlasnosc {
  dataRozrzecenia: string;
  priorytet: string;
  dataPrzewiania1: string;
  dataPrzewiania2: string;
  dataOtczania1: string;
  dataOtczania2: string;
  nazwisko: string;
  akcesur: string;
  statusWlasnosciowy: string;
}

@Component({
  selector: 'app-wnioski-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './wnioski-details.component.html',
  styleUrl: './wnioski-details.component.css'
})
export class WnioskiDetailsComponent implements OnInit {
  wniosekId: string = '';
  
  // Form fields
  imie: string = 'Krzysztof';
  drugieImie: string = '';
  nazwisko: string = 'Kowalski';
  pesel: string = '11223344550';
  email: string = 'krzysztof@example.com';
  telefon: string = '+48123456789';
  
  // Uprawnienia
  uprawnieniaLiczebnosc: boolean = false;
  uprawnieniaWykluczaneiane: boolean = true;
  
  // Table filters
  nazwaPodmiotu: string = '';
  linkNazwaRejestrowana: string = '';
  sprawyFilter: string = '';
  ignarowaneZesdeFilter: string = '';
  administratorFilter: string = '';
  statusFilter: string = '';
  
  // Podmioty data
  podmioty: Podmiot[] = [
    { nazwa: 'Nazwa podmiotu - testowy podmiot', link: 'Jan Kowalski', sprawy: 'Tak', ignarowaneZesde: 'Tak', administrator: 'Tak', status: 'Zainteresowany' },
    { nazwa: 'Nazwa podmiotu - testowy podmiot', link: 'Jan Kowalski', sprawy: 'Tak', ignarowaneZesde: 'Tak', administrator: 'Tak', status: 'Zainteresowany' },
    { nazwa: 'Nazwa podmiotu - testowy podmiot', link: 'Jan Kowalski', sprawy: 'Tak', ignarowaneZesde: 'Tak', administrator: 'Tak', status: 'Zainteresowany' },
    { nazwa: 'Nazwa podmiotu - testowy podmiot', link: 'Jan Kowalski', sprawy: 'Tak', ignarowaneZesde: 'Tak', administrator: 'Tak', status: 'Zainteresowany' },
    { nazwa: 'Nazwa podmiotu - testowy podmiot', link: 'Jan Kowalski', sprawy: 'Tak', ignarowaneZesde: 'Tak', administrator: 'Tak', status: 'Zainteresowany' }
  ];
  
  filteredPodmioty: Podmiot[] = [];
  currentPage: number = 1;
  itemsPerPage: number = 10;
  totalItems: number = 0;
  
  // Załączniki
  zalaczniki: Zalacznik[] = [
    { nazwa: 'Zalacznik_umowa.pdf', typ: 'PDF' },
    { nazwa: 'Zalacznik_druga.pdf', typ: 'PDF' },
    { nazwa: 'Zalacznik_trzeci.txt', typ: 'TXT' }
  ];
  
  // Wnioski section
  wniosekDostepu: boolean = false;
  wniosekKorzystania: boolean = false;
  
  // Własności data
  wlasnosci: Wlasnosc[] = [
    { dataRozrzecenia: '2025-01-11 13:48:16', priorytet: 'Branze', dataPrzewiania1: '2025-01-02 18:48:25', dataPrzewiania2: '2025-01-02 17:34:25', dataOtczania1: '', dataOtczania2: '', nazwisko: 'Jan Nowak', akcesur: 'UKNF', statusWlasnosciowy: 'Oczekuje na odpowiedz' },
    { dataRozrzecenia: '2025-01-11 13:48:16', priorytet: 'Branze', dataPrzewiania1: '2025-01-02 18:48:25', dataPrzewiania2: '2025-01-02 17:34:25', dataOtczania1: '', dataOtczania2: '', nazwisko: 'Jan Nowak', akcesur: 'UKNF', statusWlasnosciowy: 'Oczekuje na odpowiedz' },
    { dataRozrzecenia: '2025-01-11 13:48:16', priorytet: 'Branze', dataPrzewiania1: '2025-01-02 18:48:25', dataPrzewiania2: '2025-01-02 17:34:25', dataOtczania1: '', dataOtczania2: '', nazwisko: 'Jan Nowak', akcesur: 'UKNF', statusWlasnosciowy: 'Oczekuje na odpowiedz' },
    { dataRozrzecenia: '2025-01-11 13:48:16', priorytet: 'Branze', dataPrzewiania1: '2025-01-02 18:48:25', dataPrzewiania2: '2025-01-02 17:34:25', dataOtczania1: '', dataOtczania2: '', nazwisko: 'Jan Nowak', akcesur: 'UKNF', statusWlasnosciowy: 'Oczekuje na odpowiedz' }
  ];
  
  wlasnosciCurrentPage: number = 1;
  wlasnosciItemsPerPage: number = 10;
  
  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    this.wniosekId = this.route.snapshot.paramMap.get('id') || '';
    this.filterPodmioty();
  }
  
  filterPodmioty(): void {
    this.filteredPodmioty = this.podmioty.filter(p => {
      return (
        (!this.nazwaPodmiotu || p.nazwa.toLowerCase().includes(this.nazwaPodmiotu.toLowerCase())) &&
        (!this.linkNazwaRejestrowana || p.link.toLowerCase().includes(this.linkNazwaRejestrowana.toLowerCase())) &&
        (!this.sprawyFilter || p.sprawy === this.sprawyFilter) &&
        (!this.ignarowaneZesdeFilter || p.ignarowaneZesde === this.ignarowaneZesdeFilter) &&
        (!this.administratorFilter || p.administrator === this.administratorFilter) &&
        (!this.statusFilter || p.status === this.statusFilter)
      );
    });
    this.totalItems = this.filteredPodmioty.length;
  }
  
  goBack(): void {
    this.router.navigate(['/pulpit/wnioski']);
  }
  
  pobierzZalacznik(zalacznik: Zalacznik): void {
    console.log('Pobieranie załącznika:', zalacznik.nazwa);
    // Implement download logic here
  }
  
  usunZalacznik(zalacznik: Zalacznik): void {
    const index = this.zalaczniki.indexOf(zalacznik);
    if (index > -1) {
      this.zalaczniki.splice(index, 1);
    }
  }
  
  submitWniosek(type: string): void {
    console.log(`Submitting wniosek: ${type}`);
    // Implement submission logic
  }
  
  changePage(page: number): void {
    this.currentPage = page;
  }
  
  changeWlasnosciPage(page: number): void {
    this.wlasnosciCurrentPage = page;
  }
  
  getPaginatedPodmioty(): Podmiot[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredPodmioty.slice(startIndex, startIndex + this.itemsPerPage);
  }
  
  getPaginatedWlasnosci(): Wlasnosc[] {
    const startIndex = (this.wlasnosciCurrentPage - 1) * this.wlasnosciItemsPerPage;
    return this.wlasnosci.slice(startIndex, startIndex + this.wlasnosciItemsPerPage);
  }
  
  getTotalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }
  
  getWlasnosciTotalPages(): number {
    return Math.ceil(this.wlasnosci.length / this.wlasnosciItemsPerPage);
  }
}
