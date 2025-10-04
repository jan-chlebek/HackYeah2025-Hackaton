import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard-container">
      <h1>Dashboard</h1>
      <p>Main dashboard - empty screen</p>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 2rem;
    }
  `]
})
export class DashboardComponent {}
