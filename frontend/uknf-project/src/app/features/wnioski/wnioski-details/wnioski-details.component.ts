import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { TimelineModule } from 'primeng/timeline';

interface WniosekDetails {
  id: number;
  title: string;
  type: string;
  status: string;
  submittedDate: Date;
  submittedBy: string;
  description: string;
  justification?: string;
  requestedAccess?: string[];
  timeline: Array<{
    status: string;
    date: Date;
    user: string;
    comment?: string;
  }>;
}

@Component({
  selector: 'app-wnioski-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ButtonModule,
    CardModule,
    TagModule,
    TimelineModule
  ],
  templateUrl: './wnioski-details.component.html',
  styleUrls: ['./wnioski-details.component.scss']
})
export class WnioskiDetailsComponent implements OnInit {
  wniosek: WniosekDetails | null = null;
  wniosekId: string | null = null;

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.wniosekId = this.route.snapshot.paramMap.get('id');
    this.loadWniosekDetails();
  }

  loadWniosekDetails(): void {
    // Mock data - replace with actual API call
    this.wniosek = {
      id: Number(this.wniosekId),
      title: 'Wniosek o dostęp do modułu raportowania',
      type: 'system-access',
      status: 'in-progress',
      submittedDate: new Date('2025-01-03'),
      submittedBy: 'Jan Kowalski',
      description: 'Prośba o przyznanie uprawnień do modułu raportowania w celu przygotowania miesięcznych raportów finansowych.',
      justification: 'Jako kierownik działu finansowego potrzebuję dostępu do modułu raportowania, aby móc przygotowywać i analizować raporty finansowe wymagane przez dyrekcję.',
      requestedAccess: [
        'Moduł raportowania - odczyt',
        'Moduł raportowania - eksport danych',
        'Archiwum raportów - odczyt'
      ],
      timeline: [
        {
          status: 'Złożony',
          date: new Date('2025-01-03 10:00'),
          user: 'Jan Kowalski'
        },
        {
          status: 'W weryfikacji',
          date: new Date('2025-01-03 14:30'),
          user: 'Administrator systemu',
          comment: 'Wniosek przekazany do weryfikacji przez kierownika IT'
        },
        {
          status: 'W trakcie akceptacji',
          date: new Date('2025-01-04 09:15'),
          user: 'Kierownik IT',
          comment: 'Weryfikacja pozytywna, oczekiwanie na akceptację dyrektora'
        }
      ]
    };
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

  approveWniosek(): void {
    console.log('Approving wniosek', this.wniosekId);
    // Implement approval logic
  }

  rejectWniosek(): void {
    console.log('Rejecting wniosek', this.wniosekId);
    // Implement rejection logic
  }
}
