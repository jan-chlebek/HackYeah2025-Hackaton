import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-report-submit',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="report-submit-container">
      <h1>Submit Report</h1>
      <p>Submit new report - empty</p>
    </div>
  `,
  styles: [`
    .report-submit-container {
      padding: 2rem;
    }
  `]
})
export class ReportSubmitComponent {}
