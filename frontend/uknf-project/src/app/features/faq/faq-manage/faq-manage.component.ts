import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-faq-manage',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="faq-manage-container">
      <h1>Manage FAQ</h1>
      <p>Manage questions and answers - empty</p>
    </div>
  `,
  styles: [`
    .faq-manage-container {
      padding: 2rem;
    }
  `]
})
export class FaqManageComponent {}
