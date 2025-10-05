import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-faq-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="faq-details-container">
      <h1>FAQ Details</h1>
      <p>Question and answer details - empty</p>
    </div>
  `,
  styles: [`
    .faq-details-container {
      padding: 2rem;
    }
  `]
})
export class FaqDetailsComponent {}
