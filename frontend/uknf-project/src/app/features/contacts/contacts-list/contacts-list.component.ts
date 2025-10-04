import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-contacts-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="contacts-list-container">
      <h1>Contacts (Adresaci)</h1>
      <p>Contact registry - empty</p>
    </div>
  `,
  styles: [`
    .contacts-list-container {
      padding: 2rem;
    }
  `]
})
export class ContactsListComponent {}
