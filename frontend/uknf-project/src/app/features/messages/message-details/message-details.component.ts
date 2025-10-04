import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-message-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="message-details-container">
      <h1>Message Details</h1>
      <p>Message details and conversation - empty</p>
    </div>
  `,
  styles: [`
    .message-details-container {
      padding: 2rem;
    }
  `]
})
export class MessageDetailsComponent {}
