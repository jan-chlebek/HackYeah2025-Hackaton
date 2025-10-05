import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { MultiSelectModule } from 'primeng/multiselect';

@Component({
  selector: 'app-wnioski-create',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    TextareaModule,
    SelectModule,
    MultiSelectModule
  ],
  templateUrl: './wnioski-create.component.html',
  styleUrls: ['./wnioski-create.component.scss']
})
export class WnioskiCreateComponent implements OnInit {
  wniosekForm!: FormGroup;
  submitted = false;

  typeOptions = [
    { label: 'Dostęp do systemu', value: 'system-access' },
    { label: 'Zmiana danych', value: 'data-change' },
    { label: 'Inne', value: 'other' }
  ];

  accessOptions = [
    { label: 'Moduł raportowania - odczyt', value: 'reporting-read' },
    { label: 'Moduł raportowania - eksport', value: 'reporting-export' },
    { label: 'Archiwum dokumentów - odczyt', value: 'archive-read' },
    { label: 'Zarządzanie użytkownikami', value: 'user-management' },
    { label: 'Konfiguracja systemu', value: 'system-config' }
  ];

  constructor(
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.wniosekForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      type: ['', Validators.required],
      description: ['', [Validators.required, Validators.minLength(20)]],
      justification: ['', [Validators.required, Validators.minLength(20)]],
      requestedAccess: [[], Validators.required]
    });
  }

  get f() {
    return this.wniosekForm.controls;
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.wniosekForm.invalid) {
      return;
    }

    console.log('Submitting wniosek:', this.wniosekForm.value);
    // Implement API call to create wniosek
    
    // Navigate back to list after successful creation
    this.router.navigate(['/wnioski']);
  }

  onCancel(): void {
    this.router.navigate(['/wnioski']);
  }
}
