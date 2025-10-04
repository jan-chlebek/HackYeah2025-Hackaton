import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-faq-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="faq-list-container">
      <h1>FAQ</h1>
      <p>Frequently Asked Questions - empty</p>
    </div>
  `,
  styles: [`
    .faq-list-container {
      padding: 2rem;
    }
  `]
})
export class FaqListComponent {}
