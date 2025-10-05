import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-access-requests-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="access-requests-list-container">
      <h1>Access Requests List</h1>
      <p>List of access requests - empty</p>
    </div>
  `,
  styles: [`
    .access-requests-list-container {
      padding: 2rem;
    }
  `]
})
export class AccessRequestsListComponent {}
