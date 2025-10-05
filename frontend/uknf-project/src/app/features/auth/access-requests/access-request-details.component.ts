import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-access-request-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="access-request-details-container">
      <h1>Access Request Details</h1>
      <p>Details of access request - empty</p>
    </div>
  `,
  styles: [`
    .access-request-details-container {
      padding: 2rem;
    }
  `]
})
export class AccessRequestDetailsComponent {}
