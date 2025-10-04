import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-report-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="report-details-container">
      <h1>Report Details</h1>
      <p>Report details and validation status - empty</p>
    </div>
  `,
  styles: [`
    .report-details-container {
      padding: 2rem;
    }
  `]
})
export class ReportDetailsComponent {}
