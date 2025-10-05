import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="role-details-container">
      <h1>Role Details</h1>
      <p>Role details and permissions - empty</p>
    </div>
  `,
  styles: [`
    .role-details-container {
      padding: 2rem;
    }
  `]
})
export class RoleDetailsComponent {}
