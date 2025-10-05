import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="login-container">
      <h1>Login</h1>
      <p>Login screen - empty</p>
    </div>
  `,
  styles: [`
    .login-container {
      padding: 2rem;
    }
  `]
})
export class LoginComponent {}
