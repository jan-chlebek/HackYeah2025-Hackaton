import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-contact-create',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="contact-create-container">
      <h1>Create Contact</h1>
      <p>Add new contact - empty</p>
    </div>
  `,
  styles: [`
    .contact-create-container {
      padding: 2rem;
    }
  `]
})
export class ContactCreateComponent {}
