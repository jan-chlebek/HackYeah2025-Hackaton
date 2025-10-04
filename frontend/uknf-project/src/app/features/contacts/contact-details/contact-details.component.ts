import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-contact-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="contact-details-container">
      <h1>Contact Details</h1>
      <p>Contact details - empty</p>
    </div>
  `,
  styles: [`
    .contact-details-container {
      padding: 2rem;
    }
  `]
})
export class ContactDetailsComponent {}
