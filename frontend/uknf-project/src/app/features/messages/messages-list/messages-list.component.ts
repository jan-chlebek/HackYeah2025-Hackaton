import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-messages-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="messages-list-container">
      <h1>Messages</h1>
      <p>List of messages - empty</p>
    </div>
  `,
  styles: [`
    .messages-list-container {
      padding: 2rem;
    }
  `]
})
export class MessagesListComponent {}
