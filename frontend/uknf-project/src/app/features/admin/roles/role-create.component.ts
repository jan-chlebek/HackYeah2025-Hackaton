import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role-create',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="role-create-container">
      <h1>Create Role</h1>
      <p>Create new role - empty</p>
    </div>
  `,
  styles: [`
    .role-create-container {
      padding: 2rem;
    }
  `]
})
export class RoleCreateComponent {}
