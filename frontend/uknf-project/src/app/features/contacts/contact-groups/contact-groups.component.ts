import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-contact-groups',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="contact-groups-container">
      <h1>Contact Groups</h1>
      <p>Contact groups management - empty</p>
    </div>
  `,
  styles: [`
    .contact-groups-container {
      padding: 2rem;
    }
  `]
})
export class ContactGroupsComponent {}
