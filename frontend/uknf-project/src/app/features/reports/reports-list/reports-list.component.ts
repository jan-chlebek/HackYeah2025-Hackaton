import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-reports-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="reports-list-container">
      <h1>Reports (Sprawozdania)</h1>
      <p>List of reports - empty</p>
    </div>
  `,
  styles: [`
    .reports-list-container {
      padding: 2rem;
    }
  `]
})
export class ReportsListComponent {}
