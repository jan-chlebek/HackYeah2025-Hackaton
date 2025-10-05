import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-create',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="user-create-container">
      <h1>Create User</h1>
      <p>Create new user - empty</p>
    </div>
  `,
  styles: [`
    .user-create-container {
      padding: 2rem;
    }
  `]
})
export class UserCreateComponent {}
