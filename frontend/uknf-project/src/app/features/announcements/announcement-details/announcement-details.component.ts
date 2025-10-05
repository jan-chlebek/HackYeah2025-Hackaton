import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TagModule } from 'primeng/tag';
import { MenuItem } from 'primeng/api';
import { AnnouncementService, Announcement } from '../../../services/announcement.service';

@Component({
  selector: 'app-announcement-details',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    CardModule,
    BreadcrumbModule,
    TagModule
  ],
  templateUrl: './announcement-details.component.html',
  styleUrls: ['./announcement-details.component.css']
})
export class AnnouncementDetailsComponent implements OnInit {
  private announcementService = inject(AnnouncementService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  announcement: Announcement | null = null;
  loading = false;
  error = false;
  errorMessage = '';

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [
    { label: 'Komunikaty', routerLink: '/announcements' },
    { label: 'Szczegóły komunikatu' }
  ];
  
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadAnnouncement(Number(id));
    } else {
      this.error = true;
    }
  }

  loadAnnouncement(id: number): void {
    this.loading = true;
    this.error = false;
    // Safety timeout to avoid indefinite spinner if HTTP never resolves
    const safety = setTimeout(() => {
      if (this.loading) {
        console.warn('[AnnouncementDetails] Safety timeout triggered, forcing loading=false');
        this.loading = false;
        this.error = true;
        this.errorMessage = 'Przekroczenie czasu ładowania komunikatu.';
        this.cdr.markForCheck();
      }
    }, 15000);

    console.log('[AnnouncementDetails] Loading announcement ID:', id);
    console.log('[AnnouncementDetails] API URL will be: http://localhost:5000/api/v1/announcements/' + id);

    this.announcementService.getAnnouncementById(id).subscribe({
      next: (announcement) => {
        console.log('[AnnouncementDetails] Announcement loaded successfully:', announcement);
        this.announcement = announcement;
        this.loading = false;
        clearTimeout(safety);
        this.cdr.markForCheck();

        // Mark as read if not already read
        if (!announcement.isReadByCurrentUser) {
          this.markAsRead(id);
        }
      },
      error: (error) => {
        console.error('[AnnouncementDetails] Error loading announcement:', error);
        console.error('[AnnouncementDetails] Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url
        });
        
        // Set user-friendly error message
        if (error.status === 404) {
          this.errorMessage = 'Komunikat o podanym ID nie został znaleziony.';
        } else if (error.status === 401) {
          this.errorMessage = 'Nie masz uprawnień do wyświetlenia tego komunikatu. Zaloguj się ponownie.';
        } else if (error.status === 403) {
          this.errorMessage = 'Dostęp do tego komunikatu jest zabroniony.';
        } else if (error.status === 0) {
          this.errorMessage = 'Brak połączenia z serwerem. Sprawdź, czy backend działa.';
        } else {
          this.errorMessage = `Błąd serwera (${error.status}): ${error.statusText || 'Nieznany błąd'}`;
        }
        
        this.error = true;
        this.loading = false;
        clearTimeout(safety);
        this.cdr.markForCheck();
      }
    });
  }

  markAsRead(id: number): void {
    this.announcementService.markAsRead(id).subscribe({
      next: () => {
        console.log('Announcement marked as read');
        if (this.announcement) {
          this.announcement.isReadByCurrentUser = true;
          this.announcement.readAt = new Date().toISOString();
          this.cdr.markForCheck();
        }
      },
      error: (error) => {
        console.error('Error marking announcement as read:', error);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/announcements']);
  }

  editAnnouncement(): void {
    if (this.announcement) {
      // TODO: Navigate to edit page (to be implemented)
      console.log('Edit announcement:', this.announcement.id);
    }
  }

  deleteAnnouncement(): void {
    if (this.announcement && confirm('Czy na pewno chcesz usunąć ten komunikat?')) {
      this.announcementService.deleteAnnouncement(this.announcement.id).subscribe({
        next: () => {
          console.log('Announcement deleted successfully');
          this.router.navigate(['/announcements']);
        },
        error: (error) => {
          console.error('Error deleting announcement:', error);
          alert('Nie udało się usunąć komunikatu. Spróbuj ponownie.');
        }
      });
    }
  }

  formatDate(date: string | null | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleString('pl-PL', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getReadStatusSeverity(isRead: boolean): 'success' | 'info' {
    return isRead ? 'success' : 'info';
  }

  getReadStatusLabel(isRead: boolean): string {
    return isRead ? 'Przeczytane' : 'Nieprzeczytane';
  }
}
