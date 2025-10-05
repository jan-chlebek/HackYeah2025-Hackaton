import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="user-details-container">
      <h1>User Details</h1>
      <p>User details and permissions - empty</p>
    </div>
  `,
  styles: [`
    .user-details-container {
      padding: 2rem;
    }
  `]
})
export class UserDetailsComponent {}
