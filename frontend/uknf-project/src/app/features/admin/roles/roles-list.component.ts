import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-roles-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="roles-list-container">
      <h1>Roles Management</h1>
      <p>List of roles - empty</p>
    </div>
  `,
  styles: [`
    .roles-list-container {
      padding: 2rem;
    }
  `]
})
export class RolesListComponent {}
