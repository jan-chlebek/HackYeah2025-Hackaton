import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ToastModule } from 'primeng/toast';
import { MessageService, MenuItem } from 'primeng/api';
import { AnnouncementService, CreateAnnouncementRequest, UpdateAnnouncementRequest } from '../../../services/announcement.service';

@Component({
  selector: 'app-announcement-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    TextareaModule,
    BreadcrumbModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './announcement-create.component.html',
  styleUrls: ['./announcement-create.component.css']
})
export class AnnouncementCreateComponent implements OnInit {
  private announcementService = inject(AnnouncementService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);

  announcementForm!: FormGroup;
  loading = false;
  isEditMode = false;
  announcementId: number | null = null;

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [

    { label: 'Komunikaty', routerLink: '/announcements' },
    { label: 'Nowy komunikat' }
  ];
  
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  ngOnInit(): void {
    this.initializeForm();

    // Check if we're in edit mode
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.announcementId = Number(id);
      this.breadcrumbItems[2] = { label: 'Edytuj komunikat' };
      this.loadAnnouncement(this.announcementId);
    }
  }

  initializeForm(): void {
    this.announcementForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      content: ['', [Validators.required, Validators.maxLength(10000)]]
    });
  }

  loadAnnouncement(id: number): void {
    this.loading = true;
    this.announcementService.getAnnouncementById(id).subscribe({
      next: (announcement) => {
        this.announcementForm.patchValue({
          title: announcement.title,
          content: announcement.content
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading announcement:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Błąd',
          detail: 'Nie udało się załadować komunikatu'
        });
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.announcementForm.invalid) {
      Object.keys(this.announcementForm.controls).forEach(key => {
        this.announcementForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;

    if (this.isEditMode && this.announcementId) {
      this.updateAnnouncement();
    } else {
      this.createAnnouncement();
    }
  }

  createAnnouncement(): void {
    const request: CreateAnnouncementRequest = {
      title: this.announcementForm.get('title')?.value,
      content: this.announcementForm.get('content')?.value
    };

    this.announcementService.createAnnouncement(request).subscribe({
      next: (announcement) => {
        console.log('Announcement created successfully:', announcement);
        this.messageService.add({
          severity: 'success',
          summary: 'Sukces',
          detail: 'Komunikat został utworzony pomyślnie'
        });
        
        setTimeout(() => {
          this.router.navigate(['/announcements', announcement.id]);
        }, 1000);
      },
      error: (error) => {
        console.error('Error creating announcement:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Błąd',
          detail: 'Nie udało się utworzyć komunikatu'
        });
        this.loading = false;
      }
    });
  }

  updateAnnouncement(): void {
    if (!this.announcementId) return;

    const request: UpdateAnnouncementRequest = {
      title: this.announcementForm.get('title')?.value,
      content: this.announcementForm.get('content')?.value
    };

    this.announcementService.updateAnnouncement(this.announcementId, request).subscribe({
      next: (announcement) => {
        console.log('Announcement updated successfully:', announcement);
        this.messageService.add({
          severity: 'success',
          summary: 'Sukces',
          detail: 'Komunikat został zaktualizowany pomyślnie'
        });
        
        setTimeout(() => {
          this.router.navigate(['/announcements', announcement.id]);
        }, 1000);
      },
      error: (error) => {
        console.error('Error updating announcement:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Błąd',
          detail: 'Nie udało się zaktualizować komunikatu'
        });
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    if (this.isEditMode && this.announcementId) {
      this.router.navigate(['/announcements', this.announcementId]);
    } else {
      this.router.navigate(['/announcements']);
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.announcementForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.announcementForm.get(fieldName);
    if (field?.hasError('required')) {
      return 'To pole jest wymagane';
    }
    if (field?.hasError('maxlength')) {
      const maxLength = field.errors?.['maxlength']?.requiredLength;
      return `Maksymalna długość to ${maxLength} znaków`;
    }
    return '';
  }
}
