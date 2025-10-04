import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-faq-ask',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="faq-ask-container">
      <h1>Ask Question</h1>
      <p>Submit a question - empty</p>
    </div>
  `,
  styles: [`
    .faq-ask-container {
      padding: 2rem;
    }
  `]
})
export class FaqAskComponent {}
