import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

interface Attachment {
  name: string;
  selected?: boolean;
}

@Component({
  selector: 'app-wiadomosci-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './wiadomosci-details.component.html',
  styleUrl: './wiadomosci-details.component.css'
})
export class WiadomosciDetailsComponent implements OnInit {
  messageId: string = '';
  
  // Form fields
  priorytet: string = 'Sredni';
  status: string = 'Oczekuje na odpowiedź UKNF';
  uzytkownik: string = 'Krzysztof Kowalski';
  dataPrzeslania: string = '2025-05-14 13:45:01';
  wiadomoscUzytkownika: string = 'Treść wiadomości użytkownika...';
  
  // User attachments
  zalacznikiUzytkownika: Attachment[] = [
    { name: 'Zalacznik_pierwszy.pdf' },
    { name: 'Zalacznik_drugi.pdf' },
    { name: 'Zalacznik_trzeci.txt' }
  ];
  
  // UKNF response
  pracownikUKNF: string = 'Jan Nowak';
  dataPrzeslaniaUKNF: string = '2025-05-14 13:45:01';
  wiadomoscPracownikaUKNF: string = '';
  
  // UKNF attachments
  zalacznikiUKNF: Attachment[] = [
    { name: 'Zalacznik_pierwszy.txt', selected: true },
    { name: 'Zalacznik_drugi.pdf', selected: false }
  ];
  
  // Options
  statusOptions = [
    'Oczekuje na odpowiedź UKNF',
    'Odpowiedziano',
    'Zamknięte'
  ];
  
  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    this.messageId = this.route.snapshot.params['id'] || '';
    this.loadMessageDetails();
  }
  
  loadMessageDetails(): void {
    // TODO: Load actual message details from API
    console.log('Loading message details for ID:', this.messageId);
  }
  
  pobierzZalacznik(filename: string): void {
    console.log('Pobierz załącznik:', filename);
    // TODO: Implement file download
  }
  
  dodajZalacznik(): void {
    console.log('Dodaj załącznik');
    // TODO: Implement add attachment
  }
  
  edytujZalacznik(): void {
    console.log('Edytuj załącznik');
    // TODO: Implement edit attachment
  }
  
  usunZalacznik(): void {
    console.log('Usuń załącznik');
    // TODO: Implement delete attachment
  }
  
  anuluj(): void {
    this.router.navigate(['/wiadomosci']);
  }
  
  zapiszIWyslij(): void {
    console.log('Zapisz i wyślij');
    // TODO: Implement save and send
    // Validate and send response
    if (!this.wiadomoscPracownikaUKNF.trim()) {
      alert('Proszę wpisać wiadomość pracownika UKNF');
      return;
    }
    
    const selectedAttachments = this.zalacznikiUKNF.filter(z => z.selected);
    console.log('Selected attachments:', selectedAttachments);
    
    // TODO: Send to API
    alert('Wiadomość została wysłana');
    this.router.navigate(['/wiadomosci']);
  }
  
  closeModal(): void {
    this.router.navigate(['/wiadomosci']);
  }
}
