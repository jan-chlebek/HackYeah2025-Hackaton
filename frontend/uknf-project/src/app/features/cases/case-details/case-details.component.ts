import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-case-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="case-details-container">
      <h1>Case Details</h1>
      <p>Case details and history - empty</p>
    </div>
  `,
  styles: [`
    .case-details-container {
      padding: 2rem;
    }
  `]
})
export class CaseDetailsComponent {}
