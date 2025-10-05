import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-password-policy',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="password-policy-container">
      <h1>Password Policy</h1>
      <p>Configure password requirements - empty</p>
    </div>
  `,
  styles: [`
    .password-policy-container {
      padding: 2rem;
    }
  `]
})
export class PasswordPolicyComponent {}
