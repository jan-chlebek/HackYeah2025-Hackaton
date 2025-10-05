import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-message-compose',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="message-compose-container">
      <h1>Compose Message</h1>
      <p>Compose new message - empty</p>
    </div>
  `,
  styles: [`
    .message-compose-container {
      padding: 2rem;
    }
  `]
})
export class MessageComposeComponent {}
