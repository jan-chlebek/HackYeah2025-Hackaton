import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="users-list-container">
      <h1>User Management</h1>
      <p>List of users - empty</p>
    </div>
  `,
  styles: [`
    .users-list-container {
      padding: 2rem;
    }
  `]
})
export class UsersListComponent {}
