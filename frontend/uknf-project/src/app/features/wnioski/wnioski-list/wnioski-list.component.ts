import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

interface Wniosek {
  id: number;
  title: string;
  type: string;
  status: string;
  submittedDate: Date;
  submittedBy: string;
  description?: string;
}

@Component({
  selector: 'app-wnioski-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    TagModule,
    TooltipModule
  ],
  templateUrl: './wnioski-list.component.html',
  styleUrls: ['./wnioski-list.component.scss']
})
export class WnioskiListComponent implements OnInit {
  wnioski: Wniosek[] = [];
  filteredWnioski: Wniosek[] = [];
  
  searchTerm = '';
  selectedStatus: string | null = null;
  selectedType: string | null = null;

  statusOptions = [
    { label: 'Wszystkie', value: null },
    { label: 'Nowy', value: 'new' },
    { label: 'W trakcie', value: 'in-progress' },
    { label: 'Zatwierdzony', value: 'approved' },
    { label: 'Odrzucony', value: 'rejected' }
  ];

  typeOptions = [
    { label: 'Wszystkie', value: null },
    { label: 'Dostęp do systemu', value: 'system-access' },
    { label: 'Zmiana danych', value: 'data-change' },
    { label: 'Inne', value: 'other' }
  ];

  ngOnInit(): void {
    this.loadWnioski();
  }

  loadWnioski(): void {
    // Mock data - replace with actual API call
    this.wnioski = [
      {
        id: 1,
        title: 'Wniosek o dostęp do modułu raportowania',
        type: 'system-access',
        status: 'new',
        submittedDate: new Date('2025-01-03'),
        submittedBy: 'Jan Kowalski',
        description: 'Prośba o przyznanie uprawnień do modułu raportowania'
      },
      {
        id: 2,
        title: 'Aktualizacja danych kontaktowych',
        type: 'data-change',
        status: 'in-progress',
        submittedDate: new Date('2025-01-02'),
        submittedBy: 'Anna Nowak',
        description: 'Zmiana adresu email i numeru telefonu'
      },
      {
        id: 3,
        title: 'Dostęp do archiwum dokumentów',
        type: 'system-access',
        status: 'approved',
        submittedDate: new Date('2024-12-28'),
        submittedBy: 'Piotr Wiśniewski',
        description: 'Dostęp do dokumentów archiwalnych za rok 2024'
      },
      {
        id: 4,
        title: 'Zgłoszenie nieprawidłowości w danych',
        type: 'other',
        status: 'rejected',
        submittedDate: new Date('2024-12-20'),
        submittedBy: 'Maria Lewandowska',
        description: 'Zgłoszenie błędnych danych w rejestrze'
      }
    ];
    
    this.filteredWnioski = [...this.wnioski];
  }

  applyFilters(): void {
    this.filteredWnioski = this.wnioski.filter(wniosek => {
      const matchesSearch = !this.searchTerm || 
        wniosek.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        wniosek.submittedBy.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      const matchesStatus = !this.selectedStatus || wniosek.status === this.selectedStatus;
      const matchesType = !this.selectedType || wniosek.type === this.selectedType;
      
      return matchesSearch && matchesStatus && matchesType;
    });
  }

  getStatusSeverity(status: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast' {
    const severityMap: Record<string, 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast'> = {
      'new': 'info',
      'in-progress': 'warn',
      'approved': 'success',
      'rejected': 'danger'
    };
    return severityMap[status] || 'info';
  }

  getStatusLabel(status: string): string {
    const labelMap: Record<string, string> = {
      'new': 'Nowy',
      'in-progress': 'W trakcie',
      'approved': 'Zatwierdzony',
      'rejected': 'Odrzucony'
    };
    return labelMap[status] || status;
  }

  getTypeLabel(type: string): string {
    const labelMap: Record<string, string> = {
      'system-access': 'Dostęp do systemu',
      'data-change': 'Zmiana danych',
      'other': 'Inne'
    };
    return labelMap[type] || type;
  }
}
